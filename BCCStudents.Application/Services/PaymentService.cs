using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using Core.Models;

namespace BCCStudents.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly IStudentGroupRepository _studentGroupRepo;
        private readonly IStudentSubGroupRepository _studentSubGroupRepo;
        private readonly ILoggerRepository _loggerRepository;
        private readonly ISmsService _smsService;
        private readonly IConfigurationService _configService;

        public PaymentService(
            IPaymentRepository paymentRepo,
            IStudentRepository studentRepo,
            IGroupRepository groupRepo,
            IStudentGroupRepository studentGroupRepo,
            IStudentSubGroupRepository studentSubGroupRepo,
            ILoggerRepository loggerRepository,
            ISmsService smsService,
            IConfigurationService configService)
        {
            _paymentRepo = paymentRepo;
            _studentRepo = studentRepo;
            _groupRepo = groupRepo;
            _studentGroupRepo = studentGroupRepo;
            _studentSubGroupRepo = studentSubGroupRepo;
            _loggerRepository = loggerRepository;
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        }

        /// <summary>
        /// გადახდის დამუშავება - მოსწავლის ბალანსიდან ან ახლად ჩარიცხული თანხიდან
        /// </summary>
        /// <param name="studentId">მოსწავლის ID</param>
        /// <param name="paymentAmount">ახლად ჩარიცხული თანხა (0 = მხოლოდ ბალანსიდან)</param>
        /// <param name="useFullBalance">თუ true - გამოიყენებს სრულ ბალანსს (რამდენ თვესაც ფარავს), თუ false - მხოლოდ ერთი თვე</param>
        /// <returns>გადახდის შედეგი</returns>
        /// <remarks>
        /// პროცესი:
        /// 1. ვალიდაცია - მოსწავლის არსებობა, აქტიური ჯგუფები
        /// 2. ბალანსის გამოთვლა = student.Balance + paymentAmount
        /// 3. ყველა ჯგუფის გადამოწმება (თარიღის მიხედვით დალაგებული)
        /// 4. თითოეული ჯგუფისთვის:
        ///    - თუ balance >= finalFee → სრული გადახდა
        ///    - თუ balance > 0 მაგრამ < finalFee → ნაწილობრივი გადახდა
        ///    - თუ balance = 0 → მხოლოდ ბალანსზე დაკრედიტება
        /// 5. ბალანსის განახლება
        /// </remarks>
        public async Task<PaymentResult> ProcessPayment(int studentId, decimal paymentAmount, bool useFullBalance = true)
        {
            try
            {
                // ==================== 1. ვალიდაცია ====================

                // 1.1. მოსწავლის არსებობის შემოწმება
                var student = _studentRepo.GetStudentById(studentId);
                if (student == null)
                {
                    return new PaymentResult(false, "StudentNotFound", new List<string> { "სტუდენტი ვერ მოიძებნა" });
                }

                // 1.2. აქტიური ჯგუფების მიღება
                // ⚠️ შენიშვნა: GetCurrentMonthPayments შემოწმება ამოღებულია, რადგან IsGroupPaidForPeriod 
                // უკვე ამოწმებს გადახდას კონკრეტული პერიოდისთვის
                // ⚠️ მნიშვნელოვანი: GetForPayment აბრუნებს Groups.Price-ს (არა StudentGroups.Price)
                var groups = _studentGroupRepo.GetForPayment(studentId);
                if (groups == null || !groups.Any())
                {
                    return new PaymentResult(false, "NoGroups", new List<string> { "სტუდენტს არ აქვს აქტიური ჯგუფები" });
                }
                int groupCount = groups.Count;

                // ==================== 2. ბალანსის გამოთვლა ====================
                // balance = მიმდინარე ბალანსი + ახლად ჩარიცხული თანხა
                // თუ paymentAmount = 0 → მხოლოდ ბალანსიდან იხდის (ავტომატური გადახდა)
                decimal initialBalance = student.Balance;
                decimal balance = initialBalance + paymentAmount;
                string paymentSource = paymentAmount > 0 ? $"ახალი თანხა: {paymentAmount} ₾" : "ბალანსიდან";

                var logs = new List<string>();
                var paymentResults = new List<PaymentDetail>();
                bool paymentMade = false;      // სრული გადახდა მოხდა
                bool partialPayment = false;  // ნაწილობრივი გადახდა მოხდა
                bool onlyCredited = false;    // მხოლოდ ბალანსზე დაკრედიტება

                // ==================== 3. გადახდის პროცესი - ვადაგადაცილებული თვეების დაფარვა ====================
                // ვაგროვებთ ყველა ჯგუფს და ყველა ვადაგადაცილებულ თვეს (DateOfPayment-დან დღემდე)
                var dueItems = new List<(StudentGroups group, decimal finalFee, DateTime dueDate)>();
                var groupStates = new Dictionary<int, GroupPaymentState>();

                foreach (var group in groups.OrderBy(g => g.DateOfPayment))
                {
                    // ==================== 3.1. გადახდის თარიღის შემოწმება ====================
                    if (!group.DateOfPayment.HasValue)
                    {
                        logs.Add($"ჯგუფი {group.GroupId}: გადახდის თარიღი არ არის დაყენებული");
                        continue; // გამოტოვება - თარიღი არ არის
                    }

                    if (group.DateOfPayment.Value > DateTime.Today)
                    {
                        logs.Add($"ჯგუფი {group.GroupId}: გადახდის დრო ჯერ არ დამდგარა ({group.DateOfPayment.Value:dd.MM.yyyy})");
                        continue; // გამოტოვება - ჯერ არ დადგა დრო
                    }

                    // ==================== 3.2. ფასდაკლების გამოთვლა ====================
                    // ⚠️ მნიშვნელოვანი: group.Price არის Groups.Price (GetForPayment-ში გადაიწერა)
                    // group.Discount არის StudentGroups.Discount (ფასდაკლების პროცენტი)
                    decimal discount = (group.Discount > 0) ? group.Price * (decimal)group.Discount / 100m : 0m;
                    decimal finalFee = group.Price - discount; // ფასდაკლებული საბოლოო ფასი

                    var state = new GroupPaymentState
                    {
                        Group = group,
                        FinalFee = finalFee
                    };

                    // ==================== 3.3. ვადაგადაცილებული თვეების ამოღება ====================
                    // პერიოდები: DateOfPayment-დან დღემდე, თვეებით
                    var dueDate = group.DateOfPayment.Value.Date;
                    while (dueDate <= DateTime.Today)
                    {
                        state.DueDates.Add(dueDate);

                        // უკვე გადახდილია თუ არა ამ თვისთვის
                        if (_paymentRepo.IsGroupPaidForPeriod(studentId, group.GroupId, dueDate))
                        {
                            state.PaidDueDates.Add(dueDate);
                        }
                        else
                        {
                            dueItems.Add((group, finalFee, dueDate));
                        }

                        dueDate = dueDate.AddMonths(1);
                    }

                    groupStates[group.GroupId] = state;
                }

                // ==================== 3.4. ვადაგადაცილებული თვეების გადახდა ====================
                int totalDueCount = dueItems.Count;
                int paidMonthsCount = 0;
                var paidDueDatesThisRun = new List<DateTime>();
                var remainingDueDates = new List<DateTime>();

                if (dueItems.Any())
                {
                    int processedItems = 0;
                    int maxItems = useFullBalance ? int.MaxValue : 1;

                    foreach (var (group, finalFee, dueDate) in dueItems
                        .OrderBy(i => i.dueDate)
                        .ThenBy(i => i.group.GroupId))
                    {
                        if (processedItems >= maxItems)
                        {
                            break;
                        }

                        if (balance <= 0)
                        {
                            break;
                        }

                        string groupName = _groupRepo.GetGroupNameById(group.GroupId);
                        var state = groupStates[group.GroupId];

                        if (balance >= finalFee)
                        {
                            // სრულად გადახდა კონკრეტული თვისთვის
                            string description = $"ჯგუფი: {groupName} | " +
                                              $"თვე: {dueDate:yyyy-MM} | " +
                                              $"თანხა: {finalFee} ₾ | " +
                                              $"წყარო: {paymentSource} | " +
                                              $"თავდაპირველი ბალანსი: {initialBalance} ₾";

                            var payment = new Payment
                            {
                                StudentId = studentId,
                                GroupId = group.GroupId,
                                Amount = finalFee,
                                PaymentDate = dueDate,
                                PaymentStatus = "Paid",
                                Description = description
                            };

                            var paymentId = _paymentRepo.InsertPayment(payment);
                            if (paymentId > 0)
                            {
                                payment.Id = paymentId;

                                balance -= finalFee;
                                paymentMade = true;
                                paidMonthsCount++;
                                paidDueDatesThisRun.Add(dueDate);
                                state.PaidDueDatesThisRun.Add(dueDate);
                                state.HasNewPayment = true;
                                state.PaidDueDates.Add(dueDate);
                                processedItems++;

                                paymentResults.Add(new PaymentDetail
                                {
                                    GroupId = group.GroupId,
                                    GroupName = groupName,
                                    Amount = finalFee,
                                    Status = "Paid",
                                    PaymentDate = dueDate,
                                    Note = $"თვე: {dueDate:yyyy-MM}"
                                });

                                logs.Add($"ჯგუფი {group.GroupId}: გადახდილია {dueDate:yyyy-MM} ({finalFee} ₾)");
                            }
                        }
                        else
                        {
                            if (!_configService.AllowPartialPayments)
                            {
                                logs.Add($"ჯგუფი {group.GroupId}: ნაწილობრივი გადახდა გამორთულია, თანხა დარჩება ბალანსზე ({balance} ₾)");
                                break;
                            }

                            // ნაწილობრივი გადახდა კონკრეტული თვისთვის
                            decimal remainingAmount = finalFee - balance;
                            string description = $"ჯგუფი: {groupName} | " +
                                              $"თვე: {dueDate:yyyy-MM} | " +
                                              $"ნაწილობრივი გადახდა: {balance} ₾ | " +
                                              $"დარჩენილი: {remainingAmount} ₾ | " +
                                              $"წყარო: {paymentSource} | " +
                                              $"თავდაპირველი ბალანსი: {initialBalance} ₾";

                            var payment = new Payment
                            {
                                StudentId = studentId,
                                GroupId = group.GroupId,
                                Amount = balance,
                                PaymentDate = dueDate,
                                PaymentStatus = "Partial",
                                Description = description,
                                Note = $"ნაწილობრივი გადახდა (დარჩენილი: {remainingAmount} ₾)"
                            };

                            var paymentId = _paymentRepo.InsertPayment(payment);
                            if (paymentId > 0)
                            {
                                payment.Id = paymentId;

                                state.HasNewPayment = true;
                                state.HasPartial = true;
                                state.PartialDueDate = dueDate;

                                paymentResults.Add(new PaymentDetail
                                {
                                    GroupId = group.GroupId,
                                    GroupName = groupName,
                                    Amount = balance,
                                    Status = "Partial",
                                    RemainingAmount = remainingAmount,
                                    PaymentDate = dueDate,
                                    Note = payment.Note
                                });

                                logs.Add($"ჯგუფი {group.GroupId}: ნაწილობრივად გადახდილია {balance} ₾ ({dueDate:yyyy-MM})");

                                balance = 0;
                                partialPayment = true;
                            }
                        }
                    }

                    // ==================== 3.5. StudentGroups/StudentSubGroups სტატუსის განახლება ====================
                    foreach (var state in groupStates.Values.Where(s => s.HasNewPayment || s.HasPartial))
                    {
                        if (!state.DueDates.Any())
                        {
                            continue;
                        }

                        var unpaidDueDates = state.DueDates
                            .Where(d => !state.PaidDueDates.Contains(d))
                            .OrderBy(d => d)
                            .ToList();

                        DateTime? nextPaymentDate;
                        string status;

                        if (unpaidDueDates.Any())
                        {
                            nextPaymentDate = unpaidDueDates.First();
                            status = "PartiallyPaid";
                        }
                        else
                        {
                            var lastDueDate = state.DueDates.Max();
                            nextPaymentDate = lastDueDate.AddMonths(1);
                            status = "Paid";
                        }

                        _studentGroupRepo.UpdatePaymentStatusAndDate(studentId, state.Group.GroupId, status, nextPaymentDate);

                        if (state.Group.SubGroupId.HasValue)
                        {
                            _studentSubGroupRepo.UpdatePaymentStatus(studentId, state.Group.GroupId, state.Group.SubGroupId.Value, status);
                        }
                    }

                    remainingDueDates = groupStates.Values
                        .SelectMany(s => s.DueDates.Where(d => !s.PaidDueDates.Contains(d)))
                        .OrderBy(d => d)
                        .ToList();
                }
                else
                {
                    // თუ dueItems ცარიელია, მაგრამ paymentAmount > 0, ბალანსზე უნდა დაემატოს
                    if (paymentAmount > 0)
                    {
                        onlyCredited = true;
                        logs.Add("ყველა ჯგუფი გამოტოვებულია (თარიღი არ არის, ან ჯერ არ დადგა დრო, ან უკვე გადახდილია)");
                    }
                }

                // ==================== 4. ბალანსის განახლება ====================
                // განახლება მხოლოდ თუ გადახდა მოხდა ან ბალანსზე დაკრედიტდა
                // ⚠️ მნიშვნელოვანი: UpdateStudentBalance აყენებს აბსოლუტურ ბალანსს
                // balance ცვლადი შეიცავს დარჩენილ ბალანსს (ან 0 თუ ყველაფერი გადაიხადა)

                // ⚠️ მნიშვნელოვანი: თუ ყველა ჯგუფი გამოტოვებულია (DateOfPayment არ არის, ან ჯერ არ დადგა დრო, ან უკვე გადახდილია),
                // მაგრამ paymentAmount > 0, მაშინ ბალანსზე უნდა დაემატოს თანხა
                if (paymentMade || partialPayment || onlyCredited)
                {
                    _studentRepo.UpdateStudentBalance(studentId, balance);
                }
                else if (paymentAmount > 0 && !paymentMade && !partialPayment)
                {
                    // თუ ყველა ჯგუფი გამოტოვებულია, მაგრამ თანხა ჩარიცხულია, ბალანსზე უნდა დაემატოს
                    // balance = student.Balance + paymentAmount (წინა ხაზზე გამოთვლილი)
                    _studentRepo.UpdateStudentBalance(studentId, balance);
                    logs.Add($"ყველა ჯგუფი გამოტოვებულია (თარიღი არ არის, ან ჯერ არ დადგა დრო, ან უკვე გადახდილია). თანხა დაემატა ბალანსზე: {paymentAmount} ₾");
                    onlyCredited = true; // რომ Status იყოს "Credited"
                }

                // ==================== 5. ლოგირება ====================
                _loggerRepository.LogPaymentAction(
                    "გადახდის პროცესი",
                    paymentMade ? "Success" : (partialPayment ? "Partial" : (onlyCredited ? "Credited" : "NoPayment")),
                    $"სტუდენტი: {student.FirstName} {student.LastName}, ID: {studentId}\n" + $"გადახდის დეტალები:\n{string.Join("\n", paymentResults.Select(p => $"ჯგუფი {p.GroupName}: {p.Amount} ₾ ({p.Status})" + (p.RemainingAmount.HasValue ? $", დარჩენილი: {p.RemainingAmount} ₾" : "") + (p.NextPaymentDate.HasValue ? $", შემდეგი გადახდა: {p.NextPaymentDate:dd.MM.yyyy}" : "")))}",
                    "System"
                );

                // ==================== 5.1 SMS შეტყობინება ====================
                if (paymentMade || partialPayment)
                {
                    if (groupCount > 1)
                    {
                        foreach (var state in groupStates.Values.Where(s => s.HasNewPayment || s.HasPartial))
                        {
                            decimal groupTotalPaid = paymentResults
                                .Where(r => r.GroupId == state.Group.GroupId)
                                .Sum(r => r.Amount);

                            if (groupTotalPaid <= 0)
                            {
                                continue;
                            }

                            var unpaidDueDates = state.DueDates
                                .Where(d => !state.PaidDueDates.Contains(d))
                                .OrderBy(d => d)
                                .ToList();

                            bool isFullPayment = unpaidDueDates.Count == 0;
                            int paidMonths = state.PaidDueDatesThisRun.Count;
                            int remainingMonths = unpaidDueDates.Count;

                            string paidMonthsText = FormatMonthList(state.PaidDueDatesThisRun);
                            string remainingMonthsText = FormatMonthList(unpaidDueDates);

                            var smsResult = await SendGroupPaymentSummarySms(
                                student,
                                state.Group.Name,
                                groupTotalPaid,
                                isFullPayment,
                                paidMonths,
                                remainingMonths,
                                paidMonthsText,
                                remainingMonthsText);

                            _loggerRepository.LogSMSAction(
                                "Payment SMS",
                                smsResult.Success ? "Success" : "Failed",
                                smsResult.Status,
                                student.PhoneNumber);
                        }
                    }
                    else
                    {
                        decimal totalPaid = paymentResults.Sum(r => r.Amount);
                        bool isFullPayment = paymentMade && !partialPayment;
                        int? remainingMonths = null;
                        string paidMonthsText = null;
                        string remainingMonthsText = null;
                        if (!isFullPayment)
                        {
                            remainingMonths = Math.Max(0, totalDueCount - paidMonthsCount);
                            paidMonthsText = FormatMonthList(paidDueDatesThisRun);
                            remainingMonthsText = FormatMonthList(remainingDueDates);
                        }
                        var smsResult = await SendPaymentSummarySms(
                            student,
                            totalPaid,
                            isFullPayment,
                            paidMonthsCount,
                            remainingMonths,
                            paidMonthsText,
                            remainingMonthsText);
                        _loggerRepository.LogSMSAction("Payment SMS", smsResult.Success ? "Success" : "Failed", smsResult.Status, student.PhoneNumber);
                    }
                }

                // ==================== 6. შედეგის დაბრუნება ====================
                return new PaymentResult(
                    paymentMade || partialPayment || onlyCredited,
                    paymentMade ? "Paid"           // სრული გადახდა
                            : partialPayment ? "Partial"  // ნაწილობრივი გადახდა
                            : onlyCredited ? "Credited"   // მხოლოდ ბალანსზე დაკრედიტება
                            : "NoPayment",                // გადახდა არ მოხდა
                    logs
                );
            }
            catch (Exception ex)
            {
                // ==================== შეცდომის დამუშავება ====================
                _loggerRepository.LogPaymentAction("გადახდის შეცდომა", "Error", ex.ToString(), "System");
                return new PaymentResult(false, "Error", new List<string> { $"შეცდომა: {ex.Message}" });
            }
        }


        /// <summary>
        /// SMS გაგზავნა გადახდის შემდეგ
        /// </summary>
        private class GroupPaymentState
        {
            public StudentGroups Group { get; set; }
            public decimal FinalFee { get; set; }
            public List<DateTime> DueDates { get; } = new List<DateTime>();
            public HashSet<DateTime> PaidDueDates { get; } = new HashSet<DateTime>();
            public List<DateTime> PaidDueDatesThisRun { get; } = new List<DateTime>();
            public bool HasNewPayment { get; set; }
            public bool HasPartial { get; set; }
            public DateTime? PartialDueDate { get; set; }
        }

        private async Task<SmsSendResult> SendPaymentSummarySms(
            Student student,
            decimal amount,
            bool isFullPayment,
            int? paidMonths,
            int? remainingMonths,
            string paidMonthsText,
            string remainingMonthsText)
        {
            string message = $"გამარჯობა {student.FirstName}!\n" +
                           $"თქვენი გადახდა შესრულდა.\n" +
                           $"თანხა: {amount} ₾\n" +
                           $"სტატუსი: {(isFullPayment ? "სრული გადახდა" : "ნაწილობრივი გადახდა")}\n";

            if (!isFullPayment && paidMonths.HasValue && remainingMonths.HasValue)
            {
                message += $"გადახდილი თვეები: {paidMonths}\n" +
                           $"დარჩენილი თვეები: {remainingMonths}\n";

                if (!string.IsNullOrWhiteSpace(paidMonthsText) || !string.IsNullOrWhiteSpace(remainingMonthsText))
                {
                    message += $"დაიფარა: {(!string.IsNullOrWhiteSpace(paidMonthsText) ? paidMonthsText : "-")}\n" +
                               $"გადასახდელია: {(!string.IsNullOrWhiteSpace(remainingMonthsText) ? remainingMonthsText : "-")}\n";
                }
            }

            message +=
                           $"თარიღი: {DateTime.Now:dd.MM.yyyy HH:mm}";

            return await _smsService.SendSmsAsync(student.PhoneNumber, message);
        }

        private async Task<SmsSendResult> SendGroupPaymentSummarySms(
            Student student,
            string groupName,
            decimal amount,
            bool isFullPayment,
            int paidMonths,
            int remainingMonths,
            string paidMonthsText,
            string remainingMonthsText)
        {
            string message = $"გამარჯობა {student.FirstName}!\n" +
                           $"თქვენი გადახდა შესრულდა.\n" +
                           $"ჯგუფი: {groupName}\n" +
                           $"თანხა: {amount} ₾\n" +
                           $"სტატუსი: {(isFullPayment ? "სრული გადახდა" : "ნაწილობრივი გადახდა")}\n";

            if (!isFullPayment)
            {
                message += $"გადახდილი თვეები: {paidMonths}\n" +
                           $"დარჩენილი თვეები: {remainingMonths}\n" +
                           $"დაიფარა: {(!string.IsNullOrWhiteSpace(paidMonthsText) ? paidMonthsText : "-")}\n" +
                           $"გადასახდელია: {(!string.IsNullOrWhiteSpace(remainingMonthsText) ? remainingMonthsText : "-")}\n";
            }

            message += $"თარიღი: {DateTime.Now:dd.MM.yyyy HH:mm}";

            return await _smsService.SendSmsAsync(student.PhoneNumber, message);
        }

        private static string FormatMonthList(IReadOnlyCollection<DateTime> dates)
        {
            if (dates == null || dates.Count == 0)
            {
                return null;
            }

            var ordered = dates.OrderBy(d => d).ToList();
            bool multipleYears = ordered.Select(d => d.Year).Distinct().Count() > 1;

            return string.Join(
                ", ",
                ordered.Select(d => multipleYears ? $"{GetGeorgianMonthName(d.Month)} {d.Year}" : GetGeorgianMonthName(d.Month))
            );
        }

        private static string GetGeorgianMonthName(int month)
        {
            switch (month)
            {
                case 1: return "იანვარი";
                case 2: return "თებერვალი";
                case 3: return "მარტი";
                case 4: return "აპრილი";
                case 5: return "მაისი";
                case 6: return "ივნისი";
                case 7: return "ივლისი";
                case 8: return "აგვისტო";
                case 9: return "სექტემბერი";
                case 10: return "ოქტომბერი";
                case 11: return "ნოემბერი";
                case 12: return "დეკემბერი";
                default: return month.ToString();
            }
        }

        /// <summary>
        /// ავტომატური გადახდა ყველა მოსწავლისთვის (ბალანსიდან)
        /// </summary>
        /// <param name="progressCallback">პროგრესის კალბექი (current, total)</param>
        /// <remarks>
        /// პროცესი:
        /// 1. ყველა აქტიური მოსწავლის მიღება
        /// 2. თითოეული მოსწავლისთვის (თუ Balance > 0):
        ///    - ProcessPayment(studentId, 0) - მხოლოდ ბალანსიდან
        /// </remarks>
        public async Task ProcessAutoPaymentsForAllStudentsAsync(Action<int, int> progressCallback = null, bool useFullBalance = true)
        {
            var students = _studentRepo.GetAllActiveStudents();
            int total = students.Count;
            int current = 0;

            foreach (var student in students)
            {
                // მხოლოდ მოსწავლეებს რომელთაც აქვთ ბალანსი > 0
                if (student.Balance > 0)
                {
                    // paymentAmount = 0 → მხოლოდ ბალანსიდან იხდის
                    await ProcessPayment(student.Id, 0, useFullBalance);
                }
                current++;
                progressCallback?.Invoke(current, total);
            }
        }

        public List<PaymentSummary> GetPaymentSummaries()
        {
            return _paymentRepo.GetPaymentSummaries();
        }

        public List<PaymentSummary> GetPendingPayments()
        {
            return _paymentRepo.GetPendingPayments();
        }
    }
}




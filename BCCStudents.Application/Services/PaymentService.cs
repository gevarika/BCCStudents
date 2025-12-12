using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Domain.Entities;
using Core.Models;

using BCCStudents.Application.Interfaces;

namespace BCCStudents.Application.Services {
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly IStudentGroupRepository _studentGroupRepo;
        private readonly IStudentSubGroupRepository _studentSubGroupRepo;
        private readonly ILoggerRepository _loggerRepository;
        private readonly ISmsService _smsService;

        public PaymentService(
            IPaymentRepository paymentRepo,
            IStudentRepository studentRepo,
            IGroupRepository groupRepo,
            IStudentGroupRepository studentGroupRepo,
            IStudentSubGroupRepository studentSubGroupRepo,
            ILoggerRepository loggerRepository,
            ISmsService smsService)
        {
            _paymentRepo = paymentRepo;
            _studentRepo = studentRepo;
            _groupRepo = groupRepo;
            _studentGroupRepo = studentGroupRepo;
            _studentSubGroupRepo = studentSubGroupRepo;
            _loggerRepository = loggerRepository;
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
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

                // ==================== 3. გადახდის პროცესი - რამდენიმე თვის გადასახადის გადახდა ====================
                // ვაგროვებთ ყველა ჯგუფს, რომელთაც დადგა გადახდის დრო და არ არის გადახდილი
                var eligibleGroups = new List<(StudentGroups group, decimal finalFee, DateTime dateOfPayment)>();
                
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

                    // ==================== 3.2. გადახდილია თუ არა შემოწმება ====================
                    // შეამოწმებს Payments ცხრილში არის თუ არა გადახდა ამ პერიოდისთვის
                    // პერიოდი = DateOfPayment თვის/წლის ფარგლებში
                    if (_paymentRepo.IsGroupPaidForPeriod(studentId, group.GroupId, group.DateOfPayment.Value))
                    {
                        logs.Add($"ჯგუფი {group.GroupId}: უკვე გადახდილია ამ თვეში");
                        continue; // გამოტოვება - უკვე გადახდილია
                    }

                    // ==================== 3.3. ფასდაკლების გამოთვლა ====================
                    // ⚠️ მნიშვნელოვანი: group.Price არის Groups.Price (GetForPayment-ში გადაიწერა)
                    // group.Discount არის StudentGroups.Discount (ფასდაკლების პროცენტი)
                    decimal discount = (group.Discount > 0) ? group.Price * (decimal)group.Discount / 100m : 0m;
                    decimal finalFee = group.Price - discount; // ფასდაკლებული საბოლოო ფასი

                    eligibleGroups.Add((group, finalFee, group.DateOfPayment.Value));
                }

                // ==================== 3.4. რამდენიმე თვის გადასახადის გადახდა ====================
                if (eligibleGroups.Any())
                {
                    // ვითვლით რამდენი თვის გადასახადს ფარავს balance
                    int monthsToPay = 0;
                    decimal totalAmount = 0;
                    var paidMonths = new List<DateTime>();
                    var lastPaymentDate = DateTime.MinValue;

                    // თუ useFullBalance = false, მხოლოდ ერთი თვე
                    int maxMonths = useFullBalance ? eligibleGroups.Count : 1;

                    foreach (var (group, finalFee, dateOfPayment) in eligibleGroups)
                    {
                        // თუ useFullBalance = false, მხოლოდ პირველი თვე
                        if (monthsToPay >= maxMonths)
                        {
                            break;
                        }

                        if (balance >= totalAmount + finalFee)
                        {
                            monthsToPay++;
                            totalAmount += finalFee;
                            paidMonths.Add(dateOfPayment);
                            lastPaymentDate = dateOfPayment;
                        }
                        else
                        {
                            break; // ბალანსი აღარ საკმარისია
                        }
                    }

                    if (monthsToPay > 0)
                    {
                        // ========== 3.4.1. რამდენიმე თვის გადასახადის გადახდა ==========
                        var firstGroup = eligibleGroups[0].group;
                        string groupName = _groupRepo.GetGroupNameById(firstGroup.GroupId);
                        
                        // დეტალური Description-ის შექმნა
                        var monthDetails = new List<string>();
                        decimal runningTotal = 0;
                        foreach (var (group, finalFee, dateOfPayment) in eligibleGroups.Take(monthsToPay))
                        {
                            runningTotal += finalFee;
                            monthDetails.Add($"{dateOfPayment:yyyy-MM} ({finalFee} ₾)");
                        }
                        
                        decimal remainingBalance = balance - totalAmount;
                        string description = $"ჯგუფი: {groupName} | " +
                                          $"დაიფარა {monthsToPay} თვის გადასახადი: {totalAmount} ₾ | " +
                                          $"თვეები: {string.Join(", ", monthDetails)} | " +
                                          $"წყარო: {paymentSource} | " +
                                          $"თავდაპირველი ბალანსი: {initialBalance} ₾ | " +
                                          $"დარჩენილი ბალანსი: {remainingBalance:F2} ₾";
                        
                        // 3.4.1.1. Payment ცხრილში ჩანაწერის დამატება
                        var payment = new Payment
                        {
                            StudentId = studentId,
                            GroupId = firstGroup.GroupId,
                            Amount = totalAmount, // რამდენიმე თვის გადასახადის ჯამი
                            PaymentDate = DateTime.Now,
                            PaymentStatus = "Paid",
                            Description = description
                        };

                        if (_paymentRepo.InsertPayment(payment))
                        {
                            // 3.4.1.2. შემდეგი გადახდის თარიღის გამოთვლა (ბოლო გადახდილი თვე + 1 თვე)
                            var nextPaymentDate = lastPaymentDate.AddMonths(1);
                            
                            // 3.4.1.3. StudentGroups ცხრილში განახლება
                            // PaymentStatus = "Paid", DateOfPayment = nextPaymentDate
                            _studentGroupRepo.UpdatePaymentStatusAndDate(studentId, firstGroup.GroupId, "Paid", nextPaymentDate);

                            // 3.4.1.4. StudentSubGroups ცხრილში განახლება (თუ არსებობს)
                            if (firstGroup.SubGroupId.HasValue)
                            {
                                _studentSubGroupRepo.UpdatePaymentStatus(studentId, firstGroup.GroupId, firstGroup.SubGroupId.Value, "Paid");
                            }

                            // 3.4.1.5. ბალანსის განახლება
                            balance -= totalAmount;
                            paymentMade = true;
                            
                            // 3.4.1.6. შედეგის დამატება
                            paymentResults.Add(new PaymentDetail
                            {
                                GroupId = firstGroup.GroupId,
                                GroupName = _groupRepo.GetGroupNameById(firstGroup.GroupId),
                                Amount = totalAmount,
                                Status = "Paid",
                                NextPaymentDate = nextPaymentDate,
                                PaymentDate = DateTime.Now,
                                Note = $"დაიფარა {monthsToPay} თვის გადასახადი"
                            });

                            // 3.4.1.7. SMS გაგზავნა
                            var smsResult = await SendPaymentSms(student, firstGroup, totalAmount, true);
                            _loggerRepository.LogSMSAction("Payment SMS", smsResult.Success ? "Success" : "Failed", smsResult.Status, student.PhoneNumber);

                            logs.Add($"ჯგუფი {firstGroup.GroupId}: დაიფარა {monthsToPay} თვის გადასახადი ({totalAmount} ₾)");
                        }
                    }
                    // ========== 3.4.2. ნაწილობრივი გადახდა ==========
                    else if (balance > 0)
                    {
                        // ბალანსი არ არის საკმარისი სრული გადახდისთვის, მაგრამ > 0
                        var firstGroup = eligibleGroups[0].group;
                        var firstFinalFee = eligibleGroups[0].finalFee;
                        string groupName = _groupRepo.GetGroupNameById(firstGroup.GroupId);
                        decimal remainingAmount = firstFinalFee - balance;
                        decimal remainingBalance = 0; // ბალანსი გამოიყენება სრულად
                        
                        // დეტალური Description-ის შექმნა
                        string description = $"ჯგუფი: {groupName} | " +
                                          $"ნაწილობრივი გადახდა: {balance} ₾ | " +
                                          $"სრული თანხა: {firstFinalFee} ₾ | " +
                                          $"დარჩენილი: {remainingAmount} ₾ | " +
                                          $"თვე: {eligibleGroups[0].dateOfPayment:yyyy-MM} | " +
                                          $"წყარო: {paymentSource} | " +
                                          $"თავდაპირველი ბალანსი: {initialBalance} ₾ | " +
                                          $"დარჩენილი ბალანსი: {remainingBalance:F2} ₾";
                        
                        // 3.4.2.1. Payment ცხრილში ჩანაწერის დამატება (Status = "Partial")
                        var payment = new Payment
                        {
                            StudentId = studentId,
                            GroupId = firstGroup.GroupId,
                            Amount = balance, // მხოლოდ ის რაც ბალანსზეა
                            PaymentDate = DateTime.Now,
                            PaymentStatus = "Partial",
                            Description = description,
                            Note = $"ნაწილობრივი გადახდა (დარჩენილი: {remainingAmount} ₾)"
                        };

                        if (_paymentRepo.InsertPayment(payment))
                        {
                            // 3.4.2.2. StudentGroups ცხრილში განახლება
                            // PaymentStatus = "PartiallyPaid", DateOfPayment არ იცვლება (იგივე თარიღი რჩება)
                            _studentGroupRepo.UpdatePaymentStatusAndDate(studentId, firstGroup.GroupId, "PartiallyPaid", eligibleGroups[0].dateOfPayment);

                            // 3.4.2.3. StudentSubGroups ცხრილში განახლება
                            if (firstGroup.SubGroupId.HasValue)
                            {
                                _studentSubGroupRepo.UpdatePaymentStatus(studentId, firstGroup.GroupId, firstGroup.SubGroupId.Value, "PartiallyPaid");
                            }

                            // 3.4.2.4. შედეგის დამატება
                            paymentResults.Add(new PaymentDetail
                            {
                                GroupId = firstGroup.GroupId,
                                GroupName = _groupRepo.GetGroupNameById(firstGroup.GroupId),
                                Amount = balance,
                                Status = "Partial",
                                RemainingAmount = firstFinalFee - balance, // დარჩენილი თანხა
                                PaymentDate = DateTime.Now,
                                Note = payment.Note
                            });

                            // 3.4.2.5. SMS გაგზავნა
                            var smsResult = await SendPaymentSms(student, firstGroup, balance, false);
                            _loggerRepository.LogSMSAction("Payment SMS", smsResult.Success ? "Success" : "Failed", smsResult.Status, student.PhoneNumber);

                            logs.Add($"ჯგუფი {firstGroup.GroupId}: ნაწილობრივად გადახდილია {balance} ₾ (დარჩენილი: {firstFinalFee - balance} ₾)");
                            
                            // 3.4.2.6. ბალანსის განულება (ყველაფერი გამოყენებულია)
                            balance = 0;
                            partialPayment = true;
                        }
                    }
                }
                else
                {
                    // თუ eligibleGroups ცარიელია, მაგრამ paymentAmount > 0, ბალანსზე უნდა დაემატოს
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
        private async Task<SmsSendResult> SendPaymentSms(Student student, StudentGroups group, decimal amount, bool isFullPayment)
        {
            string message = $"გამარჯობა {student.FirstName}!\n" +
                           $"თქვენი გადახდა {group.Name} ჯგუფისთვის შესრულდა.\n" +
                           $"თანხა: {amount} ₾\n" +
                           $"სტატუსი: {(isFullPayment ? "სრული გადახდა" : "ნაწილობრივი გადახდა")}\n" +
                           $"თარიღი: {DateTime.Now:dd.MM.yyyy HH:mm}";

            return await _smsService.SendSmsAsync(student.PhoneNumber, message);
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
    }
}




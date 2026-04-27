using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services
{
    /// <summary>
    /// გადახდის თარიღის მართვის სერვისი
    /// ამუშავებს გადახდის თარიღის გამოთვლას და განახლებას სხვადასხვა სცენარებისთვის
    /// </summary>
    public class PaymentDateService : IPaymentDateService
    {
        private readonly IStudentGroupRepository _studentGroupRepo;
        private readonly IStudentSubGroupRepository _studentSubGroupRepo;
        private readonly IPaymentRepository _paymentRepo;

        public PaymentDateService(
            IStudentGroupRepository studentGroupRepo,
            IStudentSubGroupRepository studentSubGroupRepo,
            IPaymentRepository paymentRepo)
        {
            _studentGroupRepo = studentGroupRepo;
            _studentSubGroupRepo = studentSubGroupRepo;
            _paymentRepo = paymentRepo;
        }

        #region ==================== საწყისი თარიღის დაყენება ====================

        /// <summary>
        /// რეგისტრაციისას საწყისი გადახდის თარიღის დაყენება
        /// </summary>
        /// <param name="registrationDate">რეგისტრაციის თარიღი</param>
        /// <param name="dayOfMonth">თვის რომელ დღეს უნდა იყოს გადახდა (default: რეგისტრაციის დღე)</param>
        /// <returns>პირველი გადახდის თარიღი</returns>
        public DateTime CalculateInitialPaymentDate(DateTime registrationDate, int? dayOfMonth = null)
        {
            int paymentDay = dayOfMonth ?? registrationDate.Day;

            // თვის ბოლო დღის შემოწმება
            int daysInMonth = DateTime.DaysInMonth(registrationDate.Year, registrationDate.Month);
            paymentDay = Math.Min(paymentDay, daysInMonth);

            // თუ რეგისტრაციის დღე უკვე გავიდა, მომდევნო თვიდან
            if (registrationDate.Day > paymentDay)
            {
                var nextMonth = registrationDate.AddMonths(1);
                int daysInNextMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
                paymentDay = Math.Min(paymentDay, daysInNextMonth);
                return new DateTime(nextMonth.Year, nextMonth.Month, paymentDay);
            }

            return new DateTime(registrationDate.Year, registrationDate.Month, paymentDay);
        }

        /// <summary>
        /// მოსწავლის ჯგუფში დამატებისას გადახდის თარიღის დაყენება
        /// </summary>
        public void SetInitialPaymentDate(int studentId, int groupId, DateTime registrationDate, int? preferredPaymentDay = null)
        {
            var paymentDate = CalculateInitialPaymentDate(registrationDate, preferredPaymentDay);
            _studentGroupRepo.UpdatePaymentDate(studentId, groupId, paymentDate);
        }

        /// <summary>
        /// მოსწავლის ქვეჯგუფში დამატებისას გადახდის თარიღის დაყენება
        /// </summary>
        public void SetInitialPaymentDateForSubGroup(int studentId, int groupId, int subGroupId, DateTime registrationDate, int? preferredPaymentDay = null)
        {
            var paymentDate = CalculateInitialPaymentDate(registrationDate, preferredPaymentDay);
            _studentSubGroupRepo.UpdatePaymentDate(studentId, groupId, subGroupId, paymentDate);
        }

        #endregion

        #region ==================== გადახდის შემდეგ თარიღის განახლება ====================

        /// <summary>
        /// სრული გადახდის შემდეგ შემდეგი თარიღის გამოთვლა
        /// </summary>
        /// <param name="currentPaymentDate">მიმდინარე გადახდის თარიღი</param>
        /// <returns>შემდეგი გადახდის თარიღი (+1 თვე)</returns>
        public DateTime CalculateNextPaymentDateAfterFullPayment(DateTime currentPaymentDate)
        {
            var nextDate = currentPaymentDate.AddMonths(1);

            // თვის ბოლო დღის კორექცია (მაგ: 31 იანვარი -> 28 თებერვალი)
            int originalDay = currentPaymentDate.Day;
            int daysInNextMonth = DateTime.DaysInMonth(nextDate.Year, nextDate.Month);
            int correctedDay = Math.Min(originalDay, daysInNextMonth);

            return new DateTime(nextDate.Year, nextDate.Month, correctedDay);
        }

        /// <summary>
        /// სრული გადახდის შემდეგ თარიღის განახლება ბაზაში
        /// </summary>
        public void UpdatePaymentDateAfterFullPayment(int studentId, int groupId, DateTime currentPaymentDate)
        {
            var nextPaymentDate = CalculateNextPaymentDateAfterFullPayment(currentPaymentDate);
            _studentGroupRepo.UpdatePaymentDate(studentId, groupId, nextPaymentDate);
        }

        /// <summary>
        /// სრული გადახდის შემდეგ ქვეჯგუფის თარიღის განახლება
        /// </summary>
        public void UpdateSubGroupPaymentDateAfterFullPayment(int studentId, int groupId, int subGroupId, DateTime currentPaymentDate)
        {
            var nextPaymentDate = CalculateNextPaymentDateAfterFullPayment(currentPaymentDate);
            _studentSubGroupRepo.UpdatePaymentDate(studentId, groupId, subGroupId, nextPaymentDate);
        }

        #endregion

        #region ==================== ზედმეტი ჩარიცხვის დამუშავება ====================

        /// <summary>
        /// ზედმეტი ჩარიცხვისას გადახდის თარიღის გამოთვლა
        /// </summary>
        /// <param name="currentPaymentDate">მიმდინარე გადახდის თარიღი</param>
        /// <param name="excessAmount">ზედმეტი თანხა</param>
        /// <param name="monthlyFee">თვიური საფასური</param>
        /// <returns>ახალი გადახდის თარიღი</returns>
        public DateTime CalculatePaymentDateWithExcessCredit(DateTime currentPaymentDate, decimal excessAmount, decimal monthlyFee)
        {
            if (monthlyFee <= 0) return currentPaymentDate;

            // რამდენი თვე ფარავს ზედმეტი თანხა
            int monthsCovered = (int)Math.Floor(excessAmount / monthlyFee);

            if (monthsCovered > 0)
            {
                // გადახდის თარიღი გადაინაცვლებს წინ
                var newDate = currentPaymentDate.AddMonths(monthsCovered);

                // თვის ბოლო დღის კორექცია
                int originalDay = currentPaymentDate.Day;
                int daysInMonth = DateTime.DaysInMonth(newDate.Year, newDate.Month);
                int correctedDay = Math.Min(originalDay, daysInMonth);

                return new DateTime(newDate.Year, newDate.Month, correctedDay);
            }

            return currentPaymentDate;
        }

        /// <summary>
        /// ზედმეტი ჩარიცხვის დამუშავება - თარიღის განახლება და ნაშთის დაბრუნება
        /// </summary>
        /// <param name="studentId">მოსწავლის ID</param>
        /// <param name="groupId">ჯგუფის ID</param>
        /// <param name="currentPaymentDate">მიმდინარე გადახდის თარიღი</param>
        /// <param name="totalAmount">მთლიანი ჩარიცხული თანხა</param>
        /// <param name="monthlyFee">თვიური საფასური</param>
        /// <returns>დარჩენილი ნაშთი (ბალანსზე დასამატებელი)</returns>
        public decimal ProcessExcessPayment(int studentId, int groupId, DateTime currentPaymentDate, decimal totalAmount, decimal monthlyFee)
        {
            if (monthlyFee <= 0) return totalAmount;

            // რამდენი სრული თვე ფარავს
            int monthsCovered = (int)Math.Floor(totalAmount / monthlyFee);
            decimal remainder = totalAmount % monthlyFee;

            if (monthsCovered > 0)
            {
                var newPaymentDate = CalculatePaymentDateWithExcessCredit(currentPaymentDate, totalAmount, monthlyFee);
                _studentGroupRepo.UpdatePaymentDate(studentId, groupId, newPaymentDate);
            }

            return remainder;
        }

        #endregion

        #region ==================== თვის დასაწყისის პროცესი ====================

        /// <summary>
        /// თვის დასაწყისში ყველა მოსწავლის გადახდის სტატუსის შემოწმება და განახლება
        /// </summary>
        public void ProcessMonthlyPaymentReset()
        {
            var today = DateTime.Today;

            // ყველა აქტიური StudentGroup-ის მიღება რომელთაც გადახდის თარიღი გავიდა
            var overdueGroups = _studentGroupRepo.GetOverduePayments(today);

            foreach (var sg in overdueGroups)
            {
                // თუ გადახდის თარიღი გავიდა და არ არის გადახდილი
                if (sg.DateOfPayment.HasValue && sg.DateOfPayment.Value < today)
                {
                    // შემოწმება - გადახდილია თუ არა ამ პერიოდისთვის
                    bool isPaid = _paymentRepo.IsGroupPaidForPeriod(sg.StudentId, sg.GroupId, sg.DateOfPayment.Value);

                    if (!isPaid)
                    {
                        // სტატუსის განახლება "Overdue"-ზე
                        _studentGroupRepo.UpdatePaymentStatus(sg.StudentId, sg.GroupId, "Overdue");
                    }
                }
            }
        }

        /// <summary>
        /// კონკრეტული მოსწავლის გადახდის სტატუსის შემოწმება
        /// </summary>
        public PaymentDateStatus GetPaymentStatus(int studentId, int groupId)
        {
            var sg = _studentGroupRepo.GetByStudentAndGroup(studentId, groupId);
            if (sg == null || !sg.DateOfPayment.HasValue)
            {
                return new PaymentDateStatus
                {
                    Status = PaymentDateStatusType.Unknown,
                    Message = "გადახდის თარიღი არ არის დაყენებული"
                };
            }

            var today = DateTime.Today;
            var paymentDate = sg.DateOfPayment.Value;

            // გადახდილია თუ არა ამ პერიოდისთვის
            bool isPaid = _paymentRepo.IsGroupPaidForPeriod(studentId, groupId, paymentDate);

            if (isPaid)
            {
                return new PaymentDateStatus
                {
                    Status = PaymentDateStatusType.Paid,
                    PaymentDate = paymentDate,
                    NextPaymentDate = CalculateNextPaymentDateAfterFullPayment(paymentDate),
                    Message = "გადახდილია"
                };
            }

            int daysUntilPayment = (paymentDate - today).Days;

            if (daysUntilPayment > 7)
            {
                return new PaymentDateStatus
                {
                    Status = PaymentDateStatusType.Normal,
                    PaymentDate = paymentDate,
                    DaysRemaining = daysUntilPayment,
                    Message = $"გადახდამდე დარჩა {daysUntilPayment} დღე"
                };
            }
            else if (daysUntilPayment > 0)
            {
                return new PaymentDateStatus
                {
                    Status = PaymentDateStatusType.Approaching,
                    PaymentDate = paymentDate,
                    DaysRemaining = daysUntilPayment,
                    Message = $"გადახდამდე დარჩა {daysUntilPayment} დღე!"
                };
            }
            else if (daysUntilPayment == 0)
            {
                return new PaymentDateStatus
                {
                    Status = PaymentDateStatusType.DueToday,
                    PaymentDate = paymentDate,
                    DaysRemaining = 0,
                    Message = "გადახდის დღეა!"
                };
            }
            else
            {
                return new PaymentDateStatus
                {
                    Status = PaymentDateStatusType.Overdue,
                    PaymentDate = paymentDate,
                    DaysOverdue = Math.Abs(daysUntilPayment),
                    Message = $"გადახდა ვადაგადაცილებულია {Math.Abs(daysUntilPayment)} დღით!"
                };
            }
        }

        #endregion

        #region ==================== გადახდის თარიღის ხელით ცვლილება ====================

        /// <summary>
        /// ყველა აქტიური მოსწავლისთვის შემდეგი გადახდის თარიღის განახლება
        /// გამოიყენება სწავლის დაწყების თარიღის დაყენებისას
        /// </summary>
        /// <param name="studyStartDate">სწავლის დაწყების თარიღი</param>
        public void UpdateNextPaymentDate(DateTime paymentDate)
        {
            // ყველა აქტიური StudentGroup-ისთვის განახლება
            var allActiveGroups = _studentGroupRepo.GetOverduePayments(DateTime.MaxValue); // მიიღებს ყველას
                                                                                           // ან შეგვიძლია დავამატოთ GetAllActive მეთოდი

            // ამჯერად მარტივი მიდგომა - ბაზაში პირდაპირი განახლება
            UpdateAllActivePaymentDates(paymentDate);
        }

        /// <summary>
        /// ყველა აქტიური ჩანაწერის გადახდის თარიღის განახლება
        /// </summary>
        private void UpdateAllActivePaymentDates(DateTime newPaymentDate)
        {
            // StudentGroups
            var activeGroups = _studentGroupRepo.GetOverduePayments(DateTime.MaxValue);
            foreach (var sg in activeGroups)
            {
                _studentGroupRepo.UpdatePaymentDate(sg.StudentId, sg.GroupId, newPaymentDate);
            }
        }

        /// <summary>
        /// გადახდის თარიღის ხელით შეცვლა (ადმინისტრატორის მიერ)
        /// </summary>
        public bool ManuallySetPaymentDate(int studentId, int groupId, DateTime newPaymentDate)
        {
            return _studentGroupRepo.UpdatePaymentDate(studentId, groupId, newPaymentDate);
        }

        /// <summary>
        /// გადახდის დღის (თვის რიცხვის) შეცვლა ყველა მომავალი გადახდისთვის
        /// </summary>
        public bool ChangePaymentDayOfMonth(int studentId, int groupId, int newDayOfMonth)
        {
            var sg = _studentGroupRepo.GetByStudentAndGroup(studentId, groupId);
            if (sg == null || !sg.DateOfPayment.HasValue) return false;

            var currentDate = sg.DateOfPayment.Value;
            int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            int correctedDay = Math.Min(newDayOfMonth, daysInMonth);

            var newDate = new DateTime(currentDate.Year, currentDate.Month, correctedDay);

            // თუ ახალი დღე უკვე გავიდა ამ თვეში, მომდევნო თვიდან
            if (newDate < DateTime.Today)
            {
                newDate = newDate.AddMonths(1);
                daysInMonth = DateTime.DaysInMonth(newDate.Year, newDate.Month);
                correctedDay = Math.Min(newDayOfMonth, daysInMonth);
                newDate = new DateTime(newDate.Year, newDate.Month, correctedDay);
            }

            return _studentGroupRepo.UpdatePaymentDate(studentId, groupId, newDate);
        }

        #endregion

        #region ==================== დამხმარე მეთოდები ====================

        /// <summary>
        /// მომდევნო გადახდის თარიღის მიღება
        /// </summary>
        public DateTime? GetNextPaymentDate(int studentId, int groupId)
        {
            var sg = _studentGroupRepo.GetByStudentAndGroup(studentId, groupId);
            return sg?.DateOfPayment;
        }

        /// <summary>
        /// მოსწავლის ყველა ჯგუფის გადახდის თარიღების მიღება
        /// </summary>
        public List<StudentGroupPaymentInfo> GetAllPaymentDates(int studentId)
        {
            var groups = _studentGroupRepo.GetActiveByStudentId(studentId);
            var result = new List<StudentGroupPaymentInfo>();

            foreach (var sg in groups)
            {
                var status = GetPaymentStatus(studentId, sg.GroupId);
                result.Add(new StudentGroupPaymentInfo
                {
                    StudentId = studentId,
                    GroupId = sg.GroupId,
                    PaymentDate = sg.DateOfPayment,
                    Status = status
                });
            }

            return result;
        }

        #endregion
    }

    #region ==================== დამხმარე კლასები ====================

    /// <summary>
    /// გადახდის თარიღის სტატუსის ტიპი
    /// </summary>
    public enum PaymentDateStatusType
    {
        Unknown,      // უცნობი
        Normal,       // ნორმალური (7+ დღე დარჩენილი)
        Approaching,  // უახლოვდება (1-7 დღე)
        DueToday,     // დღეს არის გადახდა
        Overdue,      // ვადაგადაცილებული
        Paid          // გადახდილი
    }

    /// <summary>
    /// გადახდის თარიღის სტატუსი
    /// </summary>
    public class PaymentDateStatus
    {
        public PaymentDateStatusType Status { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public int? DaysRemaining { get; set; }
        public int? DaysOverdue { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// მოსწავლე-ჯგუფის გადახდის ინფორმაცია
    /// </summary>
    public class StudentGroupPaymentInfo
    {
        public int StudentId { get; set; }
        public int GroupId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public PaymentDateStatus Status { get; set; }
    }

    #endregion
}




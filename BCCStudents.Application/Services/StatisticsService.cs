using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Domain.Entities;
using System.Globalization;

using BCCStudents.Application.Interfaces;

namespace BCCStudents.Application.Services {
    public class StatisticsService : IStatisticsService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IStudentGroupRepository _studentGroupRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ISubGroupRepository _subGroupRepository;
        private readonly IStudentSubGroupRepository _studentSubGroupRepository;

        public StatisticsService(
            IStudentRepository studentRepository,
            IGroupRepository groupRepository,
            IStudentGroupRepository studentGroupRepository,
            IPaymentRepository paymentRepository,
            ISubGroupRepository subGroupRepository,
            IStudentSubGroupRepository studentSubGroupRepository)
        {
            _studentRepository = studentRepository;
            _groupRepository = groupRepository;
            _studentGroupRepository = studentGroupRepository;
            _paymentRepository = paymentRepository;
            _subGroupRepository = subGroupRepository;
            _studentSubGroupRepository = studentSubGroupRepository;
        }

        #region 1. სტუდენტების რაოდენობა ჯგუფებში (აქტიური, არააქტიური)

        public DataTable GetStudentCountByGroup(int? groupId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var groups = _groupRepository.GetAllGroups();
            var dt = new DataTable();
            dt.Columns.Add("ჯგუფი", typeof(string));
            dt.Columns.Add("აქტიური", typeof(int));
            dt.Columns.Add("არააქტიური", typeof(int));
            dt.Columns.Add("სულ", typeof(int));

            foreach (var group in groups)
            {
                if (groupId.HasValue && group.Id != groupId.Value)
                    continue;

                var activeStudents = _studentGroupRepository.GetActiveByGroupId(group.Id);
                var allStudents = _studentGroupRepository.GetByGroupId(group.Id);
                var inactiveCount = allStudents.Count - activeStudents.Count;

                dt.Rows.Add(group.Name, activeStudents.Count, inactiveCount, allStudents.Count);
            }

            return dt;
        }

        #endregion

        #region 2. გადახდების სტატისტიკა

        public DataTable GetPaymentStatisticsByGroup(int? groupId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var groups = _groupRepository.GetAllGroups();
            var dt = new DataTable();
            dt.Columns.Add("ჯგუფი", typeof(string));
            dt.Columns.Add("გადახდილი", typeof(decimal));
            dt.Columns.Add("გადასახდელი", typeof(decimal));
            dt.Columns.Add("სულ", typeof(decimal));

            foreach (var group in groups)
            {
                if (groupId.HasValue && group.Id != groupId.Value)
                    continue;

                var studentGroups = _studentGroupRepository.GetByGroupId(group.Id);
                decimal paid = 0;
                decimal pending = 0;

                foreach (var sg in studentGroups)
                {
                    var payments = _paymentRepository.GetStudentPayments(sg.StudentId);
                    var groupPayments = payments.Where(p => 
                        (!startDate.HasValue || p.PaymentDate >= startDate.Value) &&
                        (!endDate.HasValue || p.PaymentDate <= endDate.Value)).ToList();

                    paid += groupPayments.Sum(p => p.Amount);

                    if (sg.PaymentStatus == "Pending" || sg.PaymentStatus == "Overdue")
                    {
                        pending += sg.Price;
                    }
                }

                dt.Rows.Add(group.Name, paid, pending, paid + pending);
            }

            return dt;
        }

        public (decimal TotalPaid, decimal TotalPending) GetTotalPaymentStatistics(int? groupId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var groups = _groupRepository.GetAllGroups();
            decimal totalPaid = 0;
            decimal totalPending = 0;

            foreach (var group in groups)
            {
                if (groupId.HasValue && group.Id != groupId.Value)
                    continue;

                var studentGroups = _studentGroupRepository.GetByGroupId(group.Id);

                foreach (var sg in studentGroups)
                {
                    var payments = _paymentRepository.GetStudentPayments(sg.StudentId);
                    var groupPayments = payments.Where(p => 
                        (!startDate.HasValue || p.PaymentDate >= startDate.Value) &&
                        (!endDate.HasValue || p.PaymentDate <= endDate.Value)).ToList();

                    totalPaid += groupPayments.Sum(p => p.Amount);

                    if (sg.PaymentStatus == "Pending" || sg.PaymentStatus == "Overdue")
                    {
                        totalPending += sg.Price;
                    }
                }
            }

            return (totalPaid, totalPending);
        }

        public DataTable GetPaymentStatisticsByTime(DateTime? startDate = null, DateTime? endDate = null)
        {
            var dt = new DataTable();
            dt.Columns.Add("თარიღი", typeof(string));
            dt.Columns.Add("გადახდილი", typeof(decimal));
            dt.Columns.Add("გადასახდელი", typeof(decimal));

            var payments = _paymentRepository.GetPaymentSummaries();
            var filteredPayments = payments.Where(p =>
                (!startDate.HasValue || p.PaymentDate >= startDate.Value) &&
                (!endDate.HasValue || p.PaymentDate <= endDate.Value))
                .GroupBy(p => p.PaymentDate.Date)
                .OrderBy(g => g.Key)
                .ToList();

            foreach (var group in filteredPayments)
            {
                var paid = group.Where(p => p.IsSuccessful).Sum(p => p.Amount);
                var pending = group.Where(p => !p.IsSuccessful).Sum(p => p.Amount);
                dt.Rows.Add(group.Key.ToString("yyyy-MM-dd"), paid, pending);
            }

            return dt;
        }

        #endregion

        #region 3. გადახდილი და გადასახდელი მოსწავლეების სტატისტიკა

        public DataTable GetStudentsPaymentStatus(int? groupId = null)
        {
            var dt = new DataTable();
            dt.Columns.Add("სტუდენტი", typeof(string));
            dt.Columns.Add("ჯგუფი", typeof(string));
            dt.Columns.Add("სტატუსი", typeof(string));
            dt.Columns.Add("თანხა", typeof(decimal));
            dt.Columns.Add("გადახდილი", typeof(decimal));
            dt.Columns.Add("გადასახდელი", typeof(decimal));

            var groups = groupId.HasValue 
                ? new List<Group> { _groupRepository.GetGroupById(groupId.Value) }
                : _groupRepository.GetAllGroups();

            foreach (var group in groups)
            {
                var studentGroups = _studentGroupRepository.GetByGroupId(group.Id);

                foreach (var sg in studentGroups)
                {
                    var student = _studentRepository.GetStudentById(sg.StudentId);
                    if (student == null) continue;

                    var payments = _paymentRepository.GetStudentPayments(sg.StudentId);
                    var paid = payments.Sum(p => p.Amount);
                    var pending = sg.Price - paid;

                    dt.Rows.Add(
                        $"{student.FirstName} {student.LastName}",
                        group.Name,
                        sg.PaymentStatus,
                        sg.Price,
                        paid,
                        pending > 0 ? pending : 0m
                    );
                }
            }

            return dt;
        }

        public (int PaidCount, int PendingCount) GetStudentsPaymentStatusCount(int? groupId = null)
        {
            var groups = groupId.HasValue 
                ? new List<Group> { _groupRepository.GetGroupById(groupId.Value) }
                : _groupRepository.GetAllGroups();

            int paidCount = 0;
            int pendingCount = 0;

            foreach (var group in groups)
            {
                var studentGroups = _studentGroupRepository.GetByGroupId(group.Id);

                foreach (var sg in studentGroups)
                {
                    var payments = _paymentRepository.GetStudentPayments(sg.StudentId);
                    var paid = payments.Sum(p => p.Amount);
                    var pending = sg.Price - paid;

                    if (paid >= sg.Price)
                        paidCount++;
                    else
                        pendingCount++;
                }
            }

            return (paidCount, pendingCount);
        }

        #endregion

        #region 4. გენდერული სტატისტიკა

        public DataTable GetGenderStatistics(int? groupId = null)
        {
            var dt = new DataTable();
            dt.Columns.Add("გენდერი", typeof(string));
            dt.Columns.Add("რაოდენობა", typeof(int));
            dt.Columns.Add("პროცენტი", typeof(double));

            var students = _studentRepository.GetAllStudents();
            var filteredStudents = students;

            if (groupId.HasValue)
            {
                var studentIds = _studentGroupRepository.GetByGroupId(groupId.Value)
                    .Select(sg => sg.StudentId).ToList();
                filteredStudents = students.Where(s => studentIds.Contains(s.Id)).ToList();
            }

            // გენდერის განსაზღვრა სახელის მიხედვით (სავარაუდო)
            var maleNames = new[] { "გიორგი", "დავით", "ნიკა", "ლუკა", "ალექსანდრე", "ილია", "გიგა", "თორნიკე" };
            var femaleNames = new[] { "ანა", "მარიამ", "ნინო", "თამარ", "ნათია", "სალომე", "თეა", "მანანა" };

            int maleCount = 0;
            int femaleCount = 0;
            int unknownCount = 0;

            foreach (var student in filteredStudents)
            {
                var firstName = (student.FirstName?.Trim() ?? "").ToLowerInvariant();
                if (maleNames.Any(n => firstName.Contains(n.ToLowerInvariant())))
                    maleCount++;
                else if (femaleNames.Any(n => firstName.Contains(n.ToLowerInvariant())))
                    femaleCount++;
                else
                    unknownCount++;
            }

            int total = filteredStudents.Count;
            if (total > 0)
            {
                dt.Rows.Add("კაცი", maleCount, Math.Round((double)maleCount / total * 100, 2));
                dt.Rows.Add("ქალი", femaleCount, Math.Round((double)femaleCount / total * 100, 2));
                dt.Rows.Add("განუსაზღვრელი", unknownCount, Math.Round((double)unknownCount / total * 100, 2));
            }

            return dt;
        }

        #endregion

        #region 5. ასაკის სტატისტიკა

        public DataTable GetAgeStatistics(int? groupId = null)
        {
            var dt = new DataTable();
            dt.Columns.Add("ასაკის დიაპაზონი", typeof(string));
            dt.Columns.Add("რაოდენობა", typeof(int));
            dt.Columns.Add("პროცენტი", typeof(double));

            var students = _studentRepository.GetAllStudents();
            var filteredStudents = students;

            if (groupId.HasValue)
            {
                var studentIds = _studentGroupRepository.GetByGroupId(groupId.Value)
                    .Select(sg => sg.StudentId).ToList();
                filteredStudents = students.Where(s => studentIds.Contains(s.Id)).ToList();
            }

            var ageGroups = new Dictionary<string, (int min, int max)>
            {
                { "3-5 წლის", (3, 5) },
                { "6-8 წლის", (6, 8) },
                { "9-12 წლის", (9, 12) },
                { "13-15 წლის", (13, 15) },
                { "16-18 წლის", (16, 18) },
                { "18+ წლის", (18, 100) }
            };

            var ageCounts = new Dictionary<string, int>();
            foreach (var group in ageGroups.Keys)
                ageCounts[group] = 0;

            int unknownCount = 0;

            foreach (var student in filteredStudents)
            {
                if (student.Age > 0)
                {
                    bool found = false;
                    foreach (var kvp in ageGroups)
                    {
                        if (student.Age >= kvp.Value.min && student.Age <= kvp.Value.max)
                        {
                            ageCounts[kvp.Key]++;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        unknownCount++;
                }
                else
                {
                    unknownCount++;
                }
            }

            int total = filteredStudents.Count;
            if (total > 0)
            {
                foreach (var kvp in ageCounts)
                {
                    dt.Rows.Add(kvp.Key, kvp.Value, Math.Round((double)kvp.Value / total * 100, 2));
                }
                if (unknownCount > 0)
                {
                    dt.Rows.Add("განუსაზღვრელი", unknownCount, Math.Round((double)unknownCount / total * 100, 2));
                }
            }

            return dt;
        }

        #endregion

        #region 6. სახელების სტატისტიკა

        public DataTable GetNameStatistics(int? groupId = null, int topCount = 10)
        {
            var dt = new DataTable();
            dt.Columns.Add("სახელი", typeof(string));
            dt.Columns.Add("რაოდენობა", typeof(int));
            dt.Columns.Add("პროცენტი", typeof(double));

            var students = _studentRepository.GetAllStudents();
            var filteredStudents = students;

            if (groupId.HasValue)
            {
                var studentIds = _studentGroupRepository.GetByGroupId(groupId.Value)
                    .Select(sg => sg.StudentId).ToList();
                filteredStudents = students.Where(s => studentIds.Contains(s.Id)).ToList();
            }

            var firstNameCounts = filteredStudents
                .Where(s => !string.IsNullOrWhiteSpace(s.FirstName))
                .GroupBy(s => s.FirstName.Trim())
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(topCount)
                .ToList();

            int total = filteredStudents.Count;
            if (total > 0)
            {
                foreach (var item in firstNameCounts)
                {
                    dt.Rows.Add(item.Name, item.Count, Math.Round((double)item.Count / total * 100, 2));
                }
            }

            return dt;
        }

        #endregion

        #region დამატებითი სტატისტიკა

        // 7. რეგისტრაციის სტატისტიკა
        public DataTable GetRegistrationStatistics(DateTime? startDate = null, DateTime? endDate = null)
        {
            var dt = new DataTable();
            dt.Columns.Add("თვე", typeof(string));
            dt.Columns.Add("რაოდენობა", typeof(int));

            var students = _studentRepository.GetAllStudents();
            var filteredStudents = students.Where(s =>
                (!startDate.HasValue || s.RegistrationDate >= startDate.Value) &&
                (!endDate.HasValue || s.RegistrationDate <= endDate.Value))
                .GroupBy(s => new { s.RegistrationDate.Year, s.RegistrationDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .ToList();

            foreach (var group in filteredStudents)
            {
                var monthName = new DateTime(group.Key.Year, group.Key.Month, 1)
                    .ToString("yyyy-MM", CultureInfo.InvariantCulture);
                dt.Rows.Add(monthName, group.Count());
            }

            return dt;
        }

        // 8. ჯგუფების სიმჭიდროვე
        public DataTable GetGroupDensityStatistics()
        {
            var dt = new DataTable();
            dt.Columns.Add("ჯგუფი", typeof(string));
            dt.Columns.Add("მოსწავლეების რაოდენობა", typeof(int));
            dt.Columns.Add("საშუალო", typeof(double));

            var groups = _groupRepository.GetAllGroups();
            var allCounts = new List<int>();

            foreach (var group in groups)
            {
                var count = _studentGroupRepository.GetActiveByGroupId(group.Id).Count;
                allCounts.Add(count);
                dt.Rows.Add(group.Name, count, 0);
            }

            double average = allCounts.Any() ? allCounts.Average() : 0;

            // საშუალოს განახლება
            foreach (DataRow row in dt.Rows)
            {
                row["საშუალო"] = Math.Round(average, 2);
            }

            return dt;
        }

        // 9. საშუალო გადახდა
        public DataTable GetAveragePaymentStatistics(int? groupId = null)
        {
            var dt = new DataTable();
            dt.Columns.Add("ჯგუფი", typeof(string));
            dt.Columns.Add("საშუალო გადახდილი", typeof(decimal));
            dt.Columns.Add("საშუალო გადასახდელი", typeof(decimal));

            var groups = groupId.HasValue 
                ? new List<Group> { _groupRepository.GetGroupById(groupId.Value) }
                : _groupRepository.GetAllGroups();

            foreach (var group in groups)
            {
                var studentGroups = _studentGroupRepository.GetByGroupId(group.Id);
                var paidList = new List<decimal>();
                var pendingList = new List<decimal>();

                foreach (var sg in studentGroups)
                {
                    var payments = _paymentRepository.GetStudentPayments(sg.StudentId);
                    var paid = payments.Sum(p => p.Amount);
                    var pending = sg.Price - paid;

                    paidList.Add(paid);
                    pendingList.Add(pending > 0 ? pending : 0m);
                }

                decimal avgPaid = paidList.Any() ? paidList.Average() : 0;
                decimal avgPending = pendingList.Any() ? pendingList.Average() : 0;

                dt.Rows.Add(group.Name, Math.Round(avgPaid, 2), Math.Round(avgPending, 2));
            }

            return dt;
        }

        // 10. ფასდაკლებების სტატისტიკა
        public DataTable GetDiscountStatistics(int? groupId = null)
        {
            var dt = new DataTable();
            dt.Columns.Add("ჯგუფი", typeof(string));
            dt.Columns.Add("ფასდაკლებების რაოდენობა", typeof(int));
            dt.Columns.Add("საერთო ფასდაკლება", typeof(decimal));
            dt.Columns.Add("საშუალო ფასდაკლება %", typeof(double));

            var groups = groupId.HasValue 
                ? new List<Group> { _groupRepository.GetGroupById(groupId.Value) }
                : _groupRepository.GetAllGroups();

            foreach (var group in groups)
            {
                var studentGroups = _studentGroupRepository.GetByGroupId(group.Id);
                int discountCount = 0;
                decimal totalDiscount = 0;
                var discountPercentages = new List<double>();

                foreach (var sg in studentGroups)
                {
                    if (sg.Discount > 0)
                    {
                        discountCount++;
                        totalDiscount += (decimal)sg.Discount;
                        if (sg.Price > 0)
                        {
                            discountPercentages.Add((sg.Discount / (double)sg.Price * 100));
                        }
                    }
                }

                double avgDiscount = discountPercentages.Any() ? discountPercentages.Average() : 0;

                dt.Rows.Add(group.Name, discountCount, Math.Round(totalDiscount, 2), Math.Round(avgDiscount, 2));
            }

            return dt;
        }

        // 11. მოსწავლეების განაწილება ქვეჯგუფებში
        public DataTable GetSubGroupDistributionStatistics(int? groupId = null)
        {
            var dt = new DataTable();
            dt.Columns.Add("ქვეჯგუფი", typeof(string));
            dt.Columns.Add("მოსწავლეების რაოდენობა", typeof(int));

            var subGroups = _subGroupRepository.GetAllSubGroups();
            var filteredSubGroups = subGroups;

            if (groupId.HasValue)
            {
                filteredSubGroups = subGroups.Where(sg => sg.GroupId == groupId.Value).ToList();
            }

            foreach (var subGroup in filteredSubGroups)
            {
                var count = _studentSubGroupRepository.GetActiveBySubGroupId(subGroup.Id).Count;
                dt.Rows.Add(subGroup.Name, count);
            }

            return dt;
        }

        // 12. გადახდების ტენდენცია
        public DataTable GetPaymentTrendStatistics(DateTime? startDate = null, DateTime? endDate = null)
        {
            var dt = new DataTable();
            dt.Columns.Add("თარიღი", typeof(string));
            dt.Columns.Add("გადახდილი", typeof(decimal));
            dt.Columns.Add("ტენდენცია", typeof(string));

            var payments = _paymentRepository.GetPaymentSummaries();
            var filteredPayments = payments.Where(p =>
                (!startDate.HasValue || p.PaymentDate >= startDate.Value) &&
                (!endDate.HasValue || p.PaymentDate <= endDate.Value))
                .GroupBy(p => p.PaymentDate.Date)
                .OrderBy(g => g.Key)
                .ToList();

            decimal previousAmount = 0;
            foreach (var group in filteredPayments)
            {
                var paid = group.Where(p => p.IsSuccessful).Sum(p => p.Amount);
                string trend = "";
                if (previousAmount > 0)
                {
                    if (paid > previousAmount)
                        trend = "↑ გაზრდა";
                    else if (paid < previousAmount)
                        trend = "↓ შემცირება";
                    else
                        trend = "→ უცვლელი";
                }
                dt.Rows.Add(group.Key.ToString("yyyy-MM-dd"), paid, trend);
                previousAmount = paid;
            }

            return dt;
        }

        // 13. ჯგუფების ეფექტურობა
        public DataTable GetGroupEfficiencyStatistics()
        {
            var dt = new DataTable();
            dt.Columns.Add("ჯგუფი", typeof(string));
            dt.Columns.Add("გადახდილი", typeof(decimal));
            dt.Columns.Add("გადასახდელი", typeof(decimal));
            dt.Columns.Add("ეფექტურობა %", typeof(double));

            var groups = _groupRepository.GetAllGroups();

            foreach (var group in groups)
            {
                var studentGroups = _studentGroupRepository.GetByGroupId(group.Id);
                decimal totalPaid = 0;
                decimal totalPending = 0;

                foreach (var sg in studentGroups)
                {
                    var payments = _paymentRepository.GetStudentPayments(sg.StudentId);
                    totalPaid += payments.Sum(p => p.Amount);
                    var pending = sg.Price - payments.Sum(p => p.Amount);
                    if (pending > 0)
                        totalPending += pending;
                }

                decimal total = totalPaid + totalPending;
                double efficiency = total > 0 ? (double)(totalPaid / total * 100) : 0;

                dt.Rows.Add(group.Name, Math.Round(totalPaid, 2), Math.Round(totalPending, 2), Math.Round(efficiency, 2));
            }

            return dt;
        }

        #endregion
    }
}




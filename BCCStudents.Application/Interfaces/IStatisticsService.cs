using System;
using System.Data;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// სტატისტიკის სერვისის ინტერფეისი
    /// </summary>
    public interface IStatisticsService
    {
        // სტუდენტების რაოდენობა ჯგუფებში
        DataTable GetStudentCountByGroup(int? groupId = null, DateTime? startDate = null, DateTime? endDate = null);

        // გადახდების სტატისტიკა
        DataTable GetPaymentStatisticsByGroup(int? groupId = null, DateTime? startDate = null, DateTime? endDate = null);
        (decimal TotalPaid, decimal TotalPending) GetTotalPaymentStatistics(int? groupId = null, DateTime? startDate = null, DateTime? endDate = null);
        DataTable GetPaymentStatisticsByTime(DateTime? startDate = null, DateTime? endDate = null);

        // გადახდილი და გადასახდელი მოსწავლეების სტატისტიკა
        DataTable GetStudentsPaymentStatus(int? groupId = null);
        (int PaidCount, int PendingCount) GetStudentsPaymentStatusCount(int? groupId = null);

        // გენდერული სტატისტიკა
        DataTable GetGenderStatistics(int? groupId = null);

        // ასაკის სტატისტიკა
        DataTable GetAgeStatistics(int? groupId = null);

        // სახელების სტატისტიკა
        DataTable GetNameStatistics(int? groupId = null, int topCount = 10);

        // დამატებითი სტატისტიკა
        DataTable GetRegistrationStatistics(DateTime? startDate = null, DateTime? endDate = null);
        DataTable GetGroupDensityStatistics();
        DataTable GetAveragePaymentStatistics(int? groupId = null);
        DataTable GetDiscountStatistics(int? groupId = null);
        DataTable GetSubGroupDistributionStatistics(int? groupId = null);
        DataTable GetPaymentTrendStatistics(DateTime? startDate = null, DateTime? endDate = null);
        DataTable GetGroupEfficiencyStatistics();
    }
}


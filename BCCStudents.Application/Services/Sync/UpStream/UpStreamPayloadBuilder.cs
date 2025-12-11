using System;
using System.Collections.Generic;
using BCCStudents.Domain.Entities;
using BCCStudents;
using BCCStudents.Application.Interfaces;

namespace BCCStudents.Application.Services.Sync.UpStream
{
    /// <summary>
    /// áƒªáƒ®áƒ áƒ˜áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ›áƒ˜áƒ®áƒ”áƒ“áƒ•áƒ˜áƒ— áƒžáƒ”áƒ˜áƒšáƒáƒáƒ“áƒ”áƒ‘áƒ˜áƒ¡ áƒáƒ’áƒ”áƒ‘áƒ (Students, Groups, SubGroups, StudentGroups, StudentSubGroups).
    /// </summary>
    public class UpStreamPayloadBuilder : IUpStreamPayloadBuilder
    {
        private static readonly DateTime MinSqlDate = new DateTime(1970, 1, 1);

        public SyncChangePayload BuildStudentPayload(int studentId, SyncOperationType operation, Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));

            var data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["Id"] = studentId,
                ["FirstName"] = student.FirstName,
                ["LastName"] = student.LastName,
                ["Age"] = student.Age,
                ["ParentName"] = student.ParentName,
                ["PhoneNumber"] = student.PhoneNumber,
                ["Id_Numb"] = student.Id_Numb,
                ["Address"] = student.Address,
                ["RegistrationDate"] = EnsureDate(student.RegistrationDate),
                ["StudentCode"] = student.StudentCode,
                ["Info"] = student.Info,
                ["User_Id"] = student.User_Id,
                ["Balance"] = student.Balance,
               // ["Discount"] = student.Discount,
                ["Status"] = student.Status, // Students.Status áƒáƒ áƒ˜áƒ¡ bool
               // ["PaymentStatus"] = student.PaymentStatus,
               // ["DateOfPayment"] = student.DateOfPayment,
                ["UpdatedAt"] = EnsureUpdatedAt(student.UpdatedAt),
                ["IdCardPath"] = student.IdCardPath,
                ["AdditionalDocsPath"] = student.AdditionalDocsPath
            };

            return new SyncChangePayload("Students", operation, data, studentId);
        }

        public SyncChangePayload BuildGroupPayload(int groupId, SyncOperationType operation, Group group, int? studentCount = null)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));

            var data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["Id"] = groupId,
                ["Name"] = group.Name,
                ["Price"] = group.Price,
                ["Teacher"] = group.Teacher,
                ["ContractTemplatePath"] = group.ContractTemplatePath,
                ["Status"] = group.Status,
                ["StudentCount"] = studentCount ?? group.StudentCount,
                ["UpdatedAt"] = EnsureUpdatedAt(group.UpdatedAt)
            };

            return new SyncChangePayload("Groups", operation, data, groupId);
        }

        public SyncChangePayload BuildSubGroupPayload(int subGroupId, SyncOperationType operation, SubGroup subGroup)
        {
            if (subGroup == null) throw new ArgumentNullException(nameof(subGroup));

            var data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["Id"] = subGroupId,
                ["Name"] = subGroup.Name,
                ["GroupId"] = subGroup.GroupId,
                ["TuitionFee"] = subGroup.TuitionFee,
                ["StudentCount"] = subGroup.StudentCount,
                ["Status"] = subGroup.Status,
                ["UpdatedAt"] = EnsureUpdatedAt(subGroup.UpdatedAt)
            };

            return new SyncChangePayload("SubGroups", operation, data, subGroupId);
        }

        public SyncChangePayload BuildStudentGroupPayload(int recordId, SyncOperationType operation, StudentGroups studentGroup)
        {
            if (studentGroup == null) throw new ArgumentNullException(nameof(studentGroup));

            var data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["Id"] = recordId,
                ["StudentId"] = studentGroup.StudentId,
                ["GroupId"] = studentGroup.GroupId,
                ["Status"] = studentGroup.Status,
                ["PaymentStatus"] = studentGroup.PaymentStatus,
                ["DateOfPayment"] = studentGroup.DateOfPayment,
                ["Price"] = studentGroup.Price,
                ["Discount"] = studentGroup.Discount,
                ["UpdatedAt"] = EnsureUpdatedAt(studentGroup.UpdatedAt)
            };

            var keys = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["StudentId"] = studentGroup.StudentId,
                ["GroupId"] = studentGroup.GroupId
            };

            return new SyncChangePayload("StudentGroups", operation, data, recordId > 0 ? (int?)recordId : null, keys);
        }

        public SyncChangePayload BuildStudentSubGroupPayload(int recordId, SyncOperationType operation, StudentSubGroups studentSubGroup)
        {
            if (studentSubGroup == null) throw new ArgumentNullException(nameof(studentSubGroup));

            var data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["Id"] = recordId,
                ["StudentId"] = studentSubGroup.StudentId,
                ["GroupId"] = studentSubGroup.GroupId,
                ["SubGroupId"] = studentSubGroup.SubGroupId,
                ["Status"] = studentSubGroup.Status,
                ["PaymentStatus"] = studentSubGroup.PaymentStatus,
                ["DateOfPayment"] = studentSubGroup.DateOfPayment,
                ["Price"] = studentSubGroup.Price,
                ["Discount"] = studentSubGroup.Discount,
                ["UpdatedAt"] = EnsureUpdatedAt(studentSubGroup.UpdatedAt)
            };

            var keys = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["StudentId"] = studentSubGroup.StudentId,
                ["GroupId"] = studentSubGroup.GroupId,
                ["SubGroupId"] = studentSubGroup.SubGroupId
            };

            return new SyncChangePayload("StudentSubGroups", operation, data, recordId > 0 ? (int?)recordId : null, keys);
        }

        public SyncChangePayload BuildFailedPaymentPayload(int failedPaymentId, SyncOperationType operation, FailedPayment failedPayment)
        {
            if (failedPayment == null) throw new ArgumentNullException(nameof(failedPayment));

            var data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["Id"] = failedPaymentId,
                ["RowNumber"] = failedPayment.RowNumber,
                ["PaymentDate"] = EnsureDate(failedPayment.PaymentDate),
                ["Amount"] = failedPayment.Amount,
                ["PersonalId"] = failedPayment.PersonalId ?? (object)DBNull.Value,
                ["Description"] = failedPayment.Description ?? (object)DBNull.Value,
                ["Reason"] = failedPayment.Reason,
                ["CreatedAt"] = EnsureDate(failedPayment.CreatedAt)
            };

            return new SyncChangePayload("FailedPayments", operation, data, failedPaymentId);
        }

        public SyncChangePayload BuildImportedPaymentLogPayload(int importedPaymentLogId, SyncOperationType operation, ImportedPaymentLog importedPaymentLog)
        {
            if (importedPaymentLog == null) throw new ArgumentNullException(nameof(importedPaymentLog));

            var data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["Id"] = importedPaymentLogId,
                ["PaymentDate"] = EnsureDate(importedPaymentLog.PaymentDate),
                ["Amount"] = importedPaymentLog.Amount,
                ["PersonalId"] = importedPaymentLog.PersonalId ?? (object)DBNull.Value,
                ["Description"] = importedPaymentLog.Description ?? (object)DBNull.Value,
                ["ImportSource"] = importedPaymentLog.ImportSource ?? (object)DBNull.Value,
                ["CreatedAt"] = EnsureDate(importedPaymentLog.CreatedAt)
            };

            return new SyncChangePayload("ImportedPaymentsLog", operation, data, importedPaymentLogId);
        }

        private static DateTime EnsureDate(DateTime input)
        {
            if (input == default || input < MinSqlDate)
            {
                return DateTime.Now;
            }
            return input;
        }

        private static DateTime EnsureUpdatedAt(DateTime input)
        {
            // áƒ–áƒ£áƒ¡áƒ¢áƒáƒ“ áƒ˜áƒ¡ áƒ›áƒœáƒ˜áƒ¨áƒ•áƒœáƒ”áƒšáƒáƒ‘áƒ áƒ£áƒœáƒ“áƒ áƒ’áƒáƒ˜áƒ’áƒ–áƒáƒ•áƒœáƒáƒ¡, áƒ áƒáƒª áƒšáƒáƒ™áƒáƒšáƒ£áƒ  áƒ‘áƒáƒ–áƒáƒ¨áƒ˜ áƒ¬áƒ”áƒ áƒ˜áƒ
            // áƒ•áƒáƒ‘áƒ áƒ£áƒœáƒ”áƒ‘áƒ— áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ£áƒš input-áƒ¡ áƒ¨áƒ”áƒªáƒ•áƒšáƒ˜áƒ¡ áƒ’áƒáƒ áƒ”áƒ¨áƒ”
            return input;
        }
    }
}



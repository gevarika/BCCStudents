using BCCStudents.Domain.Entities;
using BCCStudents;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// UpStream payload-ის შემქმნელის ინტერფეისი
    /// </summary>
    public interface IUpStreamPayloadBuilder
    {
        /// <summary>
        /// ქმნის სტუდენტის payload-ს
        /// </summary>
        SyncChangePayload BuildStudentPayload(int studentId, SyncOperationType operation, Student student);

        /// <summary>
        /// ქმნის ჯგუფის payload-ს
        /// </summary>
        SyncChangePayload BuildGroupPayload(int groupId, SyncOperationType operation, Group group, int? studentCount = null);

        /// <summary>
        /// ქმნის ქვეჯგუფის payload-ს
        /// </summary>
        SyncChangePayload BuildSubGroupPayload(int subGroupId, SyncOperationType operation, SubGroup subGroup);

        /// <summary>
        /// ქმნის სტუდენტ-ჯგუფის payload-ს
        /// </summary>
        SyncChangePayload BuildStudentGroupPayload(int recordId, SyncOperationType operation, StudentGroups studentGroup);

        /// <summary>
        /// ქმნის სტუდენტ-ქვეჯგუფის payload-ს
        /// </summary>
        SyncChangePayload BuildStudentSubGroupPayload(int recordId, SyncOperationType operation, StudentSubGroups studentSubGroup);

        /// <summary>
        /// ქმნის წარუმატებელი გადახდის payload-ს
        /// </summary>
        SyncChangePayload BuildFailedPaymentPayload(int failedPaymentId, SyncOperationType operation, FailedPayment failedPayment);

        /// <summary>
        /// ქმნის იმპორტირებული გადახდის ლოგის payload-ს
        /// </summary>
        SyncChangePayload BuildImportedPaymentLogPayload(int importedPaymentLogId, SyncOperationType operation, ImportedPaymentLog importedPaymentLog);
    }
}


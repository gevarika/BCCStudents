using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    /// <summary>
    /// StudentSubGroups áƒªáƒ®áƒ áƒ˜áƒšáƒ—áƒáƒœ áƒ¡áƒáƒ›áƒ£áƒ¨áƒáƒ áƒ˜áƒœáƒ¢áƒ”áƒ áƒ¤áƒ”áƒ˜áƒ¡áƒ˜
    /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”-áƒ¥áƒ•áƒ”áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜áƒ¡ áƒ›áƒáƒ áƒ—áƒ•áƒ
    /// </summary>
    public interface IStudentSubGroupRepository
    {
        #region INSERT - áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ áƒ“áƒáƒ›áƒáƒ¢áƒ”áƒ‘áƒ

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ¥áƒ•áƒ”áƒ¯áƒ’áƒ£áƒ¤áƒ¨áƒ˜ áƒ“áƒáƒ›áƒáƒ¢áƒ”áƒ‘áƒ (áƒ¡áƒ áƒ£áƒšáƒ˜ áƒáƒ‘áƒ˜áƒ”áƒ¥áƒ¢áƒ˜áƒ—)
        /// </summary>
        /// <param name="studentSubGroup">StudentSubGroups áƒáƒ‘áƒ˜áƒ”áƒ¥áƒ¢áƒ˜</param>
        /// <param name="connection">áƒáƒ áƒ¡áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜ (áƒáƒ¤áƒªáƒ˜áƒáƒœáƒáƒšáƒ£áƒ áƒ˜ - áƒ¢áƒ áƒáƒœáƒ–áƒáƒ¥áƒªáƒ˜áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡)</param>
        /// <param name="transaction">áƒáƒ áƒ¡áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ¢áƒ áƒáƒœáƒ–áƒáƒ¥áƒªáƒ˜áƒ (áƒáƒ¤áƒªáƒ˜áƒáƒœáƒáƒšáƒ£áƒ áƒ˜)</param>
        /// <returns>áƒáƒ®áƒáƒšáƒ˜ áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ ID</returns>
        int InsertStudentSubGroup(StudentSubGroups studentSubGroup, MySqlConnection connection = null, MySqlTransaction transaction = null);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ¥áƒ•áƒ”áƒ¯áƒ’áƒ£áƒ¤áƒ¨áƒ˜ áƒ“áƒáƒ›áƒáƒ¢áƒ”áƒ‘áƒ (áƒ›áƒ˜áƒœáƒ˜áƒ›áƒáƒšáƒ£áƒ áƒ˜ áƒžáƒáƒ áƒáƒ›áƒ”áƒ¢áƒ áƒ”áƒ‘áƒ˜áƒ—)
        /// </summary>
        /// <param name="studentId">áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ ID</param>
        /// <param name="groupId">áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ ID</param>
        /// <param name="subGroupId">áƒ¥áƒ•áƒ”áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ ID</param>
        /// <param name="connection">áƒáƒ áƒ¡áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜ (áƒáƒ¤áƒªáƒ˜áƒáƒœáƒáƒšáƒ£áƒ áƒ˜ - áƒ¢áƒ áƒáƒœáƒ–áƒáƒ¥áƒªáƒ˜áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡)</param>
        /// <param name="transaction">áƒáƒ áƒ¡áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ¢áƒ áƒáƒœáƒ–áƒáƒ¥áƒªáƒ˜áƒ (áƒáƒ¤áƒªáƒ˜áƒáƒœáƒáƒšáƒ£áƒ áƒ˜)</param>
        /// <returns>áƒáƒ®áƒáƒšáƒ˜ áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ ID</returns>
        int InsertStudentSubGroup(int studentId, int groupId, int subGroupId, MySqlConnection connection = null, MySqlTransaction transaction = null);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ¥áƒ•áƒ”áƒ¯áƒ’áƒ£áƒ¤áƒ¨áƒ˜ áƒ“áƒáƒ›áƒáƒ¢áƒ”áƒ‘áƒ (áƒ¤áƒáƒ¡áƒ“áƒáƒ™áƒšáƒ”áƒ‘áƒ˜áƒ—)
        /// </summary>
        int InsertStudentSubGroupWithDiscount(int studentId, int groupId, int subGroupId, double discount);

        #endregion

        #region SELECT - áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ áƒ¬áƒáƒ™áƒ˜áƒ—áƒ®áƒ•áƒ

        /// <summary>
        /// áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ ID-áƒ˜áƒ—
        /// </summary>
        StudentSubGroups GetById(int id);

        /// <summary>
        /// áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ StudentId, GroupId áƒ“áƒ SubGroupId-áƒ˜áƒ—
        /// </summary>
        StudentSubGroups GetByStudentGroupAndSubGroup(int studentId, int groupId, int subGroupId);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡
        /// </summary>
        StudentSubGroups GetByStudentAndGroup(int studentId, int groupId);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ§áƒ•áƒ”áƒšáƒ áƒ¥áƒ•áƒ”áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ
        /// </summary>
        List<StudentSubGroups> GetByStudentId(int studentId);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜ áƒ¥áƒ•áƒ”áƒ¯áƒ’áƒ£áƒ¤áƒ”áƒ‘áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ
        /// </summary>
        List<StudentSubGroups> GetActiveByStudentId(int studentId);

        /// <summary>
        /// áƒ¥áƒ•áƒ”áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒ§áƒ•áƒ”áƒšáƒ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ
        /// </summary>
        List<StudentSubGroups> GetBySubGroupId(int subGroupId);

        /// <summary>
        /// áƒ¥áƒ•áƒ”áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”áƒ”áƒ‘áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ
        /// </summary>
        List<StudentSubGroups> GetActiveBySubGroupId(int subGroupId);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜ SubGroupId-áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡
        /// </summary>
        int? GetActiveSubGroupId(int studentId, int groupId);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ¥áƒ•áƒ”áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ ID-áƒ”áƒ‘áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ
        /// </summary>
        List<int> GetSubGroupIdsByStudentId(int studentId);

        /// <summary>
        /// áƒáƒ áƒ¡áƒ”áƒ‘áƒáƒ‘áƒ¡ áƒ—áƒ£ áƒáƒ áƒ áƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜ áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜
        /// </summary>
        bool ExistsActive(int studentId, int groupId, int subGroupId);

        /// <summary>
        /// áƒáƒ áƒ¡áƒ”áƒ‘áƒáƒ‘áƒ¡ áƒ—áƒ£ áƒáƒ áƒ áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜ (áƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜ áƒáƒœ áƒáƒ áƒáƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜)
        /// </summary>
        bool ExistsAny(int studentId, int groupId, int subGroupId);

        /// <summary>
        /// áƒáƒ áƒ¡áƒ”áƒ‘áƒáƒ‘áƒ¡ áƒ—áƒ£ áƒáƒ áƒ áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜ áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡ (áƒœáƒ”áƒ‘áƒ˜áƒ¡áƒ›áƒ˜áƒ”áƒ áƒ˜ áƒ¥áƒ•áƒ”áƒ¯áƒ’áƒ£áƒ¤áƒ˜)
        /// </summary>
        bool ExistsAnyForGroup(int studentId, int groupId);

        #endregion

        #region UPDATE - áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ (áƒ¡áƒ áƒ£áƒšáƒ˜)

        /// <summary>
        /// áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ áƒ¡áƒ áƒ£áƒšáƒ˜ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
        /// </summary>
        bool Update(StudentSubGroups studentSubGroup);

        #endregion

        #region UPDATE - áƒªáƒáƒšáƒ™áƒ”áƒ£áƒšáƒ˜ áƒ•áƒ”áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ

        /// <summary>
        /// áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ (áƒáƒ¥áƒ¢áƒ˜áƒ•áƒáƒªáƒ˜áƒ/áƒ“áƒ”áƒáƒ¥áƒ¢áƒ˜áƒ•áƒáƒªáƒ˜áƒ)
        /// </summary>
        bool UpdateStatus(int studentId, int groupId, int subGroupId, bool status);

        /// <summary>
        /// áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
        /// </summary>
        bool UpdatePaymentStatus(int studentId, int groupId, int subGroupId, string paymentStatus);

        /// <summary>
        /// áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
        /// </summary>
        bool UpdateDateOfPayment(int studentId, int groupId, int subGroupId, DateTime? dateOfPayment);

        /// <summary>
        /// áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ (alias áƒ›áƒ”áƒ—áƒáƒ“áƒ˜ PaymentDateService-áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡)
        /// </summary>
        bool UpdatePaymentDate(int studentId, int groupId, int subGroupId, DateTime newDate);

        /// <summary>
        /// áƒ¤áƒáƒ¡áƒ“áƒáƒ™áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
        /// </summary>
        bool UpdateDiscount(int studentId, int groupId, int subGroupId, double discount);

        /// <summary>
        /// SubGroupId-áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ (áƒ¥áƒ•áƒ”áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒ¨áƒ”áƒªáƒ•áƒšáƒ)
        /// </summary>
        bool UpdateSubGroupId(int studentId, int groupId, int oldSubGroupId, int newSubGroupId);

        /// <summary>
        /// GroupId-áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ (áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒ¨áƒ”áƒªáƒ•áƒšáƒ)
        /// </summary>
        bool UpdateGroupId(int studentId, int oldGroupId, int newGroupId);

        #endregion

        #region DELETE - áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ áƒ¬áƒáƒ¨áƒšáƒ

        /// <summary>
        /// Soft Delete - áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜áƒ¡ áƒ¨áƒ”áƒªáƒ•áƒšáƒ
        /// </summary>
        bool SoftDelete(int studentId, int groupId, int subGroupId);

        /// <summary>
        /// Hard Delete - áƒ¡áƒ áƒ£áƒšáƒ˜ áƒ¬áƒáƒ¨áƒšáƒ
        /// </summary>
        bool HardDelete(int studentId, int groupId, int subGroupId);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒ§áƒ•áƒ”áƒšáƒ áƒ¥áƒ•áƒ”áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ“áƒáƒœ Soft Delete
        /// </summary>
        bool SoftDeleteAllByStudentAndGroup(int studentId, int groupId);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ§áƒ•áƒ”áƒšáƒ áƒ¥áƒ•áƒ”áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ“áƒáƒœ Soft Delete
        /// </summary>
        bool SoftDeleteAllByStudentId(int studentId);

        #endregion
    }
}




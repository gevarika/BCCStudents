using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    /// <summary>
    /// StudentGroups áƒªáƒ®áƒ áƒ˜áƒšáƒ—áƒáƒœ áƒ¡áƒáƒ›áƒ£áƒ¨áƒáƒ áƒ˜áƒœáƒ¢áƒ”áƒ áƒ¤áƒ”áƒ˜áƒ¡áƒ˜
    /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”-áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜áƒ¡ áƒ›áƒáƒ áƒ—áƒ•áƒ
    /// </summary>
    public interface IStudentGroupRepository
    {
        #region INSERT - áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ áƒ“áƒáƒ›áƒáƒ¢áƒ”áƒ‘áƒ

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ¯áƒ’áƒ£áƒ¤áƒ¨áƒ˜ áƒ“áƒáƒ›áƒáƒ¢áƒ”áƒ‘áƒ (áƒ¡áƒ áƒ£áƒšáƒ˜ áƒáƒ‘áƒ˜áƒ”áƒ¥áƒ¢áƒ˜áƒ—)
        /// </summary>
        /// <param name="studentGroup">StudentGroups áƒáƒ‘áƒ˜áƒ”áƒ¥áƒ¢áƒ˜</param>
        /// <param name="connection">áƒáƒ áƒ¡áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜ (áƒáƒ¤áƒªáƒ˜áƒáƒœáƒáƒšáƒ£áƒ áƒ˜ - áƒ¢áƒ áƒáƒœáƒ–áƒáƒ¥áƒªáƒ˜áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡)</param>
        /// <param name="transaction">áƒáƒ áƒ¡áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ¢áƒ áƒáƒœáƒ–áƒáƒ¥áƒªáƒ˜áƒ (áƒáƒ¤áƒªáƒ˜áƒáƒœáƒáƒšáƒ£áƒ áƒ˜)</param>
        /// <returns>áƒáƒ®áƒáƒšáƒ˜ áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ ID</returns>
        int InsertStudentGroup(StudentGroups studentGroup, MySqlConnection connection = null, MySqlTransaction transaction = null);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ¯áƒ’áƒ£áƒ¤áƒ¨áƒ˜ áƒ“áƒáƒ›áƒáƒ¢áƒ”áƒ‘áƒ (áƒ›áƒ˜áƒœáƒ˜áƒ›áƒáƒšáƒ£áƒ áƒ˜ áƒžáƒáƒ áƒáƒ›áƒ”áƒ¢áƒ áƒ”áƒ‘áƒ˜áƒ—)
        /// </summary>
        /// <param name="studentId">áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ ID</param>
        /// <param name="groupId">áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ ID</param>
        /// <param name="connection">áƒáƒ áƒ¡áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜ (áƒáƒ¤áƒªáƒ˜áƒáƒœáƒáƒšáƒ£áƒ áƒ˜ - áƒ¢áƒ áƒáƒœáƒ–áƒáƒ¥áƒªáƒ˜áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡)</param>
        /// <param name="transaction">áƒáƒ áƒ¡áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ¢áƒ áƒáƒœáƒ–áƒáƒ¥áƒªáƒ˜áƒ (áƒáƒ¤áƒªáƒ˜áƒáƒœáƒáƒšáƒ£áƒ áƒ˜)</param>
        /// <returns>áƒáƒ®áƒáƒšáƒ˜ áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ ID</returns>
        int InsertStudentGroup(int studentId, int groupId, MySqlConnection connection = null, MySqlTransaction transaction = null);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ¯áƒ’áƒ£áƒ¤áƒ¨áƒ˜ áƒ“áƒáƒ›áƒáƒ¢áƒ”áƒ‘áƒ (áƒ¤áƒáƒ¡áƒ“áƒáƒ™áƒšáƒ”áƒ‘áƒ˜áƒ—)
        /// </summary>
        int InsertStudentGroupWithDiscount(int studentId, int groupId, double discount);

        #endregion

        #region SELECT - áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ áƒ¬áƒáƒ™áƒ˜áƒ—áƒ®áƒ•áƒ

        /// <summary>
        /// áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ ID-áƒ˜áƒ—
        /// </summary>
        StudentGroups GetById(int id);

        /// <summary>
        /// áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ StudentId áƒ“áƒ GroupId-áƒ˜áƒ—
        /// </summary>
        StudentGroups GetByStudentAndGroup(int studentId, int groupId);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ§áƒ•áƒ”áƒšáƒ áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ
        /// </summary>
        List<StudentGroups> GetByStudentId(int studentId);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜ áƒ¯áƒ’áƒ£áƒ¤áƒ”áƒ‘áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ
        /// </summary>
        List<StudentGroups> GetActiveByStudentId(int studentId);

        /// <summary>
        /// áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒ§áƒ•áƒ”áƒšáƒ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ
        /// </summary>
        List<StudentGroups> GetByGroupId(int groupId);

        /// <summary>
        /// áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”áƒ”áƒ‘áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ
        /// </summary>
        List<StudentGroups> GetActiveByGroupId(int groupId);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ¯áƒ’áƒ£áƒ¤áƒ”áƒ‘áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡ (Groups áƒªáƒ®áƒ áƒ˜áƒšáƒ—áƒáƒœ JOIN)
        /// </summary>
        List<StudentGroups> GetForPayment(int studentId);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ ID-áƒ”áƒ‘áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ
        /// </summary>
        List<int> GetGroupIdsByStudentId(int studentId);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜ áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ ID-áƒ”áƒ‘áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ
        /// </summary>
        List<int> GetActiveGroupIdsByStudentId(int studentId);

        /// <summary>
        /// áƒ’áƒáƒ“áƒáƒ£áƒ®áƒ“áƒ”áƒšáƒ˜ áƒ¯áƒ’áƒ£áƒ¤áƒ”áƒ‘áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ
        /// </summary>
        List<StudentGroups> GetUnpaidByStudentId(int studentId);

        /// <summary>
        /// áƒ•áƒáƒ“áƒáƒ’áƒáƒ“áƒáƒªáƒ˜áƒšáƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ”áƒ‘áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ (áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜ áƒ’áƒáƒ•áƒ˜áƒ“áƒ)
        /// </summary>
        List<StudentGroups> GetOverduePayments(DateTime asOfDate);

        /// <summary>
        /// áƒáƒ áƒ¡áƒ”áƒ‘áƒáƒ‘áƒ¡ áƒ—áƒ£ áƒáƒ áƒ áƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜ áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜
        /// </summary>
        bool ExistsActive(int studentId, int groupId);

        /// <summary>
        /// áƒáƒ áƒ¡áƒ”áƒ‘áƒáƒ‘áƒ¡ áƒ—áƒ£ áƒáƒ áƒ áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜ (áƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜ áƒáƒœ áƒáƒ áƒáƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜)
        /// </summary>
        bool ExistsAny(int studentId, int groupId);

        #endregion

        #region UPDATE - áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ (áƒ¡áƒ áƒ£áƒšáƒ˜)

        /// <summary>
        /// áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ áƒ¡áƒ áƒ£áƒšáƒ˜ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
        /// </summary>
        bool Update(StudentGroups studentGroup);

        #endregion

        #region UPDATE - áƒªáƒáƒšáƒ™áƒ”áƒ£áƒšáƒ˜ áƒ•áƒ”áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ

        /// <summary>
        /// áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ (áƒáƒ¥áƒ¢áƒ˜áƒ•áƒáƒªáƒ˜áƒ/áƒ“áƒ”áƒáƒ¥áƒ¢áƒ˜áƒ•áƒáƒªáƒ˜áƒ)
        /// </summary>
        bool UpdateStatus(int studentId, int groupId, bool status);

        /// <summary>
        /// áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
        /// </summary>
        bool UpdatePaymentStatus(int studentId, int groupId, string paymentStatus);

        /// <summary>
        /// áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
        /// </summary>
        bool UpdateDateOfPayment(int studentId, int groupId, DateTime? dateOfPayment);

        /// <summary>
        /// áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ (alias)
        /// </summary>
        bool UpdatePaymentDate(int studentId, int groupId, DateTime newDate);

        /// <summary>
        /// áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜áƒ¡ áƒ“áƒ áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜áƒ¡ áƒ”áƒ áƒ—áƒáƒ“ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
        /// </summary>
        bool UpdatePaymentStatusAndDate(int studentId, int groupId, string paymentStatus, DateTime? dateOfPayment);

        /// <summary>
        /// áƒ¤áƒáƒ¡áƒ“áƒáƒ™áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
        /// </summary>
        bool UpdateDiscount(int studentId, int groupId, double discount);

        /// <summary>
        /// áƒ¤áƒáƒ¡áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
        /// </summary>
        bool UpdatePrice(int studentId, int groupId, decimal price);

        /// <summary>
        /// GroupId-áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ (áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒ¨áƒ”áƒªáƒ•áƒšáƒ)
        /// </summary>
        bool UpdateGroupId(int studentId, int oldGroupId, int newGroupId);

        #endregion

        #region DELETE - áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ áƒ¬áƒáƒ¨áƒšáƒ

        /// <summary>
        /// Soft Delete - áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜áƒ¡ áƒ¨áƒ”áƒªáƒ•áƒšáƒ
        /// </summary>
        bool SoftDelete(int studentId, int groupId);

        /// <summary>
        /// Hard Delete - áƒ¡áƒ áƒ£áƒšáƒ˜ áƒ¬áƒáƒ¨áƒšáƒ
        /// </summary>
        bool HardDelete(int studentId, int groupId);

        /// <summary>
        /// áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ§áƒ•áƒ”áƒšáƒ áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ“áƒáƒœ Soft Delete
        /// </summary>
        bool SoftDeleteAllByStudentId(int studentId);

        #endregion

        #region IMPORT - áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡ áƒ¡áƒáƒ­áƒ˜áƒ áƒ áƒ›áƒ”áƒ—áƒáƒ“áƒ”áƒ‘áƒ˜

        /// <summary>
        /// áƒ§áƒ•áƒ”áƒšáƒ áƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”-áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒ¬áƒ§áƒ•áƒ˜áƒšáƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ (áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡ áƒ“áƒ£áƒ‘áƒšáƒ˜áƒ™áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ¡ áƒ¨áƒ”áƒ¡áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒšáƒáƒ“)
        /// </summary>
        List<(int StudentId, int GroupId)> GetAllActiveStudentGroupPairs();

        #endregion
    }
}




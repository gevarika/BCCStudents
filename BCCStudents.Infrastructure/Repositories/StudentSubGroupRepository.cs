using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace BCCStudents.Infrastructure.Repositories
{
    /// <summary>
    /// StudentSubGroups ცხრილთან სამუშაო კლასი
    /// მოსწავლის-ქვეჯგუფის კავშირის მართვა
    /// </summary>
    public class StudentSubGroupRepository : IStudentSubGroupRepository
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public StudentSubGroupRepository(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        #region ==================== INSERT - ჩანაწერის დამატება ====================

        /// <summary>
        /// მოსწავლის ქვეჯგუფში დამატება (სრული ობიექტით)
        /// </summary>
        /// <param name="studentSubGroup">StudentSubGroups ობიექტი</param>
        /// <param name="connection">არსებული კავშირი (ოფციონალური - ტრანზაქციისთვის)</param>
        /// <param name="transaction">არსებული ტრანზაქცია (ოფციონალური)</param>
        /// <returns>ახალი ჩანაწერის ID</returns>
        public int InsertStudentSubGroup(StudentSubGroups studentSubGroup, MySqlConnection connection = null, MySqlTransaction transaction = null)
        {
            bool useExternalConnection = connection != null;
            var conn = connection ?? _connectionProvider.GetLocalConnection();

            try
            {
                if (!useExternalConnection)
                    conn.Open();

                // თუ DateOfPayment არ არის მითითებული, default = დღეს + 1 თვე
                var dateOfPayment = studentSubGroup.DateOfPayment ?? DateTime.Today.AddMonths(1);

                var query = @"INSERT INTO StudentSubGroups 
                              (StudentId, GroupId, SubGroupId, PaymentStatus, DateOfPayment, Price, Discount, Status, IsDeleted,  UpdatedAt) 
                              VALUES (@StudentId, @GroupId, @SubGroupId, @PaymentStatus, @DateOfPayment, @Price, @Discount, @Status, @IsDeleted,  @UpdatedAt);
                              SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentSubGroup.StudentId);
                    cmd.Parameters.AddWithValue("@GroupId", studentSubGroup.GroupId);
                    cmd.Parameters.AddWithValue("@SubGroupId", studentSubGroup.SubGroupId);
                    cmd.Parameters.AddWithValue("@PaymentStatus", studentSubGroup.PaymentStatus ?? "Pending");
                    cmd.Parameters.AddWithValue("@Price", studentSubGroup.Price);
                    cmd.Parameters.AddWithValue("@Discount", studentSubGroup.Discount);
                    cmd.Parameters.AddWithValue("@Status", studentSubGroup.Status);
                    cmd.Parameters.AddWithValue("@IsDeleted", false);
                    cmd.Parameters.AddWithValue("@DateOfPayment", dateOfPayment);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            finally
            {
                if (!useExternalConnection && conn != null)
                    conn.Dispose();
            }
        }

        /// <summary>
        /// მოსწავლის ქვეჯგუფში დამატება (მინიმალური პარამეტრებით)
        /// </summary>
        /// <param name="studentId">მოსწავლის ID</param>
        /// <param name="groupId">ჯგუფის ID</param>
        /// <param name="subGroupId">ქვეჯგუფის ID</param>
        /// <param name="connection">არსებული კავშირი (ოფციონალური - ტრანზაქციისთვის)</param>
        /// <param name="transaction">არსებული ტრანზაქცია (ოფციონალური)</param>
        /// <returns>ახალი ჩანაწერის ID</returns>
        public int InsertStudentSubGroup(int studentId, int groupId, int subGroupId, MySqlConnection connection = null, MySqlTransaction transaction = null)
        {
            bool useExternalConnection = connection != null;
            var conn = connection ?? _connectionProvider.GetLocalConnection();

            try
            {
                if (!useExternalConnection)
                    conn.Open();

                // გადახდის თარიღი = დღეს + 1 თვე
                var dateOfPayment = DateTime.Today.AddMonths(1);

                var query = @"INSERT INTO StudentSubGroups 
                              (StudentId, GroupId, SubGroupId, PaymentStatus, Status, IsDeleted, DateOfPayment, UpdatedAt) 
                              VALUES (@StudentId, @GroupId, @SubGroupId, 'Pending', 1, 0, @DateOfPayment, @UpdatedAt);
                              SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId);
                    cmd.Parameters.AddWithValue("@DateOfPayment", dateOfPayment);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            finally
            {
                if (!useExternalConnection && conn != null)
                    conn.Dispose();
            }
        }

        /// <summary>
        /// მოსწავლის ქვეჯგუფში დამატება (ფასდაკლებით)
        /// </summary>
        public int InsertStudentSubGroupWithDiscount(int studentId, int groupId, int subGroupId, double discount)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();

                // გადახდის თარიღი = დღეს + 1 თვე
                var dateOfPayment = DateTime.Today.AddMonths(1);

                var query = @"INSERT INTO StudentSubGroups 
                              (StudentId, GroupId, SubGroupId, PaymentStatus, Discount, Status, IsDeleted, DateOfPayment, UpdatedAt) 
                              VALUES (@StudentId, @GroupId, @SubGroupId, 'Pending', @Discount, 1, 0, @DateOfPayment, @UpdatedAt);
                              SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId);
                    cmd.Parameters.AddWithValue("@Discount", discount);
                    cmd.Parameters.AddWithValue("@DateOfPayment", dateOfPayment);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        #endregion

        #region ==================== SELECT - ჩანაწერის წაკითხვა ====================

        /// <summary>
        /// ჩანაწერის მიღება ID-ით
        /// </summary>
        public StudentSubGroups GetById(int id)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, StudentId, GroupId, SubGroupId, PaymentStatus, Price, Discount, Status, IsDeleted, DateOfPayment, UpdatedAt 
                              FROM StudentSubGroups WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// ჩანაწერის მიღება StudentId, GroupId და SubGroupId-ით
        /// </summary>
        public StudentSubGroups GetByStudentGroupAndSubGroup(int studentId, int groupId, int subGroupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, StudentId, GroupId, SubGroupId, PaymentStatus, Price, Discount, Status, IsDeleted, DateOfPayment, UpdatedAt 
                              FROM StudentSubGroups 
                              WHERE StudentId = @StudentId AND GroupId = @GroupId AND SubGroupId = @SubGroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// მოსწავლის ჩანაწერის მიღება ჯგუფისთვის
        /// </summary>
        public StudentSubGroups GetByStudentAndGroup(int studentId, int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, StudentId, GroupId, SubGroupId, PaymentStatus, Price, Discount, Status, IsDeleted, DateOfPayment, UpdatedAt 
                              FROM StudentSubGroups 
                              WHERE StudentId = @StudentId AND GroupId = @GroupId 
                              AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)
                              LIMIT 1";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// მოსწავლის ყველა ქვეჯგუფის მიღება
        /// </summary>
        public List<StudentSubGroups> GetByStudentId(int studentId)
        {
            var result = new List<StudentSubGroups>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, StudentId, GroupId, SubGroupId, PaymentStatus, Price, Discount, Status, IsDeleted, DateOfPayment, UpdatedAt 
                              FROM StudentSubGroups WHERE StudentId = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// მოსწავლის აქტიური ქვეჯგუფების მიღება
        /// </summary>
        public List<StudentSubGroups> GetActiveByStudentId(int studentId)
        {
            var result = new List<StudentSubGroups>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, StudentId, GroupId, SubGroupId, PaymentStatus, Price, Discount, Status, IsDeleted, DateOfPayment, UpdatedAt 
                              FROM StudentSubGroups 
                              WHERE StudentId = @StudentId AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// ქვეჯგუფის ყველა მოსწავლის მიღება
        /// </summary>
        public List<StudentSubGroups> GetBySubGroupId(int subGroupId)
        {
            var result = new List<StudentSubGroups>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, StudentId, GroupId, SubGroupId, PaymentStatus, Price, Discount, Status, IsDeleted, DateOfPayment, UpdatedAt 
                              FROM StudentSubGroups WHERE SubGroupId = @SubGroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// ქვეჯგუფის აქტიური მოსწავლეების მიღება
        /// </summary>
        public List<StudentSubGroups> GetActiveBySubGroupId(int subGroupId)
        {
            var result = new List<StudentSubGroups>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, StudentId, GroupId, SubGroupId, PaymentStatus, Price, Discount, Status, IsDeleted, DateOfPayment, UpdatedAt 
                              FROM StudentSubGroups 
                              WHERE SubGroupId = @SubGroupId AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// მოსწავლის აქტიური SubGroupId-ის მიღება ჯგუფისთვის
        /// </summary>
        public int? GetActiveSubGroupId(int studentId, int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT SubGroupId FROM StudentSubGroups 
                              WHERE StudentId = @StudentId AND GroupId = @GroupId 
                              AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)
                              LIMIT 1";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    var result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value ? (int?)Convert.ToInt32(result) : null;
                }
            }
        }

        /// <summary>
        /// მოსწავლის ქვეჯგუფის ID-ების მიღება
        /// </summary>
        public List<int> GetSubGroupIdsByStudentId(int studentId)
        {
            var result = new List<int>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT SubGroupId FROM StudentSubGroups 
                              WHERE StudentId = @StudentId AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetInt32("SubGroupId"));
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// არსებობს თუ არა აქტიური კავშირი
        /// </summary>
        public bool ExistsActive(int studentId, int groupId, int subGroupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT COUNT(1) FROM StudentSubGroups 
                              WHERE StudentId = @StudentId AND GroupId = @GroupId AND SubGroupId = @SubGroupId
                              AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        /// <summary>
        /// არსებობს თუ არა კავშირი (აქტიური ან არააქტიური)
        /// </summary>
        public bool ExistsAny(int studentId, int groupId, int subGroupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT COUNT(1) FROM StudentSubGroups 
                              WHERE StudentId = @StudentId AND GroupId = @GroupId AND SubGroupId = @SubGroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        /// <summary>
        /// არსებობს თუ არა კავშირი ჯგუფისთვის (ნებისმიერი ქვეჯგუფი)
        /// </summary>
        public bool ExistsAnyForGroup(int studentId, int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT COUNT(1) FROM StudentSubGroups 
                              WHERE StudentId = @StudentId AND GroupId = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        #endregion

        #region ==================== UPDATE - ჩანაწერის განახლება (სრული) ====================

        /// <summary>
        /// ჩანაწერის სრული განახლება
        /// </summary>
        public bool Update(StudentSubGroups studentSubGroup)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentSubGroups 
                              SET PaymentStatus = @PaymentStatus, Price = @Price, Discount = @Discount, 
                                  Status = @Status, IsDeleted = @IsDeleted, DateOfPayment = @DateOfPayment, UpdatedAt = @UpdatedAt
                              WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", studentSubGroup.Id);
                    cmd.Parameters.AddWithValue("@PaymentStatus", studentSubGroup.PaymentStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Price", studentSubGroup.Price);
                    cmd.Parameters.AddWithValue("@Discount", studentSubGroup.Discount);
                    cmd.Parameters.AddWithValue("@Status", studentSubGroup.Status);
                    cmd.Parameters.AddWithValue("@IsDeleted", studentSubGroup.IsDeleted);
                    cmd.Parameters.AddWithValue("@DateOfPayment", studentSubGroup.DateOfPayment.HasValue ? (object)studentSubGroup.DateOfPayment.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== UPDATE - ცალკეული ველების განახლება ====================

        /// <summary>
        /// სტატუსის განახლება (აქტივაცია/დეაქტივაცია)
        /// </summary>
        public bool UpdateStatus(int studentId, int groupId, int subGroupId, bool status)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentSubGroups 
                              SET Status = @Status, IsDeleted = @IsDeleted, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId AND SubGroupId = @SubGroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@IsDeleted", !status);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// გადახდის სტატუსის განახლება
        /// </summary>
        public bool UpdatePaymentStatus(int studentId, int groupId, int subGroupId, string paymentStatus)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentSubGroups 
                              SET PaymentStatus = @PaymentStatus, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId AND SubGroupId = @SubGroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId);
                    cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// გადახდის თარიღის განახლება
        /// </summary>
        public bool UpdateDateOfPayment(int studentId, int groupId, int subGroupId, DateTime? dateOfPayment)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentSubGroups 
                              SET DateOfPayment = @DateOfPayment, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId AND SubGroupId = @SubGroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId);
                    cmd.Parameters.AddWithValue("@DateOfPayment", dateOfPayment.HasValue ? (object)dateOfPayment.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// გადახდის თარიღის განახლება (alias მეთოდი PaymentDateService-ისთვის)
        /// </summary>
        public bool UpdatePaymentDate(int studentId, int groupId, int subGroupId, DateTime newDate)
        {
            return UpdateDateOfPayment(studentId, groupId, subGroupId, newDate);
        }

        /// <summary>
        /// ფასდაკლების განახლება
        /// </summary>
        public bool UpdateDiscount(int studentId, int groupId, int subGroupId, double discount)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentSubGroups 
                              SET Discount = @Discount, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId AND SubGroupId = @SubGroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId);
                    cmd.Parameters.AddWithValue("@Discount", discount);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// SubGroupId-ის განახლება (ქვეჯგუფის შეცვლა)
        /// </summary>
        public bool UpdateSubGroupId(int studentId, int groupId, int oldSubGroupId, int newSubGroupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentSubGroups 
                              SET SubGroupId = @NewSubGroupId, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId AND SubGroupId = @OldSubGroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@OldSubGroupId", oldSubGroupId);
                    cmd.Parameters.AddWithValue("@NewSubGroupId", newSubGroupId);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// GroupId-ის განახლება (ჯგუფის შეცვლა)
        /// </summary>
        public bool UpdateGroupId(int studentId, int oldGroupId, int newGroupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentSubGroups 
                              SET GroupId = @NewGroupId, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @OldGroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@OldGroupId", oldGroupId);
                    cmd.Parameters.AddWithValue("@NewGroupId", newGroupId);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== DELETE - ჩანაწერის წაშლა ====================

        /// <summary>
        /// Soft Delete - სტატუსის შეცვლა
        /// </summary>
        public bool SoftDelete(int studentId, int groupId, int subGroupId)
        {
            return UpdateStatus(studentId, groupId, subGroupId, false);
        }

        /// <summary>
        /// Hard Delete - სრული წაშლა
        /// </summary>
        public bool HardDelete(int studentId, int groupId, int subGroupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "DELETE FROM StudentSubGroups WHERE StudentId = @StudentId AND GroupId = @GroupId AND SubGroupId = @SubGroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის ჯგუფის ყველა ქვეჯგუფიდან Soft Delete
        /// </summary>
        public bool SoftDeleteAllByStudentAndGroup(int studentId, int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentSubGroups 
                              SET Status = 0, IsDeleted = 1, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის ყველა ქვეჯგუფიდან Soft Delete
        /// </summary>
        public bool SoftDeleteAllByStudentId(int studentId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentSubGroups 
                              SET Status = 0, IsDeleted = 1, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== HELPER - დამხმარე მეთოდები ====================

        /// <summary>
        /// StudentSubGroups ობიექტის შექმნა DataReader-დან
        /// </summary>
        private StudentSubGroups MapFromReader(MySqlDataReader reader)
        {
            return new StudentSubGroups
            {
                Id = reader.GetInt32("Id"),
                StudentId = reader.GetInt32("StudentId"),
                GroupId = reader.GetInt32("GroupId"),
                SubGroupId = reader.GetInt32("SubGroupId"),
                PaymentStatus = reader.IsDBNull(reader.GetOrdinal("PaymentStatus")) ? null : reader.GetString("PaymentStatus"),
                Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? 0 : reader.GetDecimal("Price"),
                Discount = reader.IsDBNull(reader.GetOrdinal("Discount")) ? 0 : reader.GetDouble("Discount"),
                Status = !reader.IsDBNull(reader.GetOrdinal("Status")) && reader.GetBoolean("Status"),
                IsDeleted = !reader.IsDBNull(reader.GetOrdinal("IsDeleted")) && reader.GetBoolean("IsDeleted"),
                DateOfPayment = reader.IsDBNull(reader.GetOrdinal("DateOfPayment")) ? (DateTime?)null : reader.GetDateTime("DateOfPayment"),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime("UpdatedAt")
            };
        }

        #endregion
    }
}




using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace BCCStudents.Infrastructure.Repositories
{
    /// <summary>
    /// StudentGroups ცხრილთან სამუშაო კლასი
    /// მოსწავლის-ჯგუფის კავშირის მართვა
    /// </summary>
    public class StudentGroupRepository : IStudentGroupRepository
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public StudentGroupRepository(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        #region ==================== INSERT - ჩანაწერის დამატება ====================

        /// <summary>
        /// მოსწავლის ჯგუფში დამატება (სრული ობიექტით)
        /// </summary>
        /// <param name="studentGroup">StudentGroups ობიექტი</param>
        /// <param name="connection">არსებული კავშირი (ოფციონალური - ტრანზაქციისთვის)</param>
        /// <param name="transaction">არსებული ტრანზაქცია (ოფციონალური)</param>
        /// <returns>ახალი ჩანაწერის ID</returns>
        public int InsertStudentGroup(StudentGroups studentGroup, MySqlConnection connection = null, MySqlTransaction transaction = null)
        {
            bool useExternalConnection = connection != null;
            var conn = connection ?? _connectionProvider.GetLocalConnection();

            try
            {
                if (!useExternalConnection)
                    conn.Open();

                // თუ DateOfPayment არ არის მითითებული, default = დღეს + 1 თვე
                var dateOfPayment = studentGroup.DateOfPayment ?? DateTime.Today.AddMonths(1);

                // Debug ლოგი
                System.Diagnostics.Debug.WriteLine($"[InsertStudentGroup] StudentId={studentGroup.StudentId}, GroupId={studentGroup.GroupId}, DateOfPayment={dateOfPayment:yyyy-MM-dd}");

                var query = @"INSERT INTO StudentGroups 
                              (StudentId, GroupId, PaymentStatus, Price, Discount, Status, IsDeleted, DateOfPayment, UpdatedAt) 
                              VALUES (@StudentId, @GroupId, @PaymentStatus, @Price, @Discount, @Status, @IsDeleted, @DateOfPayment, @UpdatedAt);
                              SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentGroup.StudentId);
                    cmd.Parameters.AddWithValue("@GroupId", studentGroup.GroupId);
                    cmd.Parameters.AddWithValue("@PaymentStatus", studentGroup.PaymentStatus ?? "Pending");
                    cmd.Parameters.AddWithValue("@Price", studentGroup.Price);
                    cmd.Parameters.AddWithValue("@Discount", studentGroup.Discount);
                    cmd.Parameters.AddWithValue("@Status", studentGroup.Status);
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
        /// მოსწავლის ჯგუფში დამატება (მინიმალური პარამეტრებით)
        /// </summary>
        /// <param name="studentId">მოსწავლის ID</param>
        /// <param name="groupId">ჯგუფის ID</param>
        /// <param name="connection">არსებული კავშირი (ოფციონალური - ტრანზაქციისთვის)</param>
        /// <param name="transaction">არსებული ტრანზაქცია (ოფციონალური)</param>
        /// <returns>ახალი ჩანაწერის ID</returns>
        public int InsertStudentGroup(int studentId, int groupId, MySqlConnection connection = null, MySqlTransaction transaction = null)
        {
            bool useExternalConnection = connection != null;
            var conn = connection ?? _connectionProvider.GetLocalConnection();

            try
            {
                if (!useExternalConnection)
                    conn.Open();

                // გადახდის თარიღი = დღეს + 1 თვე (პირველი გადახდა მომდევნო თვეში)
                var dateOfPayment = DateTime.Today.AddMonths(1);

                var query = @"INSERT INTO StudentGroups 
                              (StudentId, GroupId, PaymentStatus, Status, IsDeleted, DateOfPayment, UpdatedAt) 
                              VALUES (@StudentId, @GroupId, 'Pending', 1, 0, @DateOfPayment, @UpdatedAt);
                              SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
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
        /// მოსწავლის ჯგუფში დამატება (ფასდაკლებით)
        /// </summary>
        public int InsertStudentGroupWithDiscount(int studentId, int groupId, double discount)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();

                // გადახდის თარიღი = დღეს + 1 თვე
                var dateOfPayment = DateTime.Today.AddMonths(1);

                var query = @"INSERT INTO StudentGroups 
                              (StudentId, GroupId, PaymentStatus, Discount, Status, IsDeleted, DateOfPayment, UpdatedAt) 
                              VALUES (@StudentId, @GroupId, 'Pending', @Discount, 1, 0, @DateOfPayment, @UpdatedAt);
                              SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
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
        public StudentGroups GetById(int id)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, StudentId, GroupId, PaymentStatus, Price, Discount, Status, IsDeleted, DateOfPayment, UpdatedAt 
                              FROM StudentGroups WHERE Id = @Id";

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
        /// ჩანაწერის მიღება StudentId და GroupId-ით
        /// </summary>
        public StudentGroups GetByStudentAndGroup(int studentId, int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, StudentId, GroupId, PaymentStatus, Price, Discount, Status, DateOfPayment, UpdatedAt 
                              FROM StudentGroups WHERE StudentId = @StudentId AND GroupId = @GroupId";

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
        /// მოსწავლის ყველა ჯგუფის მიღება
        /// </summary>
        public List<StudentGroups> GetByStudentId(int studentId)
        {
            var result = new List<StudentGroups>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, StudentId, GroupId, PaymentStatus, Price, Discount, Status, IsDeleted, DateOfPayment, UpdatedAt 
                              FROM StudentGroups WHERE StudentId = @StudentId";

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
        /// მოსწავლის აქტიური ჯგუფების მიღება
        /// </summary>
        public List<StudentGroups> GetActiveByStudentId(int studentId)
        {
            var result = new List<StudentGroups>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT sg.Id, sg.StudentId, sg.GroupId, sg.PaymentStatus, sg.Price, sg.Discount, sg.Status, sg.IsDeleted, sg.DateOfPayment, sg.UpdatedAt,
                                     ssg.SubGroupId
                              FROM StudentGroups sg
                              LEFT JOIN StudentSubGroups ssg ON ssg.StudentId = sg.StudentId AND ssg.GroupId = sg.GroupId AND ssg.Status = 1 AND (ssg.IsDeleted = 0 OR ssg.IsDeleted IS NULL)
                              WHERE sg.StudentId = @StudentId AND sg.Status = 1 AND (sg.IsDeleted = 0 OR sg.IsDeleted IS NULL)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(MapFromReaderWithSubGroup(reader));
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// ჯგუფის ყველა მოსწავლის მიღება
        /// </summary>
        public List<StudentGroups> GetByGroupId(int groupId)
        {
            var result = new List<StudentGroups>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, StudentId, GroupId, PaymentStatus, Price, Discount, Status, IsDeleted, DateOfPayment, UpdatedAt 
                              FROM StudentGroups WHERE GroupId = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
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
        /// ჯგუფის აქტიური მოსწავლეების მიღება
        /// </summary>
        public List<StudentGroups> GetActiveByGroupId(int groupId)
        {
            var result = new List<StudentGroups>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, StudentId, GroupId, PaymentStatus, Price, Discount, Status, IsDeleted, DateOfPayment, UpdatedAt 
                              FROM StudentGroups 
                              WHERE GroupId = @GroupId AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
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
        /// მოსწავლის ჯგუფების მიღება გადასახდისთვის (Groups ცხრილთან JOIN)
        /// </summary>
        /// <summary>
        /// მოსწავლის აქტიური ჯგუფების მიღება გადასახდისთვის
        /// </summary>
        /// <param name="studentId">მოსწავლის ID</param>
        /// <returns>აქტიური ჯგუფების სია გადასახდისთვის</returns>
        /// <remarks>
        /// Note:
        /// - StudentGroups.Price გადაიწერება Groups.Price-ით (სრული ფასი)
        /// - ეს აუცილებელია რადგან გადახდის დროს უნდა გამოვთვალოთ:
        ///   finalFee = Groups.Price - (Groups.Price * StudentGroups.Discount / 100)
        /// - StudentGroups.Price შეიძლება იყოს უკვე ფასდაკლებული, ამიტომ ვიყენებთ Groups.Price-ს
        /// </remarks>
        public List<StudentGroups> GetForPayment(int studentId)
        {
            var result = new List<StudentGroups>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"
                    SELECT sg.Id, sg.StudentId, sg.GroupId, sg.PaymentStatus, sg.Price AS SgPrice, sg.Discount, 
                           sg.Status, sg.IsDeleted, sg.DateOfPayment, sg.UpdatedAt,
                           g.Price AS GroupPrice, g.Name AS GroupName,
                           (SELECT ssg.SubGroupId FROM StudentSubGroups ssg 
                            WHERE ssg.StudentId = sg.StudentId AND ssg.GroupId = sg.GroupId 
                            AND ssg.Status = 1 AND (ssg.IsDeleted = 0 OR ssg.IsDeleted IS NULL) 
                            LIMIT 1) AS SubGroupId
                    FROM StudentGroups sg
                    INNER JOIN `Groups` g ON sg.GroupId = g.Id
                    WHERE sg.StudentId = @StudentId 
                      AND sg.Status = 1 
                      AND (sg.IsDeleted = 0 OR sg.IsDeleted IS NULL)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // სპეციალური mapper GetForPayment-ისთვის (SQL-ში არის SgPrice და GroupPrice, არა Price)
                            var sg = MapFromReaderForPayment(reader);

                            // გადახდისთვის Groups ცხრილის ფასი გამოიყენება (სრული ფასი)
                            // StudentGroups.Price გადაიწერება Groups.Price-ით
                            // ეს აუცილებელია რადგან ფასდაკლება ითვლება PaymentService-ში:
                            // finalFee = Groups.Price - (Groups.Price * StudentGroups.Discount / 100)
                            sg.Price = reader.IsDBNull(reader.GetOrdinal("GroupPrice")) ? 0 : reader.GetDecimal("GroupPrice");
                            sg.Name = reader.IsDBNull(reader.GetOrdinal("GroupName")) ? null : reader.GetString("GroupName");
                            sg.SubGroupId = reader.IsDBNull(reader.GetOrdinal("SubGroupId")) ? (int?)null : reader.GetInt32("SubGroupId");
                            result.Add(sg);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// მოსწავლის ჯგუფის ID-ების მიღება
        /// </summary>
        public List<int> GetGroupIdsByStudentId(int studentId)
        {
            var result = new List<int>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT GroupId FROM StudentGroups WHERE StudentId = @StudentId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetInt32("GroupId"));
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// მოსწავლის აქტიური ჯგუფის ID-ების მიღება
        /// </summary>
        public List<int> GetActiveGroupIdsByStudentId(int studentId)
        {
            var result = new List<int>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT GroupId FROM StudentGroups 
                              WHERE StudentId = @StudentId AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetInt32("GroupId"));
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// გადაუხდელი ჯგუფების მიღება
        /// </summary>
        public List<StudentGroups> GetUnpaidByStudentId(int studentId)
        {
            var result = new List<StudentGroups>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, StudentId, GroupId, PaymentStatus, Price, Discount, Status, IsDeleted, DateOfPayment, UpdatedAt 
                              FROM StudentGroups 
                              WHERE StudentId = @StudentId 
                                AND Status = 1 
                                AND (IsDeleted = 0 OR IsDeleted IS NULL)
                                AND (PaymentStatus IS NULL OR PaymentStatus = 'Pending' OR PaymentStatus = 'PartiallyPaid')";

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
        /// ვადაგადაცილებული გადახდების მიღება (გადახდის თარიღი გავიდა)
        /// </summary>
        public List<StudentGroups> GetOverduePayments(DateTime asOfDate)
        {
            var result = new List<StudentGroups>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, StudentId, GroupId, PaymentStatus, Price, Discount, Status, IsDeleted, DateOfPayment, UpdatedAt 
                              FROM StudentGroups 
                              WHERE Status = 1 
                                AND (IsDeleted = 0 OR IsDeleted IS NULL)
                                AND DateOfPayment IS NOT NULL
                                AND DateOfPayment < @AsOfDate
                                AND (PaymentStatus IS NULL OR PaymentStatus != 'Paid')";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@AsOfDate", asOfDate);
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
        /// არსებობს თუ არა აქტიური კავშირი
        /// </summary>
        public bool ExistsActive(int studentId, int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT COUNT(1) FROM StudentGroups 
                              WHERE StudentId = @StudentId AND GroupId = @GroupId 
                              AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        /// <summary>
        /// არსებობს თუ არა კავშირი (აქტიური ან არააქტიური)
        /// </summary>
        public bool ExistsAny(int studentId, int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT COUNT(1) FROM StudentGroups WHERE StudentId = @StudentId AND GroupId = @GroupId";

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
        public bool Update(StudentGroups studentGroup)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentGroups 
                              SET PaymentStatus = @PaymentStatus, Price = @Price, Discount = @Discount, 
                                  Status = @Status, IsDeleted = @IsDeleted, DateOfPayment = @DateOfPayment, UpdatedAt = @UpdatedAt
                              WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", studentGroup.Id);
                    cmd.Parameters.AddWithValue("@PaymentStatus", studentGroup.PaymentStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Price", studentGroup.Price);
                    cmd.Parameters.AddWithValue("@Discount", studentGroup.Discount);
                    cmd.Parameters.AddWithValue("@Status", studentGroup.Status);
                    cmd.Parameters.AddWithValue("@IsDeleted", studentGroup.IsDeleted);
                    cmd.Parameters.AddWithValue("@DateOfPayment", studentGroup.DateOfPayment.HasValue ? (object)studentGroup.DateOfPayment.Value : DBNull.Value);
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
        public bool UpdateStatus(int studentId, int groupId, bool status)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentGroups 
                              SET Status = @Status, IsDeleted = @IsDeleted, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
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
        public bool UpdatePaymentStatus(int studentId, int groupId, string paymentStatus)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentGroups 
                              SET PaymentStatus = @PaymentStatus, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// გადახდის თარიღის განახლება
        /// </summary>
        public bool UpdateDateOfPayment(int studentId, int groupId, DateTime? dateOfPayment)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentGroups 
                              SET DateOfPayment = @DateOfPayment, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@DateOfPayment", dateOfPayment.HasValue ? (object)dateOfPayment.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// გადახდის თარიღის განახლება (alias მეთოდი PaymentDateService-ისთვის)
        /// </summary>
        public bool UpdatePaymentDate(int studentId, int groupId, DateTime newDate)
        {
            return UpdateDateOfPayment(studentId, groupId, newDate);
        }

        /// <summary>
        /// გადახდის სტატუსის და თარიღის ერთდროული განახლება
        /// </summary>
        public bool UpdatePaymentStatusAndDate(int studentId, int groupId, string paymentStatus, DateTime? dateOfPayment)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentGroups 
                              SET PaymentStatus = @PaymentStatus, DateOfPayment = @DateOfPayment, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateOfPayment", dateOfPayment.HasValue ? (object)dateOfPayment.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// ფასდაკლების განახლება
        /// </summary>
        public bool UpdateDiscount(int studentId, int groupId, double discount)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentGroups 
                              SET Discount = @Discount, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@Discount", discount);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// ფასის განახლება
        /// </summary>
        public bool UpdatePrice(int studentId, int groupId, decimal price)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentGroups 
                              SET Price = @Price, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@Price", price);
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
                var query = @"UPDATE StudentGroups 
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
        public bool SoftDelete(int studentId, int groupId)
        {
            return UpdateStatus(studentId, groupId, false);
        }

        /// <summary>
        /// Hard Delete - სრული წაშლა
        /// </summary>
        public bool HardDelete(int studentId, int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "DELETE FROM StudentGroups WHERE StudentId = @StudentId AND GroupId = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის ყველა ჯგუფიდან Soft Delete
        /// </summary>
        public bool SoftDeleteAllByStudentId(int studentId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentGroups 
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

        #region ==================== IMPORT - იმპორტისთვის საჭირო მეთოდები ====================

        /// <summary>
        /// ყველა აქტიური მოსწავლე-ჯგუფის წყვილის მიღება (იმპორტის დროს დუბლიკატების შესამოწმებლად)
        /// </summary>
        public List<(int StudentId, int GroupId)> GetAllActiveStudentGroupPairs()
        {
            var pairs = new List<(int StudentId, int GroupId)>();

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT StudentId, GroupId FROM StudentGroups WHERE Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pairs.Add((reader.GetInt32("StudentId"), reader.GetInt32("GroupId")));
                    }
                }
            }
            return pairs;
        }

        #endregion

        #region ==================== HELPER - დამხმარე მეთოდები ====================

        /// <summary>
        /// StudentGroups ობიექტის შექმნა DataReader-დან
        /// </summary>
        private StudentGroups MapFromReader(MySqlDataReader reader)
        {
            return new StudentGroups
            {
                Id = reader.GetInt32("Id"),
                StudentId = reader.GetInt32("StudentId"),
                GroupId = reader.GetInt32("GroupId"),
                PaymentStatus = reader.IsDBNull(reader.GetOrdinal("PaymentStatus")) ? null : reader.GetString("PaymentStatus"),
                Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? 0 : reader.GetDecimal("Price"),
                Discount = reader.IsDBNull(reader.GetOrdinal("Discount")) ? 0 : reader.GetDouble("Discount"),
                Status = !reader.IsDBNull(reader.GetOrdinal("Status")) && reader.GetBoolean("Status"),
                IsDeleted = !reader.IsDBNull(reader.GetOrdinal("IsDeleted")) && reader.GetBoolean("IsDeleted"),
                DateOfPayment = reader.IsDBNull(reader.GetOrdinal("DateOfPayment")) ? (DateTime?)null : reader.GetDateTime("DateOfPayment"),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime("UpdatedAt")
            };
        }

        /// <summary>
        /// StudentGroups-ის mapper SubGroupId-ით (StudentSubGroups JOIN-ით)
        /// </summary>
        private StudentGroups MapFromReaderWithSubGroup(MySqlDataReader reader)
        {
            return new StudentGroups
            {
                Id = reader.GetInt32("Id"),
                StudentId = reader.GetInt32("StudentId"),
                GroupId = reader.GetInt32("GroupId"),
                PaymentStatus = reader.IsDBNull(reader.GetOrdinal("PaymentStatus")) ? null : reader.GetString("PaymentStatus"),
                Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? 0 : reader.GetDecimal("Price"),
                Discount = reader.IsDBNull(reader.GetOrdinal("Discount")) ? 0 : reader.GetDouble("Discount"),
                Status = !reader.IsDBNull(reader.GetOrdinal("Status")) && reader.GetBoolean("Status"),
                IsDeleted = !reader.IsDBNull(reader.GetOrdinal("IsDeleted")) && reader.GetBoolean("IsDeleted"),
                DateOfPayment = reader.IsDBNull(reader.GetOrdinal("DateOfPayment")) ? (DateTime?)null : reader.GetDateTime("DateOfPayment"),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime("UpdatedAt"),
                SubGroupId = reader.IsDBNull(reader.GetOrdinal("SubGroupId")) ? (int?)null : reader.GetInt32("SubGroupId")
            };
        }

        /// <summary>
        /// StudentGroups-ის mapper GetForPayment-ისთვის (SQL-ში არის SgPrice და GroupPrice, არა Price)
        /// </summary>
        private StudentGroups MapFromReaderForPayment(MySqlDataReader reader)
        {
            return new StudentGroups
            {
                Id = reader.GetInt32("Id"),
                StudentId = reader.GetInt32("StudentId"),
                GroupId = reader.GetInt32("GroupId"),
                PaymentStatus = reader.IsDBNull(reader.GetOrdinal("PaymentStatus")) ? null : reader.GetString("PaymentStatus"),
                Price = reader.IsDBNull(reader.GetOrdinal("SgPrice")) ? 0 : reader.GetDecimal("SgPrice"), // SgPrice-ს ვიყენებთ, შემდეგ GroupPrice-ით გადაიწერება
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




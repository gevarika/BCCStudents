using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;

namespace BCCStudents.Infrastructure.Repositories
{
    /// <summary>
    /// SubGroups ცხრილთან სამუშაო რეპოზიტორი
    /// ქვეჯგუფების მართვა
    /// </summary>
    public class SubGroupRepository : ISubGroupRepository
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public SubGroupRepository(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        #region ==================== INSERT - ქვეჯგუფის ჩასმა ====================

        /// <summary>
        /// ახალი ქვეჯგუფის დამატება
        /// </summary>
        public int AddSubGroup(SubGroup subGroup)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"INSERT INTO SubGroups (Name, GroupId, TuitionFee, MaxStudents, StudentCount, Status, UpdatedAt) 
                              VALUES (@Name, @GroupId, @TuitionFee, @MaxStudents, @StudentCount, @Status, @UpdatedAt);
                              SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", subGroup.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GroupId", subGroup.GroupId);
                    cmd.Parameters.AddWithValue("@TuitionFee", subGroup.TuitionFee);
                    cmd.Parameters.AddWithValue("@MaxStudents", subGroup.MaxStudents);
                    cmd.Parameters.AddWithValue("@StudentCount", subGroup.StudentCount);
                    cmd.Parameters.AddWithValue("@Status", subGroup.Status);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// ახალი ქვეჯგუფის დამატება (მინიმალური პარამეტრებით)
        /// </summary>
        public int AddSubGroup(string name, int groupId)
        {
            return AddSubGroup(new SubGroup
            {
                Name = name,
                GroupId = groupId,
                TuitionFee = 0,
                StudentCount = 0,
                Status = true
            });
        }

        /// <summary>
        /// ახალი ქვეჯგუფის დამატება (ფასით)
        /// </summary>
        public int AddSubGroup(string name, int groupId, decimal tuitionFee)
        {
            return AddSubGroup(new SubGroup
            {
                Name = name,
                GroupId = groupId,
                TuitionFee = tuitionFee,
                StudentCount = 0,
                Status = true
            });
        }

        #endregion

        #region ==================== SELECT - ქვეჯგუფის წაკითხვა ====================

        /// <summary>
        /// ქვეჯგუფის მიღება ID-ით
        /// </summary>
        public SubGroup GetSubGroupById(int subGroupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT sg.*, g.Name AS ParentGroupName 
                              FROM SubGroups sg
                              LEFT JOIN `Groups` g ON sg.GroupId = g.Id
                              WHERE sg.Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", subGroupId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapSubGroupFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// ქვეჯგუფის მიღება ნომრით და ჯგუფის ID-ით
        /// </summary>
        public SubGroup GetSubGroupByNumber(int groupId, int subGroupNumber)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT sg.*, g.Name AS ParentGroupName 
                              FROM SubGroups sg
                              LEFT JOIN `Groups` g ON sg.GroupId = g.Id
                              WHERE sg.GroupId = @GroupId AND sg.Id = @SubGroupNumber";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@SubGroupNumber", subGroupNumber);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapSubGroupFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// ქვეჯგუფის მიღება სახელით და ჯგუფის ID-ით
        /// </summary>
        public SubGroup GetSubGroupByName(int groupId, string name)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT sg.*, g.Name AS ParentGroupName 
                              FROM SubGroups sg
                              LEFT JOIN `Groups` g ON sg.GroupId = g.Id
                              WHERE sg.GroupId = @GroupId AND sg.Name = @Name";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@Name", name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapSubGroupFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// ჯგუფის პირველი ქვეჯგუფის მიღება
        /// </summary>
        public SubGroup GetFirstSubGroupByGroupId(int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT sg.*, g.Name AS ParentGroupName 
                              FROM SubGroups sg
                              LEFT JOIN `Groups` g ON sg.GroupId = g.Id
                              WHERE sg.GroupId = @GroupId AND sg.Status = 1
                              ORDER BY sg.Id ASC
                              LIMIT 1";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapSubGroupFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// ყველა ქვეჯგუფის მიღება
        /// </summary>
        public List<SubGroup> GetAllSubGroups()
        {
            var subGroups = new List<SubGroup>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT sg.*, g.Name AS ParentGroupName 
                              FROM SubGroups sg
                              LEFT JOIN `Groups` g ON sg.GroupId = g.Id
                              ORDER BY g.Name, sg.Name";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        subGroups.Add(MapSubGroupFromReader(reader));
                    }
                }
            }
            return subGroups;
        }

        /// <summary>
        /// ყველა აქტიური ქვეჯგუფის მიღება
        /// </summary>
        public List<SubGroup> GetAllActiveSubGroups()
        {
            var subGroups = new List<SubGroup>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT sg.*, g.Name AS ParentGroupName 
                              FROM SubGroups sg
                              LEFT JOIN `Groups` g ON sg.GroupId = g.Id
                              WHERE sg.Status = 1
                              ORDER BY g.Name, sg.Name";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        subGroups.Add(MapSubGroupFromReader(reader));
                    }
                }
            }
            return subGroups;
        }

        /// <summary>
        /// ქვეჯგუფების მიღება ჯგუფის ID-ით
        /// </summary>
        public List<SubGroup> GetSubGroupsByGroupId(int groupId)
        {
            var subGroups = new List<SubGroup>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT sg.*, g.Name AS ParentGroupName 
                              FROM SubGroups sg
                              LEFT JOIN `Groups` g ON sg.GroupId = g.Id
                              WHERE sg.GroupId = @GroupId AND sg.Status = 1
                              ORDER BY sg.Name";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            subGroups.Add(MapSubGroupFromReader(reader));
                        }
                    }
                }
            }
            return subGroups;
        }

        /// <summary>
        /// ჯგუფის ყველა ქვეჯგუფის მიღება (აქტიური და არააქტიური) — სინქრონიზაციისთვის
        /// </summary>
        public List<SubGroup> GetAllSubGroupsByGroupId(int groupId)
        {
            var subGroups = new List<SubGroup>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT sg.*, g.Name AS ParentGroupName 
                              FROM SubGroups sg
                              LEFT JOIN `Groups` g ON sg.GroupId = g.Id
                              WHERE sg.GroupId = @GroupId AND (sg.IsDeleted = 0 OR sg.IsDeleted IS NULL)
                              ORDER BY sg.Name";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            subGroups.Add(MapSubGroupFromReader(reader));
                        }
                    }
                }
            }
            return subGroups;
        }

        /// <summary>
        /// ქვეჯგუფების მიღება DataTable-ად
        /// </summary>
        public DataTable GetAllSubGroupsFor()
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT sg.Id, sg.Name, sg.TuitionFee, sg.StudentCount, sg.Status, 
                              g.Name AS ParentGroupName, sg.GroupId
                              FROM SubGroups sg
                              LEFT JOIN `Groups` g ON sg.GroupId = g.Id
                              WHERE sg.Status = 1
                              ORDER BY g.Name, sg.Name";

                using (var adapter = new MySqlDataAdapter(query, connection))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        /// <summary>
        /// მოსწავლის ქვეჯგუფების მიღება
        /// </summary>
        public List<SubGroup> GetStudentSubGroupsByStudentId(int studentId)
        {
            var subGroups = new List<SubGroup>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT sg.*, g.Name AS ParentGroupName 
                              FROM SubGroups sg
                              INNER JOIN StudentSubGroups ssg ON sg.Id = ssg.SubGroupId
                              LEFT JOIN `Groups` g ON sg.GroupId = g.Id
                              WHERE ssg.StudentId = @StudentId AND ssg.Status = 1 
                              AND (ssg.IsDeleted = 0 OR ssg.IsDeleted IS NULL)
                              ORDER BY g.Name, sg.Name";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            subGroups.Add(MapSubGroupFromReader(reader));
                        }
                    }
                }
            }
            return subGroups;
        }

        /// <summary>
        /// მოსწავლის მიმდინარე ქვეჯგუფის ID-ის მიღება
        /// </summary>
        public int GetCurrentStudentSubGroupId(int studentId, int groupId, bool status)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT SubGroupId FROM StudentSubGroups 
                              WHERE StudentId = @StudentId AND GroupId = @GroupId 
                              AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)
                              ORDER BY Id DESC LIMIT 1";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        /// <summary>
        /// ქვეჯგუფში მოსწავლეთა რაოდენობა
        /// </summary>
        public int GetStudentCountInSubGroup(int subGroupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT COUNT(*) FROM StudentSubGroups 
                              WHERE SubGroupId = @SubGroupId AND Status = 1 
                              AND (IsDeleted = 0 OR IsDeleted IS NULL)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// ქვეჯგუფის არსებობის შემოწმება
        /// </summary>
        public bool SubGroupExists(int subGroupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM SubGroups WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", subGroupId);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        /// <summary>
        /// ქვეჯგუფის არსებობის შემოწმება სახელით
        /// </summary>
        public bool SubGroupExistsByName(int groupId, string name)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM SubGroups WHERE GroupId = @GroupId AND Name = @Name";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@Name", name);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        #endregion

        #region ==================== UPDATE - ქვეჯგუფის განახლება (სრული) ====================

        /// <summary>
        /// ქვეჯგუფის სრული განახლება
        /// </summary>
        public bool UpdateSubGroup(SubGroup subGroup)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE SubGroups SET 
                              Name = @Name, TuitionFee = @TuitionFee, StudentCount = @StudentCount, 
                              Status = @Status, UpdatedAt = @UpdatedAt
                              WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", subGroup.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TuitionFee", subGroup.TuitionFee);
                    cmd.Parameters.AddWithValue("@StudentCount", subGroup.StudentCount);
                    cmd.Parameters.AddWithValue("@Status", subGroup.Status);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id", subGroup.Id);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== UPDATE - ცალკეული ველების განახლება ====================

        /// <summary>
        /// ქვეჯგუფის სახელის განახლება
        /// </summary>
        public bool UpdateSubGroupName(int subGroupId, string newName)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE SubGroups SET Name = @Name, UpdatedAt = @UpdatedAt WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", newName);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id", subGroupId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// ქვეჯგუფის ფასის განახლება
        /// </summary>
        public bool UpdateSubGroupTuitionFee(int subGroupId, decimal newFee)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE SubGroups SET TuitionFee = @TuitionFee, UpdatedAt = @UpdatedAt WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@TuitionFee", newFee);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id", subGroupId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// ქვეჯგუფის სტატუსის განახლება
        /// </summary>
        public bool UpdateSubGroupStatus(int subGroupId, bool status)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE SubGroups SET Status = @Status, UpdatedAt = @UpdatedAt WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id", subGroupId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// ჯგუფის ყველა ქვეჯგუფის სტატუსის განახლება
        /// </summary>
        public bool UpdateSubGroupsStatusByGroupId(int groupId, bool status)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE SubGroups SET Status = @Status, UpdatedAt = @UpdatedAt WHERE GroupId = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// ჯგუფის ყველა ქვეჯგუფის ფასის განახლება
        /// </summary>
        public bool UpdateSubGroupsTuitionFeeByGroupId(int groupId, decimal newFee)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE SubGroups SET TuitionFee = @TuitionFee, UpdatedAt = @UpdatedAt WHERE GroupId = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@TuitionFee", newFee);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== UPDATE - მოსწავლეთა რაოდენობის მართვა ====================

        /// <summary>
        /// მოსწავლეთა რაოდენობის განახლება
        /// </summary>
        public bool UpdateSubGroupStudentCount(int subGroupId, int count)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE SubGroups SET StudentCount = @Count, UpdatedAt = @UpdatedAt WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Count", count);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id", subGroupId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლეთა რაოდენობის გაზრდა 1-ით
        /// </summary>
        public bool IncrementSubGroupCount(int subGroupId, MySqlConnection externalConnection = null, MySqlTransaction externalTransaction = null)
        {
            bool useExternal = externalConnection != null;
            var connection = useExternal ? externalConnection : _connectionProvider.GetLocalConnection();

            try
            {
                if (!useExternal) connection.Open();

                var query = "UPDATE SubGroups SET StudentCount = StudentCount + 1, UpdatedAt = @UpdatedAt WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection, externalTransaction))
                {
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id", subGroupId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            finally
            {
                if (!useExternal && connection != null)
                    connection.Dispose();
            }
        }

        /// <summary>
        /// მოსწავლეთა რაოდენობის შემცირება 1-ით
        /// </summary>
        public bool DecreaseStudentCount(int subGroupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE SubGroups SET StudentCount = GREATEST(StudentCount - 1, 0), UpdatedAt = @UpdatedAt WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id", subGroupId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== UPDATE - StudentSubGroups ცხრილთან მუშაობა ====================

        /// <summary>
        /// მოსწავლის ქვეჯგუფის განახლება
        /// </summary>
        public bool UpdateStudentSubGroup(int studentId, int groupId, int newSubGroupId, int oldSubGroupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentSubGroups SET SubGroupId = @NewSubGroupId, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId AND SubGroupId = @OldSubGroupId
                              AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@NewSubGroupId", newSubGroupId);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@OldSubGroupId", oldSubGroupId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის ქვეჯგუფის გადახდის სტატუსის განახლება
        /// </summary>
        public bool UpdateStudentSubGroupPaymentStatus(int studentId, int groupId, int subGroupId, string status)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentSubGroups SET PaymentStatus = @Status, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId AND SubGroupId = @SubGroupId
                              AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Status", status ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის ქვეჯგუფის გადახდის თარიღის განახლება
        /// </summary>
        public void UpdateStudentSubGroupPaymentDate(int studentId, int groupId, int subGroupId, DateTime paymentDate)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentSubGroups SET DateOfPayment = @PaymentDate, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId AND SubGroupId = @SubGroupId
                              AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PaymentDate", paymentDate);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// მოსწავლის ქვეჯგუფის სტატუსის განახლება
        /// </summary>
        public bool UpdateStudentSubGroupStatus(int groupId, int studentId, int subGroupId, bool status)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentSubGroups SET Status = @Status, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId AND SubGroupId = @SubGroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== DELETE - ქვეჯგუფის წაშლა ====================

        /// <summary>
        /// ქვეჯგუფის წაშლა (Soft Delete)
        /// </summary>
        public bool DeleteSubGroup(int subGroupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE SubGroups SET Status = 0, UpdatedAt = @UpdatedAt WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id", subGroupId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// ქვეჯგუფის სრული წაშლა (Hard Delete)
        /// </summary>
        public bool HardDeleteSubGroup(int subGroupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "DELETE FROM SubGroups WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", subGroupId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლის ქვეჯგუფიდან ამოღება
        /// </summary>
        public bool DeleteStudentFromSubGroup(int studentId, int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE StudentSubGroups SET Status = 0, IsDeleted = 1, UpdatedAt = @UpdatedAt
                              WHERE StudentId = @StudentId AND GroupId = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== HELPER - დამხმარე მეთოდები ====================

        /// <summary>
        /// SubGroup ობიექტის შექმნა DataReader-დან
        /// </summary>
        private SubGroup MapSubGroupFromReader(MySqlDataReader reader)
        {
            return new SubGroup
            {
                Id = reader.GetInt32("Id"),
                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString("Name"),
                GroupId = reader.GetInt32("GroupId"),
                TuitionFee = reader.IsDBNull(reader.GetOrdinal("TuitionFee")) ? 0 : reader.GetDecimal("TuitionFee"),
                StudentCount = reader.IsDBNull(reader.GetOrdinal("StudentCount")) ? 0 : reader.GetInt32("StudentCount"),
                Status = !reader.IsDBNull(reader.GetOrdinal("Status")) && reader.GetBoolean("Status"),
                ParentGroupName = HasColumn(reader, "ParentGroupName") && !reader.IsDBNull(reader.GetOrdinal("ParentGroupName"))
                    ? reader.GetString("ParentGroupName") : null,
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime("UpdatedAt")
            };
        }

        /// <summary>
        /// შეამოწმებს არსებობს თუ არა სვეტი reader-ში
        /// </summary>
        private bool HasColumn(MySqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        #endregion
    }
}



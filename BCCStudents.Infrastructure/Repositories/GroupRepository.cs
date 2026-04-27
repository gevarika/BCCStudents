using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;

namespace BCCStudents.Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public GroupRepository(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        #region ==================== INSERT - ჯგუფის ჩასმა ====================

        /// <summary>
        /// ახალი ჯგუფის ჩასმა (სრული ობიექტით)
        /// </summary>
        /// <param name="group">ჯგუფის ობიექტი</param>
        /// <returns>ახალი ჯგუფის ID</returns>
        public int InsertGroup(Group group)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"INSERT INTO `Groups` (Name, Price, Teacher, ContractTemplatePath, Status, StudentCount, MaxStudents, UpdatedAt) 
                              VALUES (@Name, @Price, @Teacher, @ContractTemplatePath, @Status, @StudentCount, @MaxStudents, @UpdatedAt); 
                              SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", group.Name);
                    cmd.Parameters.AddWithValue("@Price", group.Price);
                    cmd.Parameters.AddWithValue("@Teacher", group.Teacher ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContractTemplatePath", group.ContractTemplatePath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", group.Status);
                    cmd.Parameters.AddWithValue("@StudentCount", group.StudentCount);
                    cmd.Parameters.AddWithValue("@MaxStudents", group.MaxStudents);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// ახალი ჯგუფის ჩასმა (მხოლოდ სახელით, დანარჩენი default მნიშვნელობებით)
        /// </summary>
        /// <param name="name">ჯგუფის სახელი</param>
        /// <returns>ახალი ჯგუფის ID</returns>
        public int InsertGroupByName(string name)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"INSERT INTO `Groups` (Name, Price, Status, StudentCount, UpdatedAt) 
                              VALUES (@Name, 0, 1, 0, @UpdatedAt); 
                              SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// ახალი ჯგუფის ჩასმა (სახელი და ფასი)
        /// </summary>
        /// <param name="name">ჯგუფის სახელი</param>
        /// <param name="price">ჯგუფის ფასი</param>
        /// <returns>ახალი ჯგუფის ID</returns>
        public int InsertGroupWithPrice(string name, decimal price)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"INSERT INTO `Groups` (Name, Price, Status, StudentCount, UpdatedAt) 
                              VALUES (@Name, @Price, 1, 0, @UpdatedAt); 
                              SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// ახალი ჯგუფის ჩასმა (სახელი, ფასი და მასწავლებელი)
        /// </summary>
        /// <param name="name">ჯგუფის სახელი</param>
        /// <param name="price">ჯგუფის ფასი</param>
        /// <param name="teacher">მასწავლებელი</param>
        /// <returns>ახალი ჯგუფის ID</returns>
        public int InsertGroupWithTeacher(string name, decimal price, string teacher)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"INSERT INTO `Groups` (Name, Price, Teacher, Status, StudentCount, UpdatedAt) 
                              VALUES (@Name, @Price, @Teacher, 1, 0, @UpdatedAt); 
                              SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Teacher", teacher ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        #endregion

        #region ==================== SELECT - ჯგუფის წაკითხვა ====================

        /// <summary>
        /// ჯგუფის მიღება ID-ით
        /// </summary>
        public Group GetGroupById(int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, Name, Price, Teacher, Status, StudentCount, MaxStudents, ContractTemplatePath, UpdatedAt 
                              FROM `Groups` WHERE Id = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapGroupFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// ჯგუფის მიღება სახელით
        /// </summary>
        public Group GetGroupByName(string name)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, Name, Price, Teacher, Status, StudentCount, MaxStudents, ContractTemplatePath, UpdatedAt 
                              FROM `Groups` WHERE Name = @Name";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapGroupFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// ყველა ჯგუფის მიღება
        /// </summary>
        public List<Group> GetAllGroups()
        {
            var groups = new List<Group>();

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, Name, Price, Teacher, Status, StudentCount, MaxStudents, ContractTemplatePath, UpdatedAt 
                              FROM `Groups` ORDER BY Name";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        groups.Add(MapGroupFromReader(reader));
                    }
                }
            }
            return groups;
        }

        /// <summary>
        /// მხოლოდ აქტიური ჯგუფების მიღება (Status = 1)
        /// </summary>
        public List<Group> GetAllActiveGroups()
        {
            var groups = new List<Group>();

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, Name, Price, Teacher, Status, StudentCount,MaxStudents,  ContractTemplatePath, UpdatedAt 
                              FROM `Groups` WHERE Status = 1 ORDER BY Name";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        groups.Add(MapGroupFromReader(reader));
                    }
                }
            }
            return groups;
        }

        /// <summary>
        /// ჯგუფების მიღება ID-ების სიით
        /// </summary>
        public List<Group> GetGroupsByIds(List<int> ids)
        {
            var groups = new List<Group>();
            if (ids == null || ids.Count == 0) return groups;

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var idList = string.Join(",", ids);
                var query = $@"SELECT Id, Name, Price, Teacher, Status, StudentCount, MaxStudents, ContractTemplatePath, UpdatedAt 
                               FROM `Groups` WHERE Id IN ({idList})";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        groups.Add(MapGroupFromReader(reader));
                    }
                }
            }
            return groups;
        }

        /// <summary>
        /// ჯგუფის სახელის მიღება ID-ით
        /// </summary>
        public string GetGroupNameById(int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT Name FROM `Groups` WHERE Id = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    var result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }

        /// <summary>
        /// ჯგუფის ფასის მიღება ID-ით
        /// </summary>
        public decimal GetGroupPrice(int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT Price FROM `Groups` WHERE Id = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToDecimal(result) : 0;
                }
            }
        }

        /// <summary>
        /// ჯგუფის მოსწავლეთა რაოდენობის მიღება ID-ით
        /// </summary>
        public int GetGroupStudentCountById(int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT StudentCount FROM `Groups` WHERE Id = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        /// <summary>
        /// ჯგუფის მოსწავლეთა რაოდენობის მიღება სახელით
        /// </summary>
        public int GetGroupStudentCountByName(string name)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT StudentCount FROM `Groups` WHERE Name = @Name";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        /// <summary>
        /// ჯგუფების მიღება DataTable-ად (ComboBox-ებისთვის)
        /// </summary>
        public DataTable GetGroupsAsDataTable()
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT Id, Name FROM `Groups` ORDER BY Name";
                using (var adapter = new MySqlDataAdapter(query, connection))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        /// <summary>
        /// ჯგუფების ჩატვირთვა Dictionary-ში (სახელი -> ID-ების სია)
        /// </summary>
        public void LoadGroupsToDictionary(Dictionary<string, List<int>> groupIds)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var cmd = new MySqlCommand("SELECT Id, Name FROM `Groups`", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString("Name");
                        var id = reader.GetInt32("Id");
                        if (!groupIds.ContainsKey(name))
                        {
                            groupIds.Add(name, new List<int> { id });
                        }
                        else
                        {
                            groupIds[name].Add(id);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Groups ცხრილის ცარიელობის შემოწმება
        /// </summary>
        public bool IsGroupsTableEmpty()
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var cmd = new MySqlCommand("SELECT EXISTS (SELECT 1 FROM `Groups` LIMIT 1)", connection);
                var result = cmd.ExecuteScalar();
                return !Convert.ToBoolean(result);
            }
        }

        #endregion

        #region ==================== UPDATE - ჯგუფის განახლება (სრული) ====================

        /// <summary>
        /// ჯგუფის სრული განახლება (ყველა ველი)
        /// </summary>
        public bool UpdateGroup(Group group)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"UPDATE `Groups` 
                              SET Name = @Name, Price = @Price, Teacher = @Teacher, 
                                  ContractTemplatePath = @ContractTemplatePath, Status = @Status,
                                  StudentCount = @StudentCount, MaxStudents = @MaxStudents, UpdatedAt = @UpdatedAt
                              WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", group.Id);
                    cmd.Parameters.AddWithValue("@Name", group.Name);
                    cmd.Parameters.AddWithValue("@Price", group.Price);
                    cmd.Parameters.AddWithValue("@Teacher", group.Teacher ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContractTemplatePath", group.ContractTemplatePath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", group.Status);
                    cmd.Parameters.AddWithValue("@StudentCount", group.StudentCount);
                    cmd.Parameters.AddWithValue("@MaxStudents", group.MaxStudents);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== UPDATE - ჯგუფის ცალკეული ველების განახლება ====================

        /// <summary>
        /// ჯგუფის სახელის განახლება
        /// </summary>
        public bool UpdateGroupName(int groupId, string newName)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE `Groups` SET Name = @Name, UpdatedAt = @UpdatedAt WHERE Id = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@Name", newName);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// ჯგუფის ფასის განახლება
        /// </summary>
        public bool UpdateGroupPrice(int groupId, decimal newPrice)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE `Groups` SET Price = @Price, UpdatedAt = @UpdatedAt WHERE Id = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@Price", newPrice);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// ჯგუფის მასწავლებლის განახლება
        /// </summary>
        public bool UpdateGroupTeacher(int groupId, string newTeacher)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE `Groups` SET Teacher = @Teacher, UpdatedAt = @UpdatedAt WHERE Id = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@Teacher", newTeacher ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// ჯგუფის სტატუსის განახლება (აქტიური/არააქტიური)
        /// </summary>
        public bool UpdateGroupStatus(int groupId, bool newStatus)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE `Groups` SET Status = @Status, UpdatedAt = @UpdatedAt WHERE Id = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@Status", newStatus);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// ჯგუფის კონტრაქტის შაბლონის პათის განახლება
        /// </summary>
        public bool UpdateGroupContractTemplatePath(int groupId, string newPath)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE `Groups` SET ContractTemplatePath = @Path, UpdatedAt = @UpdatedAt WHERE Id = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@Path", newPath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// ჯგუფის მაქსიმალური მოსწავლეების რაოდენობის განახლება
        /// </summary>
        public bool UpdateGroupMaxStudents(int groupId, int newMaxStudents)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE `Groups` SET MaxStudents = @MaxStudents, UpdatedAt = @UpdatedAt WHERE Id = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@MaxStudents", newMaxStudents);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== UPDATE - მოსწავლეთა რაოდენობის მართვა ====================

        /// <summary>
        /// მოსწავლეთა რაოდენობის გაზრდა 1-ით
        /// </summary>
        public bool IncrementStudentCount(int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE `Groups` SET StudentCount = StudentCount + 1, UpdatedAt = @UpdatedAt WHERE Id = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლეთა რაოდენობის შემცირება 1-ით
        /// </summary>
        public bool DecrementStudentCount(int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "UPDATE `Groups` SET StudentCount = StudentCount - 1, UpdatedAt = @UpdatedAt WHERE Id = @GroupId AND StudentCount > 0";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// მოსწავლეთა რაოდენობის ხელახალი გამოთვლა StudentGroups ცხრილიდან
        /// </summary>
        public bool RecalculateStudentCount(int groupId, MySqlConnection externalConnection = null, MySqlTransaction externalTransaction = null)
        {
            bool useExternal = externalConnection != null;
            var connection = useExternal ? externalConnection : _connectionProvider.GetLocalConnection();

            try
            {
                if (!useExternal) connection.Open();

                // დავთვალოთ აქტიური მოსწავლეები
                var countQuery = "SELECT COUNT(*) FROM StudentGroups WHERE GroupId = @GroupId AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)";
                int studentCount;

                using (var countCmd = new MySqlCommand(countQuery, connection))
                {
                    if (externalTransaction != null) countCmd.Transaction = externalTransaction;
                    countCmd.Parameters.AddWithValue("@GroupId", groupId);
                    studentCount = Convert.ToInt32(countCmd.ExecuteScalar());
                }

                // განვაახლოთ ჯგუფის StudentCount
                var updateQuery = "UPDATE `Groups` SET StudentCount = @Count, UpdatedAt = @UpdatedAt WHERE Id = @GroupId";
                using (var updateCmd = new MySqlCommand(updateQuery, connection))
                {
                    if (externalTransaction != null) updateCmd.Transaction = externalTransaction;
                    updateCmd.Parameters.AddWithValue("@Count", studentCount);
                    updateCmd.Parameters.AddWithValue("@GroupId", groupId);
                    updateCmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    return updateCmd.ExecuteNonQuery() > 0;
                }
            }
            finally
            {
                if (!useExternal && connection != null)
                    connection.Dispose();
            }
        }

        #endregion

        #region ==================== DELETE - ჯგუფის წაშლა ====================

        /// <summary>
        /// ჯგუფის წაშლა (Soft Delete - სტატუსის შეცვლა 0-ზე)
        /// </summary>
        public bool DeleteGroup(int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // ჯგუფის სტატუსის შეცვლა
                        var updateGroupQuery = "UPDATE `Groups` SET Status = 0, UpdatedAt = @UpdatedAt WHERE Id = @GroupId";
                        using (var cmd = new MySqlCommand(updateGroupQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@GroupId", groupId);
                            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }

                        // ქვეჯგუფების სტატუსის შეცვლა
                        var updateSubGroupsQuery = "UPDATE `SubGroups` SET Status = 0, UpdatedAt = @UpdatedAt WHERE GroupId = @GroupId";
                        using (var cmd = new MySqlCommand(updateSubGroupsQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@GroupId", groupId);
                            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// ჯგუფის სრული წაშლა (Hard Delete - ჩანაწერის წაშლა)
        /// გამოიყენეთ ფრთხილად!
        /// </summary>
        public bool HardDeleteGroup(int groupId)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "DELETE FROM `Groups` WHERE Id = @GroupId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region ==================== HELPER - დამხმარე მეთოდები ====================

        /// <summary>
        /// Group ობიექტის შექმნა DataReader-დან
        /// </summary>
        private Group MapGroupFromReader(MySqlDataReader reader)
        {
            return new Group
            {
                Id = reader.GetInt32("Id"),
                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString("Name"),
                Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? 0 : reader.GetDecimal("Price"),
                Teacher = reader.IsDBNull(reader.GetOrdinal("Teacher")) ? null : reader.GetString("Teacher"),
                Status = !reader.IsDBNull(reader.GetOrdinal("Status")) && reader.GetBoolean("Status"),
                StudentCount = reader.IsDBNull(reader.GetOrdinal("StudentCount")) ? 0 : reader.GetInt32("StudentCount"),
                MaxStudents = reader.IsDBNull(reader.GetOrdinal("MaxStudents")) ? 0 : reader.GetInt32("MaxStudents"),
                ContractTemplatePath = reader.IsDBNull(reader.GetOrdinal("ContractTemplatePath")) ? null : reader.GetString("ContractTemplatePath"),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime("UpdatedAt")
            };
        }

        #endregion
    }
}



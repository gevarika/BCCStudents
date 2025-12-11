using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;
using BCCStudents.Infrastructure.Data;
using MySql.Data.MySqlClient;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Application.Interfaces;

namespace BCCStudents.Application.Services.Sync.DownStream
{
    /// <summary>
    /// áƒ˜áƒ¦áƒ”áƒ‘áƒ¡ áƒ›áƒáƒœáƒáƒªáƒ”áƒ›áƒ”áƒ‘áƒ¡ áƒ¡áƒ”áƒ áƒ•áƒ”áƒ áƒ˜áƒ¡ MySQL áƒ‘áƒáƒ–áƒ˜áƒ“áƒáƒœ UpdatedAt + Id áƒ¤áƒ˜áƒšáƒ¢áƒ áƒ˜áƒ¡ áƒ›áƒ˜áƒ®áƒ”áƒ“áƒ•áƒ˜áƒ—.
    /// </summary>
    public class DownStreamDataFetcher : IDownStreamDataFetcher
    {
        private readonly DatabaseHelper _databaseHelper;
        private readonly ISyncLogger _logger;

        public DownStreamDataFetcher(DatabaseHelper databaseHelper, ISyncLogger logger)
        {
            _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<List<Student>> FetchStudentsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT Id, FirstName, LastName, Age, ParentName, PhoneNumber, Id_Numb, Address,
                                        RegistrationDate, StudentCode, Info, user_id, Balance, UpdatedAt
                                 FROM Students
                                 WHERE (@LastSyncedAt IS NULL)
                                    OR (UpdatedAt > @LastSyncedAt)
                                    OR (UpdatedAt = @LastSyncedAt AND Id > @LastSyncedId)
                                 ORDER BY UpdatedAt ASC, Id ASC;";
            return Task.FromResult(ExecuteReader("Students", sql, lastSyncedAt, lastSyncedId, MapStudent, cancellationToken));
        }

        public Task<List<Group>> FetchGroupsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT Id, Name, Price, Teacher, Status, StudentCount,
                                        ContractTemplatePath, UpdatedAt
                                 FROM `Groups`
                                 WHERE (@LastSyncedAt IS NULL)
                                    OR (UpdatedAt > @LastSyncedAt)
                                    OR (UpdatedAt = @LastSyncedAt AND Id > @LastSyncedId)
                                 ORDER BY UpdatedAt ASC, Id ASC;";
            return Task.FromResult(ExecuteReader("Groups", sql, lastSyncedAt, lastSyncedId, MapGroup, cancellationToken));
        }

        public Task<List<SubGroup>> FetchSubGroupsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            // áƒ¡áƒ”áƒ áƒ•áƒ”áƒ áƒ–áƒ” SubGroups áƒªáƒ®áƒ áƒ˜áƒšáƒ¡ áƒáƒ  áƒáƒ¥áƒ•áƒ¡ ActiveStatus áƒ¡áƒ•áƒ”áƒ¢áƒ˜, áƒáƒ›áƒ˜áƒ¢áƒáƒ› áƒ›áƒ®áƒáƒšáƒáƒ“ Status-áƒ¡ áƒ•áƒ™áƒ˜áƒ—áƒ®áƒ£áƒšáƒáƒ‘áƒ—
            const string sql = @"SELECT Id, Name, GroupId, TuitionFee, StudentCount, Status, UpdatedAt
                                 FROM SubGroups
                                 WHERE (@LastSyncedAt IS NULL)
                                    OR (UpdatedAt > @LastSyncedAt)
                                    OR (UpdatedAt = @LastSyncedAt AND Id > @LastSyncedId)
                                 ORDER BY UpdatedAt ASC, Id ASC;";
            return Task.FromResult(ExecuteReader("SubGroups", sql, lastSyncedAt, lastSyncedId, MapSubGroup, cancellationToken));
        }

        public Task<List<StudentGroups>> FetchStudentGroupsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT ID AS Id, StudentId, GroupId, PaymentStatus, DateOfPayment,
                                        Price, Discount, Status, UpdatedAt
                                 FROM StudentGroups
                                 WHERE (@LastSyncedAt IS NULL)
                                    OR (UpdatedAt > @LastSyncedAt)
                                    OR (UpdatedAt = @LastSyncedAt AND ID > @LastSyncedId)
                                 ORDER BY UpdatedAt ASC, ID ASC;";
            return Task.FromResult(ExecuteReader("StudentGroups", sql, lastSyncedAt, lastSyncedId, MapStudentGroup, cancellationToken));
        }

        public Task<List<StudentSubGroups>> FetchStudentSubGroupsAsync(DateTime? lastSyncedAt, int lastSyncedId, CancellationToken cancellationToken = default)
        {
            const string sql = @"SELECT Id, StudentId, GroupId, SubGroupId, Status, PaymentStatus,
                                        DateOfPayment, Price, discount AS Discount, UpdatedAt
                                 FROM StudentSubGroups
                                 WHERE (@LastSyncedAt IS NULL)
                                    OR (UpdatedAt > @LastSyncedAt)
                                    OR (UpdatedAt = @LastSyncedAt AND Id > @LastSyncedId)
                                 ORDER BY UpdatedAt ASC, Id ASC;";
            return Task.FromResult(ExecuteReader("StudentSubGroups", sql, lastSyncedAt, lastSyncedId, MapStudentSubGroup, cancellationToken));
        }

        private List<T> ExecuteReader<T>(string tableName, string sql, DateTime? lastSyncedAt, int lastSyncedId, Func<MySqlDataReader, T> mapper, CancellationToken cancellationToken)
        {
            var results = new List<T>();
            try
            {
                using (var connection = _databaseHelper.GetServerConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@LastSyncedAt", (object)lastSyncedAt ?? DBNull.Value);
                        command.Parameters.AddWithValue("@LastSyncedId", lastSyncedId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                results.Add(mapper(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"DownStreamDataFetcher query failed for table {tableName}.", ex);
                throw;
            }

            return results;
        }

        #region Mappers

        private static Student MapStudent(MySqlDataReader reader)
        {
            return new Student
            {
                Id = reader.GetInt32("Id"),
                FirstName = reader["FirstName"]?.ToString(),
                LastName = reader["LastName"]?.ToString(),
                Age = reader["Age"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Age"]),
                ParentName = reader["ParentName"]?.ToString(),
                PhoneNumber = reader["PhoneNumber"]?.ToString(),
                Id_Numb = reader["Id_Numb"] == DBNull.Value ? 0 : Convert.ToInt64(reader["Id_Numb"]),
                Address = reader["Address"]?.ToString(),
                RegistrationDate = reader["RegistrationDate"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["RegistrationDate"]),
                StudentCode = reader["StudentCode"]?.ToString(),
                Info = reader["Info"]?.ToString(),
                User_Id = reader["user_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["user_id"]),
                Balance = reader["Balance"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Balance"]),
                UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["UpdatedAt"])
            };
        }

        private static Group MapGroup(MySqlDataReader reader)
        {
            return new Group
            {
                Id = reader.GetInt32("Id"),
                Name = reader["Name"]?.ToString(),
                Price = reader["Price"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Price"]),
                Teacher = reader["Teacher"]?.ToString(),
                Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"]),
                StudentCount = reader["StudentCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["StudentCount"]),
                ContractTemplatePath = reader["ContractTemplatePath"]?.ToString(),
                UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["UpdatedAt"])
            };
        }

        private static SubGroup MapSubGroup(MySqlDataReader reader)
        {
            return new SubGroup
            {
                Id = reader.GetInt32("Id"),
                Name = reader["Name"]?.ToString(),
                GroupId = reader["GroupId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GroupId"]),
                TuitionFee = reader["TuitionFee"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TuitionFee"]),
                StudentCount = reader["StudentCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["StudentCount"]),
                Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"]),
                //Status = reader["ActiveStatus"] != DBNull.Value && Convert.ToBoolean(reader["ActiveStatus"]),
                UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["UpdatedAt"])
            };
        }

        private static StudentGroups MapStudentGroup(MySqlDataReader reader)
        {
            return new StudentGroups
            {
                Id = reader.GetInt32("Id"),
                StudentId = reader["StudentId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["StudentId"]),
                GroupId = reader["GroupId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GroupId"]),
                PaymentStatus = reader["PaymentStatus"]?.ToString(),
                DateOfPayment = reader["DateOfPayment"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DateOfPayment"]),
                Price = reader["Price"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Price"]),
                Discount = reader["Discount"] == DBNull.Value ? 0 : Convert.ToDouble(reader["Discount"]),
                Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"]),
                UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["UpdatedAt"])
            };
        }

        private static StudentSubGroups MapStudentSubGroup(MySqlDataReader reader)
        {
            return new StudentSubGroups
            {
                Id = reader.GetInt32("Id"),
                StudentId = reader["StudentId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["StudentId"]),
                GroupId = reader["GroupId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GroupId"]),
                SubGroupId = reader["SubGroupId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SubGroupId"]),
                Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"]),
                PaymentStatus = reader["PaymentStatus"] == DBNull.Value ? null : reader.GetString("PaymentStatus"),
                DateOfPayment = reader["DateOfPayment"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DateOfPayment"]),
                Price = reader["Price"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Price"]),
                Discount = reader["Discount"] == DBNull.Value ? 0 : Convert.ToDouble(reader["Discount"]),
                UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? DateTime.UtcNow : Convert.ToDateTime(reader["UpdatedAt"])
            };
        }

        #endregion
    }
}



using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace BCCStudents.Infrastructure.Repositories
{
    public class PendingStudentRepository : IPendingStudentRepository
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public PendingStudentRepository(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public List<PendingStudent> GetAll()
        {
            var list = new List<PendingStudent>();
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM PendingStudents", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new PendingStudent
                    {
                        StudentCode = reader["StudentCode"]?.ToString(),
                        Id = Convert.ToInt32(reader["Id"]),
                        FirstName = reader["FirstName"]?.ToString(),
                        LastName = reader["LastName"]?.ToString(),
                        Age = reader["Age"] != DBNull.Value ? Convert.ToInt32(reader["Age"]) : 0,
                        ParentName = reader["ParentName"]?.ToString(),
                        PhoneNumber = reader["PhoneNumber"]?.ToString(),
                        Id_Numb = reader["Id_Numb"] != DBNull.Value ? Convert.ToInt64(reader["Id_Numb"]) : 0,
                        Address = reader["Address"]?.ToString(),
                        CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : DateTime.Now,
                        TuitionFee = reader["TuitionFee"] != DBNull.Value ? Convert.ToDecimal(reader["TuitionFee"]) : 0,
                        Discount = reader["DiscountPercentage"] != DBNull.Value ? Convert.ToInt32(reader["DiscountPercentage"]) : 0,
                        //Status = Convert.ToBoolean(reader["status"]),
                        IdCardPath = reader["IdCardPath"]?.ToString(),
                        AdditionalDocsPath = reader["AdditionalDocsPath"]?.ToString()
                    });
                }
            }
            return list;
        }

        public void Delete(int id)
        {
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM PendingStudents WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public PendingStudent GetById(int id)
        {
            var list = new List<PendingStudent>();
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                var query = @"SELECT 
                    Id, 
                    FirstName, 
                    LastName, 
                    Age, 
                    ParentName, 
                    PhoneNumber, 
                    Id_Numb, 
                    Address, 
                    CreatedAt,
                    TuitionFee,
                    IdCardPath,
                    AdditionalDocsPath,
                    user_id
                  FROM PendingStudents
                  WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new PendingStudent
                            {
                                Id = reader.GetInt32("Id"),
                                FirstName = reader.GetString("FirstName"),
                                LastName = reader.GetString("LastName"),
                                Age = reader.GetInt32("Age"),
                                ParentName = reader.GetString("ParentName"),
                                PhoneNumber = reader.GetString("PhoneNumber"),
                                Id_Numb = reader.GetInt64("Id_Numb"),
                                Address = reader.GetString("Address"),
                                RegistrationDate = reader.GetDateTime("CreatedAt"),
                                TuitionFee = reader.GetDecimal("TuitionFee"),
                                //Discount = reader.IsDBNull("Discount") ? 0 : reader.GetDecimal("Discount"),
                                IdCardPath = reader.GetString("IdCardPath"),
                                AdditionalDocsPath = reader.GetString("AdditionalDocsPath"),
                                //Balance = reader.IsDBNull("Balance") ? 0 : reader.GetDecimal("Balance"),
                                UserId = reader.GetInt32("user_id")
                            };
                        }
                    }
                }
            }
            return null;
        }
        public void Update(PendingStudent s)
        {
            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand(@"
            UPDATE PendingStudents 
            SET FirstName = @FirstName, LastName = @LastName, PhoneNumber = @PhoneNumber
            -- სხვა ველები
            WHERE Id = @Id", conn);

                cmd.Parameters.AddWithValue("@FirstName", s.FirstName);
                cmd.Parameters.AddWithValue("@LastName", s.LastName);
                cmd.Parameters.AddWithValue("@PhoneNumber", s.PhoneNumber);
                // სხვა ველები...
                cmd.Parameters.AddWithValue("@Id", s.Id);

                cmd.ExecuteNonQuery();
            }
        }
        public void UpdatePartial(int id, Dictionary<string, object> fields)
        {
            if (fields.Count == 0)
                return;

            using (var conn = _connectionProvider.GetLocalConnection())
            {
                conn.Open();

                var updates = string.Join(", ", fields.Keys.Select(k => $"{k} = @{k}"));
                var query = $"UPDATE PendingStudents SET {updates} WHERE Id = @Id";

                var cmd = new MySqlCommand(query, conn);

                foreach (var pair in fields)
                {
                    cmd.Parameters.AddWithValue($"@{pair.Key}", pair.Value ?? DBNull.Value);
                }

                cmd.Parameters.AddWithValue("@Id", id);

                cmd.ExecuteNonQuery();
            }
        }

    }

}



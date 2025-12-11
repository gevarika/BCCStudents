using MySql.Data.MySqlClient;
using System;
using BCCStudents.Infrastructure.Data;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Infrastructure.Repositories
{
    public class BalanceRepository : IBalanceRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public BalanceRepository(DatabaseHelper databaseHelper)
        {
            _dbHelper = databaseHelper;
        }

        public decimal GetBalance(int studentId)
        {
            using (var conn = _dbHelper.GetLocalConnection())
            {
                conn.Open();
                string query = "SELECT Balance FROM Students WHERE StudentID = @StudentID";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);

                    object result = cmd.ExecuteScalar(); // Executes the query and returns the first column of the first row.
                    return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
            }
        }

        public void UpdateStudentBalance(int studentId, decimal amount)
        {
            using (var conn = _dbHelper.GetLocalConnection())
            {
                conn.Open();
                string query = "UPDATE Students SET Balance = Balance - @Amount WHERE StudentID = @StudentID";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);

                    cmd.ExecuteNonQuery(); // Executes the update
                }
            }
        }
    }
}


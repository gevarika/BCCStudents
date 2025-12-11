using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;
using BCCStudents.Infrastructure.Data;

namespace BCCStudents.Infrastructure.Repositories
{
    public class CleanupRepository : ICleanupRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public CleanupRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public void ResetAllData()
        {
            using (var conn = _dbHelper.GetLocalConnection())
            {
                conn.Open();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 0;";
                    cmd.ExecuteNonQuery();

                    string[] queries = new string[]
                    {
                    "TRUNCATE TABLE Payments;",
                    "TRUNCATE TABLE StudentGroups;",
                    "TRUNCATE TABLE Students;",
                    "TRUNCATE TABLE SubGroups;",
                    "TRUNCATE TABLE Groups;"
                    };

                    foreach (var query in queries)
                    {
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 1;";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}



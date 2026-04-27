using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace BCCStudents.Infrastructure.Repositories
{
    public class CleanupRepository : ICleanupRepository
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public CleanupRepository(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void ResetAllData()
        {
            using (var conn = _connectionProvider.GetLocalConnection())
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



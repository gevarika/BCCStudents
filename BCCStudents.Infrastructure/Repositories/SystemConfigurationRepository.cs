using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;

namespace BCCStudents.Infrastructure.Repositories
{
    public class SystemConfigurationRepository : ISystemConfigurationRepository
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;
        private const string STUDY_START_DATE_KEY = "StudyStartDate";
        private const string DEFAULT_PAYMENT_DATE_KEY = "DefaultPaymentDate";
        private const string VACATION_PREFIX = "Vacation_";

        public SystemConfigurationRepository(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public List<SystemConfiguration> GetAll()
        {
            var configurations = new List<SystemConfiguration>();

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT Id, `Key`, Value, Type, Description, CreatedAt, UpdatedAt FROM SystemConfig ORDER BY `Key`";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        configurations.Add(MapToConfiguration(reader));
                    }
                }
            }

            return configurations;
        }

        public SystemConfiguration GetByKey(string key)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT Id, `Key`, Value, Type, Description, CreatedAt, UpdatedAt FROM SystemConfig WHERE `Key` = @Key";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Key", key);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapToConfiguration(reader);
                        }
                    }
                }
            }

            return null;
        }

        public void Upsert(SystemConfiguration configuration)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"INSERT INTO SystemConfig (`Key`, Value, Type, Description, CreatedAt, UpdatedAt)
                              VALUES (@Key, @Value, @Type, @Description, @CreatedAt, @UpdatedAt)
                              ON DUPLICATE KEY UPDATE
                              Value = @Value,
                              Type = @Type,
                              Description = @Description,
                              UpdatedAt = @UpdatedAt";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Key", configuration.Key);
                    cmd.Parameters.AddWithValue("@Value", configuration.Value);
                    cmd.Parameters.AddWithValue("@Type", configuration.Type);
                    cmd.Parameters.AddWithValue("@Description", configuration.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedAt", configuration.CreatedAt);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(string key)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "DELETE FROM SystemConfig WHERE `Key` = @Key";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Key", key);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool Exists(string key)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM SystemConfig WHERE `Key` = @Key";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Key", key);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public DateTime? GetStudyStartDate()
        {
            var config = GetByKey(STUDY_START_DATE_KEY);
            if (config != null && DateTime.TryParse(config.Value, out DateTime date))
            {
                return date;
            }
            return null;
        }

        public void SetStudyStartDate(DateTime date)
        {
            var config = new SystemConfiguration
            {
                Key = STUDY_START_DATE_KEY,
                Value = date.ToString("yyyy-MM-dd"),
                Type = "Date",
                Description = "სწავლის დაწყების თარიღი",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            Upsert(config);
        }

        public DateTime? GetDefaultPaymentDate()
        {
            var config = GetByKey(DEFAULT_PAYMENT_DATE_KEY);
            if (config != null && DateTime.TryParse(config.Value, out DateTime date))
            {
                return date;
            }
            return null;
        }

        public void SetDefaultPaymentDate(DateTime date)
        {
            var config = new SystemConfiguration
            {
                Key = DEFAULT_PAYMENT_DATE_KEY,
                Value = date.ToString("yyyy-MM-dd"),
                Type = "Date",
                Description = "ნაგულისხმევი გადახდის თარიღი (მოსწავლეებისთვის, რომელთათვისაც არ არის ინდივიდუალური თარიღი დაყენებული)",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            Upsert(config);
        }

        public List<(DateTime StartDate, DateTime EndDate)> GetVacationPeriods()
        {
            var periods = new List<(DateTime StartDate, DateTime EndDate)>();

            // ვპოულობთ ყველა Vacation პერიოდს (JSON ფორმატში)
            var vacationConfigs = GetAll().Where(c => c.Key.StartsWith(VACATION_PREFIX) && c.Type == "VacationPeriod").ToList();

            foreach (var config in vacationConfigs)
            {
                try
                {
                    var vacationData = JsonConvert.DeserializeObject<VacationPeriodData>(config.Value);
                    if (vacationData != null && vacationData.StartDate.HasValue && vacationData.EndDate.HasValue)
                    {
                        periods.Add((vacationData.StartDate.Value, vacationData.EndDate.Value));
                    }
                }
                catch
                {
                    // JSON-ის parsing-ის შეცდომა - გავამჟღავნოთ
                }
            }

            return periods.OrderBy(p => p.StartDate).ToList();
        }

        public void AddVacationPeriod(DateTime startDate, DateTime endDate, string description = null)
        {
            var key = $"{VACATION_PREFIX}{startDate:yyyy-MM-dd}_{endDate:yyyy-MM-dd}";

            var vacationData = new VacationPeriodData
            {
                StartDate = startDate,
                EndDate = endDate,
                Description = description
            };

            var config = new SystemConfiguration
            {
                Key = key,
                Value = JsonConvert.SerializeObject(vacationData),
                Type = "VacationPeriod",
                Description = description ?? $"დასვენების პერიოდი: {startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd}",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            Upsert(config);
        }

        public void DeleteVacationPeriod(DateTime startDate, DateTime endDate)
        {
            var key = $"{VACATION_PREFIX}{startDate:yyyy-MM-dd}_{endDate:yyyy-MM-dd}";
            Delete(key);
        }

        private SystemConfiguration MapToConfiguration(IDataRecord reader)
        {
            return new SystemConfiguration
            {
                Id = Convert.ToInt32(reader["Id"]),
                Key = reader["Key"].ToString(),
                Value = reader["Value"].ToString(),
                Type = reader["Type"].ToString(),
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
            };
        }

        private class VacationPeriodData
        {
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string Description { get; set; }
        }
    }
}

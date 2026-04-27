using BCCStudents.Application.Interfaces;
// Services will be moved to Application layer

using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace BCCStudents.Infrastructure.Repositories
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;
        public PaymentRepository(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        public static Payment payment = new Payment();
        /// <summary>
        /// გადახდის ჩანაწერის დამატება Payments ცხრილში
        /// </summary>
        /// <param name="payment">გადახდის ობიექტი</param>
        /// <returns>true თუ წარმატებულია</returns>
        /// <remarks>
        /// Note:
        /// - PaymentMethod და Note ველები არ ინახება (კომენტარებშია)
        /// - ინახება: StudentId, GroupId, Amount, PaymentDate, PaymentStatus
        /// </remarks>
        public int InsertPayment(Payment payment)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                var query = @"INSERT INTO Payments 
        (StudentId, GroupId, Amount, PaymentDate, PaymentStatus, Description)
        VALUES (@StudentId, @GroupId, @Amount, @PaymentDate, @PaymentStatus, @Description);
        SELECT LAST_INSERT_ID();";
                connection.Open();
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", payment.StudentId);
                    cmd.Parameters.AddWithValue("@GroupId", payment.GroupId);
                    cmd.Parameters.AddWithValue("@Amount", payment.Amount);
                    cmd.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                    //cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod); // არ ინახება
                    cmd.Parameters.AddWithValue("@PaymentStatus", payment.PaymentStatus);
                    cmd.Parameters.AddWithValue("@Description", (object)payment.Description ?? DBNull.Value);
                    //cmd.Parameters.AddWithValue("@Note", payment.Note ?? (object)DBNull.Value); // არ ინახება
                    var id = Convert.ToInt32(cmd.ExecuteScalar());
                    payment.Id = id;
                    return id;
                }
            }
        }
        public List<PaymentSummary> GetPendingPayments()
        {
            var list = new List<PaymentSummary>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"
        SELECT 
            s.Id AS StudentID,
            s.StudentCode,
            s.FirstName,
            s.LastName,
            sg.DateOfPayment AS NextPaymentDate,
            g.Name AS GroupName,
            COALESCE(sg.Price, g.Price) - (COALESCE(sg.Price, g.Price) * COALESCE(sg.Discount, 0) / 100) AS Price,
            IFNULL(SUM(CASE WHEN p.PaymentStatus = 'Paid' THEN p.Amount ELSE 0 END), 0) AS TotalPaid
        FROM Students s
        JOIN StudentGroups sg ON sg.StudentId = s.Id
        JOIN `Groups` g ON g.Id = sg.GroupId
        LEFT JOIN Payments p 
            ON p.StudentId = s.Id AND p.GroupId = g.Id AND p.PaymentStatus = 'Paid'
        WHERE sg.Status = 1 AND (sg.IsDeleted = 0 OR sg.IsDeleted IS NULL)
        GROUP BY s.Id, g.Id, s.FirstName, s.LastName, sg.DateOfPayment, sg.Price, sg.Discount, g.Price
        ORDER BY s.LastName, s.FirstName";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PaymentSummary
                        {
                            StudentID = reader.GetInt32("StudentID"),
                            StudentCode = reader.GetString("StudentCode"),
                            FirstName = reader.GetString("FirstName"),
                            LastName = reader.GetString("LastName"),
                            GroupName = reader.GetString("GroupName"),
                            TuitionFee = reader.GetDecimal("Price"),
                            TotalPaid = reader.GetDecimal("TotalPaid"),
                            NextPaymentDate = reader["NextPaymentDate"] == DBNull.Value
                                ? (DateTime?)null
                                : reader.GetDateTime("NextPaymentDate")
                        });
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// შეამოწმებს გადახდილია თუ არა ჯგუფი კონკრეტული პერიოდისთვის
        /// </summary>
        /// <param name="studentId">მოსწავლის ID</param>
        /// <param name="groupId">ჯგუფის ID</param>
        /// <param name="periodDate">პერიოდის თარიღი (თვე/წელი გამოყენება)</param>
        /// <returns>true თუ გადახდილია (PaymentStatus = 'Paid')</returns>
        /// <remarks>
        /// Note:
        /// - ამოწმებს Payments ცხრილში არის თუ არა გადახდა
        /// - პერიოდი = periodDate თვის/წლის ფარგლებში
        /// - მხოლოდ PaymentStatus = 'Paid' ითვლება (არა "Partial")
        /// - გამოიყენება PaymentService-ში გადახდის დროს დუბლიკატების ასაცილებლად
        /// </remarks>
        public bool IsGroupPaidForPeriod(int studentId, int groupId, DateTime periodDate)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                // ვეძებთ გადახდას ამ სტუდენტისთვის, ამ ჯგუფისთვის, თვე/წელი ემთხვევა periodDate-ს
                string sql = @"
        SELECT COUNT(*) FROM Payments
        WHERE StudentId = @studentId
          AND GroupId = @groupId
          AND MONTH(PaymentDate) = @month
          AND YEAR(PaymentDate) = @year
          AND PaymentStatus = 'Paid'
    ";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    cmd.Parameters.AddWithValue("@groupId", groupId);
                    cmd.Parameters.AddWithValue("@month", periodDate.Month);
                    cmd.Parameters.AddWithValue("@year", periodDate.Year);
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// ექსელის ფაილიდან გადახდების ამოღების კოდები
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        // ვეძებთ სტუდენტის აღრიცხვას აღწერის მიხედვით
        public List<int> FindStudentsByDescription(string description)
        {
            List<int> studentIds = new List<int>();

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                string query = "SELECT id, CONCAT(FirstName, ' ', LastName) AS FullName FROM Students";

                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    var students = new Dictionary<int, string>();

                    while (reader.Read())
                    {
                        int studentId = reader.GetInt32("id");
                        payment.StudentId = studentId;
                        string fullName = reader.GetString("FullName");
                        students.Add(studentId, fullName);
                    }

                    foreach (var student in students)
                    {

                        if (IsMatch(description, student.Value))
                        {
                            studentIds.Add(student.Key);
                        }
                    }
                }
            }
            return studentIds;
        }
        private bool IsMatch(string description, string fullName)
        {
            string pattern = $@"\b{Regex.Escape(fullName)}\b";
            return Regex.IsMatch(description, pattern, RegexOptions.IgnoreCase);
        }
        public List<PaymentSummary> GetSuccessfulPayments()
        {
            var list = new List<PaymentSummary>();

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"
            SELECT 
                s.Id AS StudentID,
                s.FirstName,
                s.LastName,
                g.Name AS GroupName,
                p.Amount AS PaidAmount,
                p.PaymentDate
            FROM Payments p
            JOIN Students s ON s.Id = p.StudentId
            JOIN `Groups` g ON g.Id = p.GroupId
            WHERE p.Amount > 0
            ORDER BY p.PaymentDate DESC";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PaymentSummary
                        {
                            StudentID = reader.GetInt32("StudentID"),
                            FirstName = reader.GetString("FirstName"),
                            LastName = reader.GetString("LastName"),
                            GroupName = reader.GetString("GroupName"),
                            TotalPaid = reader.GetDecimal("PaidAmount"),
                            NextPaymentDate = reader.GetDateTime("PaymentDate") // ან სხვა ველი
                        });
                    }
                }
            }

            return list;
        }
        public void AddPayment(int studentId, decimal amount)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();

                // 1. მოვიძიოთ სტუდენტის ჯგუფი (მარტივად ვიღებთ პირველ ჯგუფს — სურვილისამებრ შეიძლება მოდიფიცირება)
                int groupId = 0;
                using (var cmd = new MySqlCommand("SELECT GroupId FROM StudentGroups WHERE StudentId = @StudentId LIMIT 1", connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                        groupId = Convert.ToInt32(result);
                }

                // 2. ჩაწერა გადახდების ცხრილში
                var insertQuery = @"
            INSERT INTO Payments (StudentId, GroupId, Amount, PaymentDate, PaymentStatus)
            VALUES (@StudentId, @GroupId, @Amount, @PaymentDate, @PaymentStatus);";

                using (var cmd = new MySqlCommand(insertQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@PaymentDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@PaymentStatus", "Paid");

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// მიმდინარე კალენდარული თვის გადახდების მიღება
        /// </summary>
        /// <param name="studentId">მოსწავლის ID</param>
        /// <returns>მიმდინარე თვის გადახდების სია</returns>
        /// <remarks>
        /// Note:
        /// - ამოწმებს მხოლოდ მიმდინარე კალენდარულ თვეს (CURRENT_DATE())
        /// - არა კონკრეტული DateOfPayment-ის პერიოდის
        /// - გამოიყენება PaymentService-ში დუბლიკატების ასაცილებლად
        /// - განსხვავდება IsGroupPaidForPeriod-ისგან: ის ამოწმებს კალენდარულ თვეს, ეს კი DateOfPayment-ის თვეს
        /// </remarks>
        public async Task<List<Payment>> GetCurrentMonthPayments(int studentId)
        {
            var payments = new List<Payment>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"
            SELECT p.*, g.Name AS GroupName
            FROM Payments p
            LEFT JOIN `Groups` g ON p.GroupId = g.Id
            WHERE p.StudentId = @StudentId 
            AND MONTH(p.PaymentDate) = MONTH(CURRENT_DATE())
            AND YEAR(p.PaymentDate) = YEAR(CURRENT_DATE())
            ORDER BY p.PaymentDate DESC";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            payments.Add(new Payment
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                GroupId = reader.GetInt32(reader.GetOrdinal("GroupId")),
                                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                                PaymentDate = reader.GetDateTime(reader.GetOrdinal("PaymentDate")),
                                PaymentStatus = reader.IsDBNull(reader.GetOrdinal("PaymentStatus")) ? null : reader.GetString(reader.GetOrdinal("PaymentStatus")),
                                GroupName = reader.IsDBNull(reader.GetOrdinal("GroupName")) ? null : reader.GetString(reader.GetOrdinal("GroupName"))
                            });
                        }
                    }
                }
            }
            return payments;
        }

        public List<Payment> GetStudentPayments(int studentId)
        {
            var payments = new List<Payment>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"
            SELECT p.*, g.Name AS GroupName
            FROM Payments p
            LEFT JOIN `Groups` g ON p.GroupId = g.Id
            WHERE p.StudentId = @StudentId
            ORDER BY p.PaymentDate DESC";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add(new Payment
                            {
                                Id = reader.GetInt32("Id"),
                                StudentId = reader.GetInt32("StudentId"),
                                GroupId = reader.GetInt32("GroupId"),
                                Amount = reader.GetDecimal("Amount"),
                                PaymentDate = reader.GetDateTime("PaymentDate"),
                                PaymentStatus = reader.GetString("PaymentStatus"),
                                GroupName = reader.GetString("GroupName")
                            });
                        }
                    }
                }
            }
            return payments;
        }

        public decimal GetTotalPaidAmount(int studentId, int groupId, DateTime startDate, DateTime endDate)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"
            SELECT COALESCE(SUM(Amount), 0) as TotalPaid
            FROM Payments
            WHERE StudentId = @StudentId 
            AND GroupId = @GroupId
            AND PaymentDate BETWEEN @StartDate AND @EndDate
            AND PaymentStatus = 'Paid'";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);

                    var result = cmd.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
            }
        }

        public async Task<bool> AddPayments(IEnumerable<Payment> payments)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = @"INSERT INTO Payments 
                            (StudentId, GroupId, Amount, PaymentDate, PaymentStatus, Description, PayerName, PersonalId)
                            VALUES (@StudentId, @GroupId, @Amount, @PaymentDate, @PaymentStatus, @Description, @PayerName, @PersonalId)";

                        foreach (var payment in payments)
                        {
                            using (var cmd = new MySqlCommand(query, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@StudentId", payment.StudentId);
                                cmd.Parameters.AddWithValue("@GroupId", payment.GroupId);
                                cmd.Parameters.AddWithValue("@Amount", payment.Amount);
                                cmd.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                                cmd.Parameters.AddWithValue("@PaymentStatus", payment.PaymentStatus);
                                cmd.Parameters.AddWithValue("@Description", payment.Description ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@PayerName", payment.PayerName ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@PersonalId", payment.PersonalId);
                                //cmd.Parameters.AddWithValue("@IsMatched", payment.IsMatched);
                                //cmd.Parameters.AddWithValue("@RequiresReview", payment.RequiresReview);
                                //cmd.Parameters.AddWithValue("@IsSelected", payment.IsSelected);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool PaymentExists(DateTime paymentDate, decimal amount, long? personalId, string description)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"
                    SELECT COUNT(*) 
                    FROM Payments p
                    JOIN Students s ON s.Id = p.StudentId
                    WHERE p.PaymentDate = @paymentDate 
                    AND p.Amount = @amount 
                    AND p.Description = @description";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@paymentDate", paymentDate.Date);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@description", description);

                    // თუ პირადი ნომერი არის, დავამატოთ დამატებითი პირობა
                    if (personalId.HasValue)
                    {
                        query += " AND s.Id_Numb = @personalId";
                        cmd.Parameters.AddWithValue("@personalId", personalId.Value);
                    }

                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public List<PaymentSummary> GetPaymentSummaries()
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"
                    SELECT 
                        p.PaymentDate,
                        p.Amount,
                        p.Description,
                        s.FirstName,
                        s.LastName,
                        s.StudentCode,
                        g.Name as GroupName,
                        CASE WHEN p.StudentId IS NOT NULL THEN 1 ELSE 0 END as IsSuccessful
                    FROM Payments p
                    JOIN Students s ON s.Id = p.StudentId
                    LEFT JOIN StudentGroups sg ON sg.StudentID = s.Id
                    LEFT JOIN `Groups` g ON g.Id = sg.GroupID
                    ORDER BY p.PaymentDate DESC";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        var payments = new List<PaymentSummary>();
                        while (reader.Read())
                        {
                            payments.Add(new PaymentSummary
                            {
                                StudentCode = reader.GetString("StudentCode"),
                                PaymentDate = reader.GetDateTime("PaymentDate"),
                                Amount = reader.GetDecimal("Amount"),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "გადახდაზე აღწერა არ არსებობს" : reader.GetString("Description"),
                                FirstName = reader.GetString("FirstName"),
                                LastName = reader.GetString("LastName"),
                                GroupName = reader.IsDBNull(reader.GetOrdinal("GroupName")) ? null : reader.GetString("GroupName"),
                                IsSuccessful = reader.GetBoolean("IsSuccessful")
                            });
                        }
                        return payments;
                    }
                }
            }
        }

        public IEnumerable<SuccessfulPayment> GetImportHistory()
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"
                    SELECT 
                        p.PaymentDate as NextPaymentDate,
                        p.Amount as TotalPaid,
                        s.FirstName,
                        s.LastName,
                        g.Name as GroupName,
                        p.Description
                    FROM Payments p
                    JOIN Students s ON s.Id = p.StudentId
                    LEFT JOIN StudentGroups sg ON sg.StudentID = s.Id
                    LEFT JOIN `Groups` g ON g.Id = sg.GroupID
                    ORDER BY p.PaymentDate DESC";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        var payments = new List<SuccessfulPayment>();
                        while (reader.Read())
                        {
                            payments.Add(new SuccessfulPayment
                            {
                                NextPaymentDate = reader.GetDateTime("NextPaymentDate"),
                                TotalPaid = reader.GetDecimal("TotalPaid"),
                                FirstName = reader.GetString("FirstName"),
                                LastName = reader.GetString("LastName"),
                                GroupName = reader.IsDBNull(reader.GetOrdinal("GroupName")) ? null : reader.GetString("GroupName"),
                                Description = reader.GetString("Description")
                            });
                        }
                        return payments;
                    }
                }
            }
        }
        public int SaveFailedPayment(FailedPayment payment)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"
                        INSERT INTO FailedPayments 
                        (RowNumber, PaymentDate, Amount, PersonalId, Description, Reason, CreatedAt) 
                        VALUES 
                        (@RowNumber, @PaymentDate, @Amount, @PersonalId, @Description, @Reason, @CreatedAt);
                        SELECT LAST_INSERT_ID();";

                    command.Parameters.AddWithValue("@RowNumber", payment.RowNumber);
                    command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                    command.Parameters.AddWithValue("@Amount", payment.Amount);
                    command.Parameters.AddWithValue("@PersonalId", (object)payment.PersonalId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Description", (object)payment.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Reason", payment.Reason);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    var id = Convert.ToInt32(command.ExecuteScalar());
                    payment.Id = id;
                    return id;
                }
            }
        }

        public IEnumerable<FailedPayment> GetFailedPayments()
        {
            var failedPayments = new List<FailedPayment>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"
                        SELECT Id, RowNumber, PaymentDate, Amount, PersonalId, Description, Reason, CreatedAt 
                        FROM FailedPayments 
                        ORDER BY CreatedAt DESC";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            failedPayments.Add(new FailedPayment
                            {
                                Id = reader.GetInt32("Id"),
                                RowNumber = reader.GetInt32("RowNumber"),
                                PaymentDate = reader.GetDateTime("PaymentDate"),
                                Amount = reader.GetDecimal("Amount"),
                                PersonalId = reader.IsDBNull(reader.GetOrdinal("PersonalId")) ? null : (long?)reader.GetInt64("PersonalId"),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
                                Reason = reader.GetString("Reason"),
                                CreatedAt = reader.GetDateTime("CreatedAt")
                            });
                        }
                    }
                }
            }
            return failedPayments;
        }

        public FailedPayment GetFailedPaymentById(int id)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"
                        SELECT Id, RowNumber, PaymentDate, Amount, PersonalId, Description, Reason, CreatedAt 
                        FROM FailedPayments 
                        WHERE Id = @Id";

                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new FailedPayment
                            {
                                Id = reader.GetInt32("Id"),
                                RowNumber = reader.GetInt32("RowNumber"),
                                PaymentDate = reader.GetDateTime("PaymentDate"),
                                Amount = reader.GetDecimal("Amount"),
                                PersonalId = reader.IsDBNull(reader.GetOrdinal("PersonalId")) ? null : (long?)reader.GetInt64("PersonalId"),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
                                Reason = reader.GetString("Reason"),
                                CreatedAt = reader.GetDateTime("CreatedAt")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void ClearFailedPayments()
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "DELETE FROM FailedPayments";
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteFailedPayment(DateTime paymentDate, decimal amount, long? personalId, string description)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"DELETE FROM FailedPayments 
                                           WHERE PaymentDate = @paymentDate 
                                           AND Amount = @amount 
                                           AND Description = @description";

                    command.Parameters.AddWithValue("@paymentDate", paymentDate);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@description", description);

                    if (personalId.HasValue)
                    {
                        command.CommandText += " AND PersonalId = @personalId";
                        command.Parameters.AddWithValue("@personalId", personalId.Value);
                    }
                    else
                    {
                        command.CommandText += " AND PersonalId IS NULL";
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Payment> GetAllPayments()
        {
            var payments = new List<Payment>();
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"
                    SELECT 
                        p.Id,
                        p.StudentId,
                        p.GroupId,
                        p.Amount,
                        p.PaymentDate,
                        p.Description,
                        p.PaymentStatus,
                        s.Id_Numb as PersonalId,
                        CONCAT(s.FirstName, ' ', s.LastName) as PayerName,
                        g.Name as GroupName,
                        p.PaymentDate
                    FROM Payments p
                    LEFT JOIN Students s ON s.Id = p.StudentId
                    LEFT JOIN `Groups` g ON g.Id = p.GroupId
                    ORDER BY p.PaymentDate DESC";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        payments.Add(new Payment
                        {
                            Id = reader.GetInt32("Id"),
                            StudentId = reader.IsDBNull(reader.GetOrdinal("StudentId")) ? null : (int?)reader.GetInt32("StudentId"),
                            GroupId = reader.GetInt32("GroupId"),
                            Amount = reader.GetDecimal("Amount"),
                            PaymentDate = reader.GetDateTime("PaymentDate"),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
                            PaymentStatus = reader.IsDBNull(reader.GetOrdinal("PaymentStatus")) ? null : reader.GetString("PaymentStatus"),
                            //Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? null : reader.GetString("Note"),
                            PersonalId = reader.IsDBNull(reader.GetOrdinal("PersonalId")) ? null : (long?)reader.GetInt64("PersonalId"),
                            PayerName = reader.IsDBNull(reader.GetOrdinal("PayerName")) ? null : reader.GetString("PayerName"),
                            GroupName = reader.IsDBNull(reader.GetOrdinal("GroupName")) ? null : reader.GetString("GroupName"),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("PaymentDate")) ? DateTime.Now : reader.GetDateTime("PaymentDate")
                        });
                    }
                }
            }
            return payments;
        }

        // ImportedPaymentsLog-ისთვის
        public bool CheckImportedPaymentDuplicate(DateTime paymentDate, decimal amount, long? personalId, string description)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT COUNT(*) FROM ImportedPaymentsLog WHERE PaymentDate = @paymentDate AND Amount = @amount AND Description = @description";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@paymentDate", paymentDate);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@description", description);
                    if (personalId.HasValue)
                    {
                        query += " AND PersonalId = @personalId";
                        cmd.Parameters.AddWithValue("@personalId", personalId.Value);
                    }
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public int AddImportedPaymentLog(DateTime paymentDate, decimal amount, long? personalId, string description, string importSource)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"INSERT INTO ImportedPaymentsLog (PaymentDate, Amount, PersonalId, Description, ImportSource, CreatedAt) VALUES (@paymentDate, @amount, @personalId, @description, @importSource, @createdAt);
                              SELECT LAST_INSERT_ID();";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@paymentDate", paymentDate);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@personalId", (object)personalId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@importSource", importSource ?? "manual");
                    cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);
                    var id = Convert.ToInt32(cmd.ExecuteScalar());
                    return id;
                }
            }
        }

        public ImportedPaymentLog GetImportedPaymentLogById(int id)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT Id, PaymentDate, Amount, PersonalId, Description, ImportSource, CreatedAt 
                              FROM ImportedPaymentsLog 
                              WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ImportedPaymentLog
                            {
                                Id = reader.GetInt32("Id"),
                                PaymentDate = reader.GetDateTime("PaymentDate"),
                                Amount = reader.GetDecimal("Amount"),
                                PersonalId = reader.IsDBNull(reader.GetOrdinal("PersonalId")) ? null : (long?)reader.GetInt64("PersonalId"),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
                                ImportSource = reader.IsDBNull(reader.GetOrdinal("ImportSource")) ? null : reader.GetString("ImportSource"),
                                CreatedAt = reader.GetDateTime("CreatedAt")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool CheckFailedPaymentDuplicate(DateTime paymentDate, decimal amount, long? personalId, string description)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                var query = @"SELECT COUNT(*) FROM FailedPayments WHERE PaymentDate = @paymentDate AND Amount = @amount AND Description = @description";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@paymentDate", paymentDate);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@description", description);
                    if (personalId.HasValue)
                    {
                        query += " AND PersonalId = @personalId";
                        cmd.Parameters.AddWithValue("@personalId", personalId.Value);
                    }
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
    }
}




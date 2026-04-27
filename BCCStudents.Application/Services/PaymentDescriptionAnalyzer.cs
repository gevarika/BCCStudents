using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace BCCStudents.Application.Services
{
    public class PaymentDescriptionAnalyzer : IPaymentDescriptionAnalyzer
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ILoggerRepository _logger;
        private readonly Dictionary<string, string[]> _nameVariations;
        private readonly Dictionary<string, string[]> _groupCodeVariations;
        private const double LEVENSHTEIN_THRESHOLD = 0.8; // 80% similarity threshold
        private const double MATCH_THRESHOLD = 0.6; // 60% match threshold
        private const int MAX_LEVENSHTEIN_DISTANCE = 3;
        private const double EXACT_MATCH_THRESHOLD = 0.75; // შეცვალეთ 0.95-დან 0.75-ზე
        private const double PARTIAL_MATCH_THRESHOLD = 0.7; // ნაწილობრივი დამთხვევის ზღვარი

        public PaymentDescriptionAnalyzer(
            IStudentRepository studentRepository,
            IGroupRepository groupRepository,
            ILoggerRepository logger)
        {
            _studentRepository = studentRepository;
            _groupRepository = groupRepository;
            _logger = logger;

            // Initialize name variations dictionary
            _nameVariations = new Dictionary<string, string[]>
            {
                { "გიორგი", new[] { "გიო", "გიორგა", "გიორგია", "გიორგი", "გიორგიკო" } },
                { "დავით", new[] { "დათო", "დათა", "დავითა", "დავით", "დათუნა" } },
                { "ნიკა", new[] { "ნიკოლოზი", "ნიკოლა", "ნიკო", "ნიკუშა", "ნიკოლაი" } },
                { "ლევანი", new[] { "ლევა", "ლევანა", "ლევან", "ლევანიკო" } },
                { "სანდრო", new[] { "ალექსანდრე", "ალექსანდრა", "ალექსი", "სანდრიკო", "ალექსო" } },
                { "ნინო", new[] { "ნინა", "ნინი", "ნინუკა", "ნინუშა" } },
                { "მარიამი", new[] { "მარი", "მარია", "მარიკო", "მარიკა", "მარიამ" } },
                { "ანა", new[] { "ანი", "ანნა", "ანუკა", "ანუშა", "ანეტა" } },
                { "სოფო", new[] { "სოფია", "სოფიკო", "სოფიკა", "სოფო" } },
                { "ნათია", new[] { "ნათო", "ნათიკო", "ნათუკა", "ნათი" } },
                { "ლუკა", new[] { "ლუკა", "ლუკა", "ლუკას", "ლუკიკო" } },
                { "გაბრიელი", new[] { "გაბო", "გაბრიელ", "გაბრი", "გაბრიკო" } },
                { "ელენე", new[] { "ელენა", "ელენა", "ელენუკა", "ელენიკო" } },
                { "დემეტრე", new[] { "დემო", "დემეტრი", "დემეტრიკო", "დემუკა" } }
            };

            // Initialize group code variations
            _groupCodeVariations = new Dictionary<string, string[]>
            {
                { "BCC", new[] { "BCC-", "BCC ", "BCC", "ბცც", "ბცც-", "ბცც " } },
                { "1", new[] { "01", "1", "I", "პირველი", "პირველი ჯგუფი" } },
                { "2", new[] { "02", "2", "II", "მეორე", "მეორე ჯგუფი" } },
                { "3", new[] { "03", "3", "III", "მესამე", "მესამე ჯგუფი" } },
                { "4", new[] { "04", "4", "IV", "მეოთხე", "მეოთხე ჯგუფი" } },
                { "5", new[] { "05", "5", "V", "მეხუთე", "მეხუთე ჯგუფი" } },
                { "6", new[] { "06", "6", "VI", "მეექვსე", "მეექვსე ჯგუფი" } }
            };
        }

        private double CalculateLevenshteinSimilarity(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2)) return 0;
            if (s1 == s2) return 1;

            int[,] d = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= s2.Length; j++) d[0, j] = j;

            for (int j = 1; j <= s2.Length; j++)
            {
                for (int i = 1; i <= s1.Length; i++)
                {
                    int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(
                        d[i - 1, j] + 1,      // deletion
                        d[i, j - 1] + 1),     // insertion
                        d[i - 1, j - 1] + cost); // substitution
                }
            }

            int maxLength = Math.Max(s1.Length, s2.Length);
            return 1.0 - ((double)d[s1.Length, s2.Length] / maxLength);
        }

        private bool IsStudentCode(string text)
        {
            return Regex.IsMatch(text, @"^B\d{4}$", RegexOptions.IgnoreCase);
        }

        private string ExtractStudentCode(string description)
        {
            var match = Regex.Match(description, @"B\d{4}", RegexOptions.IgnoreCase);
            return match.Success ? match.Value.ToUpper() : null;
        }

        private double CalculateNameMatchScore(string name1, string name2)
        {
            if (string.IsNullOrEmpty(name1) || string.IsNullOrEmpty(name2)) return 0;

            // Direct match
            if (name1.Equals(name2, StringComparison.OrdinalIgnoreCase)) return 1.0;

            // Check name variations
            foreach (var variation in _nameVariations)
            {
                if (variation.Key.Equals(name1, StringComparison.OrdinalIgnoreCase) &&
                    variation.Value.Any(v => v.Equals(name2, StringComparison.OrdinalIgnoreCase)))
                    return 0.9;
            }

            // Levenshtein similarity
            double similarity = CalculateLevenshteinSimilarity(name1, name2);
            return similarity >= LEVENSHTEIN_THRESHOLD ? similarity : 0;
        }

        private double CalculateGroupMatchScore(string groupCode1, string groupCode2)
        {
            if (string.IsNullOrEmpty(groupCode1) || string.IsNullOrEmpty(groupCode2)) return 0;

            // Direct match
            if (groupCode1.Equals(groupCode2, StringComparison.OrdinalIgnoreCase)) return 1.0;

            // Check group code variations
            var parts1 = groupCode1.Split(new[] { '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var parts2 = groupCode2.Split(new[] { '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts1.Length != parts2.Length) return 0;

            double score = 0;
            for (int i = 0; i < parts1.Length; i++)
            {
                if (_groupCodeVariations.ContainsKey(parts1[i]) &&
                    _groupCodeVariations[parts1[i]].Any(v => v.Equals(parts2[i], StringComparison.OrdinalIgnoreCase)))
                {
                    score += 1.0 / parts1.Length;
                }
            }

            return score;
        }

        private bool IsExactNameMatch(string studentFirstName, string studentLastName, string description)
        {
            var fullName = $"{studentFirstName} {studentLastName}".ToLower();
            var words = description.ToLower().Split(new[] { ' ', ',', '.', ';', ':', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            // ვეძებთ ზუსტ დამთხვევას სახელისა და გვარის
            return words.Any(w => w == fullName) ||
                   (words.Contains(studentFirstName.ToLower()) && words.Contains(studentLastName.ToLower()));
        }

        private bool IsExactGroupMatch(string groupName, string description)
        {
            var words = description.ToLower().Split(new[] { ' ', ',', '.', ';', ':', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Any(w => w == groupName.ToLower());
        }

        public async Task<AnalyzedPayment> AnalyzeDescription(string description, long? personalId = null)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                _logger.LogPaymentAction("ANALYZE", "WARNING", "Empty payment description", "SYSTEM");
                return new AnalyzedPayment { IsValid = false, ErrorMessage = "შეტყობინება ცარიელია" };
            }

            _logger.LogPaymentAction("ANALYZE", "INFO", $"Starting analysis of description: {description}", "SYSTEM");

            try
            {
                var words = description.Split(new[] { ' ', ',', '.', ';', ':', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                var matches = new List<(Student Student, double Score, string MatchType, string MatchDetails)>();

                // 1. პირველ რიგში ვეძებთ სტუდენტის კოდს
                var studentCode = ExtractStudentCode(description);
                if (!string.IsNullOrEmpty(studentCode))
                {
                    var studentByCode = _studentRepository.GetStudentByCode(studentCode);
                    if (studentByCode != null)
                    {
                        var fullName = $"{studentByCode.FirstName} {studentByCode.LastName}";
                        _logger.LogPaymentAction("ANALYZE", "SUCCESS",
                            $"Found exact match by student code: {studentByCode.Id} - {fullName}", "SYSTEM");
                        return new AnalyzedPayment
                        {
                            IsValid = true,
                            StudentId = studentByCode.Id,
                            StudentName = fullName,
                            GroupName = studentByCode.GroupName,
                            MatchConfidence = 1.0,
                            MatchType = "სტუდენტის კოდით",
                            MatchDetails = $"მაჩი ნაპოვნია სტუდენტის კოდით: {studentByCode.StudentCode}",
                            RequiresReview = false
                        };
                    }
                }

                // 2. ვეძებთ ზუსტ დამთხვევას სახელისა და გვარის, ან სახელის, გვარისა და ჯგუფის
                var allStudents = _studentRepository.GetAllStudents();
                foreach (var student in allStudents)
                {
                    var matchDetails = new List<string>();
                    double score = 0;
                    string matchType = "ნაწილობრივი";

                    // ვამოწმებთ სახელისა და გვარის ზუსტ დამთხვევას
                    if (IsExactNameMatch(student.FirstName, student.LastName, description))
                    {
                        score = 0.8; // სახელისა და გვარის ზუსტი დამთხვევის ქულა
                        matchType = "სახელით და გვარით";
                        matchDetails.Add($"ზუსტი დამთხვევა სახელისა და გვარის: {student.FirstName} {student.LastName}");

                        // თუ ჯგუფიც დაემთხვა, ვზრდით ქულას
                        if (!string.IsNullOrEmpty(student.GroupName) && IsExactGroupMatch(student.GroupName, description))
                        {
                            score = 1.0;
                            matchType = "სახელით, გვარით და ჯგუფით";
                            matchDetails.Add($"ზუსტი დამთხვევა ჯგუფის: {student.GroupName}");
                        }
                    }

                    if (score > 0)
                    {
                        matches.Add((student, score, matchType, string.Join("; ", matchDetails)));
                    }
                }

                // დავალაგოთ მაჩები ქულის მიხედვით
                matches = matches.OrderByDescending(m => m.Score).ToList();

                if (matches.Any())
                {
                    var bestMatch = matches.First();
                    var requiresReview = bestMatch.Score < EXACT_MATCH_THRESHOLD;

                    _logger.LogPaymentAction("ANALYZE", "SUCCESS",
                        $"Selected best match: Student {bestMatch.Student.Id} - {bestMatch.Student.FirstName} {bestMatch.Student.LastName} " +
                        $"with score {bestMatch.Score} ({bestMatch.MatchDetails}). RequiresReview={requiresReview} because score {bestMatch.Score} < {EXACT_MATCH_THRESHOLD}", "SYSTEM");

                    return new AnalyzedPayment
                    {
                        IsValid = true,
                        StudentId = bestMatch.Student.Id,
                        StudentName = $"{bestMatch.Student.FirstName} {bestMatch.Student.LastName}",
                        GroupName = bestMatch.Student.GroupName,
                        MatchConfidence = bestMatch.Score,
                        MatchType = bestMatch.MatchType,
                        MatchDetails = bestMatch.MatchDetails,
                        RequiresReview = requiresReview
                    };
                }

                _logger.LogPaymentAction("ANALYZE", "WARNING",
                    $"No matches found for description: {description}", "SYSTEM");
                return new AnalyzedPayment
                {
                    IsValid = false,
                    ErrorMessage = "მაჩი ვერ მოიძებნა",
                    RequiresReview = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogPaymentAction("ANALYZE", "ERROR",
                    $"Error analyzing description: {ex.Message}", "SYSTEM");
                return new AnalyzedPayment
                {
                    IsValid = false,
                    ErrorMessage = $"შეცდომა ანალიზის დროს: {ex.Message}",
                    RequiresReview = true
                };
            }
        }
    }
}



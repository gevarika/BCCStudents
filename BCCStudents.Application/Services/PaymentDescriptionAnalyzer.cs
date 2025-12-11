using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;

using BCCStudents.Application.Interfaces;

namespace BCCStudents.Application.Services {
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
        private const double EXACT_MATCH_THRESHOLD = 0.75; // áƒ¨áƒ”áƒ•áƒªáƒ•áƒáƒšáƒ”áƒ— 0.95-áƒ“áƒáƒœ 0.75-áƒ–áƒ”
        private const double PARTIAL_MATCH_THRESHOLD = 0.7; // áƒœáƒáƒ¬áƒ˜áƒšáƒáƒ‘áƒ áƒ˜áƒ•áƒ˜ áƒ“áƒáƒ›áƒ—áƒ®áƒ•áƒ”áƒ•áƒ˜áƒ¡ áƒ–áƒ¦áƒ•áƒáƒ áƒ˜

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
                { "áƒ’áƒ˜áƒáƒ áƒ’áƒ˜", new[] { "áƒ’áƒ˜áƒ", "áƒ’áƒ˜áƒáƒ áƒ’áƒ", "áƒ’áƒ˜áƒáƒ áƒ’áƒ˜áƒ", "áƒ’áƒ˜áƒáƒ áƒ’áƒ˜", "áƒ’áƒ˜áƒáƒ áƒ’áƒ˜áƒ™áƒ" } },
                { "áƒ“áƒáƒ•áƒ˜áƒ—áƒ˜", new[] { "áƒ“áƒáƒ—áƒ", "áƒ“áƒáƒ—áƒ", "áƒ“áƒáƒ•áƒ˜áƒ—áƒ", "áƒ“áƒáƒ•áƒ˜áƒ—", "áƒ“áƒáƒ—áƒ£áƒœáƒ" } },
                { "áƒœáƒ˜áƒ™áƒ", new[] { "áƒœáƒ˜áƒ™áƒáƒšáƒáƒ–áƒ˜", "áƒœáƒ˜áƒ™áƒáƒšáƒ", "áƒœáƒ˜áƒ™áƒ", "áƒœáƒ˜áƒ™áƒ£áƒ¨áƒ", "áƒœáƒ˜áƒ™áƒáƒšáƒáƒ˜" } },
                { "áƒšáƒ”áƒ•áƒáƒœáƒ˜", new[] { "áƒšáƒ”áƒ•áƒ", "áƒšáƒ”áƒ•áƒáƒœáƒ", "áƒšáƒ”áƒ•áƒáƒœ", "áƒšáƒ”áƒ•áƒáƒœáƒ˜áƒ™áƒ" } },
                { "áƒ¡áƒáƒœáƒ“áƒ áƒ", new[] { "áƒáƒšáƒ”áƒ¥áƒ¡áƒáƒœáƒ“áƒ áƒ”", "áƒáƒšáƒ”áƒ¥áƒ¡áƒáƒœáƒ“áƒ áƒ", "áƒáƒšáƒ”áƒ¥áƒ¡áƒ˜", "áƒ¡áƒáƒœáƒ“áƒ áƒ˜áƒ™áƒ", "áƒáƒšáƒ”áƒ¥áƒ" } },
                { "áƒœáƒ˜áƒœáƒ", new[] { "áƒœáƒ˜áƒœáƒ", "áƒœáƒ˜áƒœáƒ˜", "áƒœáƒ˜áƒœáƒ£áƒ™áƒ", "áƒœáƒ˜áƒœáƒ£áƒ¨áƒ" } },
                { "áƒ›áƒáƒ áƒ˜áƒáƒ›áƒ˜", new[] { "áƒ›áƒáƒ áƒ˜", "áƒ›áƒáƒ áƒ˜áƒ", "áƒ›áƒáƒ áƒ˜áƒ™áƒ", "áƒ›áƒáƒ áƒ˜áƒ™áƒ", "áƒ›áƒáƒ áƒ˜áƒáƒ›" } },
                { "áƒáƒœáƒ", new[] { "áƒáƒœáƒ˜", "áƒáƒœáƒœáƒ", "áƒáƒœáƒ£áƒ™áƒ", "áƒáƒœáƒ£áƒ¨áƒ", "áƒáƒœáƒ”áƒ¢áƒ" } },
                { "áƒ¡áƒáƒ¤áƒ", new[] { "áƒ¡áƒáƒ¤áƒ˜áƒ", "áƒ¡áƒáƒ¤áƒ˜áƒ™áƒ", "áƒ¡áƒáƒ¤áƒ˜áƒ™áƒ", "áƒ¡áƒáƒ¤áƒ" } },
                { "áƒœáƒáƒ—áƒ˜áƒ", new[] { "áƒœáƒáƒ—áƒ", "áƒœáƒáƒ—áƒ˜áƒ™áƒ", "áƒœáƒáƒ—áƒ£áƒ™áƒ", "áƒœáƒáƒ—áƒ˜" } },
                { "áƒšáƒ£áƒ™áƒ", new[] { "áƒšáƒ£áƒ™áƒ", "áƒšáƒ£áƒ™áƒ", "áƒšáƒ£áƒ™áƒáƒ¡", "áƒšáƒ£áƒ™áƒ˜áƒ™áƒ" } },
                { "áƒ’áƒáƒ‘áƒ áƒ˜áƒ”áƒšáƒ˜", new[] { "áƒ’áƒáƒ‘áƒ", "áƒ’áƒáƒ‘áƒ áƒ˜áƒ”áƒš", "áƒ’áƒáƒ‘áƒ áƒ˜", "áƒ’áƒáƒ‘áƒ áƒ˜áƒ™áƒ" } },
                { "áƒ”áƒšáƒ”áƒœáƒ”", new[] { "áƒ”áƒšáƒ”áƒœáƒ", "áƒ”áƒšáƒ”áƒœáƒ", "áƒ”áƒšáƒ”áƒœáƒ£áƒ™áƒ", "áƒ”áƒšáƒ”áƒœáƒ˜áƒ™áƒ" } },
                { "áƒ“áƒ”áƒ›áƒ”áƒ¢áƒ áƒ”", new[] { "áƒ“áƒ”áƒ›áƒ", "áƒ“áƒ”áƒ›áƒ”áƒ¢áƒ áƒ˜", "áƒ“áƒ”áƒ›áƒ”áƒ¢áƒ áƒ˜áƒ™áƒ", "áƒ“áƒ”áƒ›áƒ£áƒ™áƒ" } }
            };

            // Initialize group code variations
            _groupCodeVariations = new Dictionary<string, string[]>
            {
                { "BCC", new[] { "BCC-", "BCC ", "BCC", "áƒ‘áƒªáƒª", "áƒ‘áƒªáƒª-", "áƒ‘áƒªáƒª " } },
                { "1", new[] { "01", "1", "I", "áƒžáƒ˜áƒ áƒ•áƒ”áƒšáƒ˜", "áƒžáƒ˜áƒ áƒ•áƒ”áƒšáƒ˜ áƒ¯áƒ’áƒ£áƒ¤áƒ˜" } },
                { "2", new[] { "02", "2", "II", "áƒ›áƒ”áƒáƒ áƒ”", "áƒ›áƒ”áƒáƒ áƒ” áƒ¯áƒ’áƒ£áƒ¤áƒ˜" } },
                { "3", new[] { "03", "3", "III", "áƒ›áƒ”áƒ¡áƒáƒ›áƒ”", "áƒ›áƒ”áƒ¡áƒáƒ›áƒ” áƒ¯áƒ’áƒ£áƒ¤áƒ˜" } },
                { "4", new[] { "04", "4", "IV", "áƒ›áƒ”áƒáƒ—áƒ®áƒ”", "áƒ›áƒ”áƒáƒ—áƒ®áƒ” áƒ¯áƒ’áƒ£áƒ¤áƒ˜" } },
                { "5", new[] { "05", "5", "V", "áƒ›áƒ”áƒ®áƒ£áƒ—áƒ”", "áƒ›áƒ”áƒ®áƒ£áƒ—áƒ” áƒ¯áƒ’áƒ£áƒ¤áƒ˜" } },
                { "6", new[] { "06", "6", "VI", "áƒ›áƒ”áƒ”áƒ¥áƒ•áƒ¡áƒ”", "áƒ›áƒ”áƒ”áƒ¥áƒ•áƒ¡áƒ” áƒ¯áƒ’áƒ£áƒ¤áƒ˜" } }
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
            
            // áƒ•áƒ”áƒ«áƒ”áƒ‘áƒ— áƒ–áƒ£áƒ¡áƒ¢ áƒ“áƒáƒ›áƒ—áƒ®áƒ•áƒ”áƒ•áƒáƒ¡ áƒ¡áƒáƒ®áƒ”áƒšáƒ˜áƒ¡áƒ áƒ“áƒ áƒ’áƒ•áƒáƒ áƒ˜áƒ¡
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
                return new AnalyzedPayment { IsValid = false, ErrorMessage = "áƒ¨áƒ”áƒ¢áƒ§áƒáƒ‘áƒ˜áƒœáƒ”áƒ‘áƒ áƒªáƒáƒ áƒ˜áƒ”áƒšáƒ˜áƒ" };
            }

            _logger.LogPaymentAction("ANALYZE", "INFO", $"Starting analysis of description: {description}", "SYSTEM");

            try
            {
                var words = description.Split(new[] { ' ', ',', '.', ';', ':', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                var matches = new List<(Student Student, double Score, string MatchType, string MatchDetails)>();

                // 1. áƒžáƒ˜áƒ áƒ•áƒ”áƒš áƒ áƒ˜áƒ’áƒ¨áƒ˜ áƒ•áƒ”áƒ«áƒ”áƒ‘áƒ— áƒ¡áƒ¢áƒ£áƒ“áƒ”áƒœáƒ¢áƒ˜áƒ¡ áƒ™áƒáƒ“áƒ¡
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
                            MatchType = "áƒ¡áƒ¢áƒ£áƒ“áƒ”áƒœáƒ¢áƒ˜áƒ¡ áƒ™áƒáƒ“áƒ˜áƒ—",
                            MatchDetails = $"áƒ›áƒáƒ¢áƒ©áƒ˜ áƒœáƒáƒžáƒáƒ•áƒœáƒ˜áƒ áƒ¡áƒ¢áƒ£áƒ“áƒ”áƒœáƒ¢áƒ˜áƒ¡ áƒ™áƒáƒ“áƒ˜áƒ—: {studentByCode.StudentCode}",
                            RequiresReview = false
                        };
                    }
                }

                // 2. áƒ•áƒ”áƒ«áƒ”áƒ‘áƒ— áƒ–áƒ£áƒ¡áƒ¢ áƒ“áƒáƒ›áƒ—áƒ®áƒ•áƒ”áƒ•áƒáƒ¡ áƒ¡áƒáƒ®áƒ”áƒšáƒ˜áƒ¡áƒ áƒ“áƒ áƒ’áƒ•áƒáƒ áƒ˜áƒ¡, áƒáƒœ áƒ¡áƒáƒ®áƒ”áƒšáƒ˜áƒ¡, áƒ’áƒ•áƒáƒ áƒ˜áƒ¡áƒ áƒ“áƒ áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡
                var allStudents = _studentRepository.GetAllStudents();
                foreach (var student in allStudents)
                {
                    var matchDetails = new List<string>();
                    double score = 0;
                    string matchType = "áƒœáƒáƒ¬áƒ˜áƒšáƒáƒ‘áƒ áƒ˜áƒ•áƒ˜";

                    // áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ— áƒ¡áƒáƒ®áƒ”áƒšáƒ˜áƒ¡áƒ áƒ“áƒ áƒ’áƒ•áƒáƒ áƒ˜áƒ¡ áƒ–áƒ£áƒ¡áƒ¢ áƒ“áƒáƒ›áƒ—áƒ®áƒ•áƒ”áƒ•áƒáƒ¡
                    if (IsExactNameMatch(student.FirstName, student.LastName, description))
                    {
                        score = 0.8; // áƒ¡áƒáƒ®áƒ”áƒšáƒ˜áƒ¡áƒ áƒ“áƒ áƒ’áƒ•áƒáƒ áƒ˜áƒ¡ áƒ–áƒ£áƒ¡áƒ¢áƒ˜ áƒ“áƒáƒ›áƒ—áƒ®áƒ•áƒ”áƒ•áƒ˜áƒ¡ áƒ¥áƒ£áƒšáƒ
                        matchType = "áƒ¡áƒáƒ®áƒ”áƒšáƒ˜áƒ— áƒ“áƒ áƒ’áƒ•áƒáƒ áƒ˜áƒ—";
                        matchDetails.Add($"áƒ–áƒ£áƒ¡áƒ¢áƒ˜ áƒ“áƒáƒ›áƒ—áƒ®áƒ•áƒ”áƒ•áƒ áƒ¡áƒáƒ®áƒ”áƒšáƒ˜áƒ¡áƒ áƒ“áƒ áƒ’áƒ•áƒáƒ áƒ˜áƒ¡: {student.FirstName} {student.LastName}");

                        // áƒ—áƒ£ áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒª áƒ“áƒáƒ”áƒ›áƒ—áƒ®áƒ•áƒ, áƒ•áƒ–áƒ áƒ“áƒ˜áƒ— áƒ¥áƒ£áƒšáƒáƒ¡
                        if (!string.IsNullOrEmpty(student.GroupName) && IsExactGroupMatch(student.GroupName, description))
                        {
                            score = 1.0;
                            matchType = "áƒ¡áƒáƒ®áƒ”áƒšáƒ˜áƒ—, áƒ’áƒ•áƒáƒ áƒ˜áƒ— áƒ“áƒ áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ—";
                            matchDetails.Add($"áƒ–áƒ£áƒ¡áƒ¢áƒ˜ áƒ“áƒáƒ›áƒ—áƒ®áƒ•áƒ”áƒ•áƒ áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡: {student.GroupName}");
                        }
                    }

                    if (score > 0)
                    {
                        matches.Add((student, score, matchType, string.Join("; ", matchDetails)));
                    }
                }

                // áƒ“áƒáƒ•áƒáƒšáƒáƒ’áƒáƒ— áƒ›áƒáƒ¢áƒ©áƒ”áƒ‘áƒ˜ áƒ¥áƒ£áƒšáƒ˜áƒ¡ áƒ›áƒ˜áƒ®áƒ”áƒ“áƒ•áƒ˜áƒ—
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
                    ErrorMessage = "áƒ›áƒáƒ¢áƒ©áƒ˜ áƒ•áƒ”áƒ  áƒ›áƒáƒ˜áƒ«áƒ”áƒ‘áƒœáƒ",
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
                    ErrorMessage = $"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒáƒœáƒáƒšáƒ˜áƒ–áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}",
                    RequiresReview = true
                };
            }
        }
    }
} 



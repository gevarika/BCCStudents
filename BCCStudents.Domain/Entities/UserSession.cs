namespace BCCStudents.Domain.Entities
{
    public static class UserSession
    {
        public static int Id { get; set; }
        public static string Password { get; set; }
        public static string UserName { get; set; }
        public static string FullName { get; set; }
        public static string Email { get; set; }
        public static string Role { get; set; }
        public static DateTime? CreatedAt { get; set; }
        public static DateTime? LastLogin { get; set; }
        public static bool IsAdmin => Role == "Administrator";
        public static bool IsAuthenticated => Id > 0;
        public static void Clear()
        {
            Id = 0;
            Role = null;
            FullName = null;
        }
    }
}


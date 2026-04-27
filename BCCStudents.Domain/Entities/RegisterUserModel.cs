namespace BCCStudents.Domain.Entities
{
    public class RegisterUserModel
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Permissions { get; set; } // JSON string: {"CanImport": true, "CanDelete": false, ...}
        public DateTime CreatedAt { get; set; }
    }

}


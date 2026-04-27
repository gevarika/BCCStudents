namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// მიმდინარე მომხმარებლის კონტექსტი - გამოიყენება Security Checks-ისთვის
    /// </summary>
    public interface IUserContext
    {
        int UserId { get; }
        string Username { get; }
        string FullName { get; }
        string Role { get; }
        bool IsAdmin { get; }
        bool IsAuthenticated { get; }

        /// <summary>
        /// ამოწმებს აქვს თუ არა მომხმარებელს კონკრეტული permission
        /// Admin-ს ყოველთვის აქვს ყველა permission
        /// </summary>
        bool HasPermission(string permissionName);

        /// <summary>
        /// აბრუნებს მომხმარებლის ყველა permission-ს Dictionary-ს
        /// </summary>
        Dictionary<string, bool> GetAllPermissions();

        /// <summary>
        /// ანახლებს მომხმარებლის ინფორმაციას (მაგ: Login-ის შემდეგ)
        /// </summary>
        void Refresh();
    }
}

using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Infrastructure.Logging;
using Newtonsoft.Json;

namespace BCCStudents.Infrastructure.Services
{
    /// <summary>
    /// UserContext implementation - მიმდინარე მომხმარებლის კონტექსტი
    /// </summary>
    public class UserContext : IUserContext
    {
        private readonly IUserRepository _userRepository;
        private UserModel _currentUser;
        private Dictionary<string, bool> _permissionsCache;

        public UserContext(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            Refresh();
        }

        public int UserId => _currentUser?.Id ?? 0;
        public string Username => _currentUser?.UserName ?? string.Empty;
        public string FullName => _currentUser?.FullName ?? string.Empty;
        public string Role => _currentUser?.Role ?? string.Empty;
        public bool IsAdmin => Role?.Equals("Administrator", StringComparison.OrdinalIgnoreCase) == true;
        public bool IsAuthenticated => _currentUser != null && _currentUser.Id > 0;

        public bool HasPermission(string permissionName)
        {
            if (string.IsNullOrWhiteSpace(permissionName))
                return false;

            // Admin-ს ყოველთვის აქვს ყველა permission
            if (IsAdmin)
                return true;

            if (!IsAuthenticated)
                return false;

            // ვტვირთავთ permissions-ს cache-დან ან ბაზიდან
            LoadPermissions();

            // ვამოწმებთ კონკრეტული permission-ის არსებობას
            if (_permissionsCache != null &&
                _permissionsCache.ContainsKey(permissionName) &&
                _permissionsCache[permissionName])
            {
                return true;
            }

            // Hierarchical Permission Check: თუ კონკრეტული permission არ არის,
            // ვამოწმებთ parent permission-ს (მაგ: CanAddStudents -> CanManageStudents)
            if (TryGetParentPermission(permissionName, out string parentPermission))
            {
                if (_permissionsCache != null &&
                    _permissionsCache.ContainsKey(parentPermission) &&
                    _permissionsCache[parentPermission])
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasAnyPermission(params string[] permissionNames)
        {
            if (permissionNames == null || permissionNames.Length == 0)
                return false;

            foreach (var permission in permissionNames)
            {
                if (HasPermission(permission))
                    return true;
            }

            return false;
        }

        public bool CanAccessStudents() => HasAnyPermission(Permission.StudentsAccessPermissions);
        public bool CanAccessGroups() => HasAnyPermission(Permission.GroupsAccessPermissions);
        public bool CanAccessPayments() => HasAnyPermission(Permission.PaymentsAccessPermissions);
        public bool CanAccessUsers() => HasAnyPermission(Permission.UsersAccessPermissions);

        /// <summary>
        /// ამოწმებს აქვს თუ არა granular permission-ს parent permission
        /// მაგ: CanAddStudents -> CanManageStudents
        /// </summary>
        private bool TryGetParentPermission(string permission, out string parentPermission)
        {
            parentPermission = null;

            // Parent permission mappings - granular permissions inherit from "CanManage*" permissions
            var parentMappings = new Dictionary<string, string>
            {
                // Students
                { Permission.CanAddStudents, Permission.CanManageStudents },
                { Permission.CanEditStudents, Permission.CanManageStudents },
                { Permission.CanDeleteStudents, Permission.CanManageStudents },
                
                // Groups
                { Permission.CanAddGroups, Permission.CanManageGroups },
                { Permission.CanEditGroups, Permission.CanManageGroups },
                { Permission.CanDeleteGroups, Permission.CanManageGroups },
                
                // SubGroups (also inherit from CanManageGroups)
                { Permission.CanAddSubGroups, Permission.CanManageGroups },
                { Permission.CanEditSubGroups, Permission.CanManageGroups },
                { Permission.CanDeleteSubGroups, Permission.CanManageGroups },
                
                // Payments
                { Permission.CanAddPayments, Permission.CanManagePayments },
                { Permission.CanEditPayments, Permission.CanManagePayments },
                { Permission.CanDeletePayments, Permission.CanManagePayments },
                
                // Users
                { Permission.CanAddUsers, Permission.CanManageUsers },
                { Permission.CanEditUsers, Permission.CanManageUsers },
                { Permission.CanDeleteUsers, Permission.CanManageUsers }
            };

            return parentMappings.TryGetValue(permission, out parentPermission);
        }

        public Dictionary<string, bool> GetAllPermissions()
        {
            if (!IsAuthenticated)
                return new Dictionary<string, bool>();

            LoadPermissions();
            return _permissionsCache ?? new Dictionary<string, bool>();
        }

        public void Refresh()
        {
            // ანახლებს მომხმარებლის ინფორმაციას UserSession-დან
            var userId = UserSession.Id;
            if (userId > 0)
            {
                _currentUser = _userRepository.GetUserById(userId);
                _permissionsCache = null;
                ApplicationLogContext.Set(userId, _currentUser?.UserName);
            }
            else
            {
                _currentUser = null;
                _permissionsCache = null;
                ApplicationLogContext.Clear();
            }
        }

        private void LoadPermissions()
        {
            if (_permissionsCache != null || _currentUser == null)
                return;

            try
            {
                if (string.IsNullOrWhiteSpace(_currentUser.Permissions))
                {
                    _permissionsCache = new Dictionary<string, bool>();
                    return;
                }

                _permissionsCache = JsonConvert.DeserializeObject<Dictionary<string, bool>>(_currentUser.Permissions);
                if (_permissionsCache == null)
                {
                    _permissionsCache = new Dictionary<string, bool>();
                }
            }
            catch (Exception)
            {
                // თუ JSON არასწორია, ვქმნით ცარიელ dictionary-ს
                _permissionsCache = new Dictionary<string, bool>();
            }
        }
    }
}

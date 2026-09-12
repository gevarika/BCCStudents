using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Services.Logging
{
    public static class ApplicationLogPermissionMapper
    {
        public static string GetPermissionScopeForCategory(string category)
        {
            return category switch
            {
                LogCategory.Students => Permission.CanManageStudents,
                LogCategory.Groups => Permission.CanManageGroups,
                LogCategory.Payments => Permission.CanManagePayments,
                LogCategory.Import => Permission.CanImport,
                LogCategory.Sms => Permission.CanManageStudents,
                _ => Permission.CanEditSettings
            };
        }

        public static string GetCategoryForAuditFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return LogCategory.System;

            return fileName.ToLowerInvariant() switch
            {
                "students_log.txt" => LogCategory.Students,
                "groups_log.txt" => LogCategory.Groups,
                "payments_log.txt" => LogCategory.Payments,
                "import_log.txt" => LogCategory.Import,
                "sms_log.txt" => LogCategory.Sms,
                _ => LogCategory.System
            };
        }

        public static IReadOnlyList<string> GetAllowedCategories(Func<string, bool> hasPermission)
        {
            bool CanAccessStudents() =>
                hasPermission(Permission.CanManageStudents) ||
                hasPermission(Permission.CanAddStudents) ||
                hasPermission(Permission.CanEditStudents) ||
                hasPermission(Permission.CanDeleteStudents);

            bool CanAccessGroups() =>
                hasPermission(Permission.CanManageGroups) ||
                hasPermission(Permission.CanAddGroups) ||
                hasPermission(Permission.CanEditGroups) ||
                hasPermission(Permission.CanDeleteGroups) ||
                hasPermission(Permission.CanAddSubGroups) ||
                hasPermission(Permission.CanEditSubGroups) ||
                hasPermission(Permission.CanDeleteSubGroups);

            bool CanAccessPayments() =>
                hasPermission(Permission.CanManagePayments) ||
                hasPermission(Permission.CanAddPayments) ||
                hasPermission(Permission.CanEditPayments) ||
                hasPermission(Permission.CanDeletePayments);

            bool CanAccessUsers() =>
                hasPermission(Permission.CanManageUsers) ||
                hasPermission(Permission.CanAddUsers) ||
                hasPermission(Permission.CanEditUsers) ||
                hasPermission(Permission.CanDeleteUsers);

            var categories = new List<string>();
            if (CanAccessStudents() || hasPermission(Permission.CanViewReports))
                categories.Add(LogCategory.Students);
            if (CanAccessGroups())
                categories.Add(LogCategory.Groups);
            if (CanAccessPayments())
                categories.Add(LogCategory.Payments);
            if (hasPermission(Permission.CanImport))
                categories.Add(LogCategory.Import);
            if (hasPermission(Permission.CanEditSettings) || CanAccessUsers())
                categories.Add(LogCategory.System);
            return categories;
        }

        public static IReadOnlyList<string> GetAllowedPermissionScopes(IReadOnlyDictionary<string, bool> permissions)
        {
            var scopes = new List<string>();
            if (permissions == null)
                return scopes;

            bool Allowed(string permission) =>
                permissions.TryGetValue(permission, out var allowed) && allowed;

            void AddIfAllowed(string permission)
            {
                if (Allowed(permission))
                    scopes.Add(permission);
            }

            // Manage ან ნებისმიერი CRUD → შესაბამისი scope (ლოგის კატეგორიისთვის)
            if (Allowed(Permission.CanManageStudents) || Allowed(Permission.CanAddStudents) ||
                Allowed(Permission.CanEditStudents) || Allowed(Permission.CanDeleteStudents))
                scopes.Add(Permission.CanManageStudents);

            AddIfAllowed(Permission.CanViewReports);

            if (Allowed(Permission.CanManageGroups) || Allowed(Permission.CanAddGroups) ||
                Allowed(Permission.CanEditGroups) || Allowed(Permission.CanDeleteGroups) ||
                Allowed(Permission.CanAddSubGroups) || Allowed(Permission.CanEditSubGroups) ||
                Allowed(Permission.CanDeleteSubGroups))
                scopes.Add(Permission.CanManageGroups);

            if (Allowed(Permission.CanManagePayments) || Allowed(Permission.CanAddPayments) ||
                Allowed(Permission.CanEditPayments) || Allowed(Permission.CanDeletePayments))
                scopes.Add(Permission.CanManagePayments);

            AddIfAllowed(Permission.CanImport);
            AddIfAllowed(Permission.CanEditSettings);

            return scopes.Distinct().ToList();
        }
    }
}

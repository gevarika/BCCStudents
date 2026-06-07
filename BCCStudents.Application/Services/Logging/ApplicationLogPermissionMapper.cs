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
            var categories = new List<string>();
            if (hasPermission(Permission.CanManageStudents) || hasPermission(Permission.CanViewReports))
                categories.Add(LogCategory.Students);
            if (hasPermission(Permission.CanManageGroups))
                categories.Add(LogCategory.Groups);
            if (hasPermission(Permission.CanManagePayments))
                categories.Add(LogCategory.Payments);
            if (hasPermission(Permission.CanImport))
                categories.Add(LogCategory.Import);
            if (hasPermission(Permission.CanEditSettings) || hasPermission(Permission.CanManageUsers))
                categories.Add(LogCategory.System);
            return categories;
        }

        public static IReadOnlyList<string> GetAllowedPermissionScopes(IReadOnlyDictionary<string, bool> permissions)
        {
            var scopes = new List<string>();
            if (permissions == null)
                return scopes;

            void AddIfAllowed(string permission)
            {
                if (permissions.TryGetValue(permission, out var allowed) && allowed)
                    scopes.Add(permission);
            }

            AddIfAllowed(Permission.CanManageStudents);
            AddIfAllowed(Permission.CanViewReports);
            AddIfAllowed(Permission.CanManageGroups);
            AddIfAllowed(Permission.CanManagePayments);
            AddIfAllowed(Permission.CanImport);
            AddIfAllowed(Permission.CanEditSettings);

            return scopes.Distinct().ToList();
        }
    }
}

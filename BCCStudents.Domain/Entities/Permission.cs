namespace BCCStudents.Domain.Entities
{
    /// <summary>
    /// Permission constants - გამოიყენება User Access Management-ში
    /// </summary>
    public static class Permission
    {
        public const string CanImport = "CanImport";
        public const string CanDelete = "CanDelete";
        public const string CanEditSettings = "CanEditSettings";
        public const string CanManageUsers = "CanManageUsers";
        public const string CanManageGroups = "CanManageGroups";
        public const string CanManageStudents = "CanManageStudents";
        public const string CanManagePayments = "CanManagePayments";
        public const string CanExportData = "CanExportData";
        public const string CanViewReports = "CanViewReports";
        public const string CanAddStudents = "CanAddStudents";
        public const string CanEditStudents = "CanEditStudents";
        public const string CanDeleteStudents = "CanDeleteStudents";
        public const string CanAddGroups = "CanAddGroups";
        public const string CanEditGroups = "CanEditGroups";
        public const string CanDeleteGroups = "CanDeleteGroups";
        public const string CanAddSubGroups = "CanAddSubGroups";
        public const string CanEditSubGroups = "CanEditSubGroups";
        public const string CanDeleteSubGroups = "CanDeleteSubGroups";
        public const string CanAddPayments = "CanAddPayments";
        public const string CanEditPayments = "CanEditPayments";
        public const string CanDeletePayments = "CanDeletePayments";
        public const string CanAddLogs = "CanAddLogs";
        public const string CanEditLogs = "CanEditLogs";
        // Logs permissions removed - logs are typically read-only (append-only)
        // Reports CRUD permissions removed - reports are generated/viewed, not created/edited/deleted
        // CanViewReports is kept for viewing access

        public const string CanAddUsers = "CanAddUsers";
        public const string CanEditUsers = "CanEditUsers";
        public const string CanDeleteUsers = "CanDeleteUsers";

        /// <summary>
        /// ყველა permission-ის სია
        /// 
        /// NOTE: Hierarchical Permission System
        /// - "CanManage*" permissions are parent permissions that automatically include all CRUD operations
        /// - Granular permissions (CanAdd*, CanEdit*, CanDelete*) can be used for fine-grained control
        /// - If user has "CanManageStudents", they automatically have CanAddStudents, CanEditStudents, CanDeleteStudents
        /// - This allows both simple (assign CanManage*) and granular (assign specific operations) permission management
        /// </summary>
        public static string[] GetAllPermissions()
        {
            return new[]
            {
                CanImport,
                CanDelete,
                CanAddStudents,
                CanEditStudents,
                CanDeleteStudents,
                CanAddGroups,
                CanEditGroups,
                CanDeleteGroups,
                CanAddSubGroups,
                CanEditSubGroups,
                CanDeleteSubGroups,
                CanAddPayments,
                CanEditPayments,
                CanDeletePayments,
                // Logs permissions removed - logs are typically read-only (append-only)
                // Reports CRUD permissions removed - reports are generated/viewed, not created/edited/deleted
                // CanViewReports is kept for viewing access
                CanAddUsers,
                CanEditUsers,
                CanDeleteUsers,
                CanEditSettings,
                CanManageUsers,
                CanManageGroups,
                CanManageStudents,
                CanManagePayments,
                CanExportData,
                CanViewReports
            };
        }
    }
}

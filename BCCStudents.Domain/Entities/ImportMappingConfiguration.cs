namespace BCCStudents.Domain.Entities
{
    /// <summary>
    /// იმპორტის მეპინგის კონფიგურაცია
    /// </summary>
    public class ImportMappingConfiguration
    {
        /// <summary>
        /// Excel Sheet Name -> Database GroupId mapping
        /// </summary>
        public Dictionary<string, int> SheetToGroupMapping { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Excel Sheet Name -> Column Mapping
        /// Key: Sheet Name (case-insensitive)
        /// Value: Dictionary where Key is Excel Column Name, Value is Student Entity Property Name
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> ColumnMapping { get; set; } = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Helper method to get column mapping for a specific sheet
        /// </summary>
        public Dictionary<string, string> GetColumnMappingForSheet(string sheetName)
        {
            if (ColumnMapping == null || !ColumnMapping.TryGetValue(sheetName, out var mapping))
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            return mapping;
        }

        /// <summary>
        /// სტუდენტების Active Status-ი იმპორტისას
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// ვალიდაცია: შემოწმება რომ SheetToGroupMapping-ში ყველა sheet-ს ჰყავს group mapping
        /// </summary>
        public bool Validate()
        {
            if (SheetToGroupMapping == null || SheetToGroupMapping.Count == 0)
                return false;

            // შემოწმება რომ ყველა GroupId დადებითია
            foreach (var groupId in SheetToGroupMapping.Values)
            {
                if (groupId <= 0)
                    return false;
            }

            return true;
        }
    }
}

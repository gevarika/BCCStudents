namespace BCCStudents.Domain.Entities
{
    /// <summary>
    /// იმპორტის მეპინგის პრესეტი შენახვისთვის
    /// </summary>
    public class ImportMappingPreset
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUsed { get; set; }
        public ImportMappingConfiguration Configuration { get; set; }
        public string FileNamePattern { get; set; } // Excel ფაილის სახელის პატერნი (optional)
    }
}

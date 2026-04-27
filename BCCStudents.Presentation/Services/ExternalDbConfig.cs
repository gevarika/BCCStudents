namespace BCCStudents.Presentation.Services
{
    public sealed class ExternalDbConfig
    {
        public bool? UseLocalDb { get; set; }
        public bool? IsTestDb { get; set; }
        public string LocalMySqlConnectionString { get; set; }
        public string LocalMySqlConnectionString_Test { get; set; }
        public string ServerMySqlConnectionString { get; set; }
        public string ServerMySqlConnectionString_Test { get; set; }
        public string InitScriptPath { get; set; }
    }
}

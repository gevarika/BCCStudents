namespace BCCStudents.Domain.Entities
{
    public class ImportEventArgs : EventArgs
    {
        public List<Student> ImportedData { get; set; }

        public ImportEventArgs(List<Student> data)
        {
            ImportedData = data;
        }
    }
}


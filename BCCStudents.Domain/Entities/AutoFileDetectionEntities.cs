using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BCCStudents.Domain.Entities
{
    /// <summary>
    /// áƒáƒ¦áƒ›áƒáƒ©áƒ”áƒœáƒ˜áƒšáƒ˜ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ
    /// </summary>
    public class DetectedFile
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string FileHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }

    /// <summary>
    /// áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ
    /// </summary>
    public class ImportedFileInfo
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileHash { get; set; }
        public long FileSize { get; set; }
        public DateTime ImportedAt { get; set; }
        public string ImportedBy { get; set; }
        public string ComputerName { get; set; }
    }
}
    /// <summary>
    /// áƒ¤áƒáƒ˜áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ¢áƒ áƒ”áƒ™áƒ˜áƒœáƒ’áƒ˜áƒ¡ áƒ áƒ”áƒžáƒáƒ–áƒ˜áƒ¢áƒáƒ áƒ˜áƒ˜áƒ¡ áƒ˜áƒœáƒ¢áƒ”áƒ áƒ¤áƒ”áƒ˜áƒ¡áƒ˜
    /// </summary>

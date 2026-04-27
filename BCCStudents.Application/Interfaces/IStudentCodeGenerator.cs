namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// სტუდენტის კოდის გენერატორის ინტერფეისი
    /// </summary>
    public interface IStudentCodeGenerator
    {
        /// <summary>
        /// გენერირებს ახალ სტუდენტის კოდს
        /// </summary>
        /// <returns>უნიკალური სტუდენტის კოდი</returns>
        string GenerateStudentCode();

        /// <summary>
        /// გენერირებს უნიკალურ სტუდენტის კოდს, რომელიც არ არსებობს მოცემულ სიებში
        /// </summary>
        /// <param name="existingCodes">არსებული კოდების სია</param>
        /// <param name="pendingCodes">მომლოდინე კოდების სია</param>
        /// <returns>უნიკალური სტუდენტის კოდი</returns>
        string GenerateUniqueStudentCode(HashSet<string> existingCodes, HashSet<string> pendingCodes);
    }
}


namespace BCCStudents.Domain.Entities
{
    /// <summary>
    /// წარუმატებელი მწკრივის ინფორმაცია იმპორტის დროს
    /// </summary>
    public class FailedRow
    {
        public int RowNumber { get; set; }
        public string Reason { get; set; }
    }
}


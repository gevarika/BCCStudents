using System.Text;

namespace BCCStudents.Application.Services
{
    public static class PaymentDateManager
    {
        private static readonly string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "next_payment_date.dat");

        private static string Encrypt(string plainText) => Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        private static string Decrypt(string encryptedText) => Encoding.UTF8.GetString(Convert.FromBase64String(encryptedText));

        public static void SaveNextPaymentDate(DateTime nextPaymentDate)
        {
            string encryptedDate = Encrypt(nextPaymentDate.ToString("yyyy-MM-dd"));
            File.WriteAllText(filePath, encryptedDate);
        }
        public static DateTime? GetNextPaymentDate()
        {
            if (!File.Exists(filePath)) return null;

            string encryptedDate = File.ReadAllText(filePath);
            return DateTime.Parse(Decrypt(encryptedDate));
        }
        public static bool IsNextPaymentDateSet() => File.Exists(filePath);
    }
}


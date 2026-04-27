namespace BCCStudents.Presentation
{
    public static class FormTitleHelper
    {
        private const string BaseTitle = "ბოლნისის კულტურის ცენტრი";

        public static void SetTitle(Form form, string subtitle)
        {
            if (form == null) return;

            var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "";
            form.Text = string.IsNullOrWhiteSpace(ver)
                ? $"{BaseTitle} - {subtitle}"
                : $"(v{ver}) {BaseTitle} - {subtitle} ";
        }
    }
}


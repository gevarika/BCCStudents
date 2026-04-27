using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using System.Text.Json;

namespace BCCStudents.Infrastructure.Services
{
    /// <summary>
    /// SMS სერვისის იმპლემენტაცია
    /// იმპლემენტირებს ISmsService ინტერფეისს
    /// </summary>
    public class SmsService : ISmsService
    {
        public string _sender { get; set; } = "test";
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IConfigurationService _config;

        public SmsService(IConfigurationService config)
        {
            _httpClient = new HttpClient();
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<SmsSendResult> SendSmsAsync(string phoneNumber, string message)
        {
            if (_config.SmsEnabled)
            {
                string apiKey = _config.SmsApiKey;
                var query = $"key={Uri.EscapeDataString(apiKey)}" +
                        $"&destination={Uri.EscapeDataString(phoneNumber)}" +
                        $"&sender={Uri.EscapeDataString(_sender)}" +
                        $"&content={Uri.EscapeDataString(message)}";

                var url = $"https://smsoffice.ge/api/v2/send/?{query}";

                try
                {
                    var response = await _httpClient.GetAsync(url);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var result = new SmsSendResult { RawResponse = responseBody };

                    if (!response.IsSuccessStatusCode)
                    {
                        result.Success = false;
                        result.Status = "HTTP ERROR";
                        return result;
                    }

                    var json = JsonDocument.Parse(responseBody);
                    var root = json.RootElement;

                    result.Success = root.GetProperty("Success").GetBoolean();


                    result.Success = root.GetProperty("Success").GetBoolean();
                    result.Status = root.GetProperty("Message").GetString();
                    result.Message = root.GetProperty("Message").GetString();
                    result.ErrorCode = root.GetProperty("ErrorCode").GetInt32();


                    return result;
                }
                catch (Exception ex)
                {
                    return new SmsSendResult
                    {
                        Success = false,
                        Status = $"Exception: {ex.Message}",
                        RawResponse = ""
                    };
                }
            }
            else return new SmsSendResult { Success = false, Status = "SMS სერვისი გამორთულია", };
        }
    }

}


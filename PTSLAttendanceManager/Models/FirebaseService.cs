using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PTSLAttendanceManager.Services
{
    public class FirebaseService
    {
        private readonly string _firebaseApiKey;
        private readonly IHttpClientFactory _httpClientFactory;

        public FirebaseService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _firebaseApiKey = configuration["Firebase:ApiKey"];  
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> SendOtpAsync(string phoneNumber)
        {
            var client = _httpClientFactory.CreateClient();

            var requestBody = new
            {
                phoneNumber = phoneNumber
            };

            var response = await client.PostAsJsonAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:sendVerificationCode?key={_firebaseApiKey}",
                requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Firebase response error: {error}");  // Debug log
                throw new HttpRequestException($"Error sending OTP: {error}");
            }

            var responseData = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(responseData);

            return json.RootElement.GetProperty("sessionInfo").GetString();
        }

        public async Task<bool> VerifyOtpAsync(string sessionInfo, string otpCode)
        {
            var client = _httpClientFactory.CreateClient();

            var requestBody = new
            {
                sessionInfo = sessionInfo,
                code = otpCode
            };

            var response = await client.PostAsJsonAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPhoneNumber?key={_firebaseApiKey}",
                requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Firebase response error: {error}");  // Debug log
                throw new HttpRequestException($"Error verifying OTP: {error}");
            }

            var responseData = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(responseData);
            var idToken = json.RootElement.GetProperty("idToken").GetString();

            return !string.IsNullOrEmpty(idToken);
        }
    }
}

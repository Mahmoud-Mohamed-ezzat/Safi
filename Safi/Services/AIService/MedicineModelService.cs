using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Safi.Dto.AIModelsDto;

namespace Safi.Services.AIService
{
    public class MedicineModelService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _baseURL = "http://localhost:5001/api/";

        public MedicineModelService()
        {
        }

        public async Task<string> LookupMedicineAsync(MedicineLookupRequestDto request)
        {
            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_baseURL + "lookup", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> AnalyzePrescriptionAsync(Stream imageStream, string fileName)
        {
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(imageStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // Default to jpeg, but usually doesn't matter much for Flask

            content.Add(streamContent, "image", fileName);

            var response = await _httpClient.PostAsync(_baseURL + "prescription", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}

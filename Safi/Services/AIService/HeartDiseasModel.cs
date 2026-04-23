using System.Text;
using System.Text.Json;
using Safi.Dto.AIModelsDto;
namespace Safi.Services.AIService
{
    public class HeartDiseasModel
    {

        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _baseURL = "http://localhost:8080/predict";
        public HeartDiseasModel()
        {
        }

        public async Task<string> PredictHeartDiseasModel(HeartDiseaseRequestDto Input)
        {
            var options = new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower 
            };
            var json = JsonSerializer.Serialize(Input, options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseURL, content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}

using System.Text;
using System.Text.Json;
using Safi.Dto.AIModelsDto;
namespace Safi.Services.AIService
{
    public class LiverModel
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _baseURL = "http://127.0.0.1:5000/";
        public LiverModel()
        {
        }

        public async Task<string> PredictLiverModel(LiverModelRequestDto Input)
        {
            var json = JsonSerializer.Serialize(Input);
            var content = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await _httpClient.PostAsync(_baseURL, content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return result;

        }
        }
    }


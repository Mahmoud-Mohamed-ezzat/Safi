using Azure;
using Safi.Dto.DiseaseDto;
using System.Text;
using System.Text.Json;

namespace Safi.Services.AIService
{
    public class DiseaseModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseURL = "http://192.168.1.17:3005/";
        
        public DiseaseModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task<SymptomsResponseDto> get_symptoms()
        {
            var response = await _httpClient.GetAsync(_baseURL + "api/symptoms");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SymptomsResponseDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result;
        }
        public async Task<SymptomsPostResponseDto> PostSymptoms(SymptomsPostRequestDto input)
        {
            var json = JsonSerializer.Serialize(input);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseURL + "api/predict", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SymptomsPostResponseDto>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result;
        }
}
}

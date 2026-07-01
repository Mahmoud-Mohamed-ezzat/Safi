using Safi.Dto.MedicienesDto;
using System.Text.Json;

namespace Safi.Services.AIService
{
    public class MedicineService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _baseUrl = "http://localhost:3005"; // Replace with actual base URL
        internal async Task AnalyzePrescriptionAsync(PrescriptionAnalyzeRequestDto request)
        {


        }

        internal async Task<bool> DeleteFileAsync(object filename)
        {
            throw new NotImplementedException();
        }

        internal async Task GetAllMedicinesAsync()
        {
            throw new NotImplementedException();
        }

        internal async Task GetSuggestionsAsync(string q, int limit)
        {
            throw new NotImplementedException();
        }

        internal async Task<bool> HealthCheckAsync()
        {
            throw new NotImplementedException();
        }

        internal async Task<MedicineResultDto?> SearchMedicineAsync(string name)
        {
            JsonSerializer.Serialize(name);
             var response = await _httpClient.GetAsync($"{_baseUrl}/api/search");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<MedicineResultDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result;
        }

        internal async Task UploadImageAsync(Stream stream, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}

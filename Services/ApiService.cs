using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using QuimiOSCompanion.Models;

namespace QuimiOSCompanion.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://quimioshub.railway.app/api";

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        public async Task<List<Sample>> GetSamplesAsync(int? clientId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = $"samples?page=1&pageSize=100";
            if (clientId.HasValue) query += $"&clientId={clientId}";
            if (startDate.HasValue) query += $"&startDate={startDate:yyyy-MM-dd}";
            if (endDate.HasValue) query += $"&endDate={endDate:yyyy-MM-dd}";

            return await _httpClient.GetFromJsonAsync<List<Sample>>(query);
        }

        public async Task<List<Sample>> GetPendingSamplesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Sample>>("samples/pending");
        }

        public async Task<List<InventoryItem>> GetInventoryItemsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<InventoryItem>>("inventory-items");
        }

        public async Task<bool> CreateShiftHandoverAsync(int shiftId, int userId, DateTime handoverDate, string notes, List<PendingSample> samples)
        {
            var dto = new
            {
                shiftId,
                userId,
                handoverDate,
                notes,
                pendingSamples = samples
            };

            var response = await _httpClient.PostAsJsonAsync("shift-handovers", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<ShiftHandover>> GetShiftHandoversAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ShiftHandover>>("shift-handovers");
        }
    }
}

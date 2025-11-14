
// Proyecto3.Infrastructure/Services/ExternalApiService.cs

using Proyecto3.Core.Interfaces;
using Proyecto3.Core.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Proyecto3.Infrastructure.Services
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;

        // HttpClient inyectado por el framework
        public ExternalApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ExternalEmployeeData> GetEmployeeDetailsAsync(int colaboradorId)
        {
            // --- SIMULACIÓN DE LLAMADA A API EXTERNA ---

            // En un proyecto real, usarías:
            // var response = await _httpClient.GetAsync($"/api/hr/employees/{colaboradorId}");
            // response.EnsureSuccessStatusCode();
            // var data = await response.Content.ReadFromJsonAsync<ExternalEmployeeData>();
            // return data;

            await Task.Delay(50); // Simular latencia de red

            return new ExternalEmployeeData
            {
                ColaboradorID = colaboradorId,
                HRStatus = colaboradorId % 2 == 0 ? "Activo" : "Permiso",
                AnnualSalary = 60000.00M + (colaboradorId * 1000),
                LastLogin = DateTime.Now.AddDays(-(colaboradorId % 5)).ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }
}
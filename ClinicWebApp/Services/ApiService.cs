using System.Net.Http.Json;
using ClinicWebApp.Models;

namespace ClinicWebApp.Services;

public class ApiService
{
    private readonly HttpClient _http = new();

    // URL для чемпионата (когда будет реальный API)
    private const string ApiUrl = "http://10.30.76.66:8082/PersonLocations";

    // true = заглушка, false = реальный API (переключи на чемпионате)
    private const bool UseStub = true;

    public async Task<List<PersonLocation>> GetPersonLocationsAsync()
    {
        if (UseStub)
        {
            return GenerateStubData();
        }

        try
        {
            var result = await _http.GetFromJsonAsync<List<PersonLocation>>(ApiUrl);
            return result ?? new List<PersonLocation>();
        }
        catch
        {
            return GenerateStubData();
        }
    }

    /// <summary>
    /// Заглушка: генерирует случайные данные о местоположении людей.
    /// Используется для тренировки, пока нет реального API.
    /// </summary>
    private static List<PersonLocation> GenerateStubData()
    {
        var rnd = new Random();
        var list = new List<PersonLocation>();

        for (int i = 1; i <= 15; i++)
        {
            list.Add(new PersonLocation
            {
                PersonCode = $"EMP{i:D3}",
                PersonRole = "Сотрудник",
                LastSecurityPointNumber = rnd.Next(1, 20),
                LastSecurityPointDirection = rnd.Next(2) == 0 ? "in" : "out",
                LastSecurityPointTime = DateTime.Now.AddMinutes(-rnd.Next(1, 60))
            });
        }

        for (int i = 1; i <= 25; i++)
        {
            list.Add(new PersonLocation
            {
                PersonCode = $"CLI{i:D3}",
                PersonRole = "Клиент",
                LastSecurityPointNumber = rnd.Next(1, 20),
                LastSecurityPointDirection = rnd.Next(2) == 0 ? "in" : "out",
                LastSecurityPointTime = DateTime.Now.AddMinutes(-rnd.Next(1, 60))
            });
        }

        return list;
    }
}
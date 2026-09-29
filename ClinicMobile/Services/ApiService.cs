using System.Net.Http.Json;
using ClinicMobile.Models;

namespace ClinicMobile.Services;

public class ApiService
{
    private readonly HttpClient _http = new();
    
    private const string BaseUrl = "http://localhost:5112/api";

    public async Task<List<Medicine>> GetMedicinesAsync()
        => await _http.GetFromJsonAsync<List<Medicine>>($"{BaseUrl}/medicines") ?? new();

    public async Task<List<Stock>> GetStocksAsync(int? medicineId = null)
    {
        var url = $"{BaseUrl}/medicines/stocks";
        if (medicineId.HasValue) url += $"?medicineId={medicineId}";
        return await _http.GetFromJsonAsync<List<Stock>>(url) ?? new();
    }

    public async Task<List<Request>> GetRequestsAsync()
        => await _http.GetFromJsonAsync<List<Request>>($"{BaseUrl}/requests") ?? new();

    public async Task<List<Stock>> GetReportAsync()
        => await _http.GetFromJsonAsync<List<Stock>>($"{BaseUrl}/medicines/report") ?? new();
}
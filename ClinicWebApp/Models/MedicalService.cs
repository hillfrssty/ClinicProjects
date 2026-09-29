namespace ClinicWebApp.Models;

public class MedicalService
{
    public int Id { get; set; }
    public string ServiceName { get; set; } = "";
    public string ServiceType { get; set; } = "";
    public decimal Price { get; set; }
}
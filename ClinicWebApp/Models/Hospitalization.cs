namespace ClinicWebApp.Models;

public class Hospitalization
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = "";
    public string HospitalizationCode { get; set; } = "";
    public string Department { get; set; } = "";
    public string Purpose { get; set; } = "";
    public DateTime AdmissionDate { get; set; } = DateTime.Today;
    public DateTime? DischargeDate { get; set; }
    public string Conditions { get; set; } = "";
    public string Status { get; set; } = "";
}
namespace ClinicWebApp.Models;

public class Patient
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string MiddleName { get; set; } = "";
    public string PassportSeries { get; set; } = "";
    public string PassportNumber { get; set; } = "";
    public DateTime BirthDate { get; set; } = DateTime.Today;
    public string Gender { get; set; } = "";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Workplace { get; set; } = "";
    public string InsurancePolicy { get; set; } = "";
    public string InsuranceCompany { get; set; } = "";
    public DateTime PolicyExpiry { get; set; } = DateTime.Today;
    public string MedicalCardNumber { get; set; } = "";
    public DateTime CardIssueDate { get; set; } = DateTime.Today;
    public string? Diagnosis { get; set; }
}
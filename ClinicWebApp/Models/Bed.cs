namespace ClinicWebApp.Models;

public class Bed
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = "";
    public string BedNumber { get; set; } = "";
    public int? PatientId { get; set; }
    public string PatientName { get; set; } = "";

    public bool IsOccupied => PatientId != null;
}
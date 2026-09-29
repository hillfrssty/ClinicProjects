namespace PharmacyApi.Models;

public class RequestItem
{
    public int Id { get; set; }
    public int RequestId { get; set; }
    public int MedicineId { get; set; }
    public int Quantity { get; set; }
    public bool Issued { get; set; }
    public string MedicineName { get; set; } = "";
}
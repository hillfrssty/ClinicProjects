namespace PharmacyApi.Models;

public class Stock
{
    public int Id { get; set; }
    public string Warehouse { get; set; } = "";
    public int MedicineId { get; set; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string MedicineName { get; set; } = "";
}
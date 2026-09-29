namespace PharmacyApi.Models;

public class StockMovement
{
    public int Id { get; set; }
    public int MedicineId { get; set; }
    public string FromWarehouse { get; set; } = "";
    public string ToWarehouse { get; set; } = "";
    public int Quantity { get; set; }
    public string Reason { get; set; } = "";
    public string MovementType { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
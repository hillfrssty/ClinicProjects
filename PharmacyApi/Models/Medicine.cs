namespace PharmacyApi.Models;

public class Medicine
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Manufacturer { get; set; } = "";
    public string Form { get; set; } = "";
    public string Dosage { get; set; } = "";
    public int OptimalQuantity { get; set; }
    public decimal Price { get; set; }
}
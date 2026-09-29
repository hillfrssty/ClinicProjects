namespace PharmacyApi.Models;

public class Request
{
    public int Id { get; set; }
    public string Department { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "";
    public List<RequestItem> Items { get; set; } = new();
}
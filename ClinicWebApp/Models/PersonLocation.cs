namespace ClinicWebApp.Models;

public class PersonLocation
{
    public string PersonCode { get; set; } = "";
    public string PersonRole { get; set; } = "";      // "Клиент" или "Сотрудник"
    public int LastSecurityPointNumber { get; set; }
    public string LastSecurityPointDirection { get; set; } = "";  // "in" или "out"
    public DateTime LastSecurityPointTime { get; set; }
}
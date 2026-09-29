using MySql.Data.MySqlClient;
using ClinicWebApp.Models;

namespace ClinicWebApp.Services;

public class ServiceService
{
    private readonly string _connStr = "server=localhost;user=root;password=Hillfessty314!;database=clinic_db";

    public List<MedicalService> GetAll()
    {
        var list = new List<MedicalService>();
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        using var cmd = new MySqlCommand("SELECT id, service_name, service_type, price FROM medical_services", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new MedicalService
            {
                Id = r.GetInt32("id"),
                ServiceName = r.GetString("service_name"),
                ServiceType = r.IsDBNull(r.GetOrdinal("service_type")) ? "" : r.GetString("service_type"),
                Price = r.GetDecimal("price")
            });
        }
        return list;
    }
}
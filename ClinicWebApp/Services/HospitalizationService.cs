using MySql.Data.MySqlClient;
using ClinicWebApp.Models;

namespace ClinicWebApp.Services;

public class HospitalizationService
{
    private readonly string _connStr = "server=localhost;user=root;password=Hillfessty314!;database=clinic_db";

    public List<Hospitalization> GetAll()
    {
        var list = new List<Hospitalization>();
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        string sql = @"SELECT h.id, h.patient_id, h.hospitalization_code, h.department, h.purpose,
                       h.admission_date, h.discharge_date, h.conditions, h.status,
                       CONCAT(p.last_name, ' ', p.first_name) AS patient_name
                       FROM hospitalizations h
                       LEFT JOIN patients p ON p.id = h.patient_id
                       ORDER BY h.id DESC LIMIT 100";
        using var cmd = new MySqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Hospitalization
            {
                Id = r.GetInt32("id"),
                PatientId = r.GetInt32("patient_id"),
                PatientName = r.IsDBNull(r.GetOrdinal("patient_name")) ? "" : r.GetString("patient_name"),
                HospitalizationCode = r.IsDBNull(r.GetOrdinal("hospitalization_code")) ? "" : r.GetString("hospitalization_code"),
                Department = r.IsDBNull(r.GetOrdinal("department")) ? "" : r.GetString("department"),
                Purpose = r.IsDBNull(r.GetOrdinal("purpose")) ? "" : r.GetString("purpose"),
                AdmissionDate = r.IsDBNull(r.GetOrdinal("admission_date")) ? DateTime.Today : r.GetDateTime("admission_date"),
                DischargeDate = r.IsDBNull(r.GetOrdinal("discharge_date")) ? null : r.GetDateTime("discharge_date"),
                Conditions = r.IsDBNull(r.GetOrdinal("conditions")) ? "" : r.GetString("conditions"),
                Status = r.IsDBNull(r.GetOrdinal("status")) ? "" : r.GetString("status")
            });
        }
        return list;
    }

    public void Add(int patientId, string department, string purpose, DateTime date, string conditions)
    {
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        string sql = @"INSERT INTO hospitalizations 
                       (patient_id, hospitalization_code, department, purpose, admission_date, conditions, status)
                       VALUES (@pid, @code, @dep, @pur, @date, @cond, 'Запланирована')";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@pid", patientId);
        cmd.Parameters.AddWithValue("@code", $"HOSP-{DateTime.Now.Ticks % 100000}");
        cmd.Parameters.AddWithValue("@dep", department);
        cmd.Parameters.AddWithValue("@pur", purpose);
        cmd.Parameters.AddWithValue("@date", date);
        cmd.Parameters.AddWithValue("@cond", conditions);
        cmd.ExecuteNonQuery();
    }

    public void Cancel(int id)
    {
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        using var cmd = new MySqlCommand("UPDATE hospitalizations SET status='Отменена' WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
}
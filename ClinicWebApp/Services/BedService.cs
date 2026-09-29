using MySql.Data.MySqlClient;
using ClinicWebApp.Models;

namespace ClinicWebApp.Services;

public class BedService
{
    private readonly string _connStr = "server=localhost;user=root;password=Hillfessty314!;database=clinic_db";

    public List<Bed> GetAll()
    {
        var list = new List<Bed>();
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        string sql = @"SELECT b.id, b.room_number, b.bed_number, b.patient_id,
                       CONCAT(p.last_name, ' ', p.first_name) AS patient_name
                       FROM beds b
                       LEFT JOIN patients p ON p.id = b.patient_id
                       ORDER BY b.room_number, b.bed_number";
        using var cmd = new MySqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Bed
            {
                Id = r.GetInt32("id"),
                RoomNumber = r.GetString("room_number"),
                BedNumber = r.GetString("bed_number"),
                PatientId = r.IsDBNull(r.GetOrdinal("patient_id")) ? null : r.GetInt32("patient_id"),
                PatientName = r.IsDBNull(r.GetOrdinal("patient_name")) ? "" : r.GetString("patient_name")
            });
        }
        return list;
    }

    /// <summary>
    /// Переводит пациента на другую койку. Если patient_id == null — освобождает койку.
    /// </summary>
    public void MovePatient(int bedId, int? patientId)
    {
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        using var cmd = new MySqlCommand("UPDATE beds SET patient_id=@pid WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("@pid", (object?)patientId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@id", bedId);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Выписывает пациента — освобождает койку.
    /// </summary>
    public void Discharge(int bedId)
    {
        MovePatient(bedId, null);
    }
}
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ClinicApp.Models;

namespace ClinicApp.Services;

public class ScheduleService
{
    private readonly string _connStr = "server=localhost;user=root;password=Hillfessty314!;database=clinic_db";

    public List<Doctor> GetDoctors()
    {
        var list = new List<Doctor>();
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        using var cmd = new MySqlCommand("SELECT * FROM doctors ORDER BY last_name", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Doctor
            {
                Id = r.GetInt32("id"),
                LastName = r.GetString("last_name"),
                FirstName = r.GetString("first_name"),
                MiddleName = r.IsDBNull(r.GetOrdinal("middle_name")) ? "" : r.GetString("middle_name"),
                Specialization = r.IsDBNull(r.GetOrdinal("specialization")) ? "" : r.GetString("specialization"),
                Phone = r.IsDBNull(r.GetOrdinal("phone")) ? "" : r.GetString("phone")
            });
        }
        return list;
    }

    public List<ScheduleItem> GetAll()
    {
        var list = new List<ScheduleItem>();
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        string sql = @"SELECT s.*, 
                       CONCAT(d.last_name, ' ', d.first_name) AS doctor_name,
                       CONCAT(p.last_name, ' ', p.first_name) AS patient_name
                       FROM schedule s
                       LEFT JOIN doctors d ON d.id = s.doctor_id
                       LEFT JOIN patients p ON p.id = s.patient_id
                       ORDER BY s.visit_date, s.visit_time";
        using var cmd = new MySqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new ScheduleItem
            {
                Id = r.GetInt32("id"),
                DoctorId = r.GetInt32("doctor_id"),
                PatientId = r.IsDBNull(r.GetOrdinal("patient_id")) ? null : r.GetInt32("patient_id"),
                VisitDate = r.GetDateTime("visit_date"),
                VisitTime = r.GetTimeSpan("visit_time"),
                Status = r.IsDBNull(r.GetOrdinal("status")) ? "" : r.GetString("status"),
                IsApproved = r.GetBoolean("is_approved"),
                Notes = r.IsDBNull(r.GetOrdinal("notes")) ? "" : r.GetString("notes"),
                DoctorName = r.IsDBNull(r.GetOrdinal("doctor_name")) ? "" : r.GetString("doctor_name"),
                PatientName = r.IsDBNull(r.GetOrdinal("patient_name")) ? "" : r.GetString("patient_name")
            });
        }
        return list;
    }

    /// <summary>Запись пациента к врачу.</summary>
    public void BookPatient(int scheduleId, int patientId)
    {
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        using var cmd = new MySqlCommand(
            "UPDATE schedule SET patient_id=@pid, status='Записан' WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("@pid", patientId);
        cmd.Parameters.AddWithValue("@id", scheduleId);
        cmd.ExecuteNonQuery();

        // История
        LogChange(scheduleId, "Запись пациента", "", patientId.ToString());
    }

    /// <summary>Отмена записи.</summary>
    public void CancelBooking(int scheduleId)
    {
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        using var cmd = new MySqlCommand(
            "UPDATE schedule SET patient_id=NULL, status='Свободно' WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("@id", scheduleId);
        cmd.ExecuteNonQuery();

        LogChange(scheduleId, "Отмена записи", "", "");
    }

    /// <summary>Утверждение расписания главным врачом.</summary>
    public void Approve(DateTime date)
    {
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        using var cmd = new MySqlCommand(
            "UPDATE schedule SET is_approved=1 WHERE visit_date=@d", conn);
        cmd.Parameters.AddWithValue("@d", date);
        cmd.ExecuteNonQuery();

        LogChange(0, $"Утверждение расписания на {date:dd.MM.yyyy}", "0", "1");
    }

    private void LogChange(int scheduleId, string action, string oldVal, string newVal)
    {
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        using var cmd = new MySqlCommand(
            @"INSERT INTO schedule_history (schedule_id, changed_by, action, old_value, new_value) 
              VALUES (@sid, 'admin', @act, @old, @new)", conn);
        cmd.Parameters.AddWithValue("@sid", scheduleId);
        cmd.Parameters.AddWithValue("@act", action);
        cmd.Parameters.AddWithValue("@old", oldVal);
        cmd.Parameters.AddWithValue("@new", newVal);
        cmd.ExecuteNonQuery();
    }
}
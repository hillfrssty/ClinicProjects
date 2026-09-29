using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ClinicApp.Models;

namespace ClinicApp.Services;

public class PatientService
{
    private readonly string _connStr = "server=localhost;user=root;password=Hillfessty314!;database=clinic_db";

    public List<Patient> GetAll()
    {
        var list = new List<Patient>();
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        using var cmd = new MySqlCommand("SELECT id, first_name, last_name, phone, medical_card_number FROM patients ORDER BY last_name", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Patient
            {
                Id = r.GetInt32("id"),
                FirstName = r.IsDBNull(r.GetOrdinal("first_name")) ? "" : r.GetString("first_name"),
                LastName = r.IsDBNull(r.GetOrdinal("last_name")) ? "" : r.GetString("last_name"),
                Phone = r.IsDBNull(r.GetOrdinal("phone")) ? "" : r.GetString("phone"),
                MedicalCardNumber = r.IsDBNull(r.GetOrdinal("medical_card_number")) ? "" : r.GetString("medical_card_number")
            });
        }
        return list;
    }
}
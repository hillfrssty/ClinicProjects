using MySql.Data.MySqlClient;
using ClinicWebApp.Models;

namespace ClinicWebApp.Services;

public class PatientService
{
    private readonly string _connStr = "server=localhost;user=root;password=Hillfessty314!;database=clinic_db";

    public List<Patient> GetAll()
    {
        var list = new List<Patient>();
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        string sql = @"SELECT id, first_name, last_name, middle_name, passport_series, passport_number,
                       birth_date, gender, address, phone, email, workplace,
                       insurance_policy, insurance_company, policy_expiry,
                       medical_card_number, card_issue_date, diagnosis
                       FROM patients ORDER BY id DESC";
        using var cmd = new MySqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(Map(r));
        return list;
    }

    public Patient? GetById(int id)
    {
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        using var cmd = new MySqlCommand("SELECT * FROM patients WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var r = cmd.ExecuteReader();
        return r.Read() ? Map(r) : null;
    }

    public Patient? GetByCard(string card)
    {
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        using var cmd = new MySqlCommand("SELECT * FROM patients WHERE medical_card_number=@c", conn);
        cmd.Parameters.AddWithValue("@c", card);
        using var r = cmd.ExecuteReader();
        return r.Read() ? Map(r) : null;
    }

    private static Patient Map(MySqlDataReader r) => new Patient
    {
        Id = r.GetInt32("id"),
        FirstName = r.IsDBNull(r.GetOrdinal("first_name")) ? "" : r.GetString("first_name"),
        LastName = r.IsDBNull(r.GetOrdinal("last_name")) ? "" : r.GetString("last_name"),
        MiddleName = r.IsDBNull(r.GetOrdinal("middle_name")) ? "" : r.GetString("middle_name"),
        PassportSeries = r.IsDBNull(r.GetOrdinal("passport_series")) ? "" : r.GetString("passport_series"),
        PassportNumber = r.IsDBNull(r.GetOrdinal("passport_number")) ? "" : r.GetString("passport_number"),
        BirthDate = r.IsDBNull(r.GetOrdinal("birth_date")) ? DateTime.Today : r.GetDateTime("birth_date"),
        Gender = r.IsDBNull(r.GetOrdinal("gender")) ? "" : r.GetString("gender"),
        Address = r.IsDBNull(r.GetOrdinal("address")) ? "" : r.GetString("address"),
        Phone = r.IsDBNull(r.GetOrdinal("phone")) ? "" : r.GetString("phone"),
        Email = r.IsDBNull(r.GetOrdinal("email")) ? "" : r.GetString("email"),
        Workplace = r.IsDBNull(r.GetOrdinal("workplace")) ? "" : r.GetString("workplace"),
        InsurancePolicy = r.IsDBNull(r.GetOrdinal("insurance_policy")) ? "" : r.GetString("insurance_policy"),
        InsuranceCompany = r.IsDBNull(r.GetOrdinal("insurance_company")) ? "" : r.GetString("insurance_company"),
        PolicyExpiry = r.IsDBNull(r.GetOrdinal("policy_expiry")) ? DateTime.Today : r.GetDateTime("policy_expiry"),
        MedicalCardNumber = r.IsDBNull(r.GetOrdinal("medical_card_number")) ? "" : r.GetString("medical_card_number"),
        CardIssueDate = r.IsDBNull(r.GetOrdinal("card_issue_date")) ? DateTime.Today : r.GetDateTime("card_issue_date"),
        Diagnosis = r.IsDBNull(r.GetOrdinal("diagnosis")) ? null : r.GetString("diagnosis")
    };

    public void Add(Patient p)
    {
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        string sql = @"INSERT INTO patients (first_name, last_name, middle_name, passport_series, passport_number,
                       birth_date, gender, address, phone, email, workplace,
                       insurance_policy, insurance_company, policy_expiry, medical_card_number, card_issue_date)
                       VALUES (@fn,@ln,@mn,@ps,@pn,@bd,@g,@a,@ph,@em,@w,@ip,@ic,@pe,@mcn,@cid)";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@fn", p.FirstName);
        cmd.Parameters.AddWithValue("@ln", p.LastName);
        cmd.Parameters.AddWithValue("@mn", p.MiddleName);
        cmd.Parameters.AddWithValue("@ps", p.PassportSeries);
        cmd.Parameters.AddWithValue("@pn", p.PassportNumber);
        cmd.Parameters.AddWithValue("@bd", p.BirthDate);
        cmd.Parameters.AddWithValue("@g", p.Gender);
        cmd.Parameters.AddWithValue("@a", p.Address);
        cmd.Parameters.AddWithValue("@ph", p.Phone);
        cmd.Parameters.AddWithValue("@em", p.Email);
        cmd.Parameters.AddWithValue("@w", p.Workplace);
        cmd.Parameters.AddWithValue("@ip", p.InsurancePolicy);
        cmd.Parameters.AddWithValue("@ic", p.InsuranceCompany);
        cmd.Parameters.AddWithValue("@pe", p.PolicyExpiry);
        cmd.Parameters.AddWithValue("@mcn", p.MedicalCardNumber);
        cmd.Parameters.AddWithValue("@cid", p.CardIssueDate);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        using (var c1 = new MySqlCommand("DELETE FROM hospitalizations WHERE patient_id=@id", conn))
        { c1.Parameters.AddWithValue("@id", id); c1.ExecuteNonQuery(); }
        using (var c2 = new MySqlCommand("DELETE FROM patients WHERE id=@id", conn))
        { c2.Parameters.AddWithValue("@id", id); c2.ExecuteNonQuery(); }
    }
}
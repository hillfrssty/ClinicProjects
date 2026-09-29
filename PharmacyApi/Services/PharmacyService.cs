using MySql.Data.MySqlClient;
using PharmacyApi.Models;

namespace PharmacyApi.Services;

public class PharmacyService
{
    private readonly string _connStr = "server=localhost;user=root;password=Hillfessty314!;database=clinic_db";

   
    public List<Medicine> GetAllMedicines()
    {
        var list = new List<Medicine>();
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        using var cmd = new MySqlCommand("SELECT * FROM medicines ORDER BY name", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Medicine
            {
                Id = r.GetInt32("id"),
                Name = r.GetString("name"),
                Manufacturer = r.IsDBNull(r.GetOrdinal("manufacturer")) ? "" : r.GetString("manufacturer"),
                Form = r.IsDBNull(r.GetOrdinal("form")) ? "" : r.GetString("form"),
                Dosage = r.IsDBNull(r.GetOrdinal("dosage")) ? "" : r.GetString("dosage"),
                OptimalQuantity = r.GetInt32("optimal_quantity"),
                Price = r.GetDecimal("price")
            });
        }
        return list;
    }

    
    public List<Stock> GetStocks(int? medicineId = null, string? warehouse = null)
    {
        var list = new List<Stock>();
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        string sql = @"SELECT s.*, m.name AS medicine_name 
                       FROM stocks s 
                       JOIN medicines m ON m.id = s.medicine_id 
                       WHERE 1=1";
        if (medicineId.HasValue) sql += " AND s.medicine_id=@mid";
        if (!string.IsNullOrEmpty(warehouse)) sql += " AND s.warehouse=@wh";
        sql += " ORDER BY s.warehouse, m.name";

        using var cmd = new MySqlCommand(sql, conn);
        if (medicineId.HasValue) cmd.Parameters.AddWithValue("@mid", medicineId.Value);
        if (!string.IsNullOrEmpty(warehouse)) cmd.Parameters.AddWithValue("@wh", warehouse);

        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Stock
            {
                Id = r.GetInt32("id"),
                Warehouse = r.GetString("warehouse"),
                MedicineId = r.GetInt32("medicine_id"),
                Quantity = r.GetInt32("quantity"),
                ExpiryDate = r.IsDBNull(r.GetOrdinal("expiry_date")) ? DateTime.MinValue : r.GetDateTime("expiry_date"),
                MedicineName = r.GetString("medicine_name")
            });
        }
        return list;
    }

    
    public void Receive(int medicineId, string warehouse, int quantity, DateTime expiry)
    {
        using var conn = new MySqlConnection(_connStr);
        conn.Open();

        
        using (var cmd = new MySqlCommand(
            @"INSERT INTO stocks (warehouse, medicine_id, quantity, expiry_date) 
              VALUES (@wh, @mid, @q, @exp) 
              ON DUPLICATE KEY UPDATE quantity = quantity + @q, expiry_date = @exp", conn))
        {
            cmd.Parameters.AddWithValue("@wh", warehouse);
            cmd.Parameters.AddWithValue("@mid", medicineId);
            cmd.Parameters.AddWithValue("@q", quantity);
            cmd.Parameters.AddWithValue("@exp", expiry);
            cmd.ExecuteNonQuery();
        }

       
        using (var cmd = new MySqlCommand(
            @"INSERT INTO stock_movements (medicine_id, to_warehouse, quantity, reason, movement_type) 
              VALUES (@mid, @wh, @q, 'Поступление от поставщика', 'IN')", conn))
        {
            cmd.Parameters.AddWithValue("@mid", medicineId);
            cmd.Parameters.AddWithValue("@wh", warehouse);
            cmd.Parameters.AddWithValue("@q", quantity);
            cmd.ExecuteNonQuery();
        }
    }

   
    public bool WriteOff(int medicineId, string warehouse, int quantity, string reason)
    {
        using var conn = new MySqlConnection(_connStr);
        conn.Open();

        
        int current;
        using (var cmd = new MySqlCommand(
            "SELECT quantity FROM stocks WHERE warehouse=@wh AND medicine_id=@mid", conn))
        {
            cmd.Parameters.AddWithValue("@wh", warehouse);
            cmd.Parameters.AddWithValue("@mid", medicineId);
            var result = cmd.ExecuteScalar();
            if (result == null) return false;
            current = Convert.ToInt32(result);
        }

        if (current < quantity) return false;

       
        using (var cmd = new MySqlCommand(
            "UPDATE stocks SET quantity = quantity - @q WHERE warehouse=@wh AND medicine_id=@mid", conn))
        {
            cmd.Parameters.AddWithValue("@q", quantity);
            cmd.Parameters.AddWithValue("@wh", warehouse);
            cmd.Parameters.AddWithValue("@mid", medicineId);
            cmd.ExecuteNonQuery();
        }

     
        using (var cmd = new MySqlCommand(
            @"INSERT INTO stock_movements (medicine_id, from_warehouse, quantity, reason, movement_type) 
              VALUES (@mid, @wh, @q, @reason, 'OUT')", conn))
        {
            cmd.Parameters.AddWithValue("@mid", medicineId);
            cmd.Parameters.AddWithValue("@wh", warehouse);
            cmd.Parameters.AddWithValue("@q", quantity);
            cmd.Parameters.AddWithValue("@reason", reason);
            cmd.ExecuteNonQuery();
        }

        return true;
    }

   
    public bool Move(int medicineId, string fromWh, string toWh, int quantity)
    {
        if (fromWh == toWh) return false;

        using var conn = new MySqlConnection(_connStr);
        conn.Open();

      
        int current;
        using (var cmd = new MySqlCommand(
            "SELECT quantity FROM stocks WHERE warehouse=@wh AND medicine_id=@mid", conn))
        {
            cmd.Parameters.AddWithValue("@wh", fromWh);
            cmd.Parameters.AddWithValue("@mid", medicineId);
            var result = cmd.ExecuteScalar();
            if (result == null) return false;
            current = Convert.ToInt32(result);
        }

        if (current < quantity) return false;

       
        using (var cmd = new MySqlCommand(
            "UPDATE stocks SET quantity = quantity - @q WHERE warehouse=@wh AND medicine_id=@mid", conn))
        {
            cmd.Parameters.AddWithValue("@q", quantity);
            cmd.Parameters.AddWithValue("@wh", fromWh);
            cmd.Parameters.AddWithValue("@mid", medicineId);
            cmd.ExecuteNonQuery();
        }

       
        using (var cmd = new MySqlCommand(
            @"INSERT INTO stocks (warehouse, medicine_id, quantity, expiry_date) 
              VALUES (@wh, @mid, @q, '2027-01-01') 
              ON DUPLICATE KEY UPDATE quantity = quantity + @q", conn))
        {
            cmd.Parameters.AddWithValue("@wh", toWh);
            cmd.Parameters.AddWithValue("@mid", medicineId);
            cmd.Parameters.AddWithValue("@q", quantity);
            cmd.ExecuteNonQuery();
        }

        
        using (var cmd = new MySqlCommand(
            @"INSERT INTO stock_movements (medicine_id, from_warehouse, to_warehouse, quantity, reason, movement_type) 
              VALUES (@mid, @from, @to, @q, 'Перемещение', 'MOVE')", conn))
        {
            cmd.Parameters.AddWithValue("@mid", medicineId);
            cmd.Parameters.AddWithValue("@from", fromWh);
            cmd.Parameters.AddWithValue("@to", toWh);
            cmd.Parameters.AddWithValue("@q", quantity);
            cmd.ExecuteNonQuery();
        }

        return true;
    }

    
    public List<Request> GetAllRequests()
    {
        var list = new List<Request>();
        using var conn = new MySqlConnection(_connStr);
        conn.Open();

       
        using (var cmd = new MySqlCommand("SELECT * FROM requests ORDER BY created_at DESC", conn))
        using (var r = cmd.ExecuteReader())
        {
            while (r.Read())
            {
                list.Add(new Request
                {
                    Id = r.GetInt32("id"),
                    Department = r.GetString("department"),
                    CreatedAt = r.GetDateTime("created_at"),
                    Status = r.IsDBNull(r.GetOrdinal("status")) ? "" : r.GetString("status")
                });
            }
        }

       
        foreach (var req in list)
        {
            using var cmd = new MySqlCommand(
                @"SELECT ri.*, m.name AS med_name 
                  FROM request_items ri 
                  JOIN medicines m ON m.id = ri.medicine_id 
                  WHERE ri.request_id=@rid", conn);
            cmd.Parameters.AddWithValue("@rid", req.Id);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                req.Items.Add(new RequestItem
                {
                    Id = r.GetInt32("id"),
                    RequestId = r.GetInt32("request_id"),
                    MedicineId = r.GetInt32("medicine_id"),
                    Quantity = r.GetInt32("quantity"),
                    Issued = r.GetBoolean("issued"),
                    MedicineName = r.GetString("med_name")
                });
            }
        }

        return list;
    }

    public void IssueRequestItem(int itemId)
    {
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        using var cmd = new MySqlCommand("UPDATE request_items SET issued=1 WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("@id", itemId);
        cmd.ExecuteNonQuery();
    }

    
    public List<Stock> GetOrderReport()
    {
        var list = new List<Stock>();
        using var conn = new MySqlConnection(_connStr);
        conn.Open();
        string sql = @"SELECT 
                       m.id AS medicine_id,
                       m.name AS medicine_name,
                       m.optimal_quantity,
                       COALESCE(SUM(s.quantity), 0) AS quantity,
                       'ALL' AS warehouse
                       FROM medicines m
                       LEFT JOIN stocks s ON s.medicine_id = m.id
                       GROUP BY m.id, m.name, m.optimal_quantity
                       ORDER BY (COALESCE(SUM(s.quantity), 0) - m.optimal_quantity) ASC";
        using var cmd = new MySqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Stock
            {
                MedicineId = r.GetInt32("medicine_id"),
                MedicineName = r.GetString("medicine_name"),
                Quantity = r.GetInt32("quantity"),
                Warehouse = r.GetString("warehouse")
            });
        }
        return list;
    }
}
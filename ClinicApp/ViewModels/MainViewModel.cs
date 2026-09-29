using System;
using System.Collections.ObjectModel;
using System.Linq;
using MySql.Data.MySqlClient;
using ClinicApp.Models;
using CommunityToolkit.Mvvm.Input;

namespace ClinicApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public ObservableCollection<Patient> Patients { get; } = new();
    public ScheduleViewModel ScheduleVM { get; } = new();
    public ObservableCollection<Patient> FilteredPatients { get; } = new();

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            ApplyFilter();
        }
    }

    // Поля для нового пациента
    private string _newFirstName = "";
    private string _newLastName = "";
    private string _newPhone = "";
    private string _newCard = "";

    public string NewFirstName { get => _newFirstName; set => SetProperty(ref _newFirstName, value); }
    public string NewLastName { get => _newLastName; set => SetProperty(ref _newLastName, value); }
    public string NewPhone { get => _newPhone; set => SetProperty(ref _newPhone, value); }
    public string NewCard { get => _newCard; set => SetProperty(ref _newCard, value); }

    public Patient? SelectedPatient { get; set; }

    public MainViewModel()
    {
        LoadPatients();
    }

    private void LoadPatients()
    {
        Patients.Clear();
        string connStr = "server=localhost;user=root;password=Hillfessty314!;database=clinic_db";
        
        using var conn = new MySqlConnection(connStr);
        conn.Open();
        
        string sql = "SELECT id, first_name, last_name, phone, medical_card_number FROM patients";
        using var cmd = new MySqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            Patients.Add(new Patient
            {
                Id = reader.GetInt32("id"),
                FirstName = reader.GetString("first_name"),
                LastName = reader.GetString("last_name"),
                Phone = reader.GetString("phone"),
                MedicalCardNumber = reader.GetString("medical_card_number")
            });
        }
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        FilteredPatients.Clear();
        var query = string.IsNullOrWhiteSpace(SearchText) 
            ? Patients 
            : Patients.Where(p => p.LastName.ToLower().Contains(SearchText.ToLower()));
        
        foreach (var p in query)
            FilteredPatients.Add(p);
    }

    [RelayCommand]
    private void AddPatient()
    {
        if (string.IsNullOrWhiteSpace(NewLastName) || string.IsNullOrWhiteSpace(NewFirstName)) return;

        string connStr = "server=localhost;user=root;password=Hillfessty314!;database=clinic_db";
        using var conn = new MySqlConnection(connStr);
        conn.Open();

        string sql = "INSERT INTO patients (first_name, last_name, phone, medical_card_number) VALUES (@f, @l, @p, @c)";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@f", NewFirstName);
        cmd.Parameters.AddWithValue("@l", NewLastName);
        cmd.Parameters.AddWithValue("@p", NewPhone);
        cmd.Parameters.AddWithValue("@c", NewCard);
        cmd.ExecuteNonQuery();

        NewFirstName = ""; NewLastName = ""; NewPhone = ""; NewCard = "";
        LoadPatients();
    }

    [RelayCommand]
    private void DeletePatient()
    {
        if (SelectedPatient == null) return;

        string connStr = "server=localhost;user=root;password=Hillfessty314!;database=clinic_db";
        using var conn = new MySqlConnection(connStr);
        conn.Open();

        string sql = "DELETE FROM patients WHERE id = @id";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", SelectedPatient.Id);
        cmd.ExecuteNonQuery();

        LoadPatients();
  
    }
        [RelayCommand]
    private void Approve() => ScheduleVM.ApproveSelectedDate();

    [RelayCommand]
    private void Refresh() => ScheduleVM.Refresh();
}
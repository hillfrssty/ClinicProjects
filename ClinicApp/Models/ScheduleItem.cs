using System;
using System.Collections.Generic;

namespace ClinicApp.Models;

public class ScheduleItem
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int? PatientId { get; set; }
    public DateTime VisitDate { get; set; }
    public TimeSpan VisitTime { get; set; }
    public string Status { get; set; } = "";
    public bool IsApproved { get; set; }
    public string Notes { get; set; } = "";

    public string DoctorName { get; set; } = "";
    public string PatientName { get; set; } = "";

    public string TimeDisplay => VisitTime.ToString(@"hh\:mm");
    public string DateDisplay => VisitDate.ToString("dd.MM.yyyy");
     public string StatusColor => Status switch
    {
        "Записан" => "#2e7d32",       // зелёный
        "Свободно" => "#1565c0",      // синий
        "Отменён" => "#c62828",       // красный
        _ => "#555555"                // серый по умолчанию
    };
}
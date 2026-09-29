using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClinicApp.Models;
using ClinicApp.Services;
using CommunityToolkit.Mvvm.Input;

namespace ClinicApp.ViewModels;

public partial class ScheduleViewModel : ViewModelBase
{
    private readonly ScheduleService _service = new();
    private readonly PatientService _patientService = new();

    // Список всех записей расписания
    public ObservableCollection<ScheduleItem> AllItems { get; } = new();

    // Отфильтрованные (для отображения)
    public ObservableCollection<ScheduleItem> FilteredItems { get; } = new();

    // Врачи (для фильтра)
    public ObservableCollection<Doctor> Doctors { get; } = new();

    // Пациенты (для записи)
    public ObservableCollection<Patient> Patients { get; } = new();

    private Doctor? _selectedDoctor;
    public Doctor? SelectedDoctor
    {
        get => _selectedDoctor;
        set { SetProperty(ref _selectedDoctor, value); ApplyFilter(); }
    }

    private Patient? _selectedPatient;
    public Patient? SelectedPatient
    {
        get => _selectedPatient;
        set => SetProperty(ref _selectedPatient, value);
    }

    private ScheduleItem? _selectedSlot;
    public ScheduleItem? SelectedSlot
    {
        get => _selectedSlot;
        set => SetProperty(ref _selectedSlot, value);
    }

    private DateTime _selectedDate = DateTime.Today.AddDays(1);
    public DateTime SelectedDate
    {
        get => _selectedDate;
        set { SetProperty(ref _selectedDate, value); ApplyFilter(); }
    }

    private bool _weekView;
    public bool WeekView
    {
        get => _weekView;
        set { SetProperty(ref _weekView, value); ApplyFilter(); }
    }

    private string _filterSpecialization = "";
    public string FilterSpecialization
    {
        get => _filterSpecialization;
        set { SetProperty(ref _filterSpecialization, value); ApplyFilter(); }
    }

    public ObservableCollection<string> Specializations { get; } = new();

    public ScheduleViewModel()
    {
        LoadDoctors();
        LoadPatients();
        LoadSchedule();
    }

    private void LoadDoctors()
    {
        Doctors.Clear();
        var docs = _service.GetDoctors();
        foreach (var d in docs) Doctors.Add(d);

        Specializations.Clear();
        foreach (var s in docs.Select(d => d.Specialization).Distinct().OrderBy(s => s))
            Specializations.Add(s);
    }

    private void LoadPatients()
    {
        Patients.Clear();
        foreach (var p in _patientService.GetAll())
            Patients.Add(p);
    }

    private void LoadSchedule()
    {
        AllItems.Clear();
        foreach (var item in _service.GetAll())
            AllItems.Add(item);
        ApplyFilter();
    }

    /// <summary>Применяет фильтры к расписанию.</summary>
    private void ApplyFilter()
    {
        IEnumerable<ScheduleItem> query = AllItems;

        if (WeekView)
        {
            var weekEnd = SelectedDate.AddDays(7);
            query = query.Where(i => i.VisitDate >= SelectedDate && i.VisitDate < weekEnd);
        }
        else
        {
            query = query.Where(i => i.VisitDate.Date == SelectedDate.Date);
        }

        if (SelectedDoctor != null)
            query = query.Where(i => i.DoctorId == SelectedDoctor.Id);

        if (!string.IsNullOrWhiteSpace(FilterSpecialization))
        {
            var docIds = Doctors.Where(d => d.Specialization == FilterSpecialization)
                                .Select(d => d.Id).ToList();
            query = query.Where(i => docIds.Contains(i.DoctorId));
        }

        FilteredItems.Clear();
        foreach (var item in query.OrderBy(i => i.VisitDate).ThenBy(i => i.VisitTime))
            FilteredItems.Add(item);
    }

    public void Refresh() => LoadSchedule();

    public void ApproveSelectedDate()
    {
        _service.Approve(SelectedDate);
        LoadSchedule();
    }

    public void CancelBooking(int scheduleId)
    {
        _service.CancelBooking(scheduleId);
        LoadSchedule();
    }

    [RelayCommand]
    private void Book()
    {
        if (SelectedSlot == null || SelectedPatient == null) return;
        if (SelectedSlot.Status == "Записан") return;

        _service.BookPatient(SelectedSlot.Id, SelectedPatient.Id);
        SelectedSlot = null;
        LoadSchedule();
    }

    [RelayCommand]
    private void Cancel()
    {
        if (SelectedSlot == null) return;
        _service.CancelBooking(SelectedSlot.Id);
        SelectedSlot = null;
        LoadSchedule();
    }
}
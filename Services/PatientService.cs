using ClinicSchedulingApi.Data;
using ClinicSchedulingApi.DTOs;
using ClinicSchedulingApi.Exceptions;
using ClinicSchedulingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicSchedulingApi.Services;

public class PatientService
{
    private readonly AppDbContext _db;

    public PatientService(AppDbContext db) => _db = db;

    public async Task<List<PatientDto>> GetAllAsync() =>
        await _db.Patients.Select(p => ToDto(p)).ToListAsync();

    public async Task<PatientDto> GetByIdAsync(int id)
    {
        var patient = await _db.Patients.FindAsync(id)
            ?? throw new NotFoundException($"Patient {id} not found.");
        return ToDto(patient);
    }

    public async Task<List<AppointmentDto>> GetAppointmentsAsync(int id)
    {
        if (!await _db.Patients.AnyAsync(p => p.Id == id))
            throw new NotFoundException($"Patient {id} not found.");

        return await _db.Appointments
            .Where(a => a.PatientId == id)
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Select(a => ToAppointmentDto(a))
            .ToListAsync();
    }

    public async Task<List<DoctorDto>> GetDoctorsAsync(int id)
    {
        if (!await _db.Patients.AnyAsync(p => p.Id == id))
            throw new NotFoundException($"Patient {id} not found.");

        return await _db.Appointments
            .Where(a => a.PatientId == id)
            .Include(a => a.Doctor)
            .Select(a => a.Doctor)
            .Distinct()
            .Select(d => ToDoctorDto(d))
            .ToListAsync();
    }

    public async Task<PatientDto> CreateAsync(CreatePatientRequest request)
    {
        var patient = new Patient
        {
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            Email = request.Email,
            Phone = request.Phone
        };
        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();
        return ToDto(patient);
    }

    public async Task<PatientDto> UpdateAsync(int id, CreatePatientRequest request)
    {
        var patient = await _db.Patients.FindAsync(id)
            ?? throw new NotFoundException($"Patient {id} not found.");

        patient.FullName = request.FullName;
        patient.DateOfBirth = request.DateOfBirth;
        patient.Email = request.Email;
        patient.Phone = request.Phone;

        await _db.SaveChangesAsync();
        return ToDto(patient);
    }

    public async Task DeleteAsync(int id)
    {
        var patient = await _db.Patients.FindAsync(id)
            ?? throw new NotFoundException($"Patient {id} not found.");
        _db.Patients.Remove(patient);
        await _db.SaveChangesAsync();
    }

    private static PatientDto ToDto(Patient p) => new()
    {
        Id = p.Id,
        FullName = p.FullName,
        DateOfBirth = p.DateOfBirth,
        Email = p.Email,
        Phone = p.Phone
    };

    private static DoctorDto ToDoctorDto(Doctor d) => new()
    {
        Id = d.Id,
        FullName = d.FullName,
        Specialty = d.Specialty
    };

    private static AppointmentDto ToAppointmentDto(Appointment a) => new()
    {
        Id = a.Id,
        PatientId = a.PatientId,
        PatientName = a.Patient.FullName,
        DoctorId = a.DoctorId,
        DoctorName = a.Doctor.FullName,
        ScheduledAt = a.ScheduledAt,
        Status = a.Status,
        Notes = a.Notes
    };
}

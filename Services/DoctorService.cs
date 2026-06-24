using ClinicSchedulingApi.Data;
using ClinicSchedulingApi.DTOs;
using ClinicSchedulingApi.Exceptions;
using ClinicSchedulingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicSchedulingApi.Services;

public class DoctorService
{
    private readonly AppDbContext _db;

    public DoctorService(AppDbContext db) => _db = db;

    public async Task<List<DoctorDto>> GetAllAsync() =>
        await _db.Doctors.Select(d => ToDto(d)).ToListAsync();

    public async Task<DoctorDto> GetByIdAsync(int id)
    {
        var doctor = await _db.Doctors.FindAsync(id)
            ?? throw new NotFoundException($"Doctor {id} not found.");
        return ToDto(doctor);
    }

    public async Task<List<AppointmentDto>> GetAppointmentsAsync(int id)
    {
        if (!await _db.Doctors.AnyAsync(d => d.Id == id))
            throw new NotFoundException($"Doctor {id} not found.");

        return await _db.Appointments
            .Where(a => a.DoctorId == id)
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Select(a => ToAppointmentDto(a))
            .ToListAsync();
    }

    public async Task<List<PatientDto>> GetPatientsAsync(int id)
    {
        if (!await _db.Doctors.AnyAsync(d => d.Id == id))
            throw new NotFoundException($"Doctor {id} not found.");

        return await _db.Appointments
            .Where(a => a.DoctorId == id)
            .Include(a => a.Patient)
            .Select(a => a.Patient)
            .Distinct()
            .Select(p => ToPatientDto(p))
            .ToListAsync();
    }

    public async Task<DoctorDto> CreateAsync(CreateDoctorRequest request)
    {
        var doctor = new Doctor
        {
            FullName = request.FullName,
            Specialty = request.Specialty
        };
        _db.Doctors.Add(doctor);
        await _db.SaveChangesAsync();
        return ToDto(doctor);
    }

    private static DoctorDto ToDto(Doctor d) => new()
    {
        Id = d.Id,
        FullName = d.FullName,
        Specialty = d.Specialty
    };

    private static PatientDto ToPatientDto(Patient p) => new()
    {
        Id = p.Id,
        FullName = p.FullName,
        DateOfBirth = p.DateOfBirth,
        Email = p.Email,
        Phone = p.Phone
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

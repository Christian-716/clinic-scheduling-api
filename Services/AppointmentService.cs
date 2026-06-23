using ClinicSchedulingApi.Data;
using ClinicSchedulingApi.DTOs;
using ClinicSchedulingApi.Exceptions;
using ClinicSchedulingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicSchedulingApi.Services;

public class AppointmentService
{
    private readonly AppDbContext _db;

    public AppointmentService(AppDbContext db) => _db = db;

    public async Task<List<AppointmentDto>> GetAllAsync() =>
        await _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Select(a => ToDto(a))
            .ToListAsync();

    public async Task<AppointmentDto> GetByIdAsync(int id)
    {
        var appt = await _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new NotFoundException($"Appointment {id} not found.");
        return ToDto(appt);
    }

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentRequest request)
    {
        if (!await _db.Patients.AnyAsync(p => p.Id == request.PatientId))
            throw new NotFoundException($"Patient {request.PatientId} not found.");
        if (!await _db.Doctors.AnyAsync(d => d.Id == request.DoctorId))
            throw new NotFoundException($"Doctor {request.DoctorId} not found.");

        var appt = new Appointment
        {
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            ScheduledAt = request.ScheduledAt,
            Notes = request.Notes
        };
        _db.Appointments.Add(appt);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(appt.Id);
    }

    public async Task<AppointmentDto> UpdateAsync(int id, UpdateAppointmentRequest request)
    {
        var appt = await _db.Appointments.FindAsync(id)
            ?? throw new NotFoundException($"Appointment {id} not found.");

        if (request.ScheduledAt.HasValue) appt.ScheduledAt = request.ScheduledAt.Value;
        if (request.Status is not null) appt.Status = request.Status;
        if (request.Notes is not null) appt.Notes = request.Notes;

        await _db.SaveChangesAsync();
        return await GetByIdAsync(appt.Id);
    }

    public async Task DeleteAsync(int id)
    {
        var appt = await _db.Appointments.FindAsync(id)
            ?? throw new NotFoundException($"Appointment {id} not found.");
        _db.Appointments.Remove(appt);
        await _db.SaveChangesAsync();
    }

    private static AppointmentDto ToDto(Appointment a) => new()
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

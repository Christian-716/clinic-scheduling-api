using ClinicSchedulingApi.Data;
using ClinicSchedulingApi.DTOs;
using ClinicSchedulingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicSchedulingApi.Services;

public class DoctorService
{
    private readonly AppDbContext _db;

    public DoctorService(AppDbContext db) => _db = db;

    public async Task<List<DoctorDto>> GetAllAsync() =>
        await _db.Doctors.Select(d => ToDto(d)).ToListAsync();

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
}

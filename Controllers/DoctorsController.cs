using ClinicSchedulingApi.DTOs;
using ClinicSchedulingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicSchedulingApi.Controllers;

[ApiController]
[Route("api/doctors")]
public class DoctorsController : ControllerBase
{
    private readonly DoctorService _doctors;

    public DoctorsController(DoctorService doctors) => _doctors = doctors;

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<DoctorDto>>> GetAll() =>
        Ok(await _doctors.GetAllAsync());

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorDto>> GetById(int id) =>
        Ok(await _doctors.GetByIdAsync(id));

    [Authorize]
    [HttpGet("{id}/appointments")]
    public async Task<ActionResult<List<AppointmentDto>>> GetAppointments(int id) =>
        Ok(await _doctors.GetAppointmentsAsync(id));

    [Authorize]
    [HttpGet("{id}/patients")]
    public async Task<ActionResult<List<PatientDto>>> GetPatients(int id) =>
        Ok(await _doctors.GetPatientsAsync(id));

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<DoctorDto>> Create(CreateDoctorRequest request) =>
        Ok(await _doctors.CreateAsync(request));
}

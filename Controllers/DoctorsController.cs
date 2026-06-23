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

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<DoctorDto>> Create(CreateDoctorRequest request) =>
        Ok(await _doctors.CreateAsync(request));
}

using ClinicSchedulingApi.DTOs;
using ClinicSchedulingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicSchedulingApi.Controllers;

[ApiController]
[Route("api/appointments")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentService _appointments;

    public AppointmentsController(AppointmentService appointments) => _appointments = appointments;

    [HttpGet]
    public async Task<ActionResult<List<AppointmentDto>>> GetAll() =>
        Ok(await _appointments.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> GetById(int id) =>
        Ok(await _appointments.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> Create(CreateAppointmentRequest request)
    {
        var created = await _appointments.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AppointmentDto>> Update(int id, UpdateAppointmentRequest request) =>
        Ok(await _appointments.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _appointments.DeleteAsync(id);
        return NoContent();
    }
}

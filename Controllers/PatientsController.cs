using ClinicSchedulingApi.DTOs;
using ClinicSchedulingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicSchedulingApi.Controllers;

[ApiController]
[Route("api/patients")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly PatientService _patients;

    public PatientsController(PatientService patients) => _patients = patients;

    [HttpGet]
    public async Task<ActionResult<List<PatientDto>>> GetAll() =>
        Ok(await _patients.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDto>> GetById(int id) =>
        Ok(await _patients.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<PatientDto>> Create(CreatePatientRequest request)
    {
        var created = await _patients.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PatientDto>> Update(int id, CreatePatientRequest request) =>
        Ok(await _patients.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _patients.DeleteAsync(id);
        return NoContent();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mls.Patients.Api.DTOs;
using Mls.Patients.Api.Services;

namespace Mls.Patients.Api.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientController(IPatientService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetPatients()
        {
            var patients = await _service.GetAllPatientsAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient([FromRoute] int id)
        {
            var patient = await _service.GetPatientByIdAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] PatientDto patientDto)
        {
            var createdPatient = await _service.CreatePatientAsync(patientDto);

            return CreatedAtAction(nameof(GetPatient), new { id = createdPatient.Id }, createdPatient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient([FromRoute] int id, [FromBody] PatientDto patientDto)
        {
            var success = await _service.UpdatePatientAsync(id, patientDto);

            if (success)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}

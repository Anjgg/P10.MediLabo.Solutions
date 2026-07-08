using Microsoft.AspNetCore.Mvc;
using Mls.Patient.Api.DTOs;
using Mls.Patient.Api.Services;

namespace Mls.Patient.Api.Controllers
{
    [ApiController]
    [Route("api/patients")]
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
            var id = await _service.CreatePatientAsync(patientDto);

            return CreatedAtAction(nameof(GetPatient), id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient([FromRoute] int id, [FromBody] PatientDto patientDto)
        {
            var success = await _service.UpdatePatientAsync(id, patientDto);

            if (success)
            {
                return NoContent();
            }

            return BadRequest();
        }
    }
}

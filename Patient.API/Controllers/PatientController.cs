using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patient.API.Data;

namespace Patient.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    public class PatientController : ControllerBase
    {
        private readonly PatientDbContext _context;

        public PatientController(PatientDbContext context)
        {
            _context = context;
        }

        // GET api/patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Patient>>> GetPatients()
        {
            return await _context.Patients.ToListAsync();
        }

        // GET api/patients/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Patient>> GetPatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return patient;
        }

        // POST api/patients
        [HttpPost]
        public async Task<ActionResult<Models.Patient>> CreatePatient(Models.Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        // PUT api/patients/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, Models.Patient patient)
        {
            if (id != patient.Id)
            {
                return BadRequest("L'id de l'URL ne correspond pas à l'id du patient.");
            }

            _context.Entry(patient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Patients.AnyAsync(p => p.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }
    }
}

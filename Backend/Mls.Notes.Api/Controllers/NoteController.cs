using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mls.Notes.Api.DTOs;
using Mls.Notes.Api.Services;

namespace Mls.Notes.Api.Controllers
{
    [ApiController]
    [Route("api/notes")]
    [Authorize]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _service;

        public NoteController(INoteService service)
        {
            _service = service;
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetNotesByPatient([FromRoute] int patientId)
        {
            var notes = await _service.GetNotesByPatientIdAsync(patientId);
            return Ok(notes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNote([FromRoute] string id)
        {
            var note = await _service.GetNoteByIdAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            return Ok(note);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] NoteDto noteDto)
        {
            var createdNote = await _service.CreateNoteAsync(noteDto);

            return CreatedAtAction(nameof(GetNote), new { id = createdNote.Id }, createdNote);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mls.Assessment.Api.Services;
using Mls.Assessment.Api.DTOs;

namespace Mls.Assessment.Api.Controllers
{
    [ApiController]
    [Route("api/assessment")]
    [Authorize]
    public class AssessmentController : ControllerBase
    {
        private readonly IAssessmentService _assessmentService;

        public AssessmentController(IAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }

        [HttpGet("{patientId}")]
        public async Task<IActionResult> GetAssessment([FromRoute] int patientId)
        {
            var assessment = await _assessmentService.GetAssessmentAsync(patientId);

            if (assessment is null)
            {
                return NotFound();
            }

            return Ok(new AssessmentResult
            {
                PatientId = patientId,
                Assessment = assessment
            });
        }
    }
}

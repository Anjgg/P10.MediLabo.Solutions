using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IPatientApiClient _apiClient;
        private readonly INotesApiClient _notesApiClient;
        private readonly IAssessmentApiClient _assessmentApiClient;

        public PatientsController(IPatientApiClient apiClient, INotesApiClient notesApiClient, IAssessmentApiClient assessmentApiClient)
        {
            _apiClient = apiClient;
            _notesApiClient = notesApiClient;
            _assessmentApiClient = assessmentApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var patients = await _apiClient.GetPatientsAsync();
            return View(patients);
        }

        public async Task<IActionResult> Details(int id)
        {
            var patient = await _apiClient.GetPatientAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            var notes = await _notesApiClient.GetNotesByPatientAsync(id);

            var assessment = await _assessmentApiClient.GetAssessmentAsync(id);

            var viewModel = new PatientDetailsViewModel
            {
                Patient = patient,
                Notes = notes,
                Assessment = assessment
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNote(int id, string nouvelleNote)
        {
            if (!string.IsNullOrWhiteSpace(nouvelleNote))
            {
                await _notesApiClient.CreateNoteAsync(new NoteViewModel
                {
                    PatientId = id,
                    Contenu = nouvelleNote
                });
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        public IActionResult Create()
        {
            return View(new PatientViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientViewModel patient)
        {
            if (!ModelState.IsValid)
            {
                return View(patient);
            }

            await _apiClient.CreatePatientAsync(patient);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _apiClient.GetPatientAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PatientViewModel patient)
        {
            if (!ModelState.IsValid)
            {
                return View(patient);
            }

            await _apiClient.UpdatePatientAsync(id, patient);

            return RedirectToAction(nameof(Index));
        }
    }
}

using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IPatientApiClient _apiClient;

        public PatientsController(IPatientApiClient apiClient)
        {
            _apiClient = apiClient;
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

            return View(patient);
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

using Microsoft.AspNetCore.Mvc;
using Mls.Patients.Api.Controllers;
using Mls.Patients.Api.DTOs;
using Mls.Patients.Api.Models;
using Mls.Patients.Api.Services;
using Moq;

namespace Mls.Patients.Api.Tests.Controllers
{
    [TestClass]
    public class PatientControllerTests
    {
        private Mock<IPatientService> _serviceMock = null!;
        private PatientController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IPatientService>();
            _controller = new PatientController(_serviceMock.Object);
        }

        [TestMethod]
        public async Task GetPatients_ReturnsOkWithPatientList()
        {
            var patients = new List<PatientDto>
            {
                new() { Id = 1, Nom = "Doe", Prenom = "John", DateNaissance = new DateOnly(1990, 1, 1), Genre = Genre.M }
            };

            _serviceMock.Setup(s => s.GetAllPatientsAsync()).ReturnsAsync(patients);

            var result = await _controller.GetPatients();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(patients, okResult!.Value);
        }

        [TestMethod]
        public async Task GetPatient_ExistingId_ReturnsOkWithPatient()
        {
            var patient = new PatientDto { Id = 1, Nom = "Doe", Prenom = "John", DateNaissance = new DateOnly(1990, 1, 1), Genre = Genre.M };

            _serviceMock.Setup(s => s.GetPatientByIdAsync(1)).ReturnsAsync(patient);

            var result = await _controller.GetPatient(1);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(patient, okResult!.Value);
        }

        [TestMethod]
        public async Task GetPatient_UnknownId_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetPatientByIdAsync(It.IsAny<int>())).ReturnsAsync((PatientDto?)null);

            var result = await _controller.GetPatient(999);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task CreatePatient_ReturnsCreatedAtActionWithPatient()
        {
            var dto = new PatientDto { Nom = "Doe", Prenom = "John", DateNaissance = new DateOnly(1990, 1, 1), Genre = Genre.M };
            var created = new PatientDto { Id = 1, Nom = "Doe", Prenom = "John", DateNaissance = new DateOnly(1990, 1, 1), Genre = Genre.M };

            _serviceMock.Setup(s => s.CreatePatientAsync(dto)).ReturnsAsync(created);

            var result = await _controller.CreatePatient(dto);

            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(nameof(PatientController.GetPatient), createdResult!.ActionName);
            Assert.AreEqual(1, createdResult.RouteValues!["id"]);
            Assert.AreEqual(created, createdResult.Value);
        }

        [TestMethod]
        public async Task UpdatePatient_ExistingPatient_ReturnsNoContent()
        {
            var dto = new PatientDto { Nom = "Doe", Prenom = "John", DateNaissance = new DateOnly(1990, 1, 1), Genre = Genre.M };

            _serviceMock.Setup(s => s.UpdatePatientAsync(1, dto)).ReturnsAsync(true);

            var result = await _controller.UpdatePatient(1, dto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task UpdatePatient_UnknownPatient_ReturnsBadRequest()
        {
            var dto = new PatientDto { Nom = "Doe", Prenom = "John", DateNaissance = new DateOnly(1990, 1, 1), Genre = Genre.M };

            _serviceMock.Setup(s => s.UpdatePatientAsync(999, dto)).ReturnsAsync(false);

            var result = await _controller.UpdatePatient(999, dto);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }
    }
}

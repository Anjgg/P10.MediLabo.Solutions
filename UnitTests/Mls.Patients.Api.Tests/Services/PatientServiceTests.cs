using Microsoft.EntityFrameworkCore;
using Mls.Library.Repositories;
using Mls.Patients.Api.DTOs;
using Mls.Patients.Api.Models;
using Mls.Patients.Api.Services;
using Moq;

namespace Mls.Patients.Api.Tests.Services
{
    [TestClass]
    public class PatientServiceTests
    {
        private Mock<IRepository<Patient, int>> _repositoryMock = null!;
        private PatientService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<Patient, int>>();
            _service = new PatientService(_repositoryMock.Object);
        }

        [TestMethod]
        public async Task GetAllPatientsAsync_ReturnsAllPatientsMappedToDto()
        {
            var patients = new List<Patient>
            {
                new() { Id = 1, Nom = "Doe", Prenom = "John", DateNaissance = new DateOnly(1990, 1, 1), Genre = Genre.M },
                new() { Id = 2, Nom = "Smith", Prenom = "Jane", DateNaissance = new DateOnly(1985, 5, 20), Genre = Genre.F }
            };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(patients);

            var result = (await _service.GetAllPatientsAsync()).ToList();

            Assert.HasCount(2, result);
            Assert.AreEqual("Doe", result[0].Nom);
            Assert.AreEqual("Smith", result[1].Nom);
        }

        [TestMethod]
        public async Task GetPatientByIdAsync_ExistingId_ReturnsPatientDto()
        {
            var patient = new Patient { Id = 1, Nom = "Doe", Prenom = "John", DateNaissance = new DateOnly(1990, 1, 1), Genre = Genre.M };

            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(patient);

            var result = await _service.GetPatientByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Doe", result!.Nom);
        }

        [TestMethod]
        public async Task GetPatientByIdAsync_UnknownId_ReturnsNull()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Patient?)null);

            var result = await _service.GetPatientByIdAsync(999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task CreatePatientAsync_AddsPatientAndReturnsDto()
        {
            var dto = new PatientDto
            {
                Nom = "Doe",
                Prenom = "John",
                DateNaissance = new DateOnly(1990, 1, 1),
                Genre = Genre.M,
                Adresse = "1 rue de Paris",
                Telephone = "0102030405"
            };

            var result = await _service.CreatePatientAsync(dto);

            _repositoryMock.Verify(r => r.AddAsync(It.Is<Patient>(p => p.Nom == "Doe" && p.Prenom == "John")), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.AreEqual("Doe", result.Nom);
            Assert.AreEqual("John", result.Prenom);
        }

        [TestMethod]
        public async Task UpdatePatientAsync_ExistingPatient_ReturnsTrue()
        {
            var dto = new PatientDto
            {
                Nom = "Doe",
                Prenom = "John",
                DateNaissance = new DateOnly(1990, 1, 1),
                Genre = Genre.M
            };

            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.UpdatePatientAsync(1, dto);

            _repositoryMock.Verify(r => r.Update(It.Is<Patient>(p => p.Id == 1 && p.Nom == "Doe")), Times.Once);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task UpdatePatientAsync_UnknownPatient_ReturnsFalse()
        {
            var dto = new PatientDto
            {
                Nom = "Doe",
                Prenom = "John",
                DateNaissance = new DateOnly(1990, 1, 1),
                Genre = Genre.M
            };

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateConcurrencyException());

            var result = await _service.UpdatePatientAsync(999, dto);

            Assert.IsFalse(result);
        }
    }
}

using Mls.Assessment.Api.Calculators;
using Mls.Assessment.Api.Clients;
using Mls.Assessment.Api.DTOs;
using Mls.Assessment.Api.Services;
using Moq;

namespace Mls.Assessment.Api.Tests.Services
{
    [TestClass]
    public class AssessmentServiceTests
    {
        private Mock<IPatientsApiClient> _patientsClientMock = null!;
        private Mock<INotesApiClient> _notesClientMock = null!;
        private Mock<IRiskAssessmentCalculator> _calculatorMock = null!;
        private AssessmentService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _patientsClientMock = new Mock<IPatientsApiClient>();
            _notesClientMock = new Mock<INotesApiClient>();
            _calculatorMock = new Mock<IRiskAssessmentCalculator>();
            _service = new AssessmentService(_patientsClientMock.Object, _notesClientMock.Object, _calculatorMock.Object);
        }

        [TestMethod]
        public async Task GetAssessmentAsync_UnknownPatient_ReturnsNull()
        {
            _patientsClientMock.Setup(c => c.GetPatientAsync(It.IsAny<int>())).ReturnsAsync((PatientInfo?)null);

            var result = await _service.GetAssessmentAsync(999);

            Assert.IsNull(result);
            _notesClientMock.Verify(c => c.GetNotesAsync(It.IsAny<int>()), Times.Never);
            _calculatorMock.Verify(c => c.ComputeRisk(It.IsAny<PatientInfo>(), It.IsAny<IEnumerable<NoteInfo>>()), Times.Never);
        }

        [TestMethod]
        public async Task GetAssessmentAsync_ExistingPatient_ReturnsComputedRisk()
        {
            var patient = new PatientInfo { Id = 1, DateNaissance = new DateOnly(1990, 1, 1), Genre = Genre.M };
            var notes = new List<NoteInfo> { new() { Contenu = "Une note" } };

            _patientsClientMock.Setup(c => c.GetPatientAsync(1)).ReturnsAsync(patient);
            _notesClientMock.Setup(c => c.GetNotesAsync(1)).ReturnsAsync(notes);
            _calculatorMock.Setup(c => c.ComputeRisk(patient, notes)).Returns("Borderline");

            var result = await _service.GetAssessmentAsync(1);

            Assert.AreEqual("Borderline", result);
            _calculatorMock.Verify(c => c.ComputeRisk(patient, notes), Times.Once);
        }

        [TestMethod]
        public async Task GetAssessmentAsync_PatientWithoutNotes_CallsCalculatorWithEmptyList()
        {
            var patient = new PatientInfo { Id = 2, DateNaissance = new DateOnly(2000, 1, 1), Genre = Genre.F };

            _patientsClientMock.Setup(c => c.GetPatientAsync(2)).ReturnsAsync(patient);
            _notesClientMock.Setup(c => c.GetNotesAsync(2)).ReturnsAsync(new List<NoteInfo>());
            _calculatorMock.Setup(c => c.ComputeRisk(patient, It.IsAny<IEnumerable<NoteInfo>>())).Returns("None");

            var result = await _service.GetAssessmentAsync(2);

            Assert.AreEqual("None", result);
        }
    }
}

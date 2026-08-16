using Microsoft.AspNetCore.Mvc;
using Mls.Assessment.Api.Controllers;
using Mls.Assessment.Api.DTOs;
using Mls.Assessment.Api.Services;
using Moq;

namespace Mls.Assessment.Api.Tests.Controllers
{
    [TestClass]
    public class AssessmentControllerTests
    {
        private Mock<IAssessmentService> _serviceMock = null!;
        private AssessmentController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IAssessmentService>();
            _controller = new AssessmentController(_serviceMock.Object);
        }

        [TestMethod]
        public async Task GetAssessment_ExistingPatient_ReturnsOkWithAssessmentResult()
        {
            _serviceMock.Setup(s => s.GetAssessmentAsync(1)).ReturnsAsync("InDanger");

            var result = await _controller.GetAssessment(1);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var value = okResult!.Value as AssessmentResult;
            Assert.IsNotNull(value);
            Assert.AreEqual(1, value!.PatientId);
            Assert.AreEqual("InDanger", value.Assessment);
        }

        [TestMethod]
        public async Task GetAssessment_UnknownPatient_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetAssessmentAsync(It.IsAny<int>())).ReturnsAsync((string?)null);

            var result = await _controller.GetAssessment(999);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}

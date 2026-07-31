using Microsoft.AspNetCore.Mvc;
using Mls.Notes.Api.Controllers;
using Mls.Notes.Api.DTOs;
using Mls.Notes.Api.Services;
using Moq;

namespace Mls.Notes.Api.Tests.Controllers
{
    [TestClass]
    public class NoteControllerTests
    {
        private Mock<INoteService> _serviceMock = null!;
        private NoteController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<INoteService>();
            _controller = new NoteController(_serviceMock.Object);
        }

        [TestMethod]
        public async Task GetNotesByPatient_ReturnsOkWithNoteList()
        {
            var notes = new List<NoteDto>
            {
                new() { Id = "1", PatientId = 1, Contenu = "Note de test", DateCreation = DateTime.UtcNow }
            };

            _serviceMock.Setup(s => s.GetNotesByPatientIdAsync(1)).ReturnsAsync(notes);

            var result = await _controller.GetNotesByPatient(1);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(notes, okResult!.Value);
        }

        [TestMethod]
        public async Task GetNote_ExistingId_ReturnsOkWithNote()
        {
            var note = new NoteDto { Id = "abc123", PatientId = 1, Contenu = "Note de test", DateCreation = DateTime.UtcNow };

            _serviceMock.Setup(s => s.GetNoteByIdAsync("abc123")).ReturnsAsync(note);

            var result = await _controller.GetNote("abc123");

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(note, okResult!.Value);
        }

        [TestMethod]
        public async Task GetNote_UnknownId_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetNoteByIdAsync(It.IsAny<string>())).ReturnsAsync((NoteDto?)null);

            var result = await _controller.GetNote("unknown");

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task CreateNote_ReturnsCreatedAtActionWithNote()
        {
            var dto = new NoteDto { PatientId = 1, Contenu = "Nouvelle note" };
            var created = new NoteDto { Id = "abc123", PatientId = 1, Contenu = "Nouvelle note", DateCreation = DateTime.UtcNow };

            _serviceMock.Setup(s => s.CreateNoteAsync(dto)).ReturnsAsync(created);

            var result = await _controller.CreateNote(dto);

            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(nameof(NoteController.GetNote), createdResult!.ActionName);
            Assert.AreEqual("abc123", createdResult.RouteValues!["id"]);
            Assert.AreEqual(created, createdResult.Value);
        }
    }
}

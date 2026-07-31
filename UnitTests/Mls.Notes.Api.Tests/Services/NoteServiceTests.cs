using Mls.Library.Repositories;
using Mls.Notes.Api.DTOs;
using Mls.Notes.Api.Models;
using Mls.Notes.Api.Services;
using MongoDB.Bson;
using Moq;
using System.Linq.Expressions;

namespace Mls.Notes.Api.Tests.Services
{
    [TestClass]
    public class NoteServiceTests
    {
        private Mock<IRepository<Note, ObjectId>> _repositoryMock = null!;
        private NoteService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<Note, ObjectId>>();
            _service = new NoteService(_repositoryMock.Object);
        }

        [TestMethod]
        public async Task GetNotesByPatientIdAsync_ReturnsNotesMappedToDto()
        {
            var notes = new List<Note>
            {
                new() { Id = ObjectId.GenerateNewId(), PatientId = 1, Contenu = "Première note", DateCreation = DateTime.UtcNow },
                new() { Id = ObjectId.GenerateNewId(), PatientId = 1, Contenu = "Deuxième note", DateCreation = DateTime.UtcNow.AddMinutes(-1) }
            };

            _repositoryMock
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Note, bool>>>()))
                .ReturnsAsync(notes);

            var result = (await _service.GetNotesByPatientIdAsync(1)).ToList();

            Assert.HasCount(2, result);
            Assert.AreEqual("Première note", result[0].Contenu);
            Assert.AreEqual("Deuxième note", result[1].Contenu);
        }

        [TestMethod]
        public async Task GetNotesByPatientIdAsync_NoNotes_ReturnsEmptyList()
        {
            _repositoryMock
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Note, bool>>>()))
                .ReturnsAsync(new List<Note>());

            var result = await _service.GetNotesByPatientIdAsync(42);

            Assert.HasCount(0, result);
        }

        [TestMethod]
        public async Task GetNoteByIdAsync_ExistingId_ReturnsNoteDto()
        {
            var id = ObjectId.GenerateNewId();
            var note = new Note { Id = id, PatientId = 1, Contenu = "Une note", DateCreation = DateTime.UtcNow };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(note);

            var result = await _service.GetNoteByIdAsync(id.ToString());

            Assert.IsNotNull(result);
            Assert.AreEqual("Une note", result!.Contenu);
            Assert.AreEqual(id.ToString(), result.Id);
        }

        [TestMethod]
        public async Task GetNoteByIdAsync_UnknownId_ReturnsNull()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<ObjectId>())).ReturnsAsync((Note?)null);

            var result = await _service.GetNoteByIdAsync(ObjectId.GenerateNewId().ToString());

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetNoteByIdAsync_InvalidIdFormat_ReturnsNull()
        {
            var result = await _service.GetNoteByIdAsync("pas-un-objectid");

            Assert.IsNull(result);
            _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<ObjectId>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateNoteAsync_AddsNoteAndReturnsDto()
        {
            var dto = new NoteDto
            {
                PatientId = 3,
                Contenu = "Le patient déclare qu'il fume depuis peu"
            };

            var result = await _service.CreateNoteAsync(dto);

            _repositoryMock.Verify(r => r.AddAsync(It.Is<Note>(n => n.PatientId == 3 && n.Contenu == dto.Contenu)), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.AreEqual(3, result.PatientId);
            Assert.AreEqual(dto.Contenu, result.Contenu);
        }

        [TestMethod]
        public async Task CreateNoteAsync_PreservesMultilineContent()
        {
            var dto = new NoteDto
            {
                PatientId = 4,
                Contenu = "Ligne 1\nLigne 2\nLigne 3"
            };

            var result = await _service.CreateNoteAsync(dto);

            Assert.AreEqual("Ligne 1\nLigne 2\nLigne 3", result.Contenu);
        }
    }
}

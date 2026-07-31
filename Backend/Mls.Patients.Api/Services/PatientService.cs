using Microsoft.EntityFrameworkCore;
using Mls.Library.Repositories;
using Mls.Patients.Api.DTOs;
using Mls.Patients.Api.Models;

namespace Mls.Patients.Api.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
        Task<PatientDto?> GetPatientByIdAsync(int id);
        Task<PatientDto> CreatePatientAsync(PatientDto dto);
        Task<bool> UpdatePatientAsync(int id, PatientDto dto);
    }

    public class PatientService : IPatientService
    {
        private readonly IRepository<Patient> _repository;

        public PatientService(IRepository<Patient> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            var patients = await _repository.GetAllAsync();
            return patients.Select(ToDto);
        }

        public async Task<PatientDto?> GetPatientByIdAsync(int id)
        {
            var patient = await _repository.GetByIdAsync(id);

            return patient == null ? null : ToDto(patient);
        }

        public async Task<PatientDto> CreatePatientAsync(PatientDto dto)
        {
            var patient = new Patient
            {
                Nom = dto.Nom,
                Prenom = dto.Prenom,
                DateNaissance = dto.DateNaissance,
                Genre = dto.Genre,
                Adresse = dto.Adresse,
                Telephone = dto.Telephone
            };

            await _repository.AddAsync(patient);
            await _repository.SaveChangesAsync();

            return ToDto(patient);
        }

        public async Task<bool> UpdatePatientAsync(int id, PatientDto dto)
        {
            var patient = new Patient
            {
                Id = id,
                Nom = dto.Nom,
                Prenom = dto.Prenom,
                DateNaissance = dto.DateNaissance,
                Genre = dto.Genre,
                Adresse = dto.Adresse,
                Telephone = dto.Telephone
            };

            _repository.Update(patient);

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }

        private static PatientDto ToDto(Patient patient)
        {
            return new PatientDto
            {
                Id = patient.Id,
                Nom = patient.Nom,
                Prenom = patient.Prenom,
                DateNaissance = patient.DateNaissance,
                Genre = patient.Genre,
                Adresse = patient.Adresse,
                Telephone = patient.Telephone
            };
        }
    }
}

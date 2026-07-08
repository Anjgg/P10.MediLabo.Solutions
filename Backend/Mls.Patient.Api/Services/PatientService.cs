using Mls.Patient.API.DTOs;
using Mls.Library.Repositories;

namespace Mls.Patient.API.Services
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
        private readonly IRepository<Models.Patient> _repository;

        public PatientService(IRepository<Models.Patient> repository)
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

            if (patient == null)
            {
                throw new 
            }

            return patient is null ? null : ToDto(patient);
        }

        public async Task<PatientDto> CreatePatientAsync(PatientDto dto)
        {
            var patient = new Models.Patient
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
            if (!await _repository.ExistsAsync(id))
            {
                return false;
            }

            var patient = new Models.Patient
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
            await _repository.SaveChangesAsync();

            return true;
        }

        private static PatientDto ToDto(Models.Patient patient)
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

﻿using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// clase de negocio encargada de la logica relacionada con los Persona del sistema
    /// </summary>
    public class PersonBusiness
    {
        private readonly PersonData _PersonData;
        private readonly ILogger<PersonBusiness> _logger;

        public PersonBusiness(PersonData PersonData, ILogger<PersonBusiness> logger)
        {
            _PersonData = PersonData;
            _logger = logger;
        }

        // Atributo para obtener todos los Person como DTOs

        public async Task<IEnumerable<PersonDto>> GetAllPersonAsync()
        {
            try
            {
                var Persons = await _PersonData.GetAllPersonAsyncSQL();
                var PersonDTO = MapToDTOList(Persons);

                return PersonDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los Personas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de Personas", ex);
            }
        }

        // Atributo para obtener un Person por ID como DTO
        public async Task<PersonDto> GetByPersonIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un Persona con ID inválido: {PersonId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del Person debe ser mayor que cero");
            }

            try
            {
                var person = await _PersonData.GetByIdAsyncSQL(id);
                if (person == null)
                {
                    _logger.LogInformation("No se encontró ningún Persona con ID: {PersonId}", id);
                    throw new EntityNotFoundException("Person", id);
                }

                return MapToDTO(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Persona con ID: {PersonaId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el Persona con ID {id}", ex);
            }
        }

        // Atributo para crear un Persona desde un DTO
        public async Task<PersonDto> CreatePersonAsync(PersonDto PersonDto)
        {
            try
            {
                ValidatePerson(PersonDto);

                var Person = MapToEntity(PersonDto);

                var PersonCreado = await _PersonData.CreatePersonAsyncSQL(Person);
                _logger.LogInformation("Persona insertado con ID: {PersonId}", PersonCreado.id);

                return MapToDTO(PersonCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo Person: {PersonNombre}", PersonDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el Person", ex);
            }
        }

        public async Task<PersonDto> UpdatePersonAsync(PersonDto personDto)
        {
            try
            {
                ValidatePerson(personDto);
                var existingPerson = await _PersonData.GetByIdAsyncSQL(personDto.Id);
                if (existingPerson == null)
                {
                    throw new EntityNotFoundException("Person", "No se encontró la relación Person");
                }

                // Convertir la fecha a UTC antes de actualizar
                existingPerson.name = personDto.Name;
                existingPerson.lastname = personDto.LastName;
                existingPerson.phone = personDto.Phone;

                var success = await _PersonData.UpdateAsyncSQL(existingPerson);

                if (!success)
                {
                    throw new Exception("No se pudo actualizar la relación Person.");
                }

                return MapToDTO(existingPerson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la relación person");
                throw new ExternalServiceException("Base de datos", "Error al actualizar la relación person", ex);
            }
        }

        // Método para eliminar una relación person de manera permanente
        public async Task<bool> DeletePersonAsync(int id)
        {
            try
            {
                return await _PersonData.DeletePermissionAsyncSQL(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente la relación person");
                throw new ExternalServiceException("Base de datos", "Error al eliminar la relación person", ex);
            }
        }

        public async Task<bool> DeleteLogicoPersonAsync(int id)
        {
            try
            {
                return await _PersonData.DeleteLogicoPersonAsyncSQL(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el person");
                throw new ExternalServiceException("Base de datos", "Error al eliminar la relación person", ex);
            }
        }

        // Atributo para validar el DTO
        private void ValidatePerson(PersonDto PersonDto)
        {
            if (PersonDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Person no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(PersonDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un Person con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del Person es obligatorio");
            }
        }

        // Atributo para mapear de Person a PersonDTO 
        private PersonDto MapToDTO(Person Person)
        {
            return new PersonDto
            {
                Id = Person.id,
                Name = Person.name,
                LastName = Person.lastname,
                Phone = Person.phone
            };
        }

        // Atributo para mapear de PersonDTO a Person
        private Person MapToEntity(PersonDto PersonDto)
        {
            return new Person
            {
                id = PersonDto.Id,
                name = PersonDto.Name,
                lastname = PersonDto.LastName,
                phone = PersonDto.Phone,
                isdelete = false
            };
        }

        // Atributo para mapear una lista de Person a una Person de PersonDTO
        private IEnumerable<PersonDto> MapToDTOList(IEnumerable<Person> Personas)
        {
            var PersonDto = new List<PersonDto>();
            foreach (var Person in Personas)
            {
                PersonDto.Add(MapToDTO(Person));
            }
            return PersonDto;
        }
    }
}

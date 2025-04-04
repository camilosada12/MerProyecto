using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los roles del sistema.
    /// </summary>
    public class RolBusiness
    {
        private readonly RolData _rolData;
        private readonly ILogger<RolBusiness> _logger;

        public RolBusiness(RolData rolData, ILogger<RolBusiness> logger)
        {
            _rolData = rolData;
            _logger = logger;
        }

        // Atributo para obtener todos los roles como DTOs
        public async Task<IEnumerable<RolDto>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _rolData.GetAllAsyncSQL();
                var rolesDTO = MapToDTOList(roles);
                return rolesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles", ex);
            }
        }

        //Atributo para obtener un Rol por Id como DTO
        public async Task<RolDto> GetRolByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("se intento obtener un Rol con Id invalido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "el Id del Rol debe ser meyor a cero");
            }

            try
            {
                var Rol = await _rolData.GetByRolIdAsyncSQL(id);
                if (Rol == null)
                {
                    _logger.LogInformation("No se encontro ningun Rol con Id: {RolId}", id);
                    throw new EntityNotFoundException("Rol", id);
                }

                return MapToDTO(Rol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Rol con ID: {RolId}", id);
                throw new ExternalServiceException("base de datos", $"Error al recuperar el Rol con Id {id}", ex);
            }
        }

        // Corrección en CreateRolAsync
        public async Task<RolDto> CreateRolAsync(RolDto RolDto)
        {
            try
            {
                ValidateRol(RolDto);

                var Rol = MapToEntity(RolDto);

                // Corrección: Asegurar que se retorna el ID correcto
                var RolCreado = await _rolData.CreateAsyncSQL(Rol);
                _logger.LogInformation("Usuario insertado con ID: {RolId}", RolCreado.id);

                return MapToDTO(RolCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo Rol : {RolNombre}", RolDto?.Role ?? "null");
                throw new ExternalServiceException("base de datos ", "Error al crear el Rol", ex);
            }
        }

        public async Task<RolDto> UpdateRolAsync(int id, RolDto RolDto)
        {
            try
            {
                ValidateRol(RolDto);

                var existingRol = await _rolData.GetByIdAsync(id);
                if (existingRol == null)
                {
                    throw new EntityNotFoundException("Rol", $"No se encontró el usuario con ID {id}");
                }

                // Convertir la fecha a UTC antes de actualizar
                existingRol.role = RolDto.Role;
                existingRol.description = RolDto.Description;

                var success = await _rolData.UpdateAsyncLinq(existingRol);

                if (!success)
                {
                    throw new Exception("No se pudo actualizar el usuario.");
                }

                return MapToDTO(existingRol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el usuario con ID {id}");
                throw new ExternalServiceException("Base de datos", "Error al actualizar el usuario", ex);
            }
        }

        // Método para eliminar una relación Rol de manera permanente
        public async Task<bool> DeleteRolAsync(int id)
        {
            try
            {
                return await _rolData.DeleteRolAsyncSQL(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente la relación Rol");
                throw new ExternalServiceException("Base de datos", "Error al eliminar la relación Rol", ex);
            }
        }

        // Atributo para validar el DTO
        private void ValidateRol(RolDto RolDto)
        {
            if (RolDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto rol no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(RolDto.Role))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del rol es obligatorio");
            }
        }

        // Atributo para mapear de Rol a RolDTO 
        private RolDto MapToDTO(Rol rol)
        {
            return new RolDto
            {
                Id = rol.id,
                Role = rol.role,
                Description = rol.description // Si existe en la entidad
            };
        }

        // Atributo para mapear de RolDTO a Rol
        private Rol MapToEntity(RolDto RolDto)
        {
            return new Rol
            {
                id = RolDto.Id,
                role = RolDto.Role,
                description = RolDto.Description // Si existe en la entidad
            };
        }

        // Atributo para mapear una lista de Rol a una lista de RolDTO
        private IEnumerable<RolDto> MapToDTOList(IEnumerable<Rol> roles)
        {
            var rolesDTO = new List<RolDto>();
            foreach (var rol in roles)
            {
                rolesDTO.Add(MapToDTO(rol));
            }
            return rolesDTO;
        }

    }
}

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
        private readonly ILogger _logger;

        public RolBusiness(RolData rolData, ILogger logger)
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

        // Atributo para obtener un rol por ID como DTO
        public async Task<RolDto> GetRolByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol con ID inválido: {RolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var rol = await _rolData.GetByIdAsync(id);
                if (rol == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {RolId}", id);
                    throw new EntityNotFoundException("Rol", id);
                }

                return MapToDTO(rol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

        // Atributo para crear un rol desde un DTO
        public async Task<RolDto> CreateRolAsync(RolDto RolDto)
        {
            try
            {
                ValidateRol(RolDto);

                var rol = MapToEntity(RolDto);

                var rolCreado = await _rolData.CreateAsync(rol);

                return MapToDTO(rolCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol: {RolNombre}", RolDto?.Role ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }

        public async Task<RolDto> UpdateRolAsync(RolDto rolDto)
        {
            try
            {
                ValidateRol(rolDto);
                var existingRol = await _rolData.GetByIdAsync(rolDto.Id);
                if (existingRol == null)
                {
                    throw new EntityNotFoundException("Rol", "No se encontró la relación Rol");
                }

                existingRol.IsDeleted = rolDto.IsDeleted;
                var success = await _rolData.UpdateAsync(existingRol);

                if (!success)
                {
                    throw new Exception("No se pudo actualizar la relación Rol.");
                }

                return MapToDTO(existingRol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la relación Rol");
                throw new ExternalServiceException("Base de datos", "Error al actualizar la relación Rol", ex);
            }
        }

        // Método para eliminar una relación Rol de manera permanente
        public async Task<bool> DeleteRolPermanentAsync(int id)
        {
            try
            {
                return await _rolData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente la relación Rol");
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
                Id = rol.Id,
                Role = rol.Role,
                Description = rol.Description // Si existe en la entidad
            };
        }

        // Atributo para mapear de RolDTO a Rol
        private Rol MapToEntity(RolDto RolDto)
        {
            return new Rol
            {
                Id = RolDto.Id,
                Role = RolDto.Role,
                Description = RolDto.Description // Si existe en la entidad
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

using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class RolFormPermissionBusiness
    {
        private readonly RolFormPermissionData _RolFormPermissionData;
        private readonly ILogger _logger;

        public RolFormPermissionBusiness(RolFormPermissionData RolFormPermissionData, ILogger logger)
        {
            _RolFormPermissionData = RolFormPermissionData;
            _logger = logger;
        }

        // Atributo para obtener todos los RolFormPermission como DTOs
        public async Task<IEnumerable<RolFormPermissionDto>> GetAllRolFormPermissionAsync()
        {
            try
            {
                var rolFormPermissions = await _RolFormPermissionData.GetAllAsync();
                var rolFormPermissionsDto = MapToDTOList(rolFormPermissions);

                return rolFormPermissionsDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los RolFormPermission");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de RolFormPermission", ex);
            }
        }

        // Atributo para obtener un RolFormPermission por ID como DTO
        public async Task<RolFormPermissionDto> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un RolFormPermission con ID inválido: {RolFormPermissionId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del RolFormPermission debe ser mayor que cero");
            }

            try
            {
                var rolFormPermissionsDto = await _RolFormPermissionData.GetByIdAsync(id);
                if (rolFormPermissionsDto == null)
                {
                    _logger.LogInformation("No se encontró ningún RolFormPermission con ID: {RolFormPermissionId}", id);
                    throw new EntityNotFoundException("Permiso", "No se encontró el permiso");
                }

                return MapToDTO(rolFormPermissionsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el RolFormPermission con ID: {RolFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el RolFormPermission con ID {id}", ex);
            }
        }

        // Atributo para crear un RolFormPermission desde un DTO
        public async Task<RolFormPermissionDto> CreateRolFormPermissionAsync(RolFormPermissionDto RolFormPermissionDto)
        {
            try
            {
                ValidateRolFormPermission(RolFormPermissionDto);

                var RolFormPermission = MapToEntity(RolFormPermissionDto);

                var RolFormPermissionCreado = await _RolFormPermissionData.CreateAsync(RolFormPermission);

                return MapToDTO(RolFormPermissionCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo RolFormPermissionDto: {RolFormPermissionId}", RolFormPermissionDto?.Id ?? null);

                throw new ExternalServiceException("Base de datos", "Error al crear el RolFormPermission", ex);
            }
        }


        public async Task<RolFormPermissionDto> UpdateRolFormPermissionAsync(RolFormPermissionDto RolFormPermissionDTO)
        {
            try
            {
                ValidateRolFormPermission(RolFormPermissionDTO);
                var existingRolFormPermission = await _RolFormPermissionData.GetByIdAsync(RolFormPermissionDTO.Id);
                if (existingRolFormPermission == null)
                {
                    throw new EntityNotFoundException("RolFormPermissionDto", "No se encontró la relación RolFormPermissionDto");
                }

                existingRolFormPermission.IsDeleted = RolFormPermissionDTO.IsDeleted;
                var success = await _RolFormPermissionData.UpdateAsync(existingRolFormPermission);

                if (!success)
                {
                    throw new Exception("No se pudo actualizar la relación RolFormPermissionDto.");
                }

                return MapToDTO(existingRolFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la relación RolFormPermissionDto");
                throw new ExternalServiceException("Base de datos", "Error al actualizar la relación RolFormPermissionDto", ex);
            }
        }

        // Método para eliminar una relación  RolFormPermission de manera permanente
        public async Task<bool> DeleteRolFormPermissionPermanentAsync(int id)
        {
            try
            {
                return await _RolFormPermissionData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente la relación  RolFormPermission");
                throw new ExternalServiceException("Base de datos", "Error al eliminar la relación  RolFormPermission", ex);
            }
        }

        // Atributo para RolFormPermission el DTO
        private void ValidateRolFormPermission(RolFormPermissionDto RolFormPermissionDto)
        {
            if (RolFormPermissionDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto RolFormPermission no puede ser nulo");
            }
        }

        // Atributo para mapear de RolFormPermission a RolFormPermissionDTO 
        private RolFormPermissionDto MapToDTO(RolFormPermission RolFormPermission)
        {
            return new RolFormPermissionDto
            {
                Id = RolFormPermission.Id,
                RolId = RolFormPermission.RolId,
                FormId = RolFormPermission.FormId, // Si existe en la
                PermissionId = RolFormPermission.PermissionId // Si existe en la entidad
            };
        }

        // Atributo para mapear de RolFormPermissionDTO a RolFormPermission
        private RolFormPermission MapToEntity(RolFormPermissionDto RolFormPermissionDto)
        {
            return new RolFormPermission
            {
                Id = RolFormPermissionDto.Id,
                RolId = RolFormPermissionDto.RolId,
                FormId = RolFormPermissionDto.FormId,// Si existe en la
                PermissionId = RolFormPermissionDto.PermissionId // Si existe en la entidad
            };
        }

        // Atributo para mapear una lista de RolFormPermission a una RolFormPermission de RolFormPermissionDTO
        private IEnumerable<RolFormPermissionDto> MapToDTOList(IEnumerable<RolFormPermission> RolFormPermissions)
        {
            var RolFormPermissionDto = new List<RolFormPermissionDto>();
            foreach (var RolFormPermission in RolFormPermissions)
            {
                RolFormPermissionDto.Add(MapToDTO(RolFormPermission));
            }
            return RolFormPermissionDto;
        }
    }
}

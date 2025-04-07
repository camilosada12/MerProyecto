using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la logica relacionada con los permission del sistema
    /// </summary>
    public class PermissionBusinness
    {
        private readonly PermissionData _PermissionData;
        private readonly ILogger<PermissionBusinness> _logger;

        public PermissionBusinness(PermissionData permissionData, ILogger<PermissionBusinness> logger)
        {
            _PermissionData = permissionData;
            _logger = logger;
        }

        //Atributo para obtener todos los Permission como DTOs
        public async Task<IEnumerable<PermissionDto>> GetAllPermissionAsync()
        {
            try
            {
                var permission = await _PermissionData.GetAllAsyncSQL();
                var permissionDTO = MapToDTOList(permission);

                return permissionDTO;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los Permission ");
                throw new ExternalServiceException("base de datos", "Error al recuperar la lista de Permission", ex);
            }
        }

        //Atributo para obtener un Permiso por Id como DTO
        public async Task<PermissionDto> GetPermissionByIdAsync(int id)
        {
            if(id <= 0)
            {
                _logger.LogWarning("se intento obtener un Permiso con un Id  invalido: {PermissionId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "el Id del permiso debe ser mayor de cero");
            }

            try
            {
                var Permission = await _PermissionData.GetByPermissionIdAsyncSQL(id);
                if(Permission == null)
                {
                    _logger.LogInformation("No se encontro ningun permiso con Id : {PermissionId}", id);
                    throw new EntityNotFoundException("Permission", id);
                }
                return MapToDTO(Permission);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Permission con id: {PermissionId}", id);
                throw new ExternalServiceException("base de datos", $"Error al recuperar el Permission con Id : {id}", ex);
            }
        }

        //Atributo para crear un Permiso desde el DTO
        public async Task<PermissionDto> CreatePermissionAsync(PermissionDto PermissionDto)
        {
            try
            {
                validatePermission(PermissionDto);

                var permission = MapToEntity(PermissionDto);

                var PermissionCreado = await _PermissionData.CreatePermissionAsyncSQL(permission);
                _logger.LogInformation("Permission insertado con ID: {UserId}", PermissionCreado.id);

                return MapToDTO(PermissionCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo Permission : {PermissionNombre}", PermissionDto?.Name ?? "null");
                throw new ExternalServiceException("base de datos", "Error el crear el permiso", ex);
            }
        }

        public async Task<PermissionDto> UpdatePermissionAsync(PermissionDto permissionDto)
        {
            try
            {
                validatePermission(permissionDto);

                var existingPermission = await _PermissionData.GetByPermissionIdAsyncSQL(permissionDto.Id);

                if (existingPermission == null)
                {
                    throw new EntityNotFoundException("PermissionDto", "No se encontró la relación PermissionDto");
                }

                existingPermission.name = permissionDto.Name;
                existingPermission.description = permissionDto.Description;

                var success = await _PermissionData.UpdateAsyncSQL(existingPermission);

                if (!success)
                {
                    throw new Exception("No se pudo actualizar la relación PermissionDto.");
                }

                return MapToDTO(existingPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la relación PermissionDto");
                throw new ExternalServiceException("Base de datos", "Error al actualizar la relación PermissionDto", ex);
            }
        }

        // Método para eliminar una relación Rol de manera lógica
        public async Task<bool> DeletePermissionAsync(int id)
        {
            try
            {
                return await _PermissionData.DeletePermissionAsyncSQL(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente la relación Permission");
                throw new ExternalServiceException("Base de datos", "Error al eliminar la relación Permission", ex);
            }
        }

        public async Task<bool> DeleteLogicoPermissionAsync(int id)
        {
            try
            {
                return await _PermissionData.DeleteLogicoPermissionAsyncSQL(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el Form");
                throw new ExternalServiceException("Base de datos", "Error al eliminar la relación Form", ex);
            }
        }

        //Atributo para validar el DTO
        private void validatePermission(PermissionDto PermissionDto)
        {
            if(PermissionDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El Objeto Permiso no puede ser nulo");
            }
            if (String.IsNullOrEmpty(PermissionDto.Name))
            {
                _logger.LogWarning("se intento crear/actualizar un permission con el Name vacio");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de permission es obligario");
            }
        }

        // Atributo para mapear de Person a PersonDTO 
        private PermissionDto MapToDTO(Permission Permission)
        {
            return new PermissionDto
            {
                Id = Permission.id,
                Name = Permission.name,
                Description = Permission.description
            };
        }

        // Atributo para mapear de PersonDTO a Person
        private Permission MapToEntity(PermissionDto PermissionDto)
        {
            return new Permission
            {
                id = PermissionDto.Id,
                name = PermissionDto.Name,
                description = PermissionDto.Description,
                isdelete = false
            };
        }

        // Atributo para mapear una lista de Person a una Person de PersonDTO
        private IEnumerable<PermissionDto> MapToDTOList(IEnumerable<Permission> Permissions)
        {
            var PersonDto = new List<PermissionDto>();
            foreach (var permission in Permissions)
            {
                PersonDto.Add(MapToDTO(permission));
            }
            return PersonDto;
        }
    }
}

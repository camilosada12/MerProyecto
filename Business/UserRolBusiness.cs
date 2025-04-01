using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class UserRolBusiness
    {
        private readonly RolUserData _RolUserData;
        private readonly ILogger _logger;

        public UserRolBusiness(RolUserData rolUserData, ILogger logger)
        {
            _RolUserData = rolUserData;
            _logger = logger;
        }

        // Atributo para obtener todos los RolUser como DTOs
        public async Task<IEnumerable<RolUserDto>> GetAllUserRolAsync()
        {
            try
            {
                var Roles = await _RolUserData.GetAllAsync();
                var RolDto = MapToDTOList(Roles);

                return RolDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los RolUser");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de RolUser", ex);
            }
        }

        // Atributo para obtener un RolUser por ID como DTO
        public async Task<RolUserDto> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un RolUser con ID inválido: {RolUserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del RolUser debe ser mayor que cero");
            }

            try
            {
                var Roles = await _RolUserData.GetByIdAsync(id);
                if (Roles == null)
                {
                    _logger.LogInformation("No se encontró ningún RolUser con ID: {RolUserId}", id);
                    throw new EntityNotFoundException("Permiso", "No se encontró el permiso");
                }

                return MapToDTO(Roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el RolUser con ID: {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el RolUser con ID {id}", ex);
            }
        }

        // Atributo para crear un RolUser desde un DTO
        public async Task<RolUserDto> CreateRolUserAsync(RolUserDto RolUserDto)
        {
            try
            {
                ValidateRolUser(RolUserDto);

                var RolUser = MapToEntity(RolUserDto);

                var RolUserCreado = await _RolUserData.CreateAsync(RolUser);

                return MapToDTO(RolUserCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo RolUserDto: {RolUserId}", RolUserDto?.Id ?? null);

                throw new ExternalServiceException("Base de datos", "Error al crear el RolUser", ex);
            }
        }


        public async Task<RolUserDto> UpdateRolUserAsync(RolUserDto RolUserDTO)
        {
            try
            {
                ValidateRolUser(RolUserDTO);
                var existingRolUser = await _RolUserData.GetByIdAsync(RolUserDTO.Id);
                if (existingRolUser == null)
                {
                    throw new EntityNotFoundException("RolUserDto", "No se encontró la relación RolUserDto");
                }

                var success = await _RolUserData.UpdateAsync(existingRolUser);

                if (!success)
                {
                    throw new Exception("No se pudo actualizar la relación RolUserDto.");
                }

                return MapToDTO(existingRolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la relación RolUserDto");
                throw new ExternalServiceException("Base de datos", "Error al actualizar la relación RolUserDto", ex);
            }
        }

        // Método para eliminar una relación  RolUser de manera permanente
        public async Task<bool> DeleteRolUserPermanentAsync(int id)
        {
            try
            {
                return await _RolUserData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente la relación  RolUser");
                throw new ExternalServiceException("Base de datos", "Error al eliminar la relación  RolUser", ex);
            }
        }

        // Atributo para RolUser el DTO
        private void ValidateRolUser(RolUserDto RolUserDto)
        {
            if (RolUserDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto RolUser no puede ser nulo");
            }
        }

        // Atributo para mapear de RolUser a RolUserDTO 
        private RolUserDto MapToDTO(RolUser RolUser)
        {
            return new RolUserDto
            {
                Id = RolUser.Id,
                RolId = RolUser.RolId,
                UserId = RolUser.UserId // Si existe en la entidad
            };
        }

        // Atributo para mapear de RolUserDTO a RolUser
        private RolUser MapToEntity(RolUserDto RolUserDto)
        {
            return new RolUser
            {
                Id = RolUserDto.Id,
                RolId = RolUserDto.RolId,
                UserId = RolUserDto.UserId // Si existe en la entidad
            };
        }

        // Atributo para mapear una lista de RolUser a una RolUser de RolUserDTO
        private IEnumerable<RolUserDto> MapToDTOList(IEnumerable<RolUser> RolUsers)
        {
            var RolUserDto = new List<RolUserDto>();
            foreach (var RolUser in RolUsers)
            {
                RolUserDto.Add(MapToDTO(RolUser));
            }
            return RolUserDto;
        }
    }
}

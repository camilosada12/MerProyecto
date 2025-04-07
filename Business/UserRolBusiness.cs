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
        private readonly ILogger<UserRolBusiness> _logger;

        public UserRolBusiness(RolUserData rolUserData, ILogger<UserRolBusiness> logger)
        {
            _RolUserData = rolUserData;
            _logger = logger;
        }

        // Atributo para obtener todos los RolUser como DTOs
        public async Task<IEnumerable<RolUserDto>> GetAllUserRolAsync()
        {
            try
            {
                var Roles = await _RolUserData.GetAllRolUserAsyncLinq();
                var RolDto = MapToDTOList(Roles);

                return RolDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los RolUser");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de RolUser", ex);
            }
        }

        // Método para obtener un rol users por ID como DTO
        public async Task<RolUserDto> GetRolUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol de usuario con ID inválido: {RolUserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol de usuario debe ser mayor que cero");
            }
            try
            {
                var roluser = await _RolUserData.GetByIdAsyncLinq(id);
                if (roluser == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario con ID: {RolUserId}", id);
                    throw new EntityNotFoundException("Usuario", id);
                }

                return MapToDTO(roluser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID: {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el usuario con ID {id}", ex);
            }
        }

        // Atributo para crear un RolUser desde un DTO
        public async Task<RolUserDto> CreateRolUserAsync(RolUserDto RolUserDto)
        {
            try
            {
                ValidateRolUser(RolUserDto);

                var RolUser = MapToEntity(RolUserDto);

                var RolUserCreado = await _RolUserData.CreateAsyncSql(RolUser);

                return MapToDTO(RolUserCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo RolUserDto: {RolUserId}", RolUserDto?.Id ?? null);

                throw new ExternalServiceException("Base de datos", "Error al crear el RolUser", ex);
            }
        }


        public async Task<RolUserDto> UpdateRolUserAsync(RolUserDto rolUserDto)
        {
            try
            {
                ValidateRolUser(rolUserDto);

                var existingRolUser = await _RolUserData.GetByIdAsyncLinq(rolUserDto.Id);
                if (existingRolUser == null)
                {
                    throw new EntityNotFoundException("RolUser", $"No se encontró el RolUser con ID ");
                }

                // Convertir la fecha a UTC antes de actualizar
                existingRolUser.rolid = rolUserDto.RolId;
                existingRolUser.userid = rolUserDto.UserId;

                var success = await _RolUserData.UpdateAsyncLinq(existingRolUser);

                if (!success)
                {
                    throw new Exception("No se pudo actualizar el RolUser.");
                }

                return MapToDTO(existingRolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el RolUser con ID ");
                throw new ExternalServiceException("Base de datos", "Error al actualizar el RolUser", ex);
            }
        }

        // Método para eliminar una relación  RolUser de manera permanente
        public async Task<bool> DeleteRolUserAsync(int id)
        {
            try
            {
                var existingRoLUser = await _RolUserData.GetByIdAsyncLinq(id);
                if (existingRoLUser == null)
                {
                    throw new EntityNotFoundException("Rol User", id);
                }

                return await _RolUserData.DeleteAsyncLinq(id);
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
                Id = RolUser.id,
                RolId = RolUser.rolid,
                UserId = RolUser.userid // Si existe en la entidad
            };
        }

        // Atributo para mapear de RolUserDTO a RolUser
        private RolUser MapToEntity(RolUserDto RolUserDto)
        {
            return new RolUser
            {
                id = RolUserDto.Id,
                rolid = RolUserDto.RolId,
                userid = RolUserDto.UserId // Si existe en la entidad
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

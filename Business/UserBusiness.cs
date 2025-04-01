using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// clase de negocio encargada de la logica relacionada con User del sistema
    /// </summary>
    public class UserBusiness
    {
        private readonly UserData _userData;
        private readonly ILogger _logger;

        public UserBusiness(UserData userData, ILogger logger)
        {
            _userData = userData;
            _logger = logger;
        }

        //Atributo para obtener todos los User con DTOs
        public async Task<IEnumerable<UserDto>> GetAllUserAsync()
        {
            try
            {
                var user = await _userData.GetAllAsyncSQL();
                var UserDTO = MapToDTOList(user);
                return UserDTO;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los User");
                throw new ExternalServiceException("base de datos", "Error al recuperar la lista de User", ex);
            }
        }

        //Atributo para obtener un User por Id como DTO
        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            if(id <= 0)
            {
                _logger.LogWarning("se intento obtener un User con Id invalido: {UserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "el Id del user debe ser meyor a cero");
            }

            try
            {
                var User = await _userData.GetByIdAsync(id);
                if(User == null)
                {
                    _logger.LogInformation("No se encontro ningun User con Id: {UserId}", id);
                    throw new EntityNotFoundException("User", id);
                }

                return MapToDTO(User);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el User con ID: {UserId}", id);
                throw new ExternalServiceException("base de datos", $"Error al recuperar el User con Id {id}", ex);
            }
        }

        //Atributo para crear un User desde el DTO
        public async Task<UserDto> CreateUserAsync(UserDto UserDto)
        {
            try
            {
                valiteUser(UserDto);

                var User = MapToEntity(UserDto);

                var UserCreado = await _userData.CreateAsyncSQL(User);

                return MapToDTO(UserCreado);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo User : {UserNombre}", UserDto?.UserName ?? "null");
                throw new ExternalServiceException("base de datos ", "Error al crear el User", ex);
            }
        }

        public async Task<UserDto> UpdateUserAsync(UserDto userDto)
        {
            try
            {
                valiteUser(userDto);
                var existingUser = await _userData.GetByIdAsync(userDto.Id);
                if (existingUser == null)
                {
                    throw new EntityNotFoundException("User", "No se encontró la relación User");
                }

                existingUser.IsDeleted = userDto.IsDeleted;
                var success = await _userData.UpdateAsync(existingUser);

                if (!success)
                {
                    throw new Exception("No se pudo actualizar la relación User.");
                }

                return MapToDTO(existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la relación User");
                throw new ExternalServiceException("Base de datos", "Error al actualizar la relación User", ex);
            }
        }

        // Método para eliminar una relación Rol de manera lógica
        public async Task<bool> DeleteRolLogicalAsync(int id)
        {
            try
            {
                return await _userData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente la relación Rol");
                throw new ExternalServiceException("Base de datos", "Error al eliminar la relación Rol", ex);
            }
        }

        //Atributo para validar al DTO
        private void valiteUser(UserDto UserDto)
        {
            if(UserDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto User np puede ser Nulo");
            }

            if (string.IsNullOrWhiteSpace(UserDto.UserName))
            {
                _logger.LogWarning("se intento crear/actualizar un User Name vacio");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del User es obligatorio");
            }
        }

        //Atributo para Mapear de User a UserDTO
        private UserDto MapToDTO(User User)
        {
            return new UserDto
            {
                Id = User.Id,
                UserName = User.UserName,
                gmail = User.gmail,
            };
        }

        // Atributo para mapear de UserDTO a User
        private User MapToEntity(UserDto UserDto)
        {
            return new User
            {
                Id = UserDto.Id,
                UserName = UserDto.UserName,
                gmail = UserDto.gmail,
            };
        }

        // Atributo para mapear una lista de User a una lista de UserDTO
        private IEnumerable<UserDto> MapToDTOList(IEnumerable<User> users)
        {
            var UserDTO = new List<UserDto>();
            foreach (var user in users)
            {
                UserDTO.Add(MapToDTO(user));
            }
            return UserDTO;
        }
    }
}

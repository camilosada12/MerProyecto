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
        private readonly ILogger<UserBusiness> _logger;

        public UserBusiness(UserData userData, ILogger<UserBusiness> logger)
        {
            _userData = userData;
            _logger = logger;
        }

        //DTOS

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
                _logger.LogWarning("se intento obtener un User con Id invalido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "el Id del user debe ser meyor a cero");
            }

            try
            {
                var User = await _userData.GetByIdAsyncSQL(id);
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

        // Corrección en CreateUserAsync
        public async Task<UserDto> CreateUserAsync(UserDto UserDto)
        {
            try
            {
                valiteUser(UserDto);

                var User = MapToEntity(UserDto);

                // Corrección: Asegurar que se retorna el ID correcto
                var UserCreado = await _userData.CreateAsyncSQL(User);
                _logger.LogInformation("Usuario insertado con ID: {UserId}", UserCreado.id);

                return MapToDTO(UserCreado);
            }
            catch (Exception ex)
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

                var existingUser = await _userData.GetByIdAsyncSQL(userDto.id);
                if (existingUser == null)
                {
                    throw new EntityNotFoundException("User", $"No se encontró el usuario con ID ");
                }

                // Convertir la fecha a UTC antes de actualizar
                existingUser.user_per = userDto.UserName;
                existingUser.password = userDto.Password;
                existingUser.gmail = userDto.Gmail;
                existingUser.registrationdate = userDto.Registrationdate;

                var success = await _userData.UpdateAsyncSQL(existingUser);

                if (!success)
                {
                    throw new Exception("No se pudo actualizar el usuario.");
                }

                return MapToDTO(existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el usuario con ID ");
                throw new ExternalServiceException("Base de datos", "Error al actualizar el usuario", ex);
            }
        }


        // Método para eliminar una relación Rol de manera lógica
        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                return await _userData.DeleteAsyncSQL(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente la relación User");
                throw new ExternalServiceException("Base de datos", "Error al eliminar la relación User", ex);
            }
        }

        //Atributo para validar al DTO
        private void valiteUser(UserDto UserDto)
        {
            if (UserDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto User np puede ser Nulo");
            }

            if (string.IsNullOrWhiteSpace(UserDto.UserName)){ 
                _logger.LogWarning("se intento crear/actualizar un User Name vacio");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del User es obligatorio");
            }
        }

        //Atributo para Mapear de User a UserDTO
        private UserDto MapToDTO(User User)
        {
            return new UserDto
            {
                id = User.id,  
                UserName = User.user_per,
                Password = User.password, 
                Gmail = User.gmail,
                Registrationdate = User.registrationdate
            };
        }

        // Atributo para mapear de UserDTO a User
        private User    MapToEntity(UserDto UserDto)
        {
            return new User
            {
                id = UserDto.id, 
                user_per = UserDto.UserName,
                password = UserDto.Password, 
                gmail = UserDto.Gmail,
                registrationdate = UserDto.Registrationdate
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

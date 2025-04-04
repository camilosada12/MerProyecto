using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.ContUserlers
{
    /// <summary>
    /// ContUserador para la gestion de permisos en el sistema
    /// </summary>

    [Route("api/[Controller]")]
    [ApiController]
    [Produces("application/json")]

    public class UserController : ControllerBase
    {
        private readonly UserBusiness _UserBusiness;
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Constructor del Yser de permisos
        /// </summary>
        /// <param name="UserBusiness">Capa de negocio de permisos</param>
        ///  <param name="Logger">Logeer para registro de eventos</param>
        public UserController(UserBusiness userBusiness, ILogger<UserController> logger)
        {
            _UserBusiness = userBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los User del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllForms()
        {
            try
            {
                var User = await _UserBusiness.GetAllUserAsync();
                return Ok(User);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error al obtener los User");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Obtener un User especificio por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var User = await _UserBusiness.GetUserByIdAsync(id);
                return Ok(User);
            }
            catch (ValidationException ex)
            {
                _logger.LogInformation(ex, "Validacion fallida para User con ID: {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {

                _logger.LogInformation(ex, "User no encontrado con ID: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el User con ID: {UserId}", id);
                throw;
            }
        }

        ///<summary>
        ///crea un nuevo permiso en el sistema
        /// </summary>
        /// <param name="UserDto">Datos del permiso a crear</param>
        /// <returns> permiso creado</returns>
        /// <response code"201">retorna el permiso creado</response>
        /// <response code"400">retorna el permiso creado</response>
        /// <response code"500">retorna el permiso creado</response>

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> creadoUser([FromBody] UserDto UserDto)
        {
            _logger.LogInformation("Recibiendo petición para crear usuario");
            try
            {
                var createUser = await _UserBusiness.CreateUserAsync(UserDto);
                _logger.LogInformation("Usuario creado con ID: {UserId}", createUser.id);
                return CreatedAtAction(nameof(GetUserById), new { Id = createUser.id }, createUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpPut]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UserDto userDto)
        {
            try
            {
                userDto.Registrationdate = userDto.Registrationdate.ToUniversalTime();

                var updatedUser = await _UserBusiness.UpdateUserAsync(userDto);
                return Ok(updatedUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar usuario con ID: {UserId}");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado con ID: {UserId}");
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario con ID: {UserId}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            try
            {
                var result = await _UserBusiness.DeleteUserAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar usuario con ID: {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado con ID: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
//BadRequest = se usa cuando la solicitud del cliente no es válida. Devuelve un HTTP 400 (Bad Request) con un mensaje de error
//NotFound = Se usa cuando el recurso solicitado no existe. Devuelve un HTTP 404 (Not Found).
//StatusCode = Este método te permite devolver cualquier código de estado HTTP.


using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// ContUserRolador para gestion de permisos en el sistema
    /// </summary>
    ///

    [Route("api/[Controller]")]
    [ApiController] //Especifica que la respuesta 200 OK devolverá una lista de UserRolDto.
    [Produces("application/json")]

    public class UserRolController : ControllerBase
    {
        private readonly UserRolBusiness _UserRolBusiness;
        private readonly ILogger<UserRolController> _logger;

        /// <summary>
        /// Constructor del UserRol de permisos
        /// </summary>
        /// <param name="UserRolBusiness">Capa de negocio de permisos</param>
        ///  <param name="Logger">Logeer para registro de eventos</param>
        public UserRolController(UserRolBusiness userRolBusiness, ILogger<UserRolController> logger)
        {
            _UserRolBusiness = userRolBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los rol user del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolUserDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRolUser()
        {
            try
            {
                var RolUsers = await _UserRolBusiness.GetAllUserRolAsync();
                return Ok(RolUsers);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error al obtener los rolUser ");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Obtener un module especificio por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolUserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolUserById(int id)
        {
            try
            {
                var RolUser = await _UserRolBusiness.GetRolUserByIdAsync(id);
                return Ok(RolUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogInformation(ex, "Validacion fallida para rol user con ID: {RolUserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {

                _logger.LogInformation(ex, "Form no encontrado con ID: {RolUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el Rol user con ID: {RolUserId}", id);
                throw;
            }
        }

        ///<summary>
        ///crea un nuevo permiso en el sistema
        /// </summary>
        /// <param name="UserRolDto">Datos del permiso a crear</param>
        /// <returns> permiso creado</returns>
        /// <response code"201">retorna el permiso creado</response>
        /// <response code"400">retorna el permiso creado</response>
        /// <response code"500">retorna el permiso creado</response>

        [HttpPost]
        [ProducesResponseType(typeof(RolUserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateForm([FromBody] RolUserDto rolUserDto)
        {
            try
            {
                var createdRolUser = await _UserRolBusiness.CreateRolUserAsync(rolUserDto);
                return CreatedAtAction(nameof(GetRolUserById), new { id = createdRolUser.Id }, createdRolUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida al creal el Rol user");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el rol user");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un form existente en el sistema
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(RolUserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRolUser([FromBody] RolUserDto rolUserDto)
        {
            try
            {

                if (rolUserDto == null || rolUserDto.Id <= 0)
                {
                    return BadRequest(new { message = "El ID de la ruta no coincide con el ID del objeto." });
                }

                var updatedRolUser = await _UserRolBusiness.UpdateRolUserAsync(rolUserDto);

                return Ok(updatedRolUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el module con ID: {ModuleId}");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el module con ID: {RolUserId}");
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el module con ID: {RolUserId}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un form del sistema
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRolUser(int id)
        {
            try
            {
                var deleted = await _UserRolBusiness.DeleteRolUserAsync(id);

                if (!deleted)
                {
                    return NotFound(new { message = "rol user no encontrado o ya eliminado" });
                }

                return Ok(new { message = "rol user eliminado exitosamente" });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol user con ID: {RolUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}

//BadRequest = se usa cuando la solicitud del cliente no es válida. Devuelve un HTTP 400 (Bad Request) con un mensaje de error
//NotFound = Se usa cuando el recurso solicitado no existe. Devuelve un HTTP 404 (Not Found).
//StatusCode = Este método te permite devolver cualquier código de estado HTTP.

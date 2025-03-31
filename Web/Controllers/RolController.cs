using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para gestion de permisos en el sistema
    /// </summary>

    [Route("api/[controller]")]
    [ApiController] //Especifica que la respuesta 200 OK devolverá una lista de RolDto.
    [Produces("application/json")]

    public class RolController : ControllerBase
    {
        private readonly RolBusiness _RolBusiness;
        private readonly ILogger<RolController> _logger;

        /// <summary>
        /// Constructor del controlador de permisos
        /// </summary>
        /// <param name="RolBusiness">Capa de negocio de permisos</param>
        ///  <param name="Logger">Logeer para registro de eventos</param>
        public RolController(RolBusiness RolBusiness, ILogger<RolController> logger)
        {
            _RolBusiness = RolBusiness;
            _logger = logger;
        }

        ///<summary>
        ///obtiene todos los permisos del sistema
        /// </summary>
        /// <response code"200">Retorna la lista de permisos</response>
        /// <response code"500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var Rol = await _RolBusiness.GetAllRolesAsync(); // obtiene la lista de roles desde la capa de negocio.
                return Ok(Rol); //Devuelve un 200 OK con los datos
            }
            catch(ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el permiso con ID: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch(EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {RolId}", id);
                return NotFound(new {message = ex.Message });
            }
            catch(ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permiso con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        ///crea un nuevo permiso en el sistema
        /// </summary>
        /// <param name="RolDto">Datos del permiso a crear</param>
        /// <returns> permiso creado</returns>
        /// <response code"201">retorna el permiso creado</response>
        /// <response code"400">retorna el permiso creado</response>
        /// <response code"500">retorna el permiso creado</response>
        public async Task<IActionResult> creadoRol([FromBody] RolDto RolDto) // [FromBody] indica que los datos se recibirán en el cuerpo de la solicitud en formato JSON.
        {
            try
            {
                var createRol = await _RolBusiness.CreateRolAsync(RolDto);
                return CreatedAtAction(nameof(GetByIdAsync), new { id = createRol.Id }, createRol);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear permiso");
                return BadRequest(new { message = ex.Message });
            }
            catch  (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear permiso");
                return StatusCode(500, new { message = ex.Message });
            }

            //BadRequest = se usa cuando la solicitud del cliente no es válida. Devuelve un HTTP 400 (Bad Request) con un mensaje de error
            //NotFound = Se usa cuando el recurso solicitado no existe. Devuelve un HTTP 404 (Not Found).
            //StatusCode = Este método te permite devolver cualquier código de estado HTTP.
        }
    }
}

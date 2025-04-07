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
        /// Constructor del Rol de permisos
        /// </summary>
        /// <param name="RolBusiness">Capa de negocio de permisos</param>
        ///  <param name="Logger">Logeer para registro de eventos</param>
        public RolController(RolBusiness rolBusiness, ILogger<RolController> logger)
        {
            _RolBusiness = rolBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los forms del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllForms()
        {
            try
            {
                var Forms = await _RolBusiness.GetAllRolesAsync();
                return Ok(Forms);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error al obtener los forms");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        ///obtiene todos los permisos del sistema
        /// </summary>
        /// <response code"200">Retorna la lista de permisos</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Permiso no encontrado</response>
        /// <response code"500">Error interno del servidor</response>

        ///<summary>
        /// Obtener un form especificio por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolById(int id)
        {
            try
            {
                var Rol = await _RolBusiness.GetRolByIdAsync(id);
                return Ok(Rol);
            }
            catch (ValidationException ex)
            {
                _logger.LogInformation(ex, "Validacion fallida para Rol con ID: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {

                _logger.LogInformation(ex, "Rol no encontrado con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el Rol con ID: {RolId}", id);
                throw;
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

        [HttpPost]
        [ProducesResponseType(typeof(RolDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> creadoRol([FromBody] RolDto RolDto)
        {
            _logger.LogInformation("Recibiendo petición para crear Rol");
            try
            {
                var createRol = await _RolBusiness.CreateRolAsync(RolDto);
                _logger.LogInformation("Rol creado con ID: {RolId}", createRol.Id);
                return CreatedAtAction(nameof(GetRolById), new { id = createRol.Id }, createRol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear Rol");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(RolDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRolAsync([FromBody] RolDto RolDto)
        {
            try
            {
                var updatedRol = await _RolBusiness.UpdateRolAsync(RolDto);
                return Ok(updatedRol);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar Rol con ID: {RolId}", RolDto.Id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado con ID: {RolId}", RolDto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar Rol con ID: {RolId}", RolDto.Id);
                return StatusCode(500, new { message = ex.Message });


            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRolAsync(int id)
        {
            try
            {
                var result = await _RolBusiness.DeleteRolAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Rol no encontrado" });
                }
                return Ok(new { message = "Rol eliminado exitosamente" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar Rol con ID: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar Rol con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("logico/{id}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteLogicoRolAsync(int id)
        {
            try
            {
                var result = await _RolBusiness.DeleteLogicoRolAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Rol no encontrado o ya eliminado" });
                }

                return Ok(new { message = "Rol eliminado lógicamente exitosamente" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar Rol con ID: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar Rol con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}

//BadRequest = se usa cuando la solicitud del cliente no es válida. Devuelve un HTTP 400 (Bad Request) con un mensaje de error
//NotFound = Se usa cuando el recurso solicitado no existe. Devuelve un HTTP 404 (Not Found).
//StatusCode = Este método te permite devolver cualquier código de estado HTTP.

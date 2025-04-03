using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.ContModulelers
{
    /// <summary>
    /// ContModuleador para gestion de permisos en el sistema
    /// </summary>
    ///
    
    [Route("api/[Controller]")]
    [ApiController] //Especifica que la respuesta 200 OK devolverá una lista de ModuleDto.
    [Produces("application/json")]

    public class ModuleController : ControllerBase
    {
        private readonly ModuleBusinness _ModuleBusiness;
        private readonly ILogger<ModuleController> _logger;

        /// <summary>
        /// Constructor del Module de permisos
        /// </summary>
        /// <param name="ModuleBusiness">Capa de negocio de permisos</param>
        ///  <param name="Logger">Logeer para registro de eventos</param>
        public ModuleController(ModuleBusinness moduleBusiness, ILogger<ModuleController> logger)
        {
            _ModuleBusiness = moduleBusiness;
            _logger = logger;
        }

        ///<summary>
        ///obtiene todos los permisos del sistema
        /// </summary>
        /// <response code"200">Retorna la lista de permisos</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Permiso no encontrado</response>
        /// <response code"500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IEnumerable<ModuleDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var Module = await _ModuleBusiness.GetAllModuleAsync(); // obtiene la lista de Modulees desde la capa de negocio.
                return Ok(Module); //Devuelve un 200 OK con los datos
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el permiso con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permiso con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        ///crea un nuevo permiso en el sistema
        /// </summary>
        /// <param name="ModuleDto">Datos del permiso a crear</param>
        /// <returns> permiso creado</returns>
        /// <response code"201">retorna el permiso creado</response>
        /// <response code"400">retorna el permiso creado</response>
        /// <response code"500">retorna el permiso creado</response>

        [HttpPost]
        [ProducesResponseType(typeof(ModuleDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> creadoModule([FromBody] ModuleDto ModuleDto) // [FromBody] indica que los datos se recibirán en el cuerpo de la solicitud en formato JSON.
        {
            try
            {
                var createModule = await _ModuleBusiness.CreateModuleAsync(ModuleDto);
                return CreatedAtAction(nameof(GetByIdAsync), new { id = createModule.Id }, createModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear permiso");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
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

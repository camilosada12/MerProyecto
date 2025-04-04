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

        /// <summary>
        /// Obtener todos los User del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ModuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllForms()
        {
            try
            {
                var User = await _ModuleBusiness.GetAllModuleAsync();
                return Ok(User);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error al obtener los User");
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
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetByModuleIdAsync(int id)
        {
            try
            {
                var Module = await _ModuleBusiness.GetModulesByIdAsync(id); // obtiene la lista de Modulees desde la capa de negocio.
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

        public async Task<IActionResult> creadoModule([FromBody] ModuleDto ModuleDto)
        {
            _logger.LogInformation("Recibiendo petición para crear Module");
            try
            {
                var createModule = await _ModuleBusiness.CreateModuleAsync(ModuleDto);
                _logger.LogInformation("Module creado con ID: {ModuleId}", createModule.Id);
                return CreatedAtAction(nameof(GetByModuleIdAsync), new { id = createModule.Id }, createModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear Module");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateModuleAsync(int id, [FromBody] ModuleDto moduleDto)
        {
            try
            {

                var updatedModule = await _ModuleBusiness.UpdateModuleAsync(id, moduleDto);
                return Ok(updatedModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar Modulo con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Modulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar Modulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteModuleAsync(int id)
        {
            try
            {
                var result = await _ModuleBusiness.DeleteModuleAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Module no encontrado" });
                }
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar Module con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Module no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar Module con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}

//BadRequest = se usa cuando la solicitud del cliente no es válida. Devuelve un HTTP 400 (Bad Request) con un mensaje de error
//NotFound = Se usa cuando el recurso solicitado no existe. Devuelve un HTTP 404 (Not Found).
//StatusCode = Este método te permite devolver cualquier código de estado HTTP.
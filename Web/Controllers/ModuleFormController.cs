using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// ContModuleFormador para gestion de permisos en el sistema
    /// </summary>
    ///

    [Route("api/[Controller]")]
    [ApiController] //Especifica que la respuesta 200 OK devolverá una lista de ModuleFormDto.
    [Produces("application/json")]
    public class ModuleFormController : ControllerBase
    {
        private readonly ModuleFormBusinness _ModuleFormBusiness;
        private readonly ILogger<ModuleFormController> _logger;

        /// <summary>
        /// Constructor del ModuleForm de permisos
        /// </summary>
        /// <param name="ModuleFormBusiness">Capa de negocio de permisos</param>
        ///  <param name="Logger">Logeer para registro de eventos</param>
        public ModuleFormController(ModuleFormBusinness moduleFormBusiness, ILogger<ModuleFormController> logger)
        {
            _ModuleFormBusiness = moduleFormBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los rol user del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ModuleFormDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllModuleForm()
        {
            try
            {
                var ModuleForm = await _ModuleFormBusiness.GetAllModuleFormAsync();
                return Ok(ModuleForm);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error al obtener los ModuleForm ");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Obtener un module especificio por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ModuleFormDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetModuleFormById(int id)
        {
            try
            {
                var ModuleForm = await _ModuleFormBusiness.GetByModuleFormIdAsync(id);
                return Ok(ModuleForm);
            }
            catch (ValidationException ex)
            {
                _logger.LogInformation(ex, "Validacion fallida para ModuleForm user con ID: {ModuleFormUserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {

                _logger.LogInformation(ex, "Form no encontrado con ID: {ModuleFormUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el ModuleForm user con ID: {ModuleFormUserId}", id);
                throw;
            }
        }

        ///<summary>
        ///crea un nuevo permiso en el sistema
        /// </summary>
        /// <param name="ModuleFormDto">Datos del permiso a crear</param>
        /// <returns> permiso creado</returns>
        /// <response code"201">retorna el permiso creado</response>
        /// <response code"400">retorna el permiso creado</response>
        /// <response code"500">retorna el permiso creado</response>

        [HttpPost]
        [ProducesResponseType(typeof(ModuleFormDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateModuleForm([FromBody] ModuleFormDto moduleFormDto)
        {
            try
            {
                var createdModuleForm = await _ModuleFormBusiness.CreateModuleFormAsync(moduleFormDto);
                return CreatedAtAction(nameof(GetModuleFormById), new { id = createdModuleForm.Id }, createdModuleForm);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida al creal el Module Form");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el Module Form");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un form existente en el sistema
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(ModuleFormDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRolUser([FromBody] ModuleFormDto moduleFormDto)
        {
            try
            {

                if (moduleFormDto == null || moduleFormDto.Id <= 0)
                {
                    return BadRequest(new { message = "El ID de la ruta no coincide con el ID del objeto." });
                }

                var updatedmoduleForm = await _ModuleFormBusiness.UpdateModuleFormAsync(moduleFormDto);

                return Ok(updatedmoduleForm);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el moduleForm con ID: {moduleFormId}");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el module con ID: {moduleFormId}");
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el module con ID: {moduleFormId}");
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
                var deleted = await _ModuleFormBusiness.DeleteModuleFormAsync(id);

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

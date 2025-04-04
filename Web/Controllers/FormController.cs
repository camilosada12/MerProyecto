
using Business;
using Data;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;



namespace Web.Controllers
{
    /// <summary>
    /// ContModuleador para gestion de permisos en el sistema
    /// </summary>
    ///

    [Route("api/[Controller]")]
    [ApiController] //Especifica que la respuesta 200 OK devolverá una lista de ModuleDto.
    [Produces("application/json")]
    public class FormController : ControllerBase
    {
        private readonly FormBusiness _FormBusiness;
        private readonly ILogger<FormController> _logger;

        /// <summary>
        /// Constructor del Form de permisos
        /// </summary>
        /// <param name="FormBusiness">Capa de negocio de permisos</param>
        
        ///  <param name="Logger">Logeer para registro de eventos</param>
        public FormController(FormBusiness formBusiness, ILogger<FormController> logger)
        {
            _FormBusiness = formBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los forms del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllForms()
        {
            try
            {
                var Forms = await _FormBusiness.GetAllformAsync();
                return Ok(Forms);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error al obtener los forms");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Obtener un form especificio por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFormById(int id)
        {
            try
            {
                var form = await _FormBusiness.GetFormByAsync(id);
                return Ok(form);
            }
            catch (ValidationException ex)
            {
                _logger.LogInformation(ex, "Validacion fallida para form con ID: {formId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {

                _logger.LogInformation(ex, "form no encontrado con ID: {formId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el form con ID: {formId}", id);
                throw;
            }
        }

        ///<summary>
        ///crea un nuevo permiso en el sistema
        /// </summary>
        /// <param name="FormDto">Datos del permiso a crear</param>
        /// <returns> permiso creado</returns>
        /// <response code"201">retorna el permiso creado</response>
        /// <response code"400">retorna el permiso creado</response>
        /// <response code"500">retorna el permiso creado</response>

        [HttpPost]
        [ProducesResponseType(typeof(FormDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> creadoform([FromBody] FormDto formDto)
        {
            _logger.LogInformation("Recibiendo petición para crear  un form");
            try
            {
                var createform = await _FormBusiness.CreateFormAsync(formDto);
                _logger.LogInformation("Usuario creado con ID: {formId}", createform.Id);
                return CreatedAtAction(nameof(GetFormById), new { id = createform.Id }, createform);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(FormDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] FormDto formDto)
        {
            try
            {
                // Convertir la fecha a UTC antes de enviarla a la lógica de negocio
                formDto.DateCreation = formDto.DateCreation.ToUniversalTime();

                var updatedForm = await _FormBusiness.UpdateFormAsync(id, formDto);
                return Ok(updatedForm);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar usuario con ID: {FormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado con ID: {FormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario con ID: {FormId}", id);
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
                var result = await _FormBusiness.DeleteFormAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar usuario con ID: {FormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado con ID: {FormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario con ID: {FormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}



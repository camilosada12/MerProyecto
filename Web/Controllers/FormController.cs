
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

        [HttpPost("SetProvider")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult SetDatabaseProvider([FromBody] string provider)
        {
            try
            {
                if (string.IsNullOrEmpty(provider))
                {
                    return BadRequest(new { message = "El nombre del proveedor no puede estar vacío" });
                }

                string proveedorNormalizado = provider.ToLower();
                if (proveedorNormalizado != "postgresql" &&
                    proveedorNormalizado != "mysql" &&
                    proveedorNormalizado != "sqlserver")
                {
                    return BadRequest(new { message = $"El proveedor '{provider}' no está soportado. Use 'postgresql', 'mysql', o 'sqlserver'." });
                }

                _FormBusiness.SetDatabaseProvider(proveedorNormalizado);
                return Ok(new { message = $"Proveedor cambiado exitosamente a {provider}" });
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el proveedor de base de datos a {Provider}", provider);
                return StatusCode(500, new { message = "Ocurrió un error al cambiar el proveedor de base de datos." });
            }
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
                _logger.LogInformation("Form creado con ID: {formId}", createform.Id);
                return CreatedAtAction(nameof(GetFormById), new { id = createform.Id }, createform);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear Form");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(FormDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateFormAsync([FromBody] FormDto formDto)
        {
            try
            {

                var updatedForm = await _FormBusiness.UpdateFormAsync(formDto);
                return Ok(updatedForm);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar Form con ID: {FormId}");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Form no encontrado con ID: {FormId}");
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar Form con ID: {FormId}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteFormAsync(int id)
        {
            try
            {
                var result = await _FormBusiness.DeleteFormAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Form no encontrado" });
                }

                return Ok(new { message = "form eliminado exitosamente" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar Form con ID: {FormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Form no encontrado con ID: {FormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar Form con ID: {FormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("logico/{id}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteLogicoFormAsync(int id)
        {
            try
            {
                var result = await _FormBusiness.DeleteLogicoFormAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Form no encontrado o ya eliminado" });
                }

                return Ok(new { message = "Form eliminado lógicamente exitosamente" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar Form con ID: {FormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Form no encontrado con ID: {FormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar Form con ID: {FormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}



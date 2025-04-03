﻿
using Business;
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

        ///<summary>
        ///obtiene todos los permisos del sistema
        /// </summary>
        /// <response code"200">Retorna la lista de permisos</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Permiso no encontrado</response>
        /// <response code"500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IEnumerable<FormDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var Form = await _FormBusiness.GetAllFormsAsync(); // obtiene la lista de Formes desde la capa de negocio.
                return Ok(Form); //Devuelve un 200 OK con los datos
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el permiso con ID: {FormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {FormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permiso con ID: {FormId}", id);
                return StatusCode(500, new { message = ex.Message });
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
        public async Task<IActionResult> creadoForm([FromBody] FormDto FormDto) // [FromBody] indica que los datos se recibirán en el cuerpo de la solicitud en formato JSON.
        {
            try
            {
                var createForm = await _FormBusiness.createFormAsync(FormDto);
                return CreatedAtAction(nameof(GetByIdAsync), new { id = createForm.Id }, createForm);
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

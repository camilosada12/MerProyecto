using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// ContPersonador para la gestion de permisos en el sistema
    /// </summary>

    [Route("api/[Controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PersonController : ControllerBase
    {
        private readonly PersonBusiness _PersonBusiness;
        private readonly ILogger<PersonController> _logger;

        /// <summary>
        /// Constructor del Person de permisos
        /// </summary>
        /// <param name="PersonBusiness">Capa de negocio de permisos</param>
        ///  <param name="Logger">Logeer para registro de eventos</param>
        public PersonController(PersonBusiness personBusiness, ILogger<PersonController> logger)
        {
            _PersonBusiness = personBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los person del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PersonDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllForms()
        {
            try
            {
                var person = await _PersonBusiness.GetAllPersonAsync();
                return Ok(person);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error al obtener los person");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Obtener un User especificio por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PersonDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPersonById(int id)
        {
            try
            {
                var Person = await _PersonBusiness.GetByPersonIdAsync(id);
                return Ok(Person);
            }
            catch (ValidationException ex)
            {
                _logger.LogInformation(ex, "Validacion fallida para Person con ID: {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {

                _logger.LogInformation(ex, "Person no encontrado con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el Person con ID: {PersonId}", id);
                throw;
            }
        }

        ///<summary>
        ///crea un nuevo permiso en el sistema
        /// </summary>
        /// <param name="PersonDto">Datos del permiso a crear</param>
        /// <returns> permiso creado</returns>
        /// <response code"201">retorna el permiso creado</response>
        /// <response code"400">retorna el permiso creado</response>
        /// <response code"500">retorna el permiso creado</response>

        [HttpPost]
        [ProducesResponseType(typeof(PersonDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> creadoPerson([FromBody] PersonDto PersonDto)
        {
            _logger.LogInformation("Recibiendo petición para crear Person");
            try
            {
                var createPerson = await _PersonBusiness.CreatePersonAsync(PersonDto);
                _logger.LogInformation("Person creado con ID: {PersonId}", createPerson.Id);
                return CreatedAtAction(nameof(GetPersonById), new { id = createPerson.Id }, createPerson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear Person");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PersonDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] PersonDto personDto)
        {
            try
            {
                var updatedPerson = await _PersonBusiness.UpdatePersonAsync(id, personDto);
                return Ok(updatedPerson);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar Persona con ID: {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Persona no encontrado con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar Persona con ID: {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePersonAsync(int id)
        {
            try
            {
                var result = await _PersonBusiness.DeletePersonAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Persona no encontrado" });
                }
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar Persona con ID: {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Persona no encontrado con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar Persona con ID: {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}

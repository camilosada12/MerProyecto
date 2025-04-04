using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController] //Especifica que la respuesta 200 OK devolverá una lista de RolDto.
    [Produces("application/json")]
    public class RolFormPermissionController : ControllerBase
    {
        private readonly RolFormPermissionBusiness _RolFormPermissionBusiness;
        private readonly ILogger<RolFormPermissionController> _logger;

        /// <summary>
        /// Constructor del RolFormPermission de permisos
        /// </summary>
        /// <param name="RolFormPermissionBusiness">Capa de negocio de permisos</param>
        ///  <param name="Logger">Logeer para registro de eventos</param>
        public RolFormPermissionController(RolFormPermissionBusiness rolFormPermissionBusiness, ILogger<RolFormPermissionController> logger)
        {
            _RolFormPermissionBusiness = rolFormPermissionBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los rol user del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolFormPermissionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRolFormPermission()
        {
            try
            {
                var RolFormPermission = await _RolFormPermissionBusiness.GetAllRolFormPermissionAsync();
                return Ok(RolFormPermission);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error al obtener los RolFormPermission ");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Obtener un module especificio por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolFormPermissionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolFormModuleById(int id)
        {
            try
            {
                var RolFormPermission = await _RolFormPermissionBusiness.GetByRolFormModuleIdAsync(id);
                return Ok(RolFormPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogInformation(ex, "Validacion fallida para RolFormPermission user con ID: {RolFormPermissionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {

                _logger.LogInformation(ex, "Form no encontrado con ID: {RolFormPermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el RolFormPermission user con ID: {RolFormPermissionId}", id);
                throw;
            }
        }

        ///<summary>
        ///crea un nuevo permiso en el sistema
        /// </summary>
        /// <param name="RolFormPermissionDto">Datos del permiso a crear</param>
        /// <returns> permiso creado</returns>
        /// <response code"201">retorna el permiso creado</response>
        /// <response code"400">retorna el permiso creado</response>
        /// <response code"500">retorna el permiso creado</response>

        [HttpPost]
        [ProducesResponseType(typeof(RolFormPermissionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRolFormPermission([FromBody] RolFormPermissionDto rolFormPermissionDto)
        {
            try
            {
                var createdRolFormPermission = await _RolFormPermissionBusiness.CreateRolFormPermissionAsync(rolFormPermissionDto);
                return CreatedAtAction(nameof(GetRolFormModuleById), new { id = createdRolFormPermission.Id }, createdRolFormPermission);
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
        [ProducesResponseType(typeof(RolFormPermissionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRolUser([FromBody] RolFormPermissionDto rolFormPermissionDto)
        {
            try
            {

                if (rolFormPermissionDto == null || rolFormPermissionDto.Id <= 0)
                {
                    return BadRequest(new { message = "El ID de la ruta no coincide con el ID del objeto." });
                }

                var updatedRolFormModule = await _RolFormPermissionBusiness.UpdateRolFormPermissionAsync(rolFormPermissionDto);

                return Ok(updatedRolFormModule);
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
        public async Task<IActionResult> DeleteRolFormPermission(int id)
        {
            try
            {
                var deleted = await _RolFormPermissionBusiness.DeleteModuleFormAsync(id);

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

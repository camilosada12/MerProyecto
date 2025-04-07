using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.ContPermissionlers
{
    

    [Route("api/[controller]")]
    [ApiController]  
    [Produces("application/json")]
    public class PermissionController : ControllerBase
    {
        private readonly PermissionBusinness _PermissionBusiness;
        private readonly ILogger<PermissionController> _logger;

        /// <summary>
        /// Constructor del Permission de permisos
        /// </summary>
        /// <param name="PermissionBusiness">Capa de negocio de permisos</param>
        ///  <param name="Logger">Logeer para registro de eventos</param>
        public PermissionController(PermissionBusinness permissionBusiness, ILogger<PermissionController> logger)
        {
            _PermissionBusiness = permissionBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los Permission del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PermissionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPermission()
        {
            try
            {
                var Permission = await _PermissionBusiness.GetAllPermissionAsync();
                return Ok(Permission);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error al obtener los Permission");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Obtener un Permission especificio por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PermissionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            try
            {
                var Permission = await _PermissionBusiness.GetPermissionByIdAsync(id);
                return Ok(Permission);
            }
            catch (ValidationException ex)
            {
                _logger.LogInformation(ex, "Validacion fallida para Permission con ID: {PermissionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {

                _logger.LogInformation(ex, "Permission no encontrado con ID: {PermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el Permission con ID: {PermissionId}", id);
                throw;
            }
        }

        ///<summary>
        ///crea un nuevo permiso en el sistema
        /// </summary>
        /// <param name="PermissionDto">Datos del permiso a crear</param>
        /// <returns> permiso creado</returns>
        /// <response code"201">retorna el permiso creado</response>
        /// <response code"400">retorna el permiso creado</response>
        /// <response code"500">retorna el permiso creado</response>

        [HttpPost]
        [ProducesResponseType(typeof(PermissionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> creadoPermission([FromBody] PermissionDto permissionDto)
        {
            _logger.LogInformation("Recibiendo petición para crear permiso");
            try
            {
                var createPermission = await _PermissionBusiness.CreatePermissionAsync(permissionDto);
                _logger.LogInformation("permiso creado con ID: {PermissionId}", createPermission.Id);
                return CreatedAtAction(nameof(GetPermissionById), new { id = createPermission.Id }, createPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear permiso");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(PermissionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePermissionAsync([FromBody] PermissionDto permissionDto)
        {
            try
            {
                var updatedPermission = await _PermissionBusiness.UpdatePermissionAsync(permissionDto);
                return Ok(updatedPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar Permission con ID: {PermissionId}", permissionDto.Id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permission no encontrado con ID: {PermissionId}", permissionDto.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar Permission con ID: {PermissionId}", permissionDto.Id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]  
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePermissionAsync(int id)
        {
            try
            {
                var result = await _PermissionBusiness.DeletePermissionAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "permission no encontrado" });
                }
                return Ok(new { message = "Permission eliminado exitosamente" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar permission con ID: {permissionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "permission no encontrado con ID: {permissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar permission con ID: {permissionId}", id);
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
                var result = await _PermissionBusiness.DeleteLogicoPermissionAsync(id);
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

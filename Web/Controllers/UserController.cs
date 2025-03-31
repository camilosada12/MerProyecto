
using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestion de permisos en el sistema
    /// </summary>

    [Route("api/[Controller]")]
    [ApiController]
    [Produces("application/json")]

    public class UserController : ControllerBase
    {
        private readonly UserBusiness _userBusinness;
        private readonly ILogger<UserController> _logger;

        ///<summary>
        ///constructor del controlar de permisos
        /// </summary>
        /// <param name="UserBusiness">Capa de negocio de permisos</param>
        /// <param name="logger">Logger para registro de eventos</param>
        
        public UserController(UserBusiness UserBusiness, ILogger<UserController> logger)
        {
            _userBusinness = UserBusiness;
            _logger =  logger;
        }

        ///<summary>
        ///Obtiene todos los permisos del sistema
        /// </summary>
        /// <returns>Lista de permisos</returns>
        /// <response code"200">Retorna la lista de permisos </response>
        /// <response code"500">Error interno del servidor  </response>

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> GetAllUserAsync()
        {
            try
            {
                var Rols = await _userBusinness.GetAllUserAsync();
                return Ok(Rols);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permisos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}

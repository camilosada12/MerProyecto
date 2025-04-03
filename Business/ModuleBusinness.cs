using Data;
using Entity.DTOs;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Entity.Model;

namespace Business
{
    /// <summary>
    /// clase de negocio encargada de la logica relacionada con los Modulos de sistema
    /// </summary>
    public class ModuleBusinness
    {
        private readonly ModuleData _ModuleData;
        private readonly ILogger<ModuleBusinness> _logger;

        public ModuleBusinness(ModuleData moduleData, ILogger<ModuleBusinness> logger)
        {
            _ModuleData = moduleData;
            _logger = logger;
        }

        //Atributo para obtener todos los Module como DTOs
        public async Task<IEnumerable<ModuleDto>> GetAllModuleAsync()
        {
            try
            {
                var Module = await _ModuleData.GetAllAsync();
                var ModuleDTO = MapToDTOList(Module);
                return ModuleDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los Module");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de Module", ex);
            }
        }

        // Atributo para obtener un Module por ID como DTO
        public async Task<ModuleDto> GetModuleByIdAsync(int id)
        {
            if(id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un Module con ID inválido: {ModuleId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del Modulo debe ser mayor que cero");
            }
            try
            {
                var Module = await _ModuleData.GetByIdAsync(id);
                if(Module == null)
                {
                    _logger.LogInformation("No se encontro ningun Module con Id : {ModuleId}", id);
                    throw new EntityNotFoundException("Module", id);

                }

                return MapToDTO(Module);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Module con Id: {Moduleid}", id);
                throw new ExternalServiceException("base de datos", $"Error al recuperar el Module con Id {id}", ex);
            }
        }

        //Atributo para crear un Module desde un DTO
        public async Task<ModuleDto> CreateModuleAsync(ModuleDto ModuleDto)
        {
            try
            {
                ValidateModule(ModuleDto);

                var Module = MapToEntity(ModuleDto);

                var ModuleCreado = await _ModuleData.CreateAsync(Module);

                return MapToDTO(ModuleCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo Module: {ModuleNombre}", ModuleDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el Module", ex);
            }
        }

        public async Task<ModuleDto> UpdateModuleAsync(ModuleDto moduleDto)
        {
            try
            {
                ValidateModule(moduleDto);
                var existingModule = await _ModuleData.GetByIdAsync(moduleDto.Id);
                if (existingModule == null)
                {
                    throw new EntityNotFoundException("Module", "No se encontró la relación Module");
                }

                var success = await _ModuleData.UpdateAsync(existingModule);

                if (!success)
                {
                    throw new Exception("No se pudo actualizar la relación Module.");
                }

                return MapToDTO(existingModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la relación Module");
                throw new ExternalServiceException("Base de datos", "Error al actualizar la relación Module", ex);
            }
        }

        // Atributo para validar el DTO
        private void ValidateModule(ModuleDto ModuleDto)
        {
            if (ModuleDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Module no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(ModuleDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un Module con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del Module es obligatorio");
            }
        }

        // Atributo para mapear de  Module a ModuleDTO
        private ModuleDto MapToDTO(Module Module)
        {
            return new ModuleDto
            {
                Id = Module.Id,
                Name = Module.Name,
                description = Module.description,
                statu = Module.statu
            };
        }

        // Atributo para mapear de ModuleDTO a Module
        private Module MapToEntity(ModuleDto ModuleDto)
        {
            return new Module
            {
                Id = ModuleDto.Id,
                Name = ModuleDto.Name,
                description = ModuleDto.description,
                statu = ModuleDto.statu
            };
        }

        // Atributo para mapear una lista de Module a una lista de ModuleDTO
        private IEnumerable<ModuleDto> MapToDTOList(IEnumerable<Module> Modules)
        {
            var ModulesDTO = new List<ModuleDto>();
            foreach (var Module in Modules)
            {
                ModulesDTO.Add(MapToDTO(Module));
            }
            return ModulesDTO;
        }
    }
}

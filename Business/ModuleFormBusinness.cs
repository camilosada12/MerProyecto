using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los ModuleForm del sistema.
    /// </summary>

    public class ModuleFormBusinness
    {
        private readonly ModuleFormData _ModuleFormData;
        private readonly ILogger<ModuleFormBusinness> _logger;

        public ModuleFormBusinness(ModuleFormData moduleFormData, ILogger<ModuleFormBusinness> logger)
        {
            _ModuleFormData = moduleFormData;
            _logger = logger;
        }

        // Atributo para obtener todos los ModuleForm como DTOs
        public async Task<IEnumerable<ModuleFormDto>> GetAllModuleFormAsync()
        {
            try
            {
                var Modules = await _ModuleFormData.GetAllModuleFormAsync();
                var ModulesDTO = MapToDTOList(Modules);

                return ModulesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los ModuleForm");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de ModuleForm", ex);
            }
        }

        // Atributo para obtener un ModuleForm por ID como DTO
        public async Task<ModuleFormDto> GetByModuleFormIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un ModuleForm con ID inválido: {ModuleFormId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del ModuleForm debe ser mayor que cero");
            }

            try
            {
                var Module = await _ModuleFormData.GetByIdAsync(id);
                if (Module == null)
                {
                    _logger.LogInformation("No se encontró ningún ModuleForm con ID: {ModuleFormId}", id);
                    throw new EntityNotFoundException("Permiso", "No se encontró el permiso");
                }

                return MapToDTO(Module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el ModuleForm con ID: {ModuleFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el ModuleForm con ID {id}", ex);
            }
        }

        // Atributo para crear un ModuleForm desde un DTO
        public async Task<ModuleFormDto> CreateModuleFormAsync(ModuleFormDto ModuleFormDto)
        {
            try
            {
                ValidateModuleForm(ModuleFormDto);

                var ModuleForm = MapToEntity(ModuleFormDto);

                var ModuleFormCreado = await _ModuleFormData.CreateModuleFormAsyncSql(ModuleForm);

                return MapToDTO(ModuleFormCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo ModuleFormDto: {ModuleFormId}", ModuleFormDto?.Id ?? null);
                
                throw new ExternalServiceException("Base de datos", "Error al crear el ModuleForm", ex);
            }
        }


        public async Task<ModuleFormDto> UpdateModuleFormAsync(ModuleFormDto moduleForDTO)
        {
            try
            {
                ValidateModuleForm(moduleForDTO);
                var existingModuleForm = await _ModuleFormData.GetByIdAsync(moduleForDTO.Id);
                if (existingModuleForm == null)
                {
                    throw new EntityNotFoundException("ModuleFormDto", "No se encontró la relación FormModule");
                }


                // Convertir la fecha a UTC antes de actualizar
                existingModuleForm.moduleid = moduleForDTO.ModuleId;
                existingModuleForm.formid = moduleForDTO.FormId;

                var success = await _ModuleFormData.UpdateAsync(existingModuleForm);

                if (!success)
                {
                    throw new Exception("No se pudo actualizar la relación ModuleFormDto.");
                }

                return MapToDTO(existingModuleForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la relación ModuleFormDto");
                throw new ExternalServiceException("Base de datos", "Error al actualizar la relación ModuleFormDto", ex);
            }
        }

        // Método para eliminar una relación  RolUser de manera permanente
        public async Task<bool> DeleteModuleFormAsync(int id)
        {
            try
            {
                var existingModuleForm = await _ModuleFormData.GetByIdAsync(id);
                if (existingModuleForm == null)
                {
                    throw new EntityNotFoundException("Rol User", id);
                }

                return await _ModuleFormData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente la relación  RolUser");
                throw new ExternalServiceException("Base de datos", "Error al eliminar la relación  RolUser", ex);
            }
        }

        // Atributo para ModuleFrom el DTO
        private void ValidateModuleForm(ModuleFormDto ModuleFormDto)
        {
            if (ModuleFormDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto ModuleForm no puede ser nulo");
            }
        }

        // Atributo para mapear de ModuleFrom a ModuleFromDTO 
        private ModuleFormDto MapToDTO(ModuleForm ModuleForm)
        {
            return new ModuleFormDto
            {
                Id = ModuleForm.id,
                FormId = ModuleForm.formid,
                ModuleId = ModuleForm.moduleid // Si existe en la entidad
            };
        }

        // Atributo para mapear de ModuleFromDTO a ModuleFrom
        private ModuleForm MapToEntity(ModuleFormDto ModuleFormDto)
        {
            return new ModuleForm
            {
                id = ModuleFormDto.Id,
                formid = ModuleFormDto.FormId,
                moduleid = ModuleFormDto.ModuleId // Si existe en la entidad
            };
        }

        // Atributo para mapear una lista de ModuleFrom a una ModuleFrom deModuleFromDTO
        private IEnumerable<ModuleFormDto> MapToDTOList(IEnumerable<ModuleForm> ModuleForms)
        {
            var ModuleFormDto = new List<ModuleFormDto>();
            foreach (var ModuleForm in ModuleForms)
            {
                ModuleFormDto.Add(MapToDTO(ModuleForm));
            }
            return ModuleFormDto;
        }
    }
}

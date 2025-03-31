using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;

using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// clase de negocio encargada de la logica relacionada con los formulario del sistema
    /// </summary>
    public class FormBusiness
    {
        private readonly FormData _formData;
        private readonly ILogger _logger;

        public FormBusiness(FormData formData, ILogger logger)
        {
            _formData = formData;
            _logger = logger;
        }

        //Atributo para obtener todos los Formulario como DTOs
        public async Task<IEnumerable<FormDto>> GetAllFormsAsync()
        {
            try
            {
                var Form = await _formData.GetAllAsync();
                var FormDTO = MapToDTOList(Form);

                return FormDTO;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los formularios");
                throw new ExternalServiceException("base de datos", "Error al recuperar la lista de formularios", ex);
            }
        }

        //Atributo para obtener un formulario por Id como DTO
        public async Task<FormDto> GetFormByAsync(int id)
        {
            if (id == 0)
            {
                _logger.LogWarning("se intento obtener un Formulario con Id invalido: {FormID}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El Id del Formulario debe ser mayor que cero");
            }
            try
            {
                var Form = await _formData.GetByIdAsync(id);
                if (Form == null)
                {
                    _logger.LogInformation("No se encontro ningun formulario con ID: {FormID}", id);
                    throw new EntityNotFoundException("Formulariol", id);
                }
                return MapToDTO(Form); 
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Formulario con ID: {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el Formulario con ID {id}", ex);
            }
        }

        //Atributo para crear un Formulario desde un DTO
        public async Task<FormDto> createFormAsync(FormDto FormDto)
        {
            try
            {
                ValidateForm(FormDto);

                var Form = MapToEntity(FormDto);

                var FormCreado = await _formData.CreateAsync(Form);

                return MapToDTO(FormCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo Formulario: {FormNombre}", FormDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el Formulario", ex);
            }
        }

        public async Task<FormDto> UpdateFormAsync(FormDto formDto)
        {
            try
            {
                ValidateForm(formDto);
                var existingForm = await _formData.GetByIdAsync(formDto.Id);
                if (existingForm == null)
                {
                    throw new EntityNotFoundException("Form", "No se encontró la relación Form");
                }

                existingForm.IsDeleted = formDto.IsDeleted;
                var success = await _formData.UpdateAsync(existingForm);

                if (!success)
                {
                    throw new Exception("No se pudo actualizar la relación Form.");
                }

                return MapToDTO(existingForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la relación Form");
                throw new ExternalServiceException("Base de datos", "Error al actualizar la relación Form", ex);
            }
        }

        // Método para eliminar una relación Form de manera lógica
        public async Task<bool> DeleteFormModuleLogicalAsync(int id)
        {
            try
            {
                return await _formData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente la relación Form");
                throw new ExternalServiceException("Base de datos", "Error al eliminar la relación Form", ex);
            }
        }

        //Atributo para validar el DTO
        private void ValidateForm(FormDto FormDto)
        {
            if (FormDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Form no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(FormDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un Form con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del Form es obligatorio");
            }
        }

        //Atributo para Mapear de Form a FormDTO
        private FormDto MapToDTO(Form Form)
        {
            return new FormDto
            {
                Id = Form.Id,
                Name = Form.Name,
                Description = Form.Description,
                DateCreation = Form.DateCreation,
                statu = Form.statu
            };
        }

        // Atributo para mapear de FormDTO a Form
        private Form MapToEntity(FormDto FormDto)
        {
            return new Form
            {
                Id = FormDto.Id,
                Name = FormDto.Name,
                Description = FormDto.Description,
                DateCreation = FormDto.DateCreation,
                statu = FormDto.statu
            };
        }

        // Atributo para mapear una lista de Form a una lista de FormDTO
        private IEnumerable<FormDto> MapToDTOList(IEnumerable<Form> Formes)
        {
            var FormDto = new List<FormDto>();
            foreach (var Forme in Formes)
            {
                FormDto.Add(MapToDTO(Forme));
            }
            return FormDto;
        }
    }
}

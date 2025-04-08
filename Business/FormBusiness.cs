using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using static Data.FormData;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los formularios del sistema
    /// </summary>
    public class FormBusiness
    {
        private readonly FormDataFactory _formDataFactory;
        private readonly ILogger<FormBusiness> _logger;

        public FormBusiness(
            FormDataFactory formDataFactory,
            ILogger<FormBusiness> logger)
        {
            _formDataFactory = formDataFactory;
            _logger = logger;
        }

        // Método para cambiar el proveedor de base de datos
        public void SetDatabaseProvider(string provider)
        {
            _formDataFactory.SetCurrentProvider(provider);
        }

        // Obtener todos los formularios como DTOs
        public async Task<IEnumerable<FormDto>> GetAllformAsync()
        {
            var formData = _formDataFactory.CreateFormData();
            try
            {
                var forms = await formData.GetAllFormAsyncSQL();
                return MapToDTOList(forms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los formularios");
                throw new ExternalServiceException("base de datos", "Error al recuperar la lista de formularios", ex);
            }
        }

        // Obtener un formulario por Id como DTO
        public async Task<FormDto> GetFormByAsync(int id)
        {
            if (id == 0)
            {
                _logger.LogWarning("Se intentó obtener un formulario con Id inválido: {FormID}", id);
                throw new ValidationException("id", "El Id del formulario debe ser mayor que cero");
            }

            var formData = _formDataFactory.CreateFormData();
            try
            {
                var form = await formData.GetByFormIdAsyncSql(id);
                if (form == null)
                {
                    _logger.LogInformation("No se encontró ningún formulario con ID: {FormID}", id);
                    throw new EntityNotFoundException("Formulario", id);
                }
                return MapToDTO(form);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el formulario con ID: {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el formulario con ID {id}", ex);
            }
        }

        // Crear un nuevo formulario desde un DTO
        public async Task<FormDto> CreateFormAsync(FormDto formDto)
        {
            var formData = _formDataFactory.CreateFormData();
            try
            {
                ValidateForm(formDto);
                var form = MapToEntity(formDto);

                var formCreado = await formData.CreateAsyncSQL(form);
                _logger.LogInformation("Formulario creado con ID: {formId}", formCreado.id);

                return MapToDTO(formCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo formulario: {formNombre}", formDto?.Name ?? "null");
                throw new ExternalServiceException("base de datos", "Error al crear el formulario", ex);
            }
        }

        // Actualizar un formulario existente
        public async Task<FormDto> UpdateFormAsync(FormDto formDto)
        {
            var formData = _formDataFactory.CreateFormData();
            try
            {
                ValidateForm(formDto);

                var existingForm = await formData.GetByFormIdAsyncSql(formDto.Id);
                if (existingForm == null)
                {
                    throw new EntityNotFoundException("Form", $"No se encontró el formulario con ID {formDto.Id}");
                }

                existingForm.name = formDto.Name;
                existingForm.description = formDto.Description;

                var success = await formData.UpdateFormAsyncSQL(existingForm);

                if (!success)
                {
                    throw new Exception("No se pudo actualizar el formulario.");
                }

                return MapToDTO(existingForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el formulario");
                throw new ExternalServiceException("Base de datos", "Error al actualizar el formulario", ex);
            }
        }

        // Eliminar un formulario físicamente
        public async Task<bool> DeleteFormAsync(int id)
        {
            var formData = _formDataFactory.CreateFormData();
            try
            {
                return await formData.DeleteAsyncSQL(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el formulario");
                throw new ExternalServiceException("Base de datos", "Error al eliminar el formulario", ex);
            }
        }

        // Eliminar un formulario lógicamente
        public async Task<bool> DeleteLogicoFormAsync(int id)
        {
            var formData = _formDataFactory.CreateFormData();
            try
            {
                return await formData.DeleteLogicoAsyncSQL(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el formulario");
                throw new ExternalServiceException("Base de datos", "Error al eliminar el formulario", ex);
            }
        }

        // Validar el DTO del formulario
        private void ValidateForm(FormDto formDto)
        {
            if (formDto == null)
            {
                throw new ValidationException("El objeto Form no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(formDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un formulario con Name vacío");
                throw new ValidationException("Name", "El nombre del formulario es obligatorio");
            }
        }

        // Mapear de Form a FormDTO
        private FormDto MapToDTO(Form form)
        {
            return new FormDto
            {
                Id = form.id,
                Name = form.name,
                Description = form.description,
            };
        }

        // Mapear de FormDTO a Form
        private Form MapToEntity(FormDto formDto)
        {
            return new Form
            {
                id = formDto.Id,
                name = formDto.Name,
                description = formDto.Description,
                isdelete = false
            };
        }

        // Mapear una lista de Form a una lista de FormDTO
        private IEnumerable<FormDto> MapToDTOList(IEnumerable<Form> forms)
        {
            return forms.Select(form => MapToDTO(form)).ToList();
        }
    }
}
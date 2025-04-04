
using Entity.Contexts;
using Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class ModuleFormData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ModuleFormData> _logger;


        /// <summary>
        /// constructor que recibe el contexto de base de datos
        /// </summary>
        /// <param name="context">instacia de <see cref="ApplicationDbContext"/> para la conexion con la base de datos </param>

        public ModuleFormData(ApplicationDbContext context, ILogger<ModuleFormData> logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///obtiene todos los ModuleForm almacenados en la base de datos 
        /// </summary>
        /// <returns>lista de ModuleForm </returns>

        //Atributo Linq
        public async Task<IEnumerable<ModuleForm>> GetAllModuleFormAsync()
        {
            return await _context.Set<ModuleForm>().ToListAsync();
        }

        //Atributo SQL        
        public async Task<IEnumerable<ModuleForm>> GetAllAsyncSQL()
        {
            string query = "SELECT * FROM ModuleForm";
            return (IEnumerable<ModuleForm>)await _context.QueryAsync<IEnumerable<ModuleForm>>(query);
        }

        public async Task<ModuleForm?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<ModuleForm>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ModuleForm con Id {ModuleFormId}", id);
                throw; // Re-lanza la excepcion para que sea manejada en capas superiores
            }
        }

        ///<summary>
        ///crea un nuevo ModuleForm en la base de datos
        ///</summary>
        ///<param name="moduleForm">instancia del ModuleForm a crear </param>
        ///<returns> El ModuleForm creado.</returns>

        public async Task<ModuleForm> CreateModuleFormAsyncSql(ModuleForm moduleForm)
        {
            try
            {
                string query = @"
                    INSERT INTO public.moduleform(
	                formid, moduleid)
	                VALUES (@formid,@moduleid)
                    RETURNING id;
                    ";

                moduleForm.id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    formid = moduleForm.formid,
                    moduleid = moduleForm.moduleid,
                });

                return moduleForm;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el modulo: {ex.Message}");
                throw;
            }
        }

        //Metodo SQL
        public async Task<ModuleForm> CreateAsyncSQL(ModuleForm moduleForm)
        {
            try
            {
                // SQL para insertar en la tabla ModuleForm
                var sql = "INSERT INTO ModuleForm (ModuleId, FormId) " +  // Asegúrate de ajustar los campos a tu tabla
                          "VALUES (@moduleId, @formId);" +
                          "SELECT SCOPE_IDENTITY();"; // Esto devuelve el Id del registro insertado

                // Definir los parámetros para la consulta SQL
                var parametros = new SqlParameter[]
                {
            new SqlParameter("@moduleId", moduleForm.moduleid),  // Cambia Field1, Field2, Field3 por los nombres reales
            new SqlParameter("@formId", moduleForm.formid),
                };

                // Ejecutar la consulta SQL y obtener el Id del nuevo registro
                var result = await _context.Database.ExecuteSqlRawAsync(sql, parametros);

                // Asignar el Id devuelto al objeto moduleForm
                moduleForm.id = Convert.ToInt32(result); // Asumiendo que SCOPE_IDENTITY devuelve un int
                return moduleForm;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el ModuleForm: {ex.Message}");
                throw;
            }
        }



        /// <summary>
        /// Actualiza un ModuleForm existente en la base de datos
        /// </summary>
        /// <param name="moduleForm">objeto con la informacion actualizada </param>
        /// <returns>True si la operaccion fue exitosa, false en caso contrario.</returns>


        public async Task<bool> UpdateAsync(ModuleForm moduleForm)
        {
            try
            {
                _context.Set<ModuleForm>().Update(moduleForm);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el moduloForm: {ex.Message}");
                return false;
            }
        }

        //Metodo SQL
        public async Task<bool> UpdateAsyncSQL(ModuleForm moduleForm)
        {
            try
            {
                // SQL para actualizar el registro en la tabla ModuleForm
                var sql = "UPDATE ModuleForm SET ModuleId = @moduleId, FormId = @formId " +
                          "WHERE Id = @id";

                // Definir los parámetros para la consulta SQL
                var parametros = new SqlParameter[]
                {
            new SqlParameter("@moduleId", moduleForm.moduleid),  // Asignar el nuevo valor para ModuleId
            new SqlParameter("@formId", moduleForm.formid),      // Asignar el nuevo valor para FormId
            new SqlParameter("@id", moduleForm.id)             // Identificar el registro por su Id
                };

                // Ejecutar la consulta SQL
                var result = await _context.Database.ExecuteSqlRawAsync(sql, parametros);

                return result > 0;  // Retorna true si se actualizó al menos una fila
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el ModuleForm: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Elimina un ModuleForm de la base de datos
        /// </summary>
        /// <param name="id">Identificador unico del ModuleForm a eliminar</param>
        /// <returns>true si la eliminacion fue exitosa, false en caso contrario</returns>

        //Metodo Linq
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var moduleForm = await _context.Set<ModuleForm>().FindAsync(id);
                if (moduleForm == null)
                    return false;

                _context.Set<ModuleForm>().Remove(moduleForm);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el ModuleForm: {ex.Message}");
                return false;
            }
        }

        //Metodo sql

        public async Task<bool> DeleteAsyncSQL(int id)
        {
            try
            {
                // SQL para eliminar el registro en la tabla ModuleForm
                var sql = "DELETE FROM ModuleForm WHERE Id = @id";

                // Definir el parámetro para la consulta SQL
                var parametro = new SqlParameter("@id", id);

                // Ejecutar la consulta SQL
                var result = await _context.Database.ExecuteSqlRawAsync(sql, parametro);

                return result > 0;  // Retorna true si se eliminó al menos una fila
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el ModuleForm: {ex.Message}");
                return false;
            }
        }


    }
}

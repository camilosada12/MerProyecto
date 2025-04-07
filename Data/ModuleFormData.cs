
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
        public async Task<IEnumerable<ModuleForm>> GetAllModuleFormAsyncLinq()
        {
            return await _context.Set<ModuleForm>().ToListAsync();
        }

        public async Task<ModuleForm?> GetByIdAsyncLinq(int id)
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
      


        /// <summary>
        /// Actualiza un ModuleForm existente en la base de datos
        /// </summary>
        /// <param name="moduleForm">objeto con la informacion actualizada </param>
        /// <returns>True si la operaccion fue exitosa, false en caso contrario.</returns>


        public async Task<bool> UpdateAsyncLinq(ModuleForm moduleForm)
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
        ///<summary>
        ///Elimina un ModuleForm de la base de datos
        /// </summary>
        /// <param name="id">Identificador unico del ModuleForm a eliminar</param>
        /// <returns>true si la eliminacion fue exitosa, false en caso contrario</returns>

        //Metodo Linq
        public async Task<bool> DeleteAsyncLinq(int id)
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


        //SQL

        //Metodo SQL    
        public async Task<IEnumerable<ModuleForm>> GetAllAsyncSQL()
        {
            string query = "SELECT * FROM moduleForm";
            return (IEnumerable<ModuleForm>)await _context.QueryAsync<IEnumerable<ModuleForm>>(query);
        }

        //Metodo SQL    
        public async Task<ModuleForm?> GetByModuleIdAsyncSQL(int id)
        {
            try
            {
                string query = @"SELECT * FROM public.moduleform WHERE id = @Id";

                return await _context.QueryFirstOrDefaultAsync<ModuleForm>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con el Id {id}", id);
                throw;
            }
        }

        //Metodo SQL
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
        public async Task<bool> UpdateAsyncSQL(ModuleForm moduleForm)
        {
            try
            {
                var sql = "UPDATE moduleform SET moduleid = @ModuleId, formid = @FormId " +
                          "WHERE Id = @id";

                var parameters = new
                {
                    Id = moduleForm.id,
                    ModuleId = moduleForm.moduleid,
                    FormId = moduleForm.formid
                };

                // Ejecutar la consulta SQL
                int rowsAffected = await _context.ExecuteAsync(sql, parameters);

                // Verificar si la actualización afectó alguna fila (es decir, si el module fue actualizado)
                return rowsAffected > 0;  // Si rowsAffected > 0, la actualización fue exitosa
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el ModuleForm: {ex.Message}");
                return false;
            }
        }

        //Metodo sql
        public async Task<bool> DeleteAsyncSQL(int id)
        {
            try
            {
                // SQL para eliminar el registro en la tabla ModuleForm
                var sql = "DELETE FROM moduleform WHERE Id = @id";

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

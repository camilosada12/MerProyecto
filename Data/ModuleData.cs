using Entity.Contexts;
using Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Data
{
    public class ModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ModuleData> _logger;


        /// <summary>
        /// constructor que recibe el contexto de base de datos
        /// </summary>
        /// <param name="context">instacia de <see cref="ApplicationDbContext"/> para la conexion con la base de datos </param>

        public ModuleData(ApplicationDbContext context, ILogger<ModuleData> logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///obtiene todos los Modulos almacenados en la base de datos 
        /// </summary>
        /// <returns>lista de Modulos </returns>

        //Atributo Linq
        public async Task<IEnumerable<Module>> GetAllAsync()
        {
            return await _context.Set<Module>().ToListAsync();
        }

        //Atributo Linq
        public async Task<Module?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Module>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener modulo con Id {Id}", id);
                throw; // Re-lanza la excepcion para que sea manejada en capas superiores
            }
        }

        ///<summary>
        ///crea un nuevo Modulo en la base de datos
        ///</summary>
        ///<param name="module">instancia del Modulo a crear </param>
        ///<returns> El Modulo creado.</returns>

        //Atributo Linq
        public async Task<Module> CreateAsyncLinq(Module module)
        {
            try
            {
                await _context.Set<Module>().AddAsync(module);
                await _context.SaveChangesAsync();
                return module;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Modulo: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un Modulo existente en la base de datos
        /// </summary>
        /// <param name="module">objeto con la informacion actualizada </param>
        /// <returns>True si la operaccion fue exitosa, false en caso contrario.</returns>

        //Atributo Linq
        public async Task<bool> UpdateModuleAsyncLinq(Module module)
        {
            try
            {
                _context.Set<Module>().Update(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Modulo: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Elimina un Modulo de la base de datos
        /// </summary>
        /// <param name="id">Identificador unico del Modulo a eliminar</param>
        /// <returns>true si la eliminacion fue exitosa, false en caso contrario</returns>

        //Atributo Linq
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var Module = await _context.Set<Module>().FindAsync(id);
                if (Module == null)
                    return false;

                _context.Set<Module>().Remove(Module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el Modulo: {ex.Message}");
                return false;
            }
        }

        //SQL

        //Atributo SQL
        public async Task<IEnumerable<Module>> GetAllModuleAsyncSQL()
        {
            const string query = @"SELECT * FROM module WHERE isdelete = false";
            return await _context.Set<Module>().FromSqlRaw(query).ToListAsync();
        }

        //Atributo SQL
        public async Task<Module?> GetByModuleIdAsyncSQL(int id)
        {
            try
            {
                string query = @"SELECT * FROM public.module WHERE id = @Id  AND isdelete = false";

                return await _context.QueryFirstOrDefaultAsync<Module>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con el Id {id}", id);
                throw;
            }
        }

        //Atributo SQL
        public async Task<Module> CreateModuleAsyncSQL(Module module)
        {
            try
            {
                // Forzar valor por defecto en la creación
                module.isdelete = false;

                // SQL para insertar en la tabla Users
                const string sql = @"INSERT INTO public.""module""(
	                    ""name"", ""description"", ""isdelete"")
	                    VALUES (@Name,@Description, @IsDeleted);"
                ;

                module.id = await _context.QueryFirstOrDefaultAsync<int>(sql, new
                {
                    Name = module.name,
                    Description = module.description,
                    IsDeleted = module.isdelete,
                });

                return module;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Module: {ex.Message}");
                throw;
            }
        }

        //Atributo Sql
        public async Task<bool> UpdateAsyncSQL(Module module)
        {
            try
            {
                // Consulta SQL para actualizar el module
                var sql = "UPDATE public.module SET " +
                  "name = @Name, " +
                  "description = @Description " +
                  "WHERE id = @Id;";

                var parameters = new
                {
                    Id = module.id,
                    Name = module.name,
                    Description = module.description
                };

                // Ejecutar la consulta SQL
                int rowsAffected = await _context.ExecuteAsync(sql, parameters);

                // Verificar si la actualización afectó alguna fila (es decir, si el module fue actualizado)
                return rowsAffected > 0;  // Si rowsAffected > 0, la actualización fue exitosa
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el module: {ex.Message}");
                return false;
            }
        }

        //Atributo SQL
        public async Task<bool> DeleteModuleAsyncSQL(int id)
        {
            try
            {
                var sql = "DELETE FROM module WHERE id = @Id";
                var parametro = new NpgsqlParameter("@Id", id);
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parametro);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Modulo: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteLogicoModuleAsyncSQL(int id)
        {
            try
            {
                var sql = "UPDATE module SET isdelete = TRUE WHERE id = @Id";
                var parametro = new NpgsqlParameter("@Id", id);
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parametro);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al realizar delete lógico del module: {ex.Message}");
                return false;
            }
        }
    }
}

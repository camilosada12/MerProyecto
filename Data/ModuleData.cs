using Entity.Contexts;
using Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        //Atributo SQL
        public async Task<IEnumerable<Module>> GetAllAsyncSQL()
        {
            string query = "SELECT * FROM Module";
            return (IEnumerable<Module>)await _context.QueryAsync<IEnumerable<Module>>(query);
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

        //Atributo SQL
        public async Task<Module?> GetByIdAsyncSQL(Module id)
        {
            try
            {
                // Consulta SQL con parámetro para el Id
                var query = "SELECT * FROM Module WHERE Id = @id";

                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@id", id.Id),
                };

                // Ejecutar la consulta y obtener el Id del nuevo Module
                var result = await _context.Database.ExecuteSqlRawAsync(query, parametros);

                // Obtener el Module recién insertado con el Id generado
                id.Id = Convert.ToInt32(result); // Asumimos que SCOPE_IDENTITY devuelve un int
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Module con el Id {Id}", id);
                throw; // Re-lanza la excepción para ser manejada en las capas superiores
            }
        }

        ///<summary>
        ///crea un nuevo Modulo en la base de datos
        ///</summary>
        ///<param name="module">instancia del Modulo a crear </param>
        ///<returns> El Modulo creado.</returns>

        //Atributo Linq
        public async Task<Module> CreateAsync(Module module)
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

        //Atributo SQL
        public async Task<Module> CreateAsyncSQL(Module module)
        {
            try
            {
                // SQL para insertar en la tabla Users
                var sql = "INSERT INTO Users (Name, description, statu) " +
                          "VALUES (@Name, @Description, @Statu);" +
                          "SELECT SCOPE_IDENTITY();"; 

                // Ejecutar el comando SQL
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@Name", module.Name),
                    new SqlParameter("@Description", module.description),
                    new SqlParameter("@Statu", module.statu),
                };

                // Ejecutar la consulta y obtener el Id del nuevo Module
                var result = await _context.Database.ExecuteSqlRawAsync(sql, parametros);

                // Obtener el Module recién insertado con el Id generado
                module.Id = Convert.ToInt32(result); // Asumimos que SCOPE_IDENTITY devuelve un int
                return module;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Module: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Actualiza un Modulo existente en la base de datos
        /// </summary>
        /// <param name="module">objeto con la informacion actualizada </param>
        /// <returns>True si la operaccion fue exitosa, false en caso contrario.</returns>

        //Atributo Linq
        public async Task<bool> UpdateAsync(Module module)
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

        //Atributo Sql
        public async Task<bool> UpdateAsyncSQL(Module module)
        {
            try
            {
                // Consulta SQL para actualizar el module
                var sql = "UPDATE Users SET " +
                          "name = @Name, " +
                          "description = @Description, " +
                          "statu = @Statu, " +
                          "WHERE Id = @Id";  // Asegúrate de que la columna 'Id' es la clave primaria

                // Parámetros SQL que se pasan a la consulta
                var parametros = new SqlParameter[]
                {
                  new SqlParameter("@Name", module.Name),
                  new SqlParameter("@description", module.description),
                  new SqlParameter("@Statu", module.statu),
                  new SqlParameter("@Id", module.Id) // El ID es necesario para la condición WHERE
                };

                // Ejecutar la consulta SQL
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parametros);

                // Verificar si la actualización afectó alguna fila (es decir, si el module fue actualizado)
                return rowsAffected > 0;  // Si rowsAffected > 0, la actualización fue exitosa
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el module: {ex.Message}");
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

        //Atributo SQL

        public async Task<bool> DeleteAsyncSQL(int id)
        {
            try
            {
                // Consulta SQL para eliminar el Module con el Id proporcionado
                var sql = "DELETE FROM Module WHERE Id = @Id";  // Asegúrate de que 'Id' es la columna correcta para identificar al usuario

                // Parámetro SQL que se pasa a la consulta
                var parametro = new SqlParameter("@Id", id);  // El Id es el único parámetro en este caso

                // Ejecutar la consulta SQL para eliminar el Module
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parametro);

                // Verificar si se eliminó alguna fila (es decir, si el v fue encontrado y eliminado)
                return rowsAffected > 0;  // Si rowsAffected > 0, la eliminación fue exitosa
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el Module: {ex.Message}");
                return false;
            }
        }

    }
}



using Entity.Contexts;
using Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestion de la entiedad Rol en la base de datos 
    /// </summary>
    public class RolData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;


        /// <summary>
        /// constructor que recibe el contexto de base de datos
        /// </summary>
        /// <param name="context">instacia de <see cref="ApplicationDbContext"/> para la conexion con la base de datos </param>

        public RolData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///obtiene todos los roles almacenados en la base de datos 
        /// </summary>
        /// <returns>lista de roles </returns>

        // Atributo para linq
        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            return await _context.Set<Rol>().ToListAsync();

        }

        // Atributo para SQL
        public async Task<IEnumerable<Rol>> GetAllAsyncSQL()
        {

            string query = "SELECT * FROM Rol";
            return (IEnumerable<Rol>)await _context.QueryAsync<IEnumerable<Rol>>(query);
        }

        //Atributo Linq
        public async Task<Rol?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Rol>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol con Id {RolId}", id);
                throw; // Re-lanza la excepcion para que sea manejada en capas superiores
            }
        }

        //Atributo SQL
        public async Task<Rol?> GetByIdAsyncSQL(Rol id)
        {
            try
            {
                // Consulta SQL con parámetro para el Id
                var query = "SELECT * FROM Rol WHERE Id = @id";  // Asegúrate de que el nombre de la tabla sea correcto

                var parametrosRol = new SqlParameter[]
                {
                    new SqlParameter("@id", id.Id),
                };

                // Ejecutar la consulta y obtener el Id del nuevo usuario
                var result = await _context.Database.ExecuteSqlRawAsync(query, parametrosRol);

                // Obtener el usuario recién insertado con el Id generado
                id.Id = Convert.ToInt32(result); // Asumimos que SCOPE_IDENTITY devuelve un int
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Usuario con el Id {Id}", id);
                throw; // Re-lanza la excepción para ser manejada en las capas superiores
            }
        }

        ///<summary>
        ///crea un nuevo rol en la base de datos
        ///</summary>
        ///<param name="rolData">instancia del rol a crear </param>
        ///<returns> El rol creado.</returns>

        //Atributo Linq
        public async Task<Rol> CreateAsync(Rol rolData)
        {
            try
            {
                await _context.Set<Rol>().AddAsync(rolData);
                await _context.SaveChangesAsync();
                return rolData;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el rol: {ex.Message}");
                throw;
            }
        }

        //Atributo SQL 
        public async Task<Rol> CreateAsyncSQL(Rol rol)
        {
            try
            {
                // SQL para insertar en la tabla Users
                var query = "INSERT INTO Rol (Role , Description) " +
                          "VALUES (@role, @description);"; // Esto devuelve el Id del usuario insertado

                // Ejecutar el comando SQL
                var parametrosRol = new SqlParameter[]
                {
                    new SqlParameter("@role", rol.Role),
                    new SqlParameter("@description", rol.Description),
                };

                // Ejecutar la consulta y obtener el Id del nuevo usuario
                var result = await _context.Database.ExecuteSqlRawAsync(query, parametrosRol);

                // Obtener el usuario recién insertado con el Id generado
                rol.Id = Convert.ToInt32(result); // Asumimos que SCOPE_IDENTITY devuelve un int
                return rol;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el usuario: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un rol existente en la base de datos
        /// </summary>
        /// <param name="rolData">objeto con la informacion actualizada </param>
        /// <returns>True si la operaccion fue exitosa, false en caso contrario.</returns>

        //Atributo Linq
        public async Task<bool> UpdateAsync(Rol rol)
        {
            try
            {
                _context.Set<Rol>().Update(rol);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol: {ex.Message}");
                return false;
            }
        }

        //Atributo SQL
        public async Task<bool> UpdateAsyncSql(Rol rol)
        {
            try
            {
                // SQL para actualizar el rol en la tabla Rol
                var query = "UPDATE Rol " +
                    "       SET Role = @role," +
                    "       Description = @description " +
                    "       WHERE Id = @id";

                // Parámetros SQL que se pasan a la consulta
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@role", rol.Role),
                    new SqlParameter("@description", rol.Description),
                    new SqlParameter("@id", rol.Id) // Se usa el Id para especificar cuál rol se actualiza
                };

                // Ejecutar la consulta SQL para actualizar el rol
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(query, parametros);

                // Verificar si la actualización afectó alguna fila (es decir, si el rol fue actualizado)
                return rowsAffected > 0;  // Si rowsAffected > 0, la actualización fue exitosa
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol: {ex.Message}");
                throw;
            }
        }


        ///<summary>
        ///Elimina un rol de la base de datos
        /// </summary>
        /// <param name="id">Identificador unico del rol a eliminar</param>
        /// <returns>true si la eliminacion fue exitosa, false en caso contrario</returns>

        //Atributo Linq
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rolData = await _context.Set<Rol>().FindAsync(id);
                if (rolData == null)
                    return false;

                _context.Set<Rol>().Remove(rolData);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el rol: {ex.Message}");
                return false;
            }
        }

        //Atributo SQL
        public async Task<bool> DeleteAsyncSql(int id)
        {
            try
            {
                // SQL para eliminar el rol de la tabla Rol
                var query = "DELETE FROM Rol WHERE Id = @id";  // Consulta para eliminar el rol con el Id proporcionado

                // Parámetro SQL que se pasa a la consulta
                var parametro = new SqlParameter("@id", id);  // El Id del rol a eliminar

                // Ejecutar la consulta SQL para eliminar el rol
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(query, parametro);

                // Verificar si la eliminación afectó alguna fila (es decir, si el rol fue eliminado)
                return rowsAffected > 0;  // Si rowsAffected > 0, la eliminación fue exitosa
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el rol: {ex.Message}");
                return false;
            }
        }


    }
}
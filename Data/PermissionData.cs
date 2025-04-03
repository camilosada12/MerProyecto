
using Entity.Contexts;
using Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class PermissionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PermissionData> _logger;

        ///<summary>
        ///constructor que recibe el contexto de la base de datos
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexion con la base de datos</param>

        public PermissionData(ApplicationDbContext context, ILogger<PermissionData> logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///obtiene todos los permisos almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de permisos</returns>

        //Atributo Linq
        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            return await _context.Set<Permission>().ToListAsync();
        }

        //Atributo SQL
        public async Task<IEnumerable<Permission>> GetAllAsyncSQL()
        {
            string query = "SELECT * FROM Permission";
            return (IEnumerable<Permission>)await _context.QueryAsync<IEnumerable<Permission>>(query);
        }

        //Atributo Linq
        public async Task<Permission?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Permission>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permission con ID {PermissionId}", id);
                throw; // Re-lanza la excepcion para que sea manejada en capas superiores
            }
        }

        //Atributo SQL
        public async Task<Permission?> GetByIdAsyncSQL(Permission id)
        {
            try
            {
                // Consulta SQL con parámetro para el Id
                var query = "SELECT * FROM Permission WHERE Id = @id";  // Asegúrate de que el nombre de la tabla sea correcto

                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@id", id.Id),
                };

                // Ejecutar la consulta y obtener el Id del nuevo Permission
                var result = await _context.Database.ExecuteSqlRawAsync(query, parametros);

                // Obtener el Permission recién insertado con el Id generado
                id.Id = Convert.ToInt32(result); // Asumimos que SCOPE_IDENTITY devuelve un int
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Permission con el Id {Id}", id);
                throw; // Re-lanza la excepción para ser manejada en las capas superiores
            }
        }

        ///<summary>
        ///crea un nuevo permission en la base de datos.
        /// </summary>
        /// <param name="permission">instancia del permission a crear</param>
        /// <returns>el Permission creado</returns>

        //Atributo Linq
        public async Task<Permission> CreateAsync(Permission permission)
        {
            try
            {
                await _context.Set<Permission>().AddAsync(permission);
                await _context.SaveChangesAsync();
                return permission;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Permission: {ex.Message}");
                throw;
            }
        }

        //Atributo SQL
        public async Task<Permission> CreateAsyncSQL(Permission permission)
        {
            try
            {
                // SQL para insertar en la tabla Users
                var sql = "INSERT INTO Permission (Name, Description) " +
                          "VALUES (@name, @description);" +
                          "SELECT SCOPE_IDENTITY();"; // Esto devuelve el Id del usuario insertado

                // Ejecutar el comando SQL
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@name", permission.Name),
                    new SqlParameter("@description", permission.Description),
                };

                // Ejecutar la consulta y obtener el Id del nuevo usuario
                var result = await _context.Database.ExecuteSqlRawAsync(sql, parametros);

                // Obtener el usuario recién insertado con el Id generado
                permission.Id = Convert.ToInt32(result); // Asumimos que SCOPE_IDENTITY devuelve un int
                return permission;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el usuario: {ex.Message}");
                throw;
            }
        }

        ///<summary>
        ///Actualiza los permission  en la base de datos.
        /// </summary>
        /// <param name="permission">objeto con la informacion actualizada</param>
        /// <returns>True si la operacion es exitosa, false en caso contrario</returns>

        //Atributo Linq
        public async Task<bool> UpdateAsync(Permission permission)
        {
            try
            {
                _context.Set<Permission>().Update(permission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el permission: {ex.Message}");
                return false;
            }
        }

        //Atributo SQL
        public async Task<bool> UpdateAsyncSQL(Permission permission)
        {
            try
            {
                // Consulta SQL para actualizar el usuario
                var sql = "UPDATE Users SET " +
                          "Name = @name, " +
                          "Description = @description, " +
                          "WHERE Id = @Id";  // Asegúrate de que la columna 'Id' es la clave primaria

                // Parámetros SQL que se pasan a la consulta
                var parametros = new SqlParameter[]
                {
                      new SqlParameter("@name", permission.Name),
                      new SqlParameter("@description", permission.Description),
                      new SqlParameter("@Id", permission.Id) // El ID es necesario para la condición WHERE
                };

                // Ejecutar la consulta SQL
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parametros);

                // Verificar si la actualización afectó alguna fila (es decir, si el usuario fue actualizado)
                return rowsAffected > 0;  // Si rowsAffected > 0, la actualización fue exitosa
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el usuario: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///elimina un permission en la base de datos.
        /// </summary>
        /// <param name="id">Identificador unico del permission a eliminar</param>
        /// <returns>True si la eliminacion fue exitosa, false en caso contrario</returns>

        //Atributo Linq
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var permission = await _context.Set<Permission>().FindAsync(id);
                if (permission == null)
                    return false;

                _context.Set<Permission>().Remove(permission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el permission: {ex.Message}");
                return false;
            }
        }

        //Atributo SQL
        public async Task<bool> DeleteAsyncSQL(int id)
        {
            try
            {
                // Consulta SQL para eliminar el usuario con el Id proporcionado
                var sql = "DELETE FROM Permission WHERE Id = @Id";  // Asegúrate de que 'Id' es la columna correcta para identificar al usuario

                // Parámetro SQL que se pasa a la consulta
                var parametro = new SqlParameter("@Id", id);  // El Id es el único parámetro en este caso

                // Ejecutar la consulta SQL para eliminar el usuario
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parametro);

                // Verificar si se eliminó alguna fila (es decir, si el usuario fue encontrado y eliminado)
                return rowsAffected > 0;  // Si rowsAffected > 0, la eliminación fue exitosa
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el usuario: {ex.Message}");
                return false;
            }
        }

    }
}

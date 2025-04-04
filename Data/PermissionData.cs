
using Entity.Contexts;
using Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

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
            string query = "SELECT * FROM permission";
            return await _context.Set<Permission>().FromSqlRaw(query).ToListAsync();
        }

        //Atributo Linq
        public async Task<Permission?> GetPermissionByIdAsync(int id)
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
        public async Task<Permission?> GetByPermissionIdAsyncSQL(int id)
        {
            try
            {
                var query = "SELECT * FROM permission WHERE id = @Id";  // "id" en minúsculas
                var parametro = new NpgsqlParameter("@Id", id);

                return await _context.Set<Permission>().FromSqlRaw(query, parametro).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Permission con el Id {id}", id);
                throw;
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
        public async Task<Permission> CreatePermissionAsyncSQL(Permission permission)
        {
            try
            {
                const string query = @"
                    INSERT INTO public.""permission""(
	                ""name"", ""description"")
	                VALUES (@Name,@Description);
                    RETURNING id;";

                permission.id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    Name = permission.name,
                    Description = permission.description,
                });

                return permission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo agregar el Permission.");
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
                // Consulta SQL para actualizar el Permission
                var sql = "UPDATE permission SET " +
                          "Name = @name, " +
                          "Description = @description, " +
                          "WHERE Id = @Id";  // Asegúrate de que la columna 'Id' es la clave primaria

                // Parámetros SQL que se pasan a la consulta
                var parametros = new SqlParameter[]
                {
                      new SqlParameter("@name", permission.name),
                      new SqlParameter("@description", permission.description),
                      new SqlParameter("@Id", permission.id) // El ID es necesario para la condición WHERE
                };

                // Ejecutar la consulta SQL
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parametros);

                // Verificar si la actualización afectó alguna fila (es decir, si el Permission fue actualizado)
                return rowsAffected > 0;  // Si rowsAffected > 0, la actualización fue exitosa
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Permission: {ex.Message}");
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
        public async Task<bool> DeletePermissionAsyncSQL(int id)
        {
            try
            {
                var sql = "DELETE FROM permission WHERE id = @Id";
                var parametro = new NpgsqlParameter("@Id", id);
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parametro);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Permission: {ex.Message}");
                return false;
            }
        }

    }
}

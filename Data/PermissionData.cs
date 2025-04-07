
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
        public async Task<IEnumerable<Permission>> GetAllAsyncLinq()
        {
            return await _context.Set<Permission>().ToListAsync();
        }

        //Atributo Linq
        public async Task<Permission?> GetPermissionByIdAsyncLinq(int id)
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

        ///<summary>
        ///crea un nuevo permission en la base de datos.
        /// </summary>
        /// <param name="permission">instancia del permission a crear</param>
        /// <returns>el Permission creado</returns>

        //Atributo Linq
        public async Task<Permission> CreateAsyncLinq(Permission permission)
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

        ///<summary>
        ///Actualiza los permission  en la base de datos.
        /// </summary>
        /// <param name="permission">objeto con la informacion actualizada</param>
        /// <returns>True si la operacion es exitosa, false en caso contrario</returns>

        //Atributo Linq
        public async Task<bool> UpdateAsyncLinq(Permission permission)
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

        ///<summary>
        ///elimina un permission en la base de datos.
        /// </summary>
        /// <param name="id">Identificador unico del permission a eliminar</param>
        /// <returns>True si la eliminacion fue exitosa, false en caso contrario</returns>

        //Atributo Linq
        public async Task<bool> DeleteAsyncLinq(int id)
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

        //SQL

        //Atributo SQL
        public async Task<IEnumerable<Permission>> GetAllAsyncSQL()
        {
            const string query = @"SELECT * FROM permission WHERE isdelete = false";
            return await _context.Set<Permission>().FromSqlRaw(query).ToListAsync();
        }

        //Atributo SQL
        public async Task<Permission?> GetByPermissionIdAsyncSQL(int id)
        {
            try
            {
                string query = @"SELECT * FROM public.permission WHERE id = @Id AND isdelete = false";

                return await _context.QueryFirstOrDefaultAsync<Permission>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Permission con el Id {id}", id);
                throw;
            }
        }

        //Atributo SQL
        public async Task<Permission> CreatePermissionAsyncSQL(Permission permission)
        {
            // Forzar valor por defecto en la creación
            permission.isdelete = false;

            try
            {
                const string query = @"
                    INSERT INTO public.permission(
	                name, description, isdelete)
	                VALUES (@Name,@Description, @IsDelete)
                    RETURNING id;";

                permission.id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    Name = permission.name,
                    Description = permission.description,
                    IsDelete = permission.isdelete,
                });

                return permission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo agregar el Permission.");
                throw;
            }
        }

        //Atributo SQL
        public async Task<bool> UpdateAsyncSQL(Permission permission)
        {
            try
            {
                // Consulta SQL para actualizar el Permission
                var sql = "UPDATE permission SET " +
                          "name = @Name, " +
                          "description = @Description " +
                          "WHERE id = @Id";  // Asegúrate de que la columna 'Id' es la clave primaria

                var parameters = new
                {
                    Id = permission.id,
                    Name = permission.name,
                    Description = permission.description
                };

                // Ejecutar la consulta SQL
                int rowsAffected = await _context.ExecuteAsync(sql, parameters);

                return rowsAffected > 0;  
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Permission: {ex.Message}");
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

        public async Task<bool> DeleteLogicoPermissionAsyncSQL(int id)
        {
            try
            {
                var sql = "UPDATE permission SET isdelete = TRUE WHERE id = @Id";
                var parametro = new NpgsqlParameter("@Id", id);
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parametro);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al realizar delete lógico del form: {ex.Message}");
                return false;
            }
        }


    }
}

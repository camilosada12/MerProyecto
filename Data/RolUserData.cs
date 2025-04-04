using Entity.Contexts;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;


namespace Data
{
    /// <summary>
    /// repositorio encargado  de la gestion de la entidad RolUser en la base de datos 
    /// </summary>
    public class RolUserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolUserData> _logger;

        ///<summary>
        ///constructor que recibe el contexto de la base de datos
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexion con la base de datos</param>

        public RolUserData(ApplicationDbContext context, ILogger<RolUserData> logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///obtiene todos los RolUser almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de RolUser</returns>

        /// <summary>
        /// Obtiene todos los usuarios almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de Rol con los usuarios</returns>
        /// 
        public async Task<IEnumerable<RolUser>> GetAllRolUserAsync()
        {
            return await _context.Set<RolUser>().ToListAsync();
        }

        //Atributo SQL
        public async Task<RolUserDto?> GetAllAsyncLinqSQL(int id)
        {
            try
            {
                string query = @"
                        SELECT 
                            ru.Id, 
                            ru.RolId, 
                            r.Nombre AS RolName, 
                            ru.UserId, 
                            u.Nombre AS UserName
                        FROM RolUser ru
                        INNER JOIN Rol r ON ru.RolId = r.Id
                        INNER JOIN [User] u ON ru.UserId = u.Id
                        WHERE ru.Id = @Id;";

                var parameters = new { Id = id };

                return await _context.QueryFirstOrDefaultAsync<RolUserDto>(query, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el RolUser con ID {RolUserId}", id);
                throw;
            }
        }

        /// <summary>
        /// Obtiene un usuario con un rol especificos por su identificador CON linQ
        /// </summary>
        public async Task<RolUser?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<RolUser>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con su rol con ID {UserRolId}", id);
                throw;//Re-lanza la excepcion para sea manejada en capas superiores
            }
        }

        //Metodo SQL
        public async Task<RolUser?> GetByIdAsyncSQL(int id)
        {
            try
            {
                string query = @"
                       SELECT 
                            ru.Id AS RolUserId, 
                            ru.RolId, 
                            r.role AS RolName, 
                            ru.UserId, 
                            u.user_per AS UserName
                        FROM RolUser ru
                        INNER JOIN rol r ON ru.RolId = r.id
                        INNER JOIN usermulta u ON ru.UserId = u.id
                        WHERE ru.Id = @Id;";

                var parameters = new { Id = id };

                return await _context.QueryFirstOrDefaultAsync<RolUser>(query, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener RolUser con ID {RolUserId}", id);
                throw;
            }
        }

        ///<summary>
        ///crea un nuevo RolUser en la base de datos.
        /// </summary>
        /// <param name="rolUser">instancia del RolUser a crear</param>
        /// <returns>el RolUser creado</returns>

        public async Task<RolUser> CreateAsync(RolUser rolUser)
        {
            try
            {
                await _context.Set<RolUser>().AddAsync(rolUser);
                await _context.SaveChangesAsync();

                // Retornar directamente el objeto creado sin recargar sus relaciones
                return rolUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el RolUser");
                throw;
            }
        }

        //Metodo SQL
        /// <summary>
        /// Actualiza un usuario con su rol existente en la base de datos
        /// </summary>
        /// <param name="user">Objeto con la informacion actualizada</param>
        /// <returns>True si la operacion fue exitosa, False en caso contrario</returns>
        /// 

        //Metodo para crear un nuevo rol user con sentencia SQl

        public async Task<RolUser> CreateAsyncSql(RolUser rolUsers)
        {
            try
            {
                string query = @"
                    INSERT INTO public.roluser(
	                 rolid, userid)
	                VALUES (@RolId,@UserId)
                    RETURNING id;
                    ";

                rolUsers.id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    RolId = rolUsers.rolid,
                    UserId = rolUsers.userid,
                });

                return rolUsers;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el modulo: {ex.Message}");
                throw;
            }
        }

        ///<summary>
        ///Actualiza un nuevo RolUser en la base de datos.
        /// </summary>
        /// <param name="rolUser">objeto con la informacion actualizada</param>
        /// <returns>True si la operacion es exitosa, false en caso contrario</returns>

        //Metodo linq
        public async Task<bool> UpdateAsync(RolUser rolUser)
        {
            try
            {
                _context.Set<RolUser>().Update(rolUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el RolUser: {ex.Message}");
                return false;
            }
        }

        //Metodo SQL

        public async Task<bool> UpdateAsyncSQL(RolUser rolUser)
        {
            try
            {
                // Consulta SQL de actualización
                var sql = @"
                    UPDATE public.roluser
                    SET 
                    rolid = @rolid,
                    userid = @userid
                    WHERE id = @id;";


                var parameters = new
                {
                    rolUser.id,
                    rolUser.rolid,
                    rolUser.userid,
                };
                int rowsAffected = await _context.ExecuteAsync(sql, parameters);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Registrar el error en caso de excepción
                _logger.LogError(ex, $"Error al actualizar el rolUser con ID {rolUser.id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el RolUser con ID {rolUser.id}", ex);
            }
        }


        ///<summary>
        ///elimina un nuevo RolUser en la base de datos.
        /// </summary>
        /// <param name="id">Identificador unico del RolUser a eliminar</param>
        /// <returns>True si la eliminacion fue exitosa, false en caso contrario</returns>

        //Metodo Linq
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rolUser = await _context.Set<RolUser>().FindAsync(id);
                if (rolUser == null)
                    return false;

                _context.Set<RolUser>().Remove(rolUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el RolUser: {ex.Message}");
                return false;
            }
        }

        //Metodo Sql

        public async Task<bool> DeleteAsyncSQL(int id)
        {
            try
            {
                // SQL para eliminar el registro en la tabla RolUser
                var sql = "DELETE FROM RolUser WHERE Id = @id";

                // Definir el parámetro para la consulta SQL
                var parametro = new SqlParameter("@id", id);

                // Ejecutar la consulta SQL
                var result = await _context.Database.ExecuteSqlRawAsync(sql, parametro);

                return result > 0;  // Retorna true si se eliminó al menos una fila
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el RolUser: {ex.Message}");
                return false;
            }
        }

    }
}


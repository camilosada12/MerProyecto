using Entity.Contexts;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Data
{
    /// <summary>
    /// repositorio encargado  de la gestion de la entidad RolUser en la base de datos 
    /// </summary>
    public class RolUserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        ///<summary>
        ///constructor que recibe el contexto de la base de datos
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexion con la base de datos</param>

        public RolUserData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///obtiene todos los RolUser almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de RolUser</returns>

        //Atributo Linq
        public async Task<IEnumerable<RolUser>> GetAllAsync()
        {
            try
            {
                return await _context.Set<RolUser>()
                    .Include(f => f.Rol)  // Relación con Rol
                    .Include(f => f.User) // Relación con User
                    .ToListAsync();  // Devuelve la lista de todos los RolUser
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de RolUsers");
                throw;
            }
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

        //Metodo Linq
        public async Task<RolUser?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<RolUser>()
                    .Include(f => f.Rol)  // Carga la relación con Rol
                    .Include(f => f.User) // Carga la relación con User
                    .FirstOrDefaultAsync(f => f.Id == id); // Busca por ID
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener RolUser con ID {RolUserId}", id);
                throw; // Relanza la excepción para que sea manejada en capas superiores
            }
        }

        //Metodo SQL
        public async Task<RolUserDto?> GetByIdAsyncSQL(int id)
        {
            try
            {
                string query = @"
                        SELECT 
                            ru.Id AS RolUserId, 
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
        public async Task<RolUser> CreateAsyncSQL(RolUser rolUser)
        {
            try
            {
                // Consulta SQL para insertar con parámetros
                var sql = @"
            INSERT INTO RolUsers (RolId, UserId) 
            VALUES (@RolId, @UserId);
        ";

            // Definir los parámetros para evitar inyecciones SQL
                var parameters = new[]
                {
                new SqlParameter("@RolId", rolUser.RolId),
                new SqlParameter("@UserId", rolUser.UserId)
                };  

                // Ejecutar la consulta SQL y obtener el ID insertado
                var newId = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                // Retornar el objeto creado con el ID asignado
                return rolUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el RolUser");
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
                // Consulta SQL para actualizar la entidad RolUser con parámetros
                var sql = @"
            UPDATE RolUsers
            SET 
                RolId = @RolId, 
                UserId = @UserId
            WHERE Id = @Id;
        ";

                // Definir los parámetros para evitar inyecciones SQL
                var parameters = new[]
                {
            new SqlParameter("@RolId", rolUser.RolId),
            new SqlParameter("@UserId", rolUser.UserId),
            new SqlParameter("@Id", rolUser.Id)
        };

                // Ejecutar la consulta SQL
                await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el RolUser: {ex.Message}");
                return false;
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


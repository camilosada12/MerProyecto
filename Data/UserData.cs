

using Entity.Contexts;
using Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    ///<summary>
    ///repositorio encargado de la gestion de la entidad User en la base de datos 
    /// </summary>
    public class UserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        ///<summary>
        ///constructor que recibe el contexto de base de datos
        ///</summary>
        ///<param name="context"> Instancia de <see cref="ApplicationDbContext"/>para la conexion con la base de datos</param>

        public UserData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///obtiene todos los usuarios almacenados en la base de datos
        /// </summary>
        /// <returns>Lista de usuarios</returns>

        //Atributo con linq
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Set<User>().ToListAsync();
        }

        //Atributo con SQL
        public async Task<IEnumerable<User>> GetAllAsyncSQL()
        {
            string query = "SELECT * FROM User";
            return (IEnumerable<User>)await _context.QueryAsync<IEnumerable<User>>(query);
        }

        //Atributo Linq
        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<User>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Usuario con el Id {UserId}", id);
                throw; // Re-Lanza la excepcion para que sea manejada en capas superiores
            }
        }

        //Atributo SQL

        public async Task<User?> GetByIdAsyncSQL(User id)
        {
            try
            {
                // Consulta SQL con parámetro para el Id
                var query = "SELECT * FROM User WHERE Id = @id"; 

                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@id", id.Id),
                };

                // Ejecutar la consulta y obtener el Id del nuevo Usuario
                var result = await _context.Database.ExecuteSqlRawAsync(query, parametros);

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
        ///crea un nuevo Usuario en la base de datos
        ///</summary>
        /// <param name="user">instancia el usuario al crear</param>
        /// <returns>el usuario creado</returns>

        //Atributo Linq
        public async Task<User> CreateAsync(User user)
        {
            try
            {
                object value = await _context.Set<User>().AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error el crear el usuario {ex.Message}");
                throw;
            }
        }

        //Atributo sql 

        public async Task<User> CreateAsyncSQL(User user)
        {
            try
            {
                // SQL para insertar en la tabla Users
                var sql = "INSERT INTO Users (UserName, Password, gmail, RegistrationDate, UserNotificationId) " +
                          "VALUES (@UserName, @Password, @Gmail, @RegistrationDate);"+
                          "SELECT SCOPE_IDENTITY();"; // Esto devuelve el Id del usuario insertado

                // Ejecutar el comando SQL
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@UserName", user.UserName),
                    new SqlParameter("@Password", user.Password),
                    new SqlParameter("@Gmail", user.gmail),
                    new SqlParameter("@RegistrationDate", user.RegistrationDate),
                };

                // Ejecutar la consulta y obtener el Id del nuevo usuario
                var result = await _context.Database.ExecuteSqlRawAsync(sql, parametros);

                // Obtener el usuario recién insertado con el Id generado
                user.Id = Convert.ToInt32(result); 
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el usuario: {ex.Message}");
                throw;
            }
        }


        ///<summary>
        ///Actualiza un nuevo Usuario en la base de datos
        ///</summary>
        /// <param name="user">instancia el usuario al crear</param>
        /// <returns>true si la operacion fue exitosa, false en caso contrario</returns>

        //Atributo Linq
        public async Task<bool> UpdateAsync(User user)
        {
            try
            {
                _context.Set<User>().Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error el Actualizar al usuario {ex.Message}");
                return false;
            }
        }

        //Atributo sql 

        public async Task<bool> UpdateAsyncSQL(User user)
        {
            try
            {
                // Consulta SQL para actualizar el usuario
                var sql = "UPDATE Users SET " +
                          "UserName = @UserName, " +
                          "Password = @Password, " +
                          "gmail = @Gmail, " +
                          "RegistrationDate = @RegistrationDate, " +
                          "WHERE Id = @Id";  // Asegúrate de que la columna 'Id' es la clave primaria

                // Parámetros SQL que se pasan a la consulta
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@UserName", user.UserName),
                    new SqlParameter("@Password", user.Password),
                    new SqlParameter("@Gmail", user.gmail),
                    new SqlParameter("@RegistrationDate", user.RegistrationDate),
                    new SqlParameter("@Id", user.Id) // El ID es necesario para la condición WHERE
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
        ///Elimina un  Usuario en la base de datos
        ///</summary>
        /// <param name="id">Identificador unico del usuario a eliminar</param>
        /// <returns>true si la eliminacion fue exitosa, false en caso contrario</returns>

        //Atributo linq
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var usuario = await _context.Set<User>().FindAsync(id);
                if (usuario == null)
                    return false;

                _context.Set<User>().Remove(usuario);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error el Eliminar al usuario {ex.Message}");
                return false;
            }
        }

        //Atributo sql

        public async Task<bool> DeleteAsyncSQL(int id)
        {
            try
            {
                // Consulta SQL para eliminar el usuario con el Id proporcionado
                var sql = "DELETE FROM Users WHERE Id = @Id";  // Asegúrate de que 'Id' es la columna correcta para identificar al usuario

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

using Dapper;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;
using Utilities.Exceptions;

namespace Data
{
    public class UserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserData> _logger;

        public UserData(ApplicationDbContext context, ILogger<UserData> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Set<User>().ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsyncSQL()
        {
            string query = "SELECT * FROM usermulta";
            return await _context.Set<User>().FromSqlRaw(query).ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<User>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con el Id {UserId}", id);
                throw;
            }
        }

        public async Task<User?> GetByIdAsyncSQL(int id)
        {
            try
            {
                var query = "SELECT * FROM usermulta WHERE id = @Id";  // "id" en minúsculas
                var parametro = new NpgsqlParameter("@Id", id);

                return await _context.Set<User>().FromSqlRaw(query, parametro).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con el Id {id}", id);
                throw;
            }
        }

        public async Task<User> CreateAsync(User user)
        {
            try
            {
                await _context.Set<User>().AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el usuario {ex.Message}");
                throw;
            }
        }

        public async Task<User> CreateAsyncSQL(User user)
        {
            try
            {
                const string query = @"
                    INSERT INTO public.""usermulta""(""user_per"", ""password"", ""gmail"", ""registrationdate"")
                    VALUES (@UserPer, @Password, @Gmail, @RegistrationDate)
                    RETURNING id;";

                var parameters = new
                {
                    UserPer = user.user_per,
                    Password = user.password,  
                    Gmail = user.gmail,
                    RegistrationDate = user.registrationdate
                };

                using (var connection = _context.Database.GetDbConnection())
                {
                    if (connection.State == ConnectionState.Closed)
                        await connection.OpenAsync();

                    user.id = await connection.ExecuteScalarAsync<int>(query, parameters);

                    await connection.CloseAsync(); // Cerramos la conexión
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo agregar el usuario.");
                throw;
            }
        }
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
                _logger.LogError($"Error al actualizar el usuario {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsyncSQL(User user)
        {
            try
            {
                var sql = @"
            UPDATE public.usermulta
            SET user_per = @UserPer, 
                password = @Password, 
                gmail = @Gmail, 
                registrationdate = @RegistrationDate
            WHERE id = @Id;";

                var parametros = new[]
                {
            new NpgsqlParameter("@UserPer", user.user_per),
            new NpgsqlParameter("@Password", user.password),
            new NpgsqlParameter("@Gmail", user.gmail),
            new NpgsqlParameter("@RegistrationDate", user.registrationdate), // Asegúrate de que el tipo de dato sea DateTime
            new NpgsqlParameter("@Id", user.id) // id en minúscula según la tabla
        };

                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parametros);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el usuario con ID {user.id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el usuario con ID {user.id}", ex);
            }
        }



        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var usuario = await _context.Set<User>().FindAsync(id);
                if (usuario == null) return false;

                _context.Set<User>().Update(usuario);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el usuario con ID {id}");
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el usuario con ID {id}", ex);
            }
        }

        public async Task<bool> DeleteAsyncSQL(int id)
        {
            try
            {
                var sql = "DELETE FROM usermulta WHERE id = @Id";
                var parametro = new NpgsqlParameter("@Id", id);
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parametro);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el usuario: {ex.Message}");
                return false;
            }
        }

    }
}
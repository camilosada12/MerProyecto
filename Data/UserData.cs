using Dapper;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using Npgsql;
using System.Data;
using Utilities.Exceptions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        //DTO
        public async Task<IEnumerable<User>> GetAllAsyncLinq()
        {
            return await _context.Set<User>().ToListAsync();
        }

        public async Task<User?> GetByIdAsyncLinq(int id)
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

        public async Task<User> CreateAsyncLinq(User user)
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

        public async Task<bool> UpdateAsyncLinq(User user)
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

        public async Task<bool> DeleteAsyncLinq(int id)
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


        //SQL

        public async Task<IEnumerable<User>> GetAllAsyncSQL()
        {
            const string query = @"SELECT * FROM usermulta WHERE isdelete = false";
            return await _context.QueryAsync<User>(query);
        }

        public async Task<User?> GetByIdAsyncSQL(int id)
        {
            try
            {
                string query = @"SELECT * FROM public.usermulta WHERE id = @Id AND isdelete = false";

                return await _context.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con el Id {id}", id);
                throw;
            }
        }

        public async Task<User> CreateAsyncSQL(User user)
        {
            try
            {
                const string query = @"
                    INSERT INTO public.usermulta(user_per, password, gmail, isdelete)
                    VALUES (@UserPer, @Password, @Gmail, @IsDelete)
                    RETURNING id;;";

                user.id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    UserPer = user.user_per,
                    Password = user.password,  
                    Gmail = user.gmail,
                    IsDelete = user.isdelete
                });

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo agregar el usuario.");
                throw;
            }
        }

        public async Task<bool> UpdateAsyncSQL(User user)
        {
            try
            {
                // Consulta SQL de actualización
                var sql = @"
                        UPDATE public.usermulta SET 
                        user_per = @User_Per,
                        password = @Password,
                        gmail = @Gmail
                        WHERE id = @id;";

                var parameters = new
                {
                    Id = user.id,
                    User_Per = user.user_per,
                    Password = user.password,
                    Gmail = user.gmail,
                };
                int rowsAffected = await _context.ExecuteAsync(sql, parameters);

                return rowsAffected > 0; 
            }
            catch (Exception ex)
            {
                // Registrar el error en caso de excepción
                _logger.LogError(ex, $"Error al actualizar el usuario con ID {user.id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el usuario con ID {user.id}", ex);
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

        public async Task<bool> DeleteLogicoUserAsyncSQL(int id)
        {
            try
            {
                var sql = "UPDATE usermulta SET isdelete = TRUE WHERE id = @Id";
                var parametro = new NpgsqlParameter("@Id", id);
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parametro);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al realizar delete lógico del User: {ex.Message}");
                return false;
            }
        }
    }
}


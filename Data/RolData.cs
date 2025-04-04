using Dapper;
using Entity.Contexts;
using Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using Npgsql;
using System.Data;
using Utilities.Exceptions;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestion de la entiedad Rol en la base de datos 
    /// </summary>
    public class RolData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolData> _logger;


        /// <summary>
        /// constructor que recibe el contexto de base de datos
        /// </summary>
        /// <param name="context">instacia de <see cref="ApplicationDbContext"/> para la conexion con la base de datos </param>

        public RolData(ApplicationDbContext context, ILogger<RolData> logger)
        {
            _context = context;
            _logger = logger;
        }

        //LINQ

        ///<summary>
        ///obtiene todos los roles almacenados en la base de datos 
        /// </summary>
        /// <returns>lista de roles </returns>

        // Atributo para linq
        public async Task<IEnumerable<Rol>> GetRolesAsync()
        {
            return await _context.Set<Rol>().ToListAsync();
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
                _logger.LogError(ex, "Error al obtener el usuario con el Id {RolId}", id);
                throw;
            }
        }

        // Atributo para SQL
        public async Task<IEnumerable<Rol>> GetAllAsyncSQL()
        {
            string query = "SELECT * FROM rol";
            return await _context.Set<Rol>().FromSqlRaw(query).ToListAsync();
        }

        //Atributo Linq
        public async Task<Rol> CreateAsyncLinq(Rol rolData)
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

        //Atributo Linq
        public async Task<bool> UpdateAsyncLinq(Rol rol)
        {
            try
            {
                _context.Set<Rol>().Update(rol);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el usuario {ex.Message}");
                return false;
            }
        }

        //Atributo Linq
        public async Task<bool> DeleteAsyncLinq(int id)
        {
            try
            {
                var Role = await _context.Set<Rol>().FindAsync(id);
                if (Role == null) return false;

                _context.Set<Rol>().Update(Role);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el Role con ID {id}");
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el Role con ID {id}", ex);
            }
        }


        //SQL

        //Atributo SQL
        public async Task<Rol?> GetByRolIdAsyncSQL(int id)
        {
            try
            {
                var query = "SELECT * FROM rol WHERE id = @Id";  // "id" en minúsculas
                var parametro = new NpgsqlParameter("@Id", id);

                return await _context.Set<Rol>().FromSqlRaw(query, parametro).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Rol con el Id {id}", id);
                throw;
            }
        }

        ///<summary>
        ///crea un nuevo rol en la base de datos
        ///</summary>
        ///<param name="rolData">instancia del rol a crear </param>
        ///<returns> El rol creado.</returns>
       
        //Atributo SQL 
        public async Task<Rol> CreateAsyncSQL(Rol rol)
        {
            try
            {
                const string query = @"INSERT INTO public.""rol""(
	                                ""role"", ""description"")
	                                VALUES ( @Role, @Description)  RETURNING id;";

                rol.id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    Role = rol.role,
                    Description = rol.description
                });

                return rol;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo agregar el usuario.");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un rol existente en la base de datos
        /// </summary>
        /// <param name="rolData">objeto con la informacion actualizada </param>
        /// <returns>True si la operaccion fue exitosa, false en caso contrario.</returns>

        //Atributo SQL
        public async Task<bool> UpdateAsyncSQL(Rol Rol)
        {
            try
            {
                var sql = @"
            UPDATE public.rol
            SET role = @Role, 
            description = @Description, 
            WHERE id = @Id;";

                var parametros = new[]
                {
                    new NpgsqlParameter("@RolPer", Rol.role),
                    new NpgsqlParameter("@Password", Rol.description),
                    new NpgsqlParameter("@Id", Rol.id)
                };

                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parametros);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el usuario con ID {Rol.id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el usuario con ID {Rol.id}", ex);
            }
        }


        ///<summary>
        ///Elimina un rol de la base de datos
        /// </summary>
        /// <param name="id">Identificador unico del rol a eliminar</param>
        /// <returns>true si la eliminacion fue exitosa, false en caso contrario</returns>

        //Atributo SQL
        public async Task<bool> DeleteRolAsyncSQL(int id)
        {
            try
            {
                var sql = "DELETE FROM rol WHERE id = @Id";
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
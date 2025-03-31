﻿
using Entity.Contexts;
using Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class RolFormPermissionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        ///<summary>
        ///constructor que recibe el contexto de la base de datos
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexion con la base de datos</param>

        public RolFormPermissionData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///obtiene todos los RolFormPermissionDataId almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de RolFormPermissionDataId</returns>

        //Atributo Linq

        public async Task<IEnumerable<RolFormPermission>> GetAllAsync()
        {
            try
            {
                return await _context.Set<RolFormPermission>()
                .Include(f => f.Form) // Relación con Form
                .Include(f => f.Permission) // Relación con Permission
                .Include(f => f.Rol) // Relación con Rol
                .ToListAsync(); // Ejecuta la consulta
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al traer el RolFormPermission ");
                throw;
            }

        }


        //Atributo SQL
        public async Task<IEnumerable<RolFormPermission>> GetAllAsyncSql()
        {
            try
            {
                string query = @"
            SELECT rfp.Id, 
                   rfp.RolId, 
                   r.Name AS RolName, 
                   rfp.FormId, 
                   f.Name AS FormName, 
                   rfp.PermissionId, 
                   p.Name AS PermissionName
            FROM RolFormPermission rfp
            INNER JOIN Rol r ON rfp.RolId = r.Id
            INNER JOIN Form f ON rfp.FormId = f.Id
            INNER JOIN Permission p ON rfp.PermissionId = p.Id;";

                return await _context.QueryAsync<RolFormPermission>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al traer el RolFormPermission");
                throw;
            }
        }

        //Metodo Linq
        public async Task<RolFormPermission?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<RolFormPermission>()
                    .Include(rfp => rfp.Form) // Relación con Form
                    .Include(rfp => rfp.Permission) // Relación con Permission
                    .Include(rfp => rfp.Rol) // Relación con Rol
                    .FirstOrDefaultAsync(rfp => rfp.Id == id); // Filtra por ID
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener RolFormPermissionData con ID {RolFormPermissionDataId}", id);
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        //Metodo SQL

        public async Task<RolFormPermission?> GetByIdAsyncSql(int id)
        {
            try
            {
                string query = @"
            SELECT rfp.Id, 
                   rfp.RolId, 
                   r.Name AS RolName, 
                   rfp.FormId, 
                   f.Name AS FormName, 
                   rfp.PermissionId, 
                   p.Name AS PermissionName
            FROM RolFormPermission rfp
            INNER JOIN Rol r ON rfp.RolId = r.Id
            INNER JOIN Form f ON rfp.FormId = f.Id
            INNER JOIN Permission p ON rfp.PermissionId = p.Id
            WHERE rfp.Id = @Id;";

                var parameters = new { Id = id };
                return await _context.QueryFirstOrDefaultAsync<RolFormPermission>(query, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener RolFormPermissionData con ID {RolFormPermissionDataId}", id);
                throw;
            }
        }


        ///<summary>
        ///crea un nuevo RolFormPermissionDataId en la base de datos.
        /// </summary>
        /// <param name="RolFormPermission">objeto con la informacion actualizada</param>
        /// <returns>el RolFormPermissionDataId creado</returns>

        //Metodo Linq
        public async Task<RolFormPermission> CreateAsync(RolFormPermission rolFormPermissionData)
        {
            try
            {
                await _context.Set<RolFormPermission>().AddAsync(rolFormPermissionData);
                await _context.SaveChangesAsync();

                // Retornar directamente el objeto creado sin recargar sus relaciones
                return rolFormPermissionData;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el RolFormPermission: {ex.Message}");
                throw;
            }
        }


        //Metodo sql
        public async Task<int> CreateAsyncSQL(RolFormPermission rolFormPermission)
        {
            try
            {
                string query = @"
            INSERT INTO RolFormPermission (FormId, PermissionId, RolId) 
            VALUES (@FormId, @PermissionId, @RolId);
            SELECT SCOPE_IDENTITY(;
        ";

                var parameters = new[]
                {
            new SqlParameter("@FormId", rolFormPermission.FormId),
            new SqlParameter("@PermissionId", rolFormPermission.PermissionId),
            new SqlParameter("@RolId", rolFormPermission.RolId)
        };

                var result = await _context.Database.ExecuteSqlRawAsync(query, parameters);

                return result; // Retorna el ID generado
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el RolFormPermission: {ex.Message}");
                throw;
            }
        }




        ///<summary>
        ///Actualiza un nuevo RolFormPermissionDataId en la base de datos.
        /// </summary>
        /// <param name="RolFormPermission">objeto con la informacion actualizada</param>
        /// <returns>True si la operacion es exitosa, false en caso contrario</returns>

        //Metodo linq
        public async Task<bool> UpdateAsync(RolFormPermission RolFormPermission)
        {
            try
            {
                _context.Set<RolFormPermission>().Update(RolFormPermission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el RolFormPermission: {ex.Message}");
                return false;
            }
        }

        //Metodo sql
        public async Task<bool> UpdateAsyncSQL(RolFormPermission rolFormPermission)
        {
            try
            {
                string query = @"
            UPDATE RolFormPermission
            SET 
                FormId = @FormId, 
                PermissionId = @PermissionId, 
                RolId = @RolId
            WHERE Id = @Id;
        ";

                var parameters = new[]
                {
            new SqlParameter("@Id", rolFormPermission.Id),
            new SqlParameter("@FormId", rolFormPermission.FormId),
            new SqlParameter("@PermissionId", rolFormPermission.PermissionId),
            new SqlParameter("@RolId", rolFormPermission.RolId)
        };

                await _context.Database.ExecuteSqlRawAsync(query, parameters);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el RolFormPermission: {ex.Message}");
                return false;
            }
        }


        ///<summary>
        ///elimina un nuevo RolFormPermissionDataId en la base de datos.
        /// </summary>
        /// <param name="id">Identificador unico del RolFormPermissionDataId a eliminar</param>
        /// <returns>True si la eliminacion fue exitosa, false en caso contrario</returns>

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rolFormPermissionData = await _context.Set<RolFormPermission>().FindAsync(id);
                if (rolFormPermissionData == null)
                    return false;

                _context.Set<RolFormPermission>().Remove(rolFormPermissionData);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el RolFormPermissionDataId: {ex.Message}");
                return false;
            }
        }

        //Metodo sql

        public async Task<bool> DeleteAsyncSQL(int id)
        {
            try
            {
                string query = "DELETE FROM RolFormPermission WHERE Id = @Id;";

                var parameter = new SqlParameter("@Id", id);

                int affectedRows = await _context.Database.ExecuteSqlRawAsync(query, parameter);

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el RolFormPermission con ID {id}: {ex.Message}");
                return false;
            }
        }

    }
}

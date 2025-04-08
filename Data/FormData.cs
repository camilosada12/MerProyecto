using Entity.Contexts;
using Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Npgsql;

namespace Data
{
    public class FormData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormData> _logger;


        /// <summary>
        /// constructor que recibe el contexto de base de datos
        /// </summary>
        /// <param name="context">instacia de <see cref="ApplicationDbContext"/> para la conexion con la base de datos </param>

        public FormData(ApplicationDbContext context, ILogger<FormData> logger)
        {
            _context = context;
            _logger = logger;
        }

        //LINQ

        ///<summary>
        ///obtiene todos los formularios almacenados en la base de datos 
        /// </summary>
        /// <returns>lista de formularios </returns>

        //Atributo Linq
        public async Task<IEnumerable<Form>> GetAllAsync()
        {
            return await _context.Set<Form>().ToListAsync();
        }

        //atributo linq
        public async Task<Form?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<Form>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con el Id {FormId}", id);
                throw;
            }
        }

        //Atributo Linq
        public async Task<Form> CreateAsync(Form form)
        {
            try
            {
                await _context.Set<Form>().AddAsync(form);
                await _context.SaveChangesAsync();
                return form;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el formulario: {ex.Message}");
                throw;
            }
        }

        //Atributo Linq
        public async Task<bool> UpdateAsyncLinq(Form form)
        {
            try
            {
                _context.Set<Form>().Update(form);
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
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var form = await _context.Set<Form>().FindAsync(id);
                if (form == null)
                    return false;

                _context.Set<Form>().Remove(form);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el formulario: {ex.Message}");
                return false;
            }
        }

        public interface IFormData
        {
            Task<IEnumerable<Form>> GetAllFormAsyncSQL();
            Task<Form> GetByFormIdAsyncSql(int id);
            Task<Form> CreateAsyncSQL(Form form);
            Task<bool> UpdateFormAsyncSQL(Form form);
            Task<bool> DeleteAsyncSQL(int id);
            Task<bool> DeleteLogicoAsyncSQL(int id);
        }

        //SQL
        public class FormDataPostgreSQL : IFormData
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<FormDataPostgreSQL> _logger;

            public FormDataPostgreSQL(ApplicationDbContext context, ILogger<FormDataPostgreSQL> logger)
            {
                _context = context;
                _logger = logger;
            }

            //Atributo SQL
            public async Task<IEnumerable<Form>> GetAllFormAsyncSQL()
            {
                const string query = @"SELECT * FROM form WHERE isdelete = false";
                return await _context.QueryAsync<Form>(query);
            }

            //Atributo SQL
            public async Task<Form?> GetByFormIdAsyncSql(int id)
            {
                try
                {
                    string query = @"SELECT * FROM public.form WHERE id = @Id AND isdelete = false";

                    return await _context.QueryFirstOrDefaultAsync<Form>(query, new { Id = id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener el usuario con el Id {FormId}", id);
                    throw;
                }
            }

            ///<summary>
            ///crea un nuevo formulario en la base de datos
            ///</summary>
            ///<param name="form">instancia del formulario a crear </param>
            ///<returns> El formulario creado.</returns>

            //Atributo SQL
            public async Task<Form> CreateAsyncSQL(Form form)
            {
                try
                {
                    // Forzar valor por defecto en la creación
                    form.isdelete = false;

                    const string query = @"INSERT INTO public.form(
                name, description, isdelete)
                VALUES (@Name, @Description, @IsDeleted)
                RETURNING id;";

                    form.id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                    {
                        Name = form.name,
                        Description = form.description,
                        IsDeleted = form.isdelete,
                    });

                    return form;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error al crear el Form: {ex.Message}");
                    throw;
                }
            }


            /// <summary>
            /// Actualiza un formulario existente en la base de datos
            /// </summary>
            /// <param name="form">objeto con la informacion actualizada </param>
            /// <returns>True si la operaccion fue exitosa, false en caso contrario.</returns>

            //Atributo SQL
            public async Task<bool> UpdateFormAsyncSQL(Form form)
            {
                try
                {
                    var sql = @"
                UPDATE public.form SET 
                name = @Name,
                description = @Description
                WHERE id = @Id;";

                    var parameters = new
                    {
                        Id = form.id,
                        Name = form.name,
                        Description = form.description
                    };

                    int rowsAffected = await _context.ExecuteAsync(sql, parameters);

                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error al actualizar el Form: {ex.Message}");
                    return false;
                }
            }

            ///<summary>
            ///Elimina un formulario de la base de datos
            /// </summary>
            /// <param name="id">Identificador unico del formulario a eliminar</param>
            /// <returns>true si la eliminacion fue exitosa, false en caso contrario</returns>

            //Atributo SQL
            public async Task<bool> DeleteAsyncSQL(int id)
            {
                try
                {
                    var sql = "DELETE FROM form WHERE id = @Id";
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

            public async Task<bool> DeleteLogicoAsyncSQL(int id)
            {
                try
                {
                    var sql = "UPDATE form SET isdelete = TRUE WHERE id = @Id";
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

        public class FormDataMysqlServer : IFormData
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<FormDataMysqlServer> _logger;

            public FormDataMysqlServer(ApplicationDbContext context, ILogger<FormDataMysqlServer> logger)
            {
                _context = context;
                _logger = logger;
            }

            //Atributo SQL Server
            public async Task<IEnumerable<Form>> GetAllFormAsyncSQL()
            {
                const string query = @"SELECT * FROM form WHERE isdelete = 0";
                return await _context.QueryAsync<Form>(query);
            }

            //Atributo SQL Server
            public async Task<Form?> GetByFormIdAsyncSql(int id)
            {
                try
                {
                    string query = @"SELECT * FROM form WHERE id = @Id AND isdelete = 0";
                    return await _context.QueryFirstOrDefaultAsync<Form>(query, new { Id = id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener el formulario con el Id {FormId}", id);
                    throw;
                }
            }

            //Atributo SQL Server
            public async Task<Form> CreateAsyncSQL(Form form)
            {
                try
                {
                    // Forzar valor por defecto en la creación
                    form.isdelete = false;
                    const string query = @"INSERT INTO form(
                    name, description, isdelete)
                    OUTPUT INSERTED.id
                    VALUES (@Name, @Description, @IsDeleted)";

                    form.id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                    {
                        Name = form.name,
                        Description = form.description,
                        IsDeleted = form.isdelete ? 1 : 0, // SQL Server usa 0/1 en lugar de false/true
                    });
                    return form;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear el Form: {ex.Message}");
                    throw;
                }
            }

            //Atributo SQL Server
            public async Task<bool> UpdateFormAsyncSQL(Form form)
            {
                try
                {
                    var sql = @"
                    UPDATE form SET 
                    name = @Name,
                    description = @Description
                    WHERE id = @Id";
                    var parameters = new
                    {
                        Id = form.id,
                        Name = form.name,
                        Description = form.description
                    };
                    int rowsAffected = await _context.ExecuteAsync(sql, parameters);
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar el Form: {ex.Message}");
                    return false;
                }
            }

            //Atributo SQL Server
            public async Task<bool> DeleteAsyncSQL(int id)
            {
                try
                {
                    var sql = "DELETE FROM form WHERE id = @Id";
                    var parameter = new SqlParameter("@Id", id);
                    var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parameter);
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al eliminar el formulario: {ex.Message}");
                    return false;
                }
            }

            //Atributo SQL Server
            public async Task<bool> DeleteLogicoAsyncSQL(int id)
            {
                try
                {
                    var sql = "UPDATE form SET isdelete = 1 WHERE id = @Id";
                    var parameter = new SqlParameter("@Id", id);
                    var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parameter);
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al realizar delete lógico del form: {ex.Message}");
                    return false;
                }
            }

        }

        public class FormDataMysql : IFormData
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<FormDataMysql> _logger;

            public FormDataMysql(ApplicationDbContext context, ILogger<FormDataMysql> logger)
            {
                _context = context;
                _logger = logger;
            }

            //Atributo MySQL
            public async Task<IEnumerable<Form>> GetAllFormAsyncSQL()
            {
                const string query = @"SELECT * FROM form WHERE isdelete = 0";
                return await _context.QueryAsync<Form>(query);
            }

            //Atributo MySQL
            public async Task<Form?> GetByFormIdAsyncSql(int id)
            {
                try
                {
                    string query = @"SELECT * FROM form WHERE id = @Id AND isdelete = 0";
                    return await _context.QueryFirstOrDefaultAsync<Form>(query, new { Id = id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener el formulario con el Id {FormId}", id);
                    throw;
                }
            }

            //Atributo MySQL
            public async Task<Form> CreateAsyncSQL(Form form)
            {
                try
                {
                    // Forzar valor por defecto en la creación
                    form.isdelete = false;
                    const string query = @"INSERT INTO form(
                    name, description, isdelete)
                    VALUES (@Name, @Description, @IsDeleted);
                    SELECT LAST_INSERT_ID();";

                    form.id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                    {
                        Name = form.name,
                        Description = form.description,
                        IsDeleted = form.isdelete ? 1 : 0, // MySQL también usa 0/1 para booleanos
                    });
                    return form;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear el Form: {ex.Message}");
                    throw;
                }
            }

            //Atributo MySQL
            public async Task<bool> UpdateFormAsyncSQL(Form form)
            {
                try
                {
                    var sql = @"
                    UPDATE form SET 
                    name = @Name,
                    description = @Description
                    WHERE id = @Id";
                    var parameters = new
                    {
                        Id = form.id,
                        Name = form.name,
                        Description = form.description
                    };
                    int rowsAffected = await _context.ExecuteAsync(sql, parameters);
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar el Form: {ex.Message}");
                    return false;
                }
            }

            //Atributo MySQL
            public async Task<bool> DeleteAsyncSQL(int id)
            {
                try
                {
                    var sql = "DELETE FROM form WHERE id = @Id";
                    var parameter = new MySqlParameter("@Id", id); // Cambiado a MySqlParameter
                    var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parameter);
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al eliminar el formulario: {ex.Message}");
                    return false;
                }
            }

            //Atributo MySQL
            public async Task<bool> DeleteLogicoAsyncSQL(int id)
            {
                try
                {
                    var sql = "UPDATE form SET isdelete = 1 WHERE id = @Id";
                    var parameter = new MySqlParameter("@Id", id); // Cambiado a MySqlParameter
                    var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parameter);
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al realizar delete lógico del form: {ex.Message}");
                    return false;
                }
            }

        }


    }
}


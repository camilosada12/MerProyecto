

using Entity.Contexts;
using Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class FormData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;


        /// <summary>
        /// constructor que recibe el contexto de base de datos
        /// </summary>
        /// <param name="context">instacia de <see cref="ApplicationDbContext"/> para la conexion con la base de datos </param>

        public FormData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///obtiene todos los formularios almacenados en la base de datos 
        /// </summary>
        /// <returns>lista de formularios </returns>

        //Atributo Linq
        public async Task<IEnumerable<Form>> GetAllAsync()
        {
            return await _context.Set<Form>().ToListAsync();
        }

        //Atributo SQL
        public async Task<IEnumerable<Form>> GetAllAsyncSQL()
        {
            string query = "SELECT * FROM Form";
            return (IEnumerable<Form>)await _context.QueryAsync<IEnumerable<Form>>(query);
        }

        //Atributo Linq
        public async Task<Form?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Form>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el formulario con Id {FormId}", id);
                throw; // Re-lanza la excepcion para que sea manejada en capas superiores
            }
        }

        //Atributo SQL
        public async Task<Form?> GetByIdAsyncSQL(Form id)
        {
            try
            {
                // Consulta SQL con parámetro para el Id
                var query = "SELECT * FROM Form WHERE Id = @id";

                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@id", id.Id),
                };

                // Ejecutar la consulta y obtener el Id del nuevo Form
                var result = await _context.Database.ExecuteSqlRawAsync(query, parametros);

                // Obtener el Form recién insertado con el Id generado
                id.Id = Convert.ToInt32(result); // Asumimos que SCOPE_IDENTITY devuelve un int
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Form con el Id {Id}", id);
                throw; // Re-lanza la excepción para ser manejada en las capas superiores
            }
        }

        ///<summary>
        ///crea un nuevo formulario en la base de datos
        ///</summary>
        ///<param name="form">instancia del formulario a crear </param>
        ///<returns> El formulario creado.</returns>

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

        //Atributo SQL
        public async Task<Form> CreateAsyncSQL(Form form)
        {
            try
            {
                // SQL para insertar en la tabla Users
                var sql = "INSERT INTO Users (Name, Description, DateCreation, statu) " +
                          "VALUES (@name, @description, @dateCreation, @Statu);" +
                          "SELECT SCOPE_IDENTITY();"; // Esto devuelve el Id del Form insertado

                // Ejecutar el comando SQL
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@name", form.Name),
                    new SqlParameter("@description", form.Description),
                    new SqlParameter("@dateCreation", form.DateCreation),
                    new SqlParameter("@Statu", form.statu),
                };

                // Ejecutar la consulta y obtener el Id del nuevo Form
                var result = await _context.Database.ExecuteSqlRawAsync(sql, parametros);

                // Obtener el Form recién insertado con el Id generado
                form.Id = Convert.ToInt32(result); // Asumimos que SCOPE_IDENTITY devuelve un int
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

        //Atributo Linq
        public async Task<bool> UpdateAsync(Form form)
        {
            try
            {
                _context.Set<Form>().Update(form);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el formulario: {ex.Message}");
                return false;
            }
        }

        //Atributo SQL
        public async Task<bool> UpdateAsyncSQL(Form form)
        {
            try
            {
                // Consulta SQL para actualizar el Form
                var sql = "UPDATE Users SET " +
                          "Name = @name, " +
                          "Description = @description, " +
                          "DateCreation = @dateCreation, " +
                          "statu = @Statu, " +
                          "WHERE Id = @Id";  // Asegúrate de que la columna 'Id' es la clave primaria

                // Parámetros SQL que se pasan a la consulta
                var parametros = new SqlParameter[]
                {
                  new SqlParameter("@name", form.Name),
                    new SqlParameter("@description", form.Description),
                    new SqlParameter("@dateCreation", form.DateCreation),
                    new SqlParameter("@Statu", form.statu),
                    new SqlParameter("@Id", form.Id),
                };

                // Ejecutar la consulta SQL
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parametros);

                // Verificar si la actualización afectó alguna fila (es decir, si el Form fue actualizado)
                return rowsAffected > 0;  // Si rowsAffected > 0, la actualización fue exitosa
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

        //Atributo SQL

        public async Task<bool> DeleteAsyncSQL(int id)
        {
            try
            {
                // Consulta SQL para eliminar el Form con el Id proporcionado
                var sql = "DELETE FROM Form WHERE Id = @Id";  // Asegúrate de que 'Id' es la columna correcta para identificar al Form

                // Parámetro SQL que se pasa a la consulta
                var parametro = new SqlParameter("@Id", id);  // El Id es el único parámetro en este caso

                // Ejecutar la consulta SQL para eliminar el Form
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, parametro);

                // Verificar si se eliminó alguna fila (es decir, si el Form fue encontrado y eliminado)
                return rowsAffected > 0;  // Si rowsAffected > 0, la eliminación fue exitosa
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el Form: {ex.Message}");
                return false;
            }
        }

    }
}


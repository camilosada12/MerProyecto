

using Entity.Contexts;
using Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class PersonData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;


        /// <summary>
        /// constructor que recibe el contexto de base de datos
        /// </summary>
        /// <param name="context">instacia de <see cref="ApplicationDbContext"/> para la conexion con la base de datos </param>

        public PersonData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///obtiene todos los Personas almacenados en la base de datos 
        /// </summary>
        /// <returns>lista de Personas </returns>

        //Atributo Linq
        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _context.Set<Person>().ToListAsync();
        }

        //Atributo SQL
        public async Task<IEnumerable<Person>> GetAllAsyncSQL()
        {

            string query = "SELECT * FROM Person";
            return (IEnumerable<Person>)await _context.QueryAsync<IEnumerable<Person>>(query);
        }


        //Atributo Linq
        public async Task<Person?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Person>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener person con Id {PersonaId}", id);
                throw; // Re-lanza la excepcion para que sea manejada en capas superiores
            }
        }

        //Atributo SQL
        public async Task<Person?> GetByIdAsyncSQL(Person id)
        {
            try
            {
                // Consulta SQL con parámetro para el Id
                var query = "SELECT * FROM Person WHERE Id = @id";

                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@id", id.Id),
                };

                // Ejecutar la consulta y obtener el Id del nuevo Person
                var result = await _context.Database.ExecuteSqlRawAsync(query, parametros);

                // Obtener el Person recién insertado con el Id generado
                id.Id = Convert.ToInt32(result); // Asumimos que SCOPE_IDENTITY devuelve un int
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Person con el Id {Id}", id);
                throw; // Re-lanza la excepción para ser manejada en las capas superiores
            }
        }

        ///<summary>
        ///crea un nuevo Persona en la base de datos
        ///</summary>
        ///<param name="person">instancia del Persona a crear </param>
        ///<returns> El Persona creado.</returns>

        //Atributo Linq
        public async Task<Person> CreateAsync(Person person)
        {
            try
            {
                await _context.Set<Person>().AddAsync(person);
                await _context.SaveChangesAsync();
                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la  tabla persona: {ex.Message}");
                throw;
            }
        }

        //Atributo SQL
        public async Task<Person> CreateAsyncSQL(Person persons)
        {
            try
            {
                // SQL para insertar en la tabla Users
                var query = "INSERT INTO Person (name , lastName, phone) " +
                          "VALUES (@name, @lastName , @phone);" +
                          "SELECT SCOPE_IDENTITY();"; // Esto devuelve el Id del usuario insertado

                // Ejecutar el comando SQL
                var parametrosPerson = new SqlParameter[]
                {
                    new SqlParameter("@name", persons.Name),
                    new SqlParameter("@lastName", persons.LastName),
                    new SqlParameter("@phone", persons.Phone)
                };

                // Ejecutar la consulta y obtener el Id del nuevo usuario
                var result = await _context.Database.ExecuteSqlRawAsync(query, parametrosPerson);

                // Obtener el usuario recién insertado con el Id generado
                persons.Id = Convert.ToInt32(result); // Asumimos que SCOPE_IDENTITY devuelve un int
                return persons;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el usuario: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un Persona existente en la base de datos
        /// </summary>
        /// <param name="person">objeto con la informacion actualizada </param>
        /// <returns>True si la operaccion fue exitosa, false en caso contrario.</returns>

        //Atributo Linq
        public async Task<bool> UpdateAsync(Person person)
        {
            try
            {
                _context.Set<Person>().Update(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la tabla persona: {ex.Message}");
                return false;
            }
        }

        //Atributo SQL
        public async Task<bool> UpdateAsyncSQL(Person person)
        {
            try
            {
                // Consulta SQL para actualizar el usuario
                var sql = "UPDATE Person SET " +
                          "Name = @name, " +
                          "LastName = @lastName, " +
                          "Phone = @phone, " +
                          "WHERE Id = @Id";  // Asegúrate de que la columna 'Id' es la clave primaria

                // Parámetros SQL que se pasan a la consulta
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@name", person.Name),
                    new SqlParameter("@lastName", person.LastName),
                    new SqlParameter("@phone", person.Phone),
                    new SqlParameter("@Id", person.Id),
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
        ///Elimina una Persona de la base de datos
        /// </summary>
        /// <param name="id">Identificador unico del Persona a eliminar</param>
        /// <returns>true si la eliminacion fue exitosa, false en caso contrario</returns>

        //Atributo Linq
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var personData = await _context.Set<Person>().FindAsync(id);
                if (personData == null)
                    return false;

                _context.Set<Person>().Remove(personData);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar de la tabla persona: {ex.Message}");
                return false;
            }
        }

        //Atributo SQL
        public async Task<bool> DeleteAsyncSQL(int id)
        {
            try
            {
                // Consulta SQL para eliminar el usuario con el Id proporcionado
                var sql = "DELETE FROM Person WHERE Id = @Id";  // Asegúrate de que 'Id' es la columna correcta para identificar al usuario

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

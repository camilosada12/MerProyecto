using Entity.Contexts;
using Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Data
{
    public class PersonData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PersonData> _logger;


        /// <summary>
        /// constructor que recibe el contexto de base de datos
        /// </summary>
        /// <param name="context">instacia de <see cref="ApplicationDbContext"/> para la conexion con la base de datos </param>

        public PersonData(ApplicationDbContext context, ILogger<PersonData> logger)
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
        public async Task<IEnumerable<Person>> GetAllPersonAsyncSQL()
        {

            string query = "SELECT * FROM person";
            return await _context.Set<Person>().FromSqlRaw(query).ToListAsync();
        }


        //Atributo Linq
        public async Task<Person?> GetByIdAsyncLinq(int id)
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
        public async Task<Person?> GetByIdAsyncSQL(int id)
        {
            try
            {
                var query = "SELECT * FROM person WHERE id = @Id";  // "id" en minúsculas
                var parametro = new NpgsqlParameter("@Id", id);

                return await _context.Set<Person>().FromSqlRaw(query, parametro).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Person con el Id {id}", id);
                throw;
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
        public async Task<Person> CreatePersonAsyncSQL(Person person)
        {
            try
            {
                const string query = @"
                   INSERT INTO public.person(
	                name, lastname, phone)
	                VALUES (@Name, @LastName, @Phone)
                    RETURNING id;";

                person.id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    Name = person.name,
                    LastName = person.lastname,
                    Phone = person.phone
                });

                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo agregar la persona.");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un Persona existente en la base de datos
        /// </summary>
        /// <param name="person">objeto con la informacion actualizada </param>
        /// <returns>True si la operaccion fue exitosa, false en caso contrario.</returns>

        //Atributo Linq
        public async Task<bool> UpdatePersonAsyncLinq(Person person)
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
                    new SqlParameter("@name", person.name),
                    new SqlParameter("@lastName", person.lastname),
                    new SqlParameter("@phone", person.phone),
                    new SqlParameter("@Id", person.id),
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
        public async Task<bool> DeletePermissionAsyncSQL(int id)
        {
            try
            {
                var sql = "DELETE FROM person WHERE id = @Id";
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

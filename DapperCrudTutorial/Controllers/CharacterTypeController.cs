using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DapperCrudTutorial.Controllers
{
    /// <summary>
    /// Servicios para crear, listar, modificar o borrar CharacterType de los SuperHeroes
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterTypeController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CharacterTypeController(IConfiguration config)
        {
            _config = config;
        }
        /// <summary>Obtiene todos los objetos CharacterType</summary>
        [HttpGet]
        public async Task<ActionResult<List<CharacterType>>> GetAllCharacterTypes()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<CharacterType> characterTypes = await SelectAllCharacterTypes(connection);
            return Ok(characterTypes);
        }

        /// <summary>Obtiene un objeto CharacterType</summary>
        /// <param name="Id">CharacterType.Id a buscar</param>
        [HttpGet("{Id}")]
        public async Task<ActionResult<CharacterType>> GetCharacterType(int Id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var characterType = await connection.QueryFirstAsync<CharacterType>("SELECT * FROM CharacterTypes WHERE Id = @Id", new { Id = Id });
            return Ok(characterType);
        }

        /// <summary>Crea un objeto CharacterType</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST
        ///     {
        ///        "name": "CharacterType"
        ///     }
        ///
        /// </remarks>
        /// <param name="characterType">CharacterType</param>
        [HttpPost]
        public async Task<ActionResult<List<CharacterType>>> CreateCharacterType(CharacterType characterType)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var sql = "INSERT INTO CharacterTypes (Name) VALUES (@Name)";
            var character = await connection.QueryAsync<CharacterType>(sql, characterType);
            return Ok(await SelectAllCharacterTypes(connection));
        }

        /// <summary>Modifica un objeto CharacterType</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT
        ///     {
        ///        "id": 1,
        ///        "name": "CharacterType"
        ///     }
        ///
        /// </remarks>
        /// <param name="characterType">CharacterType</param>
        [HttpPut]
        public async Task<ActionResult<List<CharacterType>>> UpdateCharacterType(CharacterType characterType)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var sql = "UPDATE CharacterType SET Name = @Name WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = characterType.Id, Name = characterType.Name });
            return Ok(await SelectAllCharacterTypes(connection));
        }

        /// <summary>Elimina un objeto CharacterType</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE
        ///     {
        ///        "id": 1
        ///     }
        ///
        /// </remarks>
        [HttpDelete("{Id}")]
        public async Task<ActionResult<List<CharacterType>>> DeleteCharacterType(int Id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("DELETE FROM CharacterTypes WHERE Id = @Id", new { Id = Id });
            return Ok(await SelectAllCharacterTypes(connection));
        }

        //METODO REFACTORIZADO QUE DEVUELVE TODOS LOS CHARACTER
        private static async Task<IEnumerable<CharacterType>> SelectAllCharacterTypes(SqlConnection connection)
        {
            var sql = "SELECT * FROM CharacterTypes";
            var characterTypes = await connection.QueryAsync<CharacterType>(sql);

            return characterTypes;
        }
    }
}

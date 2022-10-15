using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DapperCrudTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterTypeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CharacterTypeController(IConfiguration config)
        {
            _config = config;
        }
        //METODO QUE DEVUELVE TODOS LOS CharacterType
        [HttpGet]
        public async Task<ActionResult<List<CharacterType>>> GetAllCharacterTypes()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<CharacterType> characterTypes = await SelectAllCharacterTypes(connection);
            return Ok(characterTypes);
        }

        //METODO QUE DEVUELVE UN CharacterType DE ACUERDO AL ID INGRESADO
        [HttpGet("{Id}")]
        public async Task<ActionResult<CharacterType>> GetCharacterType(int Id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var characterType = await connection.QueryFirstAsync<CharacterType>("SELECT * FROM CharacterTypes WHERE Id = @Id", new { Id = Id });
            return Ok(characterType);
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

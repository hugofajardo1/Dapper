using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Data.SqlClient;

namespace DapperCrudTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SuperHeroController(IConfiguration config)
        {
            _config = config;
        }

        //METODO QUE DEVUELVE TODOS LOS SUPERHEROES
        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHeroes()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<SuperHero> heroes = await SelectAllHeroes(connection);
            return Ok(heroes);
        }

        //METODO QUE DEVUELVE UN SUPERHEROE DE ACUERDO AL ID INGRESADO
        [HttpGet("{Id}")]
        public async Task<ActionResult<SuperHero>> GetHero(int Id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var sql = "SELECT * FROM SuperHeroes LEFT JOIN CharacterTypes ON SuperHeroes.CharacterTypeId = CharacterTypes.Id WHERE SuperHeroes.Id = @Id Order by SuperHeroes.Id";
            var hero = await connection.QueryAsync<SuperHero, CharacterType, SuperHero>(sql, (superHero, characterType) => 
            { 
                superHero.CharacterType = characterType; 
                return superHero; 
            }, new { Id = Id }, splitOn: "CharacterTypeId");
            
            return Ok(hero);
        }

        //METODO QUE CARGA UN SUPERHEROE
        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> CreateHero(SuperHero superHero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var sql = "INSERT INTO SuperHeroes (Name, FirstName, LastName, Place, CharacterTypeId) VALUES (@Name, @FirstName, @LastName, @Place, @CharacterTypeId)";
            var hero = await connection.QueryAsync<SuperHero, CharacterType, SuperHero>(sql, (superHero, characterType) =>
            {
                superHero.CharacterType = characterType;
                return superHero;
            }, new { Id = superHero.Id }, splitOn: "CharacterTypeId");
            return Ok(await SelectAllHeroes(connection));
        }

        //METODO QUE MODIFICA UN SUPERHEROE
        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> UpdateHero(SuperHero superHero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var sql = "UPDATE SuperHeroes SET Name = @Name, FirstName = @FirstName, LastName = @LastName, Place = @Place, CharacterTypeId = @CharacterTypeId WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new {Id = superHero.Id, Name = superHero.Name, FirstName = superHero.FirstName, LastName = superHero.Lastname, Place = superHero.Place, CharacterTypeId = superHero.CharacterType.Id });
            return Ok(await SelectAllHeroes(connection));
        }

        //METODO QUE CARGA UN SUPERHEROE
        [HttpDelete("{Id}")]
        public async Task<ActionResult<List<SuperHero>>> DeleteHero(int Id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("DELETE FROM SuperHeroes WHERE Id = @Id", new { Id = Id });
            return Ok(await SelectAllHeroes(connection));
        }

        //METODO REFACTORIZADO QUE DEVUELVE TODOS LOS SUPERHEROES
        private static async Task<IEnumerable<SuperHero>> SelectAllHeroes(SqlConnection connection)
        {
            var sql = "SELECT * FROM SuperHeroes LEFT JOIN CharacterTypes ON SuperHeroes.CharacterTypeId = CharacterTypes.Id ORDER BY SuperHeroes.Id";
            var hero = await connection.QueryAsync<SuperHero, CharacterType, SuperHero>(sql, (superHero, characterType) =>
            {
                superHero.CharacterType = characterType;
                return superHero;
            }, splitOn: "CharacterTypeId");

            return hero;
        }
    }
}

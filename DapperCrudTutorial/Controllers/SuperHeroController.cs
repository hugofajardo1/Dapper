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
        [HttpGet("{heroId}")]
        public async Task<ActionResult<SuperHero>> GetHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            //var hero = await connection.QueryFirstAsync<SuperHero>("SELECT * FROM SuperHeroes WHERE Id = @Id",
            //        new { Id = heroId });
            var sql = @"SELECT * FROM SuperHeroes LEFT JOIN CharacterTypes ON SuperHeroes.CharacterTypeId = CharacterTypes.Id WHERE SuperHeroes.Id = @Id Order by SuperHeroes.Id";
            var hero = await connection.QueryAsync<SuperHero, CharacterType, SuperHero>(sql, (superHero, characterType) => 
            { 
                superHero.CharacterType = characterType; 
                return superHero; 
            }, new { Id = heroId }, splitOn: "CharacterTypeId");
            
            return Ok(hero);
        }

        //METODO QUE CARGA UN SUPERHEROE
        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> CreateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("INSERT INTO SuperHeroes (name, firstname, lastname, place, charactertypeId) VALUES (@Name, @FirstName, @LastName, @Place, @CharacterTypeId)", hero);
            return Ok(await SelectAllHeroes(connection));
        }

        //METODO QUE MODIFICA UN SUPERHEROE
        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> UpdateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("UPDATE SuperHeroes SET name = @Name, firstname = @FirstName, lastname = @LastName, place = @Place where id = @Id", hero);
            return Ok(await SelectAllHeroes(connection));
        }

        //METODO QUE CARGA UN SUPERHEROE
        [HttpDelete("{heroId}")]
        public async Task<ActionResult<List<SuperHero>>> DeleteHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("DELETE FROM SuperHeroes WHERE Id = @Id", new { Id = heroId });
            return Ok(await SelectAllHeroes(connection));
        }

        //METODO REFACTORIZADO QUE DEVUELVE TODOS LOS SUPERHEROES
        private static async Task<IEnumerable<SuperHero>> SelectAllHeroes(SqlConnection connection)
        {
            return await connection.QueryAsync<SuperHero>("SELECT * FROM SuperHeroes");
        }
    }
}

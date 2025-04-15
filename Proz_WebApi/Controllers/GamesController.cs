using System.Net;
using System.Runtime;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Proz_WebApi.Data;
using Proz_WebApi.Models;
using Proz_WebApi.Models.Dto;
using Proz_WebApi.Services;
using Serilog;

namespace Proz_WebApi.Controllers
{

    //[Route("API/[controller]")] //this is used when you want it to use the name of the class automatically (this is bad because if you changed the class name in the future the class route will change as well and the frontend app must change the API calling as well, so making it static is the best choice).
    [Route("API/Games")]
    [ApiController]
    [Authorize (AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]//THIS IS USED IF YOU WANT THE USERS TO ACCESS YOUR ENDPOINT ONLY IF THEY Authorized already.   
    //[AllowAnonymous] this is the reverse of [Authorize] in which all of the freely of use of these endpoints.
    public class GamesController : ControllerBase //The name of the controller should always ends with "Controller" word!
    {
        private readonly ILogger<GamesController> _loggerr;
        private readonly ApplicationDbContext _db;
        private readonly GamesService _gamesService;
        private readonly JWTOptions _options; //we use this to access the properties from AppSettings class, and these properties will contain the values that we need (based of there is a property for it in the class or not) from any sources like appsetting.json or from system environment variable etc.. i will prevent you from typing hardcoded in which it will be hard later if we decided to change the value (we will be changing it everywhere)

        public GamesController(ILogger<GamesController> loggerr, ApplicationDbContext db, GamesService gamesService, IOptionsSnapshot<JWTOptions> options)
        {
            _loggerr = loggerr;
            _db = db;
            _gamesService = gamesService;
            _options = options.Value;
        
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Games_Dto2>>> GetGames() //When you make the return type as "IEnumerable" interface then it means that the programmer wants to return any kind of collections, like list, array etc..
        {
            
            var games = await _gamesService.GetGames();
            if (games == null || !games.Any()) //!games.Any() is better than games.count==0 because it stops on the first element when it finds one rather then countung all the elements (in enterprice there will be at least 100000 games so if we used the old check it will count from 1 to 100000 every time someone asking to get the games!
                return StatusCode(StatusCodes.Status404NotFound);
            _loggerr.LogInformation("A list of games has been fetched.");
                return StatusCode(StatusCodes.Status200OK, games);


        }
        //[Route("{id}")] 
        //[HttpGet (Name = "get_game")] //you can do it like this for example if you want in which we create a dependent route attribute and leave our [httpget] clear.
        [HttpGet("{id:int}", Name = "get_game")] //lets focus on  [HttpGet("{id:int}"]. As we know before the server owner can design his APIs and how to access or receive it as he want! and any client must follow his way in order to get his service! in this code  [HttpGet("{id:int}"] we said to access this action method inside this controller you need first to put the route of this contoller which is API/Games and then the route of this action method, in our situation we didn't put it as static (when it is betten braces like [] then it's not static and the value depends on the user) like we did in the controller put we defined that the client should enter a number after the API/Games like API/Games/2 or API/Games/6 etc.. and it's not done yet! we did told it as well to take the number that the user will give and store it inside our parameter which id. noticed that if you want to make both of them with different names like :   [HttpGet("{key:int}"] then we should put this in the parameter to tell the framework take this value from the route into this variable : [FromRoute(Name ="key")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        public async Task< ActionResult<Games_Dto2>> GetGame(/*[FromRoute(Name ="id")]*/ int id)
        {
            if (id <= 0)
            {
                _loggerr.LogError("User has entered invalid data in request"); //Please in production make these logs messages more clear and accurately reflect what’s happening
                return StatusCode(StatusCodes.Status400BadRequest);
            }
          var targeting_row=await _gamesService.GetGame(id);
            if (targeting_row == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            return StatusCode(StatusCodes.Status200OK, targeting_row);


        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task< ActionResult<Games_Dto2>> InsertingGame([FromBody]  Games_Dto game) //as we know we as the designers (the creators) of the APIs service we design our own ways to get our data and also we desgin our way to send our data and any client must follows our pattern! there are are three places that are commonly know right now (there is others but understand this for now) you can tell the client to pass the data (when he send the request) from first : the route like API/Games/2 so here you let him send the number 2 and you can use this data in your logic to give the right data to the client. The second place is the query string like this API/Games?Name=BF4%Price=25.1 like this! you can add this [FromQuery] rather then [FromBody] . Noticed that the previous two ways is used to pass simple data (like if you want the user to send with him a simple data with him in order for you logic works) but if you want him to send a complex data like object you should let him pass the value from the body of the HTTP protocol like add this [FromBody] beside your DTO object. Focus that if you used string query method then the client should enter all the expecting data of the DTO object you have after that the framework validate these data (if it's requred etc.. ) Also the default method the framework will decide to take the value from (if you didn't explicitly did) is depnding on the value type, if it's a complex type it will take it from the body (even if you didn't told it to do) but for simple values like int id or string name then will use one of the two methods. If you have two complex object with the same type (id and name) and you want the client to pass values to them using only string query (the first object is product and the second is product2) then do this API/Games?product.id=1&product.name=monder&product2.id=2&product2.name=mutaz
        {
           
            if (game == null)
                return StatusCode(StatusCodes.Status400BadRequest);

            if (await _db.Games.AnyAsync(g => g.name.ToLower() == game.name.ToLower()))
            {
                ModelState.AddModelError("", "The game is already in the database!");
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }
            if (!TryValidateModel(game)) //this will go to check a final look if there any attrubute (int the Games_Dto TYPE) that is conflicting with the data the user provided), also it will not just checks the built-in attributes like [require] for example but also the custom attrubutes that you create as well! (read about custom attibutes if you want to)
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            
           var ModelToReturn= await _gamesService.InsertingGame(game);

            return CreatedAtRoute("get_game", new { id = ModelToReturn.id }, ModelToReturn);
        }

        [HttpDelete("{id:int}",Name ="delete_game")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task< IActionResult> deleting_a_game(int id)
        {
            if (id<=0)
            {
           return StatusCode(StatusCodes.Status400BadRequest);
            }
            var game = await _db.Games.FirstOrDefaultAsync(g => g.id == id);

            if (game==null)
            {
              return StatusCode(StatusCodes.Status404NotFound);
            }

            var DTOgame=await _gamesService.DeleteGame(game);
            _loggerr.LogInformation("This game {Game} was deleted.",DTOgame);
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "update_game")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateGame(int id,[FromBody] Games_Dto updatedgame)
        {
            if (updatedgame == null || id<=0)
               return StatusCode(StatusCodes.Status400BadRequest);
            if (!TryValidateModel(updatedgame))
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            var storedgame =await _db.Games.FirstOrDefaultAsync(g => g.id == id);
            if (storedgame == null)
                return StatusCode(StatusCodes.Status404NotFound);

          await _gamesService.UpdateGame(storedgame, updatedgame);
           
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "update_game_sp")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
         public async Task<IActionResult> UpdateGamePATCH(int id, [FromBody] JsonPatchDocument<Games_Dto> updatedgamePATCH)
        {
            if (updatedgamePATCH == null || id<=0 )
                return StatusCode(StatusCodes.Status400BadRequest);

            var storedgame = await _db.Games.FirstOrDefaultAsync(g => g.id == id);

            if (storedgame == null)
               return StatusCode(StatusCodes.Status404NotFound);
            var updatedgame = storedgame.Adapt<Games_Dto>();



            updatedgamePATCH.ApplyTo(updatedgame, ModelState);
            if (!TryValidateModel(updatedgame))
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);

            try
            {
              await _gamesService.UpdateGamePATCH(storedgame, updatedgame);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,ex);
            }
            
            return NoContent();

        }
    }
}

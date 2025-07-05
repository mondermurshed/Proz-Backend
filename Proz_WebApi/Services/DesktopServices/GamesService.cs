using Mapster;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proz_WebApi.Controllers;
using Proz_WebApi.Data;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;
using Proz_WebApi.Models.DesktopModels.Dto;

namespace Proz_WebApi.Services.DesktopServices
{
    public class GamesService
    {
        private readonly ILogger<GamesService> _loggerr;
        private readonly ApplicationDbContext_Desktop _db;

        public GamesService(ILogger<GamesService> loggerr, ApplicationDbContext_Desktop db)
        {
            _loggerr = loggerr;
            _db = db;
        }
        public async Task<IEnumerable<Games_Dto2>> GetGames() //When you make the return type as "IEnumerable" interface then it means that the programmer wants to return any kind of collections, like list, array etc..
        {

            var games = await _db.Games
               .OrderBy(game => game.price)
               .ProjectToType<Games_Dto2>() //  it's simply a method that will convert the type of the object to another type by mapping it to the other object. This is good for maintenance because if we renamed/added properties of our DTO then we don't have to go back through the code and change them one by one. Also EF will know how to convert it to a sql code before fetching anything from the database then it will contact the database to filter it as the needing to brought the cells/items that we really needing (so it increasing the speed of the application) + This method is different from the Adapt<> method of mapster because this can actually send to EF some sql query to tell it what to bring in here.
               .ToListAsync();

            return games;

        }

        public async Task<Games_Dto2> GetGame(int id)
        {

            //var TargetingRow =await _db.Games.FirstOrDefaultAsync(n => n.id == id); //this method is first or default meaning it will return the first thing that has the conditions, if there is nothing it will return null for reference types or 0 or "" for value types
            var TargetingRow = await _db.Games
                .Where(g => g.id == id)      //so this will filter the records first (that will from the database)
                .ProjectToType<Games_Dto2>() //this will make sure that the EF will tell your database to bring only the important fields from the rows that was filtered by the Where statement
                .FirstOrDefaultAsync();      //and finally this will make sure that it will pick the first row that has all the requirements (id==the requested user id in it has only the needed fields on it) if there was no row returned then this responsible to return a null. This is why it placed it at the end because it's the most common way.
            return TargetingRow;


        }

        public async Task<Games_Dto2> InsertingGame(Games_Dto game)
        {




            Games_Model ModelToUpdate = game.Adapt<Games_Model>();
            ModelToUpdate.created_at = DateTime.UtcNow;
            ModelToUpdate.updated_at = DateTime.UtcNow;
            await _db.Games.AddAsync(ModelToUpdate);
            await _db.SaveChangesAsync();


            Games_Dto2 ModelToReturn = ModelToUpdate.Adapt<Games_Dto2>();
            return ModelToReturn;

        }

        public async Task<Games_Dto2> DeleteGame(Games_Model game)
        {


            _db.Games.Remove(game);
            await _db.SaveChangesAsync();
            var DTOgames = game.Adapt<Games_Dto2>();

            return DTOgames;
        }

        public async Task UpdateGame(Games_Model storedgame, Games_Dto updatedgame)
        {



            updatedgame.Adapt(storedgame);  //it will map the game as a src object to the Stored_Game as our desc object (we use this way when we have our objects already defined and we don 
            storedgame.updated_at = DateTime.Now; //Notice that we have updating everything in here because it's a PUT endpoint. Also when we don't update a property then we are like telling EF to keep the old value.

            await _db.SaveChangesAsync();


        }

        public async Task UpdateGamePATCH(Games_Model storedgame, Games_Dto updatedgame)
        {


            updatedgame.Adapt(storedgame);
            storedgame.updated_at = DateTime.Now;


            await _db.SaveChangesAsync(); //BTW! this savechanges method is not only saving the data directly to the sql server but also checks if there any attribute that is conflicting with the new changes, i mean the attubutes of the main model to be clear not the DTOs! like if there is a [KEY] attribute that's mean this field is the primary key so it blocks any number that came from the user, like if there is 2 records currently and the user wanted to change the ID value in the patch endpont then this number will be ignored and the whole request will be ignored.





        }

    }
}

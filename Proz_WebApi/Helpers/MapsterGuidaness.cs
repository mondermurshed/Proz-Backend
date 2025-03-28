using Mapster;
using Proz_WebApi.Models;
using Proz_WebApi.Models.Dto;
using System.Runtime.Intrinsics.X86;

public static class Maps
{
    public static void RegisterMappings()
    {
        //Once we call this method (for example we call in the program.cs because the program.cs is considered our Main) then all the configuration that we have defined so far in here will become part of the global settings of the mapster package.  This means that any time you call Adapt<>() or ProjectToType<>() in your code, Mapster uses these pre-configured mappings automatically as the global settings.
        //Some advice to you about mapster package, use Adapt<> for mapping objects already in memory, and ProjectToType<> when you want to tell your query provider (like EF Core) to only select the fields you need from the database as part of your database query that the EF will sent to sql server.
        //But because we are using Microsoft EF package to manage our data from our database then we should use ProjectToType<> as long as we could! (it's doesn't map two normal objects though, it maps a database object (from sql server) to another DTO object) because as we said ProjectToType<> is sending sql query to EF to tell EF to bring only the needed fields from the rows to complete the mapping process and not bringing useless properties that will not be using. If you wanna map two objects though you can freely (that doesn't related to database) use the Adapt<> method.
        //Now we have three ways to use the Adapt method, we choose one based on our needs but all will do the same goal which is mapping :
        //1 - updateGameDto.Adapt(game);  //this will map from the source object "updateGameDto" to the dect "game" object without creating any other object (this way is really fast and efficint)
        //2 - game = updateGameDto.Adapt(game); //this way doing the same exact same but, it first map the values from the "updateGameDto" as a src to a new object (that it will create it) as a des, then from the new object that has been created as a src to the game object as a des (same as the previous way but it will create a new object in the middle of the process)
        //3 - Games_Model ModelToUpdate = game.Adapt<Games_Model>(); //we use this way when we first want to create an object, then map this object to the src, like the ModelToUpdate is our new and desc object and our game object is the src here.
        TypeAdapterConfig<Games_Model, Games_Dto2>.NewConfig()
            .Map(dest => dest.id, src => src.id)
            .Map(dest => dest.name, src => src.name)
            .Map(dest => dest.description, src => src.description)
            .Map(dest => dest.sold_copies, src => src.sold_copies)
            .Map(dest => dest.created_at, src => src.created_at)
            .Map(dest => dest.updated_at, src => src.updated_at)
            .Map(dest => dest.price, src => src.price).IgnoreNonMapped(true);

             TypeAdapterConfig<Games_Model, Games_Dto>.NewConfig()
            .Map(dest => dest.name, src => src.name)
            .Map(dest => dest.description, src => src.description)
            .Map(dest => dest.sold_copies, src => src.sold_copies)
            .Map(dest => dest.price, src => src.price).IgnoreNonMapped(true);
          
             TypeAdapterConfig<Games_Dto, Games_Model>.NewConfig()
            .Map(dest => dest.name, src => src.name)
            .Map(dest => dest.price, src => src.price)
            .Map(dest => dest.sold_copies, src => src.sold_copies)
            .Map(dest => dest.description, src => src.description).IgnoreNonMapped(true);

             TypeAdapterConfig<UserRegisteration, UserLogin>.NewConfig()
            .Map(dest => dest.Username, src => src.Username)
            .Map(dest => dest.Password, src => src.Password).IgnoreNonMapped(true);


    }
}
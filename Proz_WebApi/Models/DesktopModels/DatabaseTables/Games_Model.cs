using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{

    public class Games_Model
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        //public Guid Id { get; set; } = Guid.NewGuid(); you may use the GUID which stands for (global unique identitfier) and it's more security efficient but not performance efficent like INTs datatypes. read about them, if you want you table to be smooth or it's used a lot by the users, for your performance use INTs! but if you data inside a table may expose to other systems and you want to protect it use the GUID.
        public string name { get; set; }

        public float price { get; set; }

        public int sold_copies { get; set; }
        public string description { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        [Timestamp]
        public byte[] Version { get; set; }
    }
    //here is the possible ORM attributes (data annotations) for database mapping that you can use  (noticed that these and data Validations are completely differ because data Validations are meant to be used for DTOs (data transfer objects) and not for models, for models we use something called ORM attributes (data annotations) to desgin our database with using just C# and without touching the database MANAGEMENT SYSTEM (SSMS) :
    /* 1-[Key] //sets the column as the primary key for the table
     * 2- [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //commonly used for primary keys to increase them automatically in the database when new record is stored.
     * 3-[Required] //basically is setting the column to not null. So it does the following ❌ SQL Server will also say "this cannot be NULL" ❌ User can’t leave it empty (in forms / API). If you wanna make a type to me null and sql write nullable then do "public string? Name { get; set; }" for example, make a '?' at the end of any type you want. This will do ✅ User can leave it empty ✅ SQL Server allows it to be NULL. Here is a big information for you, for value type like  ( int, double, DateTime, bool, Guid) are always required and not null in the database by default, to make them null you should you need to put '?' at the type. for reference types (string, class, object) these are null by default (you can add the '?' but it's optional for readability only by default it IS null no need for '?') but if you don't want reference types to be null then put the [required] attrubites above the reference type. You can still using '?' or [required] at any type you want to allow null or not allow null. Another thing is that values types like int,fload, bool are all struct meaning they have a null value (so the struct can contain null but the null value !=  the default value meaning to make an int variable 0 doesn't need from you to make it null because null means NOTHING)
     * 4-[MaxLength(100)] //Sets the maximum length for a string property
     * 5-[StringLength(100, MinimumLength = 3)] //Similar to [MaxLength] but also allows specifying minimum length.
     * 6-[Column("YOUR_COLUMN_NAME")] //Maps a property to a specific database column name, so if you don't wanna use the name of the property as the column name then use this.
     * 7-[Table("YOUR TABLE NAME")] //similar as the previous [column] attribute but this is to set the whole table, so you need to put it above the class name.
     * 8-[NotMapped] Tells EF Core not to map a property to a database column. Useful for properties that exist in your C# model but should not be stored in the database (e.g., calculated fields). Like for example when you want to add a property inside the model for specific logic purpose or something (like a helper property) but you don't want to join it as a database column in the table then add this attribute above the property.
     * 9-[Unicode] //this is used for strings in which will make any string column accepts unicode letters/digits/etc.. (e.g., NVARCHAR in SQL Server). In small words will use the NVARCHAR type rather then then VARCHAR type.
     * 10-[Precision(18, 2)] //this will decides the size that it can holds for the database columns. (this is for double and any type with Decimal places
     * 11-[Column(TypeName = "long")] //normally just changing the data type from c# will automaticlly will convert it to the desier datatype in the database, like you want long database you can change your c# property to long and when add migration it will automatically place to bigint in sqlserver for example, but if you really want to ou want to explicitly specify the SQL type that in your mind (you will not use this for like 99% of the operations)
       12-[Timestamp] //i think this is the most important one so far! when you have two users for example and these two are tring to change the same single row in the database, the first one to save his changes will success but the other will not be able to commit his change because the value that he has is only (he doesn't got the lastest version of the value). Do you remember git ? it's the same thing it prevent any conflict between the users. How it works ? so first of all create this property (public byte[] Version { get; set; } also put [Timestamp] attribute above it, This property will hold the "version number" of every row. and you should put this one property for every entity (not DTO but entities only). Example of the version data is 0x00000000000007D1 and if a value was change of this row it will be 0x00000000000007D2 for example and everything is done automatically using the Microsoft EF for you and you don't need to touch the database management system. Also we don't use it for every entity for all the type of projects! only the projects that the users may conflict with each other. Also one last thing, this column is Used for concurrency control (like a version number) and Entity Framework automatically updates it when the row changes. So DO NOT ADD [NotMapped] besides it because Tells EF not to include this property in the database. So if you use both together, you cancel out the [Timestamp] behavior — EF won’t even track the column!
    We use it only for the tables that are  Editable user content like frequently updated/shared tables (there is chance that more than one person edit this table), Financial/sensitive data (tables that are really really really important to protect and you don't want any errors even by admins). We don't use it for  Read-only or rarely updated data (tables that are meant to get information from and it's rarely to update a field inside them),  Logs or Append-only tables (tables that are meant to track or store some logs happened in the system) 
                                     
       //continue to [Timestamp] "so it works as the following, the client will first get the data from the database and when he get it he should get the version of the data as well (because each row will have its own version code) and when the user wanna update (or delete) a row he must pass his version of the data he has, if the version = the version that is in the database he can update or delete but if the version changed (the data changed) from the last time he fetch this data (that's mean he is not looking to the latest data) then the server should this return Conflict("This record has been modified. Refresh and try again."); which is 409 Conflict code.
    //another information is that EF Core will use Fluent API settings over data annotations if there’s any conflict (if there is two similar configuration that are places in the two places then EF will choose to take from the Fluent API.
    
    Also EVERYTHING we did so far with the built-in data annotations can be done with the Fluent API as well but not everything we can do with Fluent API can also be done in the data annotations because data annotations are basically attributes that was built using the Fluent API! 
    For example do you remember the [Table("GamesList")] that can change the name of the table that is showing in the database (if you want to keep your c# class name but wanna change the actual table that will be shown in the database) then we can do the exact same using Fluent API by using this command "builder.Entity<Games_Model>().ToTable("GamesList");"

     * 
     */


}

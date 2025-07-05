using System;
using FluentValidation;
using Proz_WebApi.Models;
using Proz_WebApi.Models.DesktopModels.Dto;

namespace Proz_WebApi.Validators.DesktopValidators
{
    public class GameValidator : AbstractValidator<Games_Dto>
    {
        public GameValidator()
        {
            RuleFor(user => user.name)

                .NotEmpty().WithMessage("Name is required.")
                //we didn't put the .MinimumLength(8).WithMessage("Name should be atleast 8 letters") because the name of the games in general may contain 3 letter like BF4 GAME! SO DON'T LIMIT IT.
                .MaximumLength(30).WithMessage("Name has reached the maximum number to be entered");


            // Rule 2: Email is required and valid
            RuleFor(user => user.price)
           .GreaterThanOrEqualTo(0).WithMessage("Price has a negetive number")
           .When(copies => copies.sold_copies < 20); //meaning that execute the rule above me (start to check all the content of an rule (all of them)) only if copies were indeed <20. So When works as a gate for a rule, if it was true then the compiler check the whole rule as well.

            RuleFor(user => user.price)
                .ScalePrecision(2, 18).WithMessage("Price can have up to 2 decimal places."); //this ScalePrecision method works only with decimal (i think so) and now it's telling that the number can be 2431 pr 161212612 or 3 pr 15.2 or 15.12 but NOT 15.125 because it accepts only 


            // Rule 3: Age must be positive
            RuleFor(user => user.sold_copies)
            .GreaterThanOrEqualTo(0).WithMessage("The number of copies is a negetive number");





            //        RuleFor(game => game.sold_copies)
            //.GreaterThanOrEqualTo(0)
            //.WithMessage("Sold copies cannot be negative")
            //.Must(copies => copies >= 0 && copies <= int.MaxValue)  this is an example of using the Must()
            //.WithMessage("Invalid sold copies value");

        }
    }
}
// noticed that the .Matches method that we will talk about works only for string property.
//  .Matches(@"^\d+$").WithMessage("Product code must contain only numbers.");  //this is used when you want your property to accept only numbers in which ^ is always putted in the start and $ is at the end.
//  .Matches(@"^\d{3}").WithMessage("First 3 characters must be numbers."); //this to tell it i want the property to start with 3 digits always.
//  .Matches(@"^[A-Za-z].*").WithMessage("Name must start with a letter."); //if i want my name to start atleast with a letter and then anything else, the '.' alonn means any characters (including numbers, spaces, etc.) and with the star (*) mean zero or one of any number.
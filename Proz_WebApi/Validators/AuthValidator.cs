using FluentValidation;
using Proz_WebApi.Models.Dto.Auth;

namespace Proz_WebApi.Validators
{
    public class UserRegisterationValidator : AbstractValidator<UserRegisteration>
    {
        public UserRegisterationValidator()
        {
            RuleFor(user => user.Username)
                //the difference betten null and empyt rules are that the NotNull() is dumb, it Applies to: Reference types (string, class, collections, etc.) but doesn't effect the value types. 
                //the NotEmpty() will make sure that the property has meaningful content. For example it can detect if the user entred whitespaces like "    " < this is unvalid. for value types (int, decimal, etc.): the default value (e.g., 0 for int) will also be unacceptable! 
                .NotEmpty().WithErrorCode("Username is empty!")
                .MinimumLength(8).WithErrorCode("Username should be atleast 8 characters!")
                .MaximumLength(20).WithErrorCode("Username has reached the maximum number to be entered which is 20 characters")
                .Matches(@"^[A-Za-z].*").WithErrorCode("Username should starts only with a letter.");

            RuleFor(user => user.Password)
    .NotEmpty().WithErrorCode("Password is empty!")
    .MinimumLength(10).WithErrorCode("Password should be atleast 10 characters!")
    .MaximumLength(28).WithErrorCode("Password has reached the maximum number to be entered which is 28 characters")
    .Matches(@"^[A-Za-z].*").WithErrorCode("Password should starts only with a letter.");

            RuleFor(user => user.Email)
            .NotEmpty().WithErrorCode("Email is empty!")
             .EmailAddress().WithErrorCode("Email is not in the correct format") //this only checks that there is exactly one @, not at the first or last position ONLY! ANYTHING OTHER THEN THAT CAN PASS through it
             .Matches(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$").WithErrorCode("Email is not in the correct format");


        }
    }

    public class UserLoginValidator : AbstractValidator<UserLogin>
    {
        public UserLoginValidator()
        {


        }
    }
}

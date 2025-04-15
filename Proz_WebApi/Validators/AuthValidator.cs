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
                .NotEmpty().WithMessage("Username is empty!")
                .MinimumLength(8).WithMessage("Username should be atleast 8 characters!")
                .MaximumLength(20).WithMessage("Username has reached the maximum number to be entered which is 20 characters")
                .Matches(@"^[A-Za-z].*").WithMessage("Username should starts only with a letter.");

            RuleFor(user => user.Password)
    .NotEmpty().WithMessage("Password is empty!")
    .MinimumLength(10).WithMessage("Password should be atleast 10 characters!")
    .MaximumLength(28).WithMessage("Password has reached the maximum number to be entered which is 28 characters")
    .Matches(@"^[A-Za-z].*").WithMessage("Password should starts only with a letter.");




        }
    }

    public class UserLoginValidator : AbstractValidator<UserLogin>
    {
        public UserLoginValidator()
        {


        }
    }
}

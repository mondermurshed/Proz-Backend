using FluentValidation;
using Proz_WebApi.Models.DesktopModels.Dto.Auth;
using Proz_WebApi.Models.DesktopModels.DTO.Admin;
using Proz_WebApi.Models.DesktopModels.DTO.Auth;

namespace Proz_WebApi.Validators.DesktopValidators
{
    public class UserRegisterationValidator : AbstractValidator<UserRegisteration>
    {
        public UserRegisterationValidator()
        {
            RuleFor(p => p.Username)
                //the difference betten null and empyt rules are that the NotNull() is dumb, it Applies to: Reference types (string, class, collections, etc.) but doesn't effect the value types. 
                //the NotEmpty() will make sure that the property has meaningful content. For example it can detect if the user entred whitespaces like "    " < this is unvalid. for value types (int, decimal, etc.): the default value (e.g., 0 for int) will also be unacceptable! 
                .NotEmpty().WithErrorCode("Username is empty!")
                .MinimumLength(8).WithErrorCode("Username should be atleast 8 characters!")
                .MaximumLength(25).WithErrorCode("Username has reached the maximum number to be entered which is 25 characters")
                .Matches(@"^[A-Za-z].*").WithErrorCode("Username should starts only with a letter.");

            RuleFor(p => p.Password)
    .NotEmpty().WithErrorCode("Password is empty!")
    .MinimumLength(10).WithErrorCode("Password should be atleast 10 characters!")
    .MaximumLength(30).WithErrorCode("Password has reached the maximum number to be entered which is 30 characters")
    .Matches(@"^[A-Za-z].*").WithErrorCode("Password should starts only with a letter.");

            RuleFor(p => p.Email)
            .NotEmpty().WithErrorCode("Email is empty!")
             .EmailAddress().WithErrorCode("Email is not in the correct format") //this only checks that there is exactly one @, not at the first or last position ONLY! ANYTHING OTHER THEN THAT CAN PASS through it
             .Matches(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$").WithErrorCode("Email is not in the correct format");


            RuleFor(p => p.FullName)
.NotEmpty().WithMessage("Full name is empty!")
.MinimumLength(8).WithMessage("Full name field should be atleast 6 characters!")
.MaximumLength(25).WithMessage("Full name has reached the maximum number to be entered which is 100 characters")
.Matches("^[a-zA-Z ]+$").WithMessage("Full name must contain only letters and spaces.");

            RuleFor(p => p.Age)
  .NotEmpty().WithErrorCode("Age is required.")
   .InclusiveBetween(10, 99)
    .WithErrorCode("Age must be exactly two digits.");

            RuleFor(x => x.Gender)
        .NotEmpty().WithMessage("Gender is required.")
      .Must(value => new[] { "Male", "Female" }.Contains(value))
      .WithMessage("Gender must either be 'Male' or 'Female'");

            RuleFor(p => p.Date_Of_Birth)
           .NotEmpty().WithErrorCode("Date of birth you entered is empty!");

            RuleFor(p => p.Nationality)
            .MinimumLength(3).WithErrorCode("Nationality's value has to be 3 or more letters")
.MaximumLength(25).WithErrorCode("Nationality's value has reached the maximum number to be entered which is 30 characters")
.Matches("^[a-zA-Z ]+$").WithErrorCode("Nationality must be a text that contains only letters and spaces.")
     .When(x => !string.IsNullOrWhiteSpace(x.Nationality));

        }
    }
    public class UserRegisterationFinalValidator : AbstractValidator<UserRegistrationFinal>
    {
        public UserRegisterationFinalValidator()
        {
            RuleFor(p => p.Email)
               .NotEmpty().WithErrorCode("Email is empty!")
             .EmailAddress().WithErrorCode("Email is not in the correct format") //this only checks that there is exactly one @, not at the first or last position ONLY! ANYTHING OTHER THEN THAT CAN PASS through it
             .Matches(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$").WithErrorCode("Email is not in the correct format");

            RuleFor(p => p.Code)
           .Matches(@"^(?!0)\d{6}$").WithErrorCode("Code must be 6 digits and between 100000 - 999999 only");

       
        }

    }



        public class ChangeMyGettingstartedDTOStageOne : AbstractValidator<GettingStartedStageOneDTO>
        {
            public ChangeMyGettingstartedDTOStageOne()
            {
                RuleFor(p => p.AdminUsername)
    //the difference betten null and empyt rules are that the NotNull() is dumb, it Applies to: Reference types (string, class, collections, etc.) but doesn't effect the value types. 
    //the NotEmpty() will make sure that the property has meaningful content. For example it can detect if the user entred whitespaces like "    " < this is unvalid. for value types (int, decimal, etc.): the default value (e.g., 0 for int) will also be unacceptable! 
    .NotEmpty().WithErrorCode("Username is empty!")
    .MinimumLength(8).WithErrorCode("Username should be atleast 8 characters!")
    .MaximumLength(25).WithErrorCode("Username has reached the maximum number to be entered which is 25 characters")
    .Matches(@"^[A-Za-z].*").WithErrorCode("Username should starts only with a letter.");

                RuleFor(p => p.AdminEmail)
              .NotEmpty().WithErrorCode("Email is empty!")
             .EmailAddress().WithErrorCode("Email is not in the correct format") //this only checks that there is exactly one @, not at the first or last position ONLY! ANYTHING OTHER THEN THAT CAN PASS through it
             .Matches(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$").WithErrorCode("Email is not in the correct format");


                RuleFor(p => p.AdminPassword)
.NotEmpty().WithErrorCode("Password is empty!")
.MinimumLength(10).WithErrorCode("Password should be atleast 10 characters!")
.MaximumLength(30).WithErrorCode("Password has reached the maximum number to be entered which is 30 characters")
.Matches(@"^[A-Za-z].*").WithErrorCode("Password should starts only with a letter.");



            RuleFor(p => p.CompanyName)
               .NotEmpty().WithErrorCode("Company Name is empty!")
               .MinimumLength(2).WithErrorCode("Company Name should be atleast 2 characters!")
               .MaximumLength(100).WithErrorCode("Company Name has reached the maximum number to be entered which is 100 characters");

            RuleFor(p => p.Currency)
       .NotEmpty().WithMessage("Currency's value is empty!")
       .Length(3).WithMessage("Currency must be exactly three characters.")
       .Must(value => new[] { "USD", "SAR", "YER" }.Contains(value))
           .WithMessage("Currency must be one of the following: USD, SAR, or YER.");


            RuleFor(p => p.PaymentFrequency)
                .NotEmpty().WithMessage("PaymentFrequency's value is empty!")
                .Must(value => new[] { "Every Month", "Every 3 weeks" }.Contains(value))
                    .WithMessage("PaymentFrequency must be either 'Every Month' or 'Every 3 weeks'.");

            RuleFor(x => x.Gender)
       .NotEmpty().WithMessage("Gender is required.")
     .Must(value => new[] { "Male", "Female" }.Contains(value))
     .WithMessage("Gender must either be 'Male' or 'Female'");

            RuleFor(p => p.FullName)
         .NotEmpty().WithErrorCode("Full name is empty!")
.MinimumLength(8).WithErrorCode("Full name field should be atleast 6 characters!")
.MaximumLength(25).WithErrorCode("Full name has reached the maximum number to be entered which is 100 characters")
.Matches("^[a-zA-Z ]+$").WithErrorCode("Full name must contain only letters and spaces.");

            RuleFor(x => x.Gender)
       .NotEmpty().WithMessage("Gender is required.")
       .Must(gender => gender == "Male" || gender == "Female")
       .WithMessage("Gender must be either 'male' or 'female'.");

            RuleFor(p => p.Age)
  .NotEmpty().WithErrorCode("Age is required.")
   .InclusiveBetween(10, 99)
    .WithErrorCode("Age must be exactly two digits.");


            RuleFor(p => p.Date_Of_Birth)
              .NotEmpty().WithErrorCode("Date of birth you entered is empty!");

            RuleFor(p => p.Nationality)
            .MinimumLength(3).WithErrorCode("Nationality's value has to be 3 or more letters")
.MaximumLength(25).WithErrorCode("Nationality's value has reached the maximum number to be entered which is 30 characters")
.Matches("^[a-zA-Z ]+$").WithErrorCode("Nationality must be a text that contains only letters and spaces.")
     .When(x => !string.IsNullOrWhiteSpace(x.Nationality));


        }
        }

        public class ChangeMyGettingstartedDTOStageTwo : AbstractValidator<GettingStartedStageTwoDTO>
        {
            public ChangeMyGettingstartedDTOStageTwo()
            {
                RuleFor(p => p.Code)
                 .Matches(@"^(?!0)\d{6}$").WithErrorCode("Code must be 6 digits and between 100000 - 999999 only");

                RuleFor(p => p.AdminEmail)
              .NotEmpty().WithErrorCode("Email is empty!")
             .EmailAddress().WithErrorCode("Email is not in the correct format") //this only checks that there is exactly one @, not at the first or last position ONLY! ANYTHING OTHER THEN THAT CAN PASS through it
             .Matches(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$").WithErrorCode("Email is not in the correct format");


               




            }



        public class UserLoginValidator : AbstractValidator<UserLogin>
        {
            public UserLoginValidator()
            {
                RuleFor(p => p.Username)
    //the difference betten null and empyt rules are that the NotNull() is dumb, it Applies to: Reference types (string, class, collections, etc.) but doesn't effect the value types. 
    //the NotEmpty() will make sure that the property has meaningful content. For example it can detect if the user entred whitespaces like "    " < this is unvalid. for value types (int, decimal, etc.): the default value (e.g., 0 for int) will also be unacceptable! 
    .NotEmpty().WithErrorCode("Username is empty!")
    .MinimumLength(8).WithErrorCode("Username/email should be atleast 8 characters!")
    .Matches(@"^[A-Za-z].*").WithErrorCode("Username should starts only with a letter.");

                RuleFor(p => p.Password)
    .NotEmpty().WithErrorCode("Password is empty!")
    .MinimumLength(10).WithErrorCode("Password should be atleast 10 characters!")
    .MaximumLength(30).WithErrorCode("Password has reached the maximum number to be entered which is 30 characters")
    .Matches(@"^[A-Za-z].*").WithErrorCode("Password should starts only with a letter.");
            }
        }


        public class ChangeMyUsername : AbstractValidator<ChangeUsernameDTO>
        {
            public ChangeMyUsername()
            {
                RuleFor(p => p.NewUsername)
                    //the difference betten null and empyt rules are that the NotNull() is dumb, it Applies to: Reference types (string, class, collections, etc.) but doesn't effect the value types. 
                    //the NotEmpty() will make sure that the property has meaningful content. For example it can detect if the user entred whitespaces like "    " < this is unvalid. for value types (int, decimal, etc.): the default value (e.g., 0 for int) will also be unacceptable! 
                    .NotEmpty().WithErrorCode("Username is empty!")
                    .MinimumLength(8).WithErrorCode("Username should be atleast 8 characters!")
                    .MaximumLength(25).WithErrorCode("Username has reached the maximum number to be entered which is 25 characters")
                    .Matches(@"^[A-Za-z].*").WithErrorCode("Username should starts only with a letter.");






            }

            public class ChangeMyPassword : AbstractValidator<ChangePasswordDTO>
            {
                public ChangeMyPassword()
                {
                    RuleFor(p => p.NewPassword)
            .NotEmpty().WithErrorCode("Password is empty!")
            .MinimumLength(10).WithErrorCode("Password should be atleast 10 characters!")
            .MaximumLength(30).WithErrorCode("Password has reached the maximum number to be entered which is 30 characters")
            .Matches(@"^[A-Za-z].*").WithErrorCode("Password should starts only with a letter.");






                }
            }
        }
    }
}

using FluentValidation;
using Proz_WebApi.Models.DesktopModels.Dto.Auth;
using Proz_WebApi.Models.DesktopModels.DTO.Admin;

namespace Proz_WebApi.Validators.DesktopValidators
{
    public class CreateFeedbackType : AbstractValidator<CreateAFeedbackTypeRequest>
    {
        public CreateFeedbackType()
        {
            RuleFor(p => p.feedbackTypeName)
                //the difference betten null and empyt rules are that the NotNull() is dumb, it Applies to: Reference types (string, class, collections, etc.) but doesn't effect the value types. 
                //the NotEmpty() will make sure that the property has meaningful content. For example it can detect if the user entred whitespaces like "    " < this is unvalid. for value types (int, decimal, etc.): the default value (e.g., 0 for int) will also be unacceptable! 
                .NotEmpty().WithErrorCode("Feedback Name is empty!")
                .MinimumLength(3).WithErrorCode("Feedback Name should be atleast 3 characters!")
                .MaximumLength(30).WithErrorCode("Feedback Name has reached the maximum number to be entered which is 30 characters")
               .Matches("^(?!.* {2,})[a-zA-Z ]+$").WithErrorCode("Feedback Name must contain only letters and spaces.");

        }
    }
  
    public class DeleteFeedbackType : AbstractValidator<RemoveFeedbackTypeDTO>
    {
        public DeleteFeedbackType()
        {
            RuleFor(x => x.ReplaceWith)
      .MinimumLength(3).WithMessage("Must be at least 3 characters.")
      .MaximumLength(30).WithMessage("Must be at most 30 characters.")
      .Matches("^(?!.* {2,})[a-zA-Z ]+$").WithErrorCode("The string you want to replace the feedback type with is not valid. Please try to clear it from digits.")
      .When(x => !string.IsNullOrWhiteSpace(x.ReplaceWith));

        }
    }

    public class CreateADepartment : AbstractValidator<DepartmentCreatingRequest>
    {
        public CreateADepartment()
        {
            RuleFor(p => p.DepartmentName)
                      //the difference betten null and empyt rules are that the NotNull() is dumb, it Applies to: Reference types (string, class, collections, etc.) but doesn't effect the value types. 
                      //the NotEmpty() will make sure that the property has meaningful content. For example it can detect if the user entred whitespaces like "    " < this is unvalid. for value types (int, decimal, etc.): the default value (e.g., 0 for int) will also be unacceptable! 
                      .NotEmpty().WithErrorCode("Department Name is empty!")
                      .MinimumLength(3).WithErrorCode("Department Name should be atleast 5 characters!")
                      .MaximumLength(30).WithErrorCode("Department Name has reached the maximum number to be entered which is 35 characters")
                     .Matches("^(?!.* {2,})[a-zA-Z ]+$").WithErrorCode("Department Name must contain only letters and spaces.");

            
   

        }
    }
    public class UpdatingCompanyName : AbstractValidator<UpdateCompanyNameRequest>
    {
        public UpdatingCompanyName()
        {
            RuleFor(p => p.CompanyName)
                      //the difference betten null and empyt rules are that the NotNull() is dumb, it Applies to: Reference types (string, class, collections, etc.) but doesn't effect the value types. 
                      //the NotEmpty() will make sure that the property has meaningful content. For example it can detect if the user entred whitespaces like "    " < this is unvalid. for value types (int, decimal, etc.): the default value (e.g., 0 for int) will also be unacceptable! 
                      .NotEmpty().WithErrorCode("Company Name is empty!")
                      .MinimumLength(3).WithErrorCode("Company Name should be atleast 2 characters!")
                      .MaximumLength(30).WithErrorCode("Company Name has reached the maximum number to be entered which is 38 characters")
                    .Matches(@"^(?!.* {2,})[a-zA-Z][a-zA-Z0-9 ]*$").WithErrorCode("Company Name must contain only letters and spaces.");




        }
    }
    
}

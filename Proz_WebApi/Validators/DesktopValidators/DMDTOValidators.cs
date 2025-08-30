using FluentValidation;
using Proz_WebApi.Models.DesktopModels.DTO.DepartmentManager;
using Proz_WebApi.Models.DesktopModels.DTO.Employee;

namespace Proz_WebApi.Validators.DesktopValidators
{
    public class CreateFeedbackAnswerDTO : AbstractValidator<AddAnAnswerForAFeedbackRequest>
    {
        public CreateFeedbackAnswerDTO()
        {
            RuleFor(p => p.Answer)
          //the difference betten null and empyt rules are that the NotNull() is dumb, it Applies to: Reference types (string, class, collections, etc.) but doesn't effect the value types. 
          //the NotEmpty() will make sure that the property has meaningful content. For example it can detect if the user entred whitespaces like "    " < this is unvalid. for value types (int, decimal, etc.): the default value (e.g., 0 for int) will also be unacceptable! 
          .NotEmpty().WithMessage("Feedback Answer Should NOT be empty!")
          .MinimumLength(25).WithMessage("Feedback Answer should be atleast 25 characters to be sent.")
          .MaximumLength(1500).WithMessage("Feedback Answer has reached the maximum number to be entered which is 1500 characters");





        }
    }

    public class CreateLeaveRequestAnswerDTO : AbstractValidator<LeaveRequestAcceptRejectRequest>
    {
        public CreateLeaveRequestAnswerDTO()
        {
            RuleFor(p => p.Comment)
          //the difference betten null and empyt rules are that the NotNull() is dumb, it Applies to: Reference types (string, class, collections, etc.) but doesn't effect the value types. 
          //the NotEmpty() will make sure that the property has meaningful content. For example it can detect if the user entred whitespaces like "    " < this is unvalid. for value types (int, decimal, etc.): the default value (e.g., 0 for int) will also be unacceptable! 
          .NotEmpty().WithMessage("Answer Should NOT be empty!")
          .MinimumLength(25).WithMessage("Answer should be atleast 25 characters to be sent.")
          .MaximumLength(1500).WithMessage("Answer has reached the maximum number to be entered which is 1500 characters");





        }
    }

    public class CreatePerformanceRecord : AbstractValidator<SubmitPerformanceAnswerRequest>
    {
        public CreatePerformanceRecord()
        {
            RuleFor(p => p.Comment)
          //the difference betten null and empyt rules are that the NotNull() is dumb, it Applies to: Reference types (string, class, collections, etc.) but doesn't effect the value types. 
          //the NotEmpty() will make sure that the property has meaningful content. For example it can detect if the user entred whitespaces like "    " < this is unvalid. for value types (int, decimal, etc.): the default value (e.g., 0 for int) will also be unacceptable! 
          .NotEmpty().WithMessage("Answer Should NOT be empty!")
          .MinimumLength(25).WithMessage("Answer should be atleast 25 characters to be sent.")
          .MaximumLength(125).WithMessage("Answer has reached the maximum number to be entered which is 125 characters");





        }
    }
}

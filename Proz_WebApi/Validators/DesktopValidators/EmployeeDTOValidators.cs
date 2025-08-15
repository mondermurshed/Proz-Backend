using FluentValidation;
using Proz_WebApi.Models.DesktopModels.DTO.Admin;
using Proz_WebApi.Models.DesktopModels.DTO.Employee;

namespace Proz_WebApi.Validators.DesktopValidators
{
    public class CreateFeedbackRequest : AbstractValidator<CreateANewFeedbackRequest_Request>
    {
        public CreateFeedbackRequest()
        {
            RuleFor(p => p.FeedbackTitle)
                      //the difference betten null and empyt rules are that the NotNull() is dumb, it Applies to: Reference types (string, class, collections, etc.) but doesn't effect the value types. 
                      //the NotEmpty() will make sure that the property has meaningful content. For example it can detect if the user entred whitespaces like "    " < this is unvalid. for value types (int, decimal, etc.): the default value (e.g., 0 for int) will also be unacceptable! 
                      .NotEmpty().WithMessage("Feedback Title Should NOT be empty!")
                      .MinimumLength(6).WithMessage("Feedback Title should be atleast 6 characters!")
                      .MaximumLength(100).WithMessage("Feedback Title has reached the maximum number to be entered which is 100 characters");

            RuleFor(p => p.FeedbackDescription)
          //the difference betten null and empyt rules are that the NotNull() is dumb, it Applies to: Reference types (string, class, collections, etc.) but doesn't effect the value types. 
          //the NotEmpty() will make sure that the property has meaningful content. For example it can detect if the user entred whitespaces like "    " < this is unvalid. for value types (int, decimal, etc.): the default value (e.g., 0 for int) will also be unacceptable! 
          .NotEmpty().WithMessage("Feedback Description Should NOT be empty!")
          .MinimumLength(45).WithMessage("Feedback Description should be atleast 45 characters to be sent.")
          .MaximumLength(1500).WithMessage("Feedback Description has reached the maximum number to be entered which is 1500 characters");



        }
    }


    public class CreateLeaveRequest : AbstractValidator<CreateANewLeaveRequest_Request>
    {
        public CreateLeaveRequest()
        {

            RuleFor(p => p.Reason)
          //the difference betten null and empyt rules are that the NotNull() is dumb, it Applies to: Reference types (string, class, collections, etc.) but doesn't effect the value types. 
          //the NotEmpty() will make sure that the property has meaningful content. For example it can detect if the user entred whitespaces like "    " < this is unvalid. for value types (int, decimal, etc.): the default value (e.g., 0 for int) will also be unacceptable! 
          .NotEmpty().WithMessage("Leave request message Should NOT be empty!")
          .MinimumLength(30).WithMessage("Leave request message should be atleast 30 characters to be sent.")
          .MaximumLength(500).WithMessage("Leave request message has reached the maximum number to be entered which is 500 characters");



        }
    }
}

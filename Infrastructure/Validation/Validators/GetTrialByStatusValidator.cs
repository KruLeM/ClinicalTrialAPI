using Domain.Entities;
using FluentValidation;
using Infrastructure.DTOs;

namespace Infrastructure.Validation.Validators
{
    public class GetTrialByStatusValidator : AbstractValidator<GetTrialByStatusDTO>
    {
        public GetTrialByStatusValidator()
        {
            RuleFor(x => x.Page)
               .GreaterThan(0)
               .When(x => x.Page.HasValue)
               .WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.Size)
               .GreaterThan(0)
               .LessThanOrEqualTo(100)
               .When(x => x.Size.HasValue)
               .WithMessage("Size must be between 1 and 100.");

            RuleFor(x => x.Status)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Status must be populated.")
                .Must(ValidateTrialStatus).WithMessage("Invalid status.");
        }

        private bool ValidateTrialStatus(string status)
        {
            try
            {
                return Enum.IsDefined(typeof(TrialStatus), status.Replace(" ", ""));
            }
            catch
            {
                return false;
            }
        }
    }
}

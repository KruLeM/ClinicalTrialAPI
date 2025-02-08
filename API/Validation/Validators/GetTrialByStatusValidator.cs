using Domain.Entities;
using FluentValidation;
using API.Validation.DTOs;

namespace API.Validation.Validators
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
                .Must(IsValidEnumTrialStatus).WithMessage("Invalid status.");
        }

        private bool IsValidEnumTrialStatus(string status)
        {
            return Enum.TryParse(typeof(TrialStatus), status.Replace(" ", ""), true, out _);
        }
    }
}

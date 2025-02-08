using Domain.Entities;
using FluentValidation;
using API.Validation.RequestModels;

namespace API.Validation.Validators
{
    public class GetTrialByStatusValidator : AbstractValidator<GetTrialByStatusRequestModel>
    {
        public GetTrialByStatusValidator()
        {
            RuleFor(x => x.Page)
               .GreaterThan(0)
               .When(x => x.Page.HasValue)
               .WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.Size)
                .Must(size => !size.HasValue || (size.Value > 0 && size.Value <= 100))
                .WithMessage("Size must be greather then 0 and less or equal to 100.");

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

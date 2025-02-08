using FluentValidation;
using API.Validation.RequestModels;

namespace API.Validation.Validators
{
    public class PaginationValidator : AbstractValidator<PaginationQueryRequestModel>
    {
        public PaginationValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .When(x => x.Page.HasValue)
                .WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.Size)
                .Must(size => !size.HasValue || (size.Value > 0 && size.Value <= 100))
                .WithMessage("Size must be greather then 0 and less or equal to 100.");
        }
    }
}

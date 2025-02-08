using FluentValidation;
using API.Validation.DTOs;

namespace API.Validation.Validators
{
    public class PaginationValidator : AbstractValidator<PaginationQueryDTO>
    {
        public PaginationValidator()
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
        }
    }
}

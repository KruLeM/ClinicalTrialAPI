
using API.Validation.RequestModels;
using API.Validation.Validators;
using FluentValidation.TestHelper;

namespace API.Test.Validation.Validators
{
    public class GetTrialByStatusValidatorTests
    {
        private readonly GetTrialByStatusValidator _validator;

        public GetTrialByStatusValidatorTests()
        {
            _validator = new GetTrialByStatusValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Page_Is_Less_Than_One()
        {
            var model = new GetTrialByStatusRequestModel { Page = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Page)
                  .WithErrorMessage("Page number must be greater than 0.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Page_Is_Greater_Than_Zero()
        {
            var model = new GetTrialByStatusRequestModel { Page = 1 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Page);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(101)]
        public void Should_Have_Error_When_Size_Is_Invalid(int size)
        {
            var model = new GetTrialByStatusRequestModel { Size = size };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Size)
                  .WithErrorMessage("Size must be greather then 0 and less or equal to 100.");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        public void Should_Not_Have_Error_When_Size_Is_Valid(int size)
        {
            var model = new GetTrialByStatusRequestModel { Size = size };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Size);
        }

        [Fact]
        public void Should_Have_Error_When_Status_Is_Null()
        {
            var model = new GetTrialByStatusRequestModel { Status = null };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Status)
                  .WithErrorMessage("Status must be populated.");
        }

        [Theory]
        [InlineData("NotStarted")]
        [InlineData("Not Started")]
        [InlineData("Ongoing")]
        [InlineData("Completed")]
        public void Should_Not_Have_Error_When_Status_Is_Valid(string status)
        {
            var model = new GetTrialByStatusRequestModel { Status = status };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Status);
        }

        [Theory]
        [InlineData("InvalidStatus")]
        [InlineData("Unknown Status")]
        public void Should_Have_Error_When_Status_Is_Invalid(string status)
        {
            var model = new GetTrialByStatusRequestModel { Status = status };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Status)
                  .WithErrorMessage("Invalid status.");
        }
    }
}

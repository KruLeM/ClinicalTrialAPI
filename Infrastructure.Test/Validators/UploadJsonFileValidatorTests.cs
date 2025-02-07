using FluentValidation.TestHelper;
using Infrastructure.DTOs;
using Infrastructure.Validation.Validators;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;


namespace Infrastructure.Test.Validators
{
    public class UploadJsonFileValidatorTests
    {
        private readonly UploadJsonFileValidator _validator;

        public UploadJsonFileValidatorTests()
        {
            _validator = new UploadJsonFileValidator();
        }

        private IFormFile CreateMockFile(string content, string fileName = "test.json", long length = 1024)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var mockFile = new Mock<IFormFile>();

            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(length);
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Returns((Stream target, CancellationToken _) => stream.CopyToAsync(target));

            return mockFile.Object;
        }

        [Fact]
        public void Should_Have_Error_When_File_Is_Null()
        {
            // Arrange
            var model = new UploadJsonFileRequestDTO();

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.File)
                  .WithErrorMessage("File is required.");
        }

        [Fact]
        public void Should_Have_Error_When_File_Is_Empty()
        {
            // Arrange
            var file = CreateMockFile("", length: 0);
            var model = new UploadJsonFileRequestDTO { File = file };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.File)
                  .WithErrorMessage("File is empty.");
        }

        [Fact]
        public void Should_Have_Error_When_File_Size_Exceeds_Limit()
        {
            // Arrange
            var file = CreateMockFile("{}", length: 3000);
            var model = new UploadJsonFileRequestDTO { File = file };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.File)
                  .WithErrorMessage("File exceeds the maximum allowed size.");
        }

        [Fact]
        public void Should_Have_Error_When_File_Is_Not_Json()
        {
            // Arrange
            var file = CreateMockFile("<xml></xml>", fileName: "test.xml");
            var model = new UploadJsonFileRequestDTO { File = file };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.File)
                  .WithErrorMessage("Only .json files are allowed.");
        }

        [Fact]
        public void Should_Have_Error_When_Invalid_Json_Format()
        {
            // Arrange
            var file = CreateMockFile("{ invalid json }");
            var model = new UploadJsonFileRequestDTO { File = file };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.File)
                  .WithErrorMessage("Invalid JSON format according to schema.");
        }

        [Fact]
        public void Should_Pass_When_Valid_Json_File()
        {
            // Arrange
            var validJson = "{ \"trialId\": \"123e\", \"title\": \"Test 1b\",\"startDate\": \"2025-02-07\",\"participants\": 15,\"status\": \"Not Started\" }";
            var file = CreateMockFile(validJson);
            var model = new UploadJsonFileRequestDTO { File = file };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

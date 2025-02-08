using API.Controllers;
using Application.Commands;
using Application.DTOs;
using Application.Queries;
using API.Validation.RequestModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Text;
using Application.Responses;
using FluentAssertions;

namespace API.Test.Controllers
{

    public class ClinicalTrialControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<ClinicalTrialController>> _loggerMock;
        private readonly ClinicalTrialController _controller;

        public ClinicalTrialControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<ClinicalTrialController>>();
            _controller = new ClinicalTrialController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddMedicalTrial_ReturnsCreatedResult()
        {
            //Arrange
            var trialId = Guid.NewGuid().ToString();
            var request = CreateUploadJsonFileRequestDTO(new AddTrialCommand(
               trialId,
                "Test Trial",
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5),
                null,
                It.IsAny<int>(),
                "NotStarted"
            ));
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddTrialCommand>(), default))
                .ReturnsAsync(new ClinicalTrialDTO { TrialId = trialId });

            //Act
            var result = await _controller.AddMedicalTrial(request);

            //Asert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetTrial", createdResult.ActionName);
        }
        [Fact]
        public async Task UpdateMedicalTrial_ReturnsOkResult()
        {
            //Arrange
            var trialId = Guid.NewGuid().ToString();
            var request = CreateUploadJsonFileRequestDTO(new UpdateTrialCommand(
              trialId,
               "Test Trial",
               DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5),
               null,
               It.IsAny<int>(),
               "NotStarted"
           ));
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateTrialCommand>(), default))
                .ReturnsAsync(new ClinicalTrialDTO { TrialId = trialId });

            //Act
            var result = await _controller.UpdateMedicalTrial(request);

            //Asert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetAllTrials_ReturnsOkResult_WithData()
        {
            // Arrange
            var mockData = new PaginatedResponse<ClinicalTrialDTO>(new List<ClinicalTrialDTO>(), 0, null, null);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetTrialsQuery>(), default))
                         .ReturnsAsync(mockData);
            PaginationQueryRequestModel request = new PaginationQueryRequestModel();

            // Act
            var result = await _controller.GetAllTrials(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(mockData, okResult.Value);
        }
        [Fact]
        public async Task GetAllTrials_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetTrialsQuery>(), default))
                         .ThrowsAsync(new Exception("Database error"));
            PaginationQueryRequestModel request = new PaginationQueryRequestModel();

            // Act
            var result = await _controller.GetAllTrials(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while processing your request.", statusCodeResult.Value);
        }

        [Fact]
        public async Task GetTrial_ReturnsNotFound_WhenTrialDoesNotExist()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetTrialByTrialIdQuery>(), default))
                .ReturnsAsync((ClinicalTrialDTO)null);

            var result = await _controller.GetTrial("123");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetTrials_ReturnsOkResult_WithListOfTrials()
        {
            // Arrange
            int page = 1;
            int size = 10;
            var requestModel = new GetTrialByStatusRequestModel { Status = "Ongoing", Page = page, Size = size };
            var expectedResponse = new PaginatedResponse<ClinicalTrialDTO>(new List<ClinicalTrialDTO>(), It.IsAny<int>(), 1, 10);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetTrialByStatusQuery>(), default))
                         .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetTrials(requestModel);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedResponse);
        }
        [Fact]
        public async Task GetTrials_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var requestModel = new GetTrialByStatusRequestModel { Status = "Ongoing", Page = 1, Size = 10 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetTrialByStatusQuery>(), default))
                         .ThrowsAsync(new System.Exception());

            // Act
            var result = await _controller.GetTrials(requestModel);

            // Assert
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(500);
        }

        private UploadJsonFileRequestModel CreateUploadJsonFileRequestDTO(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var file = new FormFile(stream, 0, stream.Length, "file", "test.json");
            return new UploadJsonFileRequestModel { File = file };
        }
    }
}

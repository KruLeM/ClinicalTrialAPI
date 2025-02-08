using API.Controllers;
using Application.Commands;
using Application.DTOs;
using Application.Queries;
using Domain.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using API.Validation.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using Newtonsoft.Json;
using System.Text;

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
        public async Task GetAllTrials_ReturnsOkResult()
        {
            //Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetTrialsQuery>(), default))
            .ReturnsAsync(new List<ClinicalTrialDTO> { new ClinicalTrialDTO { TrialId = "123" } });

            //Act
            var result = await _controller.GetAllTrials();

            //Asert
            Assert.IsType<OkObjectResult>(result);
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
        public async Task GetTrials_ReturnsBadRequest_WhenStatusIsInvalid()
        {
            var result = await _controller.GetTrials("InvalidStatus");

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid parameter", badRequestResult.Value);
        }

        private UploadJsonFileRequestDTO CreateUploadJsonFileRequestDTO(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var file = new FormFile(stream, 0, stream.Length, "file", "test.json");
            return new UploadJsonFileRequestDTO { File = file };
        }
    }
}

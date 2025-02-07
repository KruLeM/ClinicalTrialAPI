using Application.Exceptions;
using Application.Handlers.QueryHandlers;
using Application.Queries;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Test.Handlers.QueryHandlers
{
    public class GetTrialsHandlerTests
    {
        private readonly Mock<IQueryRepository<ClinicalTrial>> _mockRepository;
        private readonly Mock<ILogger<GetTrialsHandler>> _mockLogger;
        private readonly GetTrialsHandler _handler;

        public GetTrialsHandlerTests()
        {
            _mockRepository = new Mock<IQueryRepository<ClinicalTrial>>();
            _mockLogger = new Mock<ILogger<GetTrialsHandler>>();
            _handler = new GetTrialsHandler(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnClinicalTrialDTOs_WhenRepositoryReturnsData()
        {
            // Arrange
            var trials = new List<ClinicalTrial>
            {
                new ClinicalTrial {
                    TrialId = Guid.NewGuid().ToString(),
                    Title = "Trial 1",
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5),
                    Participants = It.IsAny<int>(),
                    Status = TrialStatus.NotStarted
                },
                new ClinicalTrial {
                    TrialId = Guid.NewGuid().ToString(),
                    Title = "Trial 2",
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(2),
                    Participants = It.IsAny<int>(),
                    Status = TrialStatus.Ongoing,
                    Duration = 30
                }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(trials);

            // Act
            var result = await _handler.Handle(new GetTrialsQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Handle_ShouldThrowRepositoryException_WhenRepositoryThrowsRepositoryException()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new RepositoryException("Repository error"));

            // Act & Assert
            await Assert.ThrowsAsync<RepositoryException>(async () =>
                await _handler.Handle(new GetTrialsQuery(), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldLogErrorAndThrowException_WhenUnexpectedExceptionOccurs()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("Unexpected error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _handler.Handle(new GetTrialsQuery(), CancellationToken.None));

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Exception occured in handler")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ), Times.Once);
        }
    }
}

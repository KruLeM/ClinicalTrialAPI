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
    public class GetTrialByTrialIdHandlerTests
    {
        private readonly Mock<IQueryRepository<ClinicalTrial>> _mockQueryRepository;
        private readonly Mock<ILogger<GetTrialByTrialIdHandler>> _mockLogger;
        private readonly GetTrialByTrialIdHandler _handler;

        public GetTrialByTrialIdHandlerTests()
        {
            _mockQueryRepository = new Mock<IQueryRepository<ClinicalTrial>>();
            _mockLogger = new Mock<ILogger<GetTrialByTrialIdHandler>>();
            _handler = new GetTrialByTrialIdHandler(_mockQueryRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ReturnsClinicalTrialDTO_WhenTrialExists()
        {
            // Arrange
            var trialId = Guid.NewGuid().ToString();
            var trial = new ClinicalTrial { TrialId = trialId, Title = "Trial 1" };
            var query = new GetTrialByTrialIdQuery(trialId);

            _mockQueryRepository.Setup(repo => repo.GetByIdAsync(trialId))
                .ReturnsAsync(trial);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(trialId, result.TrialId);
            Assert.Equal("Trial 1", result.Title);
        }

        [Fact]
        public async Task Handle_ThrowsRepositoryException_WhenRepositoryFails()
        {
            // Arrange
            var trialId = Guid.NewGuid().ToString();
            var query = new GetTrialByTrialIdQuery(trialId);

            _mockQueryRepository.Setup(repo => repo.GetByIdAsync(trialId))
                .ThrowsAsync(new RepositoryException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<RepositoryException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_LogsErrorAndThrowsException_WhenUnexpectedExceptionOccurs()
        {
            // Arrange
            var trialId = Guid.NewGuid().ToString();
            var query = new GetTrialByTrialIdQuery(trialId);

            _mockQueryRepository.Setup(repo => repo.GetByIdAsync(trialId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Unexpected error", exception.Message);

            _mockLogger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, type) => state.ToString().Contains("Exception occured in handler")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once);
        }
    }
}

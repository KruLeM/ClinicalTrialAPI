using Application.Exceptions;
using Application.Handlers.QueryHandlers;
using Application.Queries;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

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
        public async Task Handle_ReturnsPaginatedResponse_WhenDataExists()
        {
            // Arrange
            var page = 1;
            var size = 10;
            var query = new GetTrialsQuery(page, size);
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
            var count = trials.Count;

            _mockRepository.Setup(repo => repo.GetAllAsync(page, size)).ReturnsAsync(trials);
            _mockRepository.Setup(repo => repo.GetAllCountAsync()).ReturnsAsync(count);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(count, result.Data.Count());
            Assert.Equal(count, result.TotalCount);
        }

        [Fact]
        public async Task Handle_ShouldThrowRepositoryException_WhenRepositoryThrowsRepositoryException()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllAsync(null, null)).ThrowsAsync(new RepositoryException("Repository error"));

            // Act & Assert
            await Assert.ThrowsAsync<RepositoryException>(async () =>
                await _handler.Handle(new GetTrialsQuery(null, null), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldLogErrorAndThrowException_WhenUnexpectedExceptionOccurs()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllAsync(null, null)).ThrowsAsync(new Exception("Unexpected error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _handler.Handle(new GetTrialsQuery(null, null), CancellationToken.None));

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

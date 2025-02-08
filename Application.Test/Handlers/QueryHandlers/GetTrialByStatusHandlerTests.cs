using Application.Exceptions;
using Application.Handlers.QueryHandlers;
using Application.Queries;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Test.Handlers.QueryHandlers
{
    public class GetTrialByStatusHandlerTests
    {
        private readonly Mock<IQueryRepository<ClinicalTrial>> _queryRepositoryMock;
        private readonly Mock<ILogger<GetTrialByStatusHandler>> _loggerMock;
        private readonly GetTrialByStatusHandler _handler;

        public GetTrialByStatusHandlerTests()
        {
            _queryRepositoryMock = new Mock<IQueryRepository<ClinicalTrial>>();
            _loggerMock = new Mock<ILogger<GetTrialByStatusHandler>>();
            _handler = new GetTrialByStatusHandler(_queryRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsClinicalTrialDTOs_WhenRepositoryReturnsData()
        {
            //TODO RMladen - prvi objekat u listi ima status NotStarted i ne bi trebalo da ga prikaze u result...

            // Arrange
            var status = "Ongoing";
            var page = 1;
            var size = 10;
            var query = new GetTrialByStatusQuery(status, page, size);
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

            _queryRepositoryMock.Setup(repo => repo.GetByStatusAsync(status, page, size)).ReturnsAsync(trials);
            _queryRepositoryMock.Setup(repo => repo.GetCountByStatusAsync(status)).ReturnsAsync(count);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(count, result.TotalCount);
            Assert.Single(result.Data);
        }

        [Fact]
        public async Task Handle_ThrowsRepositoryException_WhenRepositoryFails()
        {
            // Arrange
            var status = "Active";
            var query = new GetTrialByStatusQuery(status, null, null);
            _queryRepositoryMock.Setup(repo => repo.GetByStatusAsync(status, null, null)).ThrowsAsync(new RepositoryException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<RepositoryException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_LogsErrorAndThrowsException_WhenUnexpectedExceptionOccurs()
        {
            // Arrange
            var status = "Active";
            var query = new GetTrialByStatusQuery(status, null, null);
            _queryRepositoryMock.Setup(repo => repo.GetByStatusAsync(status, null, null)).ThrowsAsync(new Exception("Unexpected error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((obj, type) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((obj, ex) => true)),
                Times.Once);
        }
    }
}

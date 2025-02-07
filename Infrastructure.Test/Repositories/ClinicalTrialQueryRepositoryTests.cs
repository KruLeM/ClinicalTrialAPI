using Application.Exceptions;
using Domain.Entities;
using Infrastructure.Persistance;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infrastructure.Test.Repositories
{
    public class ClinicalTrialQueryRepositoryTests
    {
        private readonly ClinicalTrialQueryRepository _repository;
        private readonly AppDbContext _dbContext;
        private readonly Mock<ILogger<ClinicalTrialQueryRepository>> _mockLogger;

        public ClinicalTrialQueryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AppDbContext(options);
            _mockLogger = new Mock<ILogger<ClinicalTrialQueryRepository>>();
            _repository = new ClinicalTrialQueryRepository(_dbContext, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTrials()
        {
            // Arrange
            var trials = new List<ClinicalTrial>
            {
                new ClinicalTrial { TrialId = "Test1", Title = "Trial 1", StartDate = new DateOnly(2025,02,06) , Status = TrialStatus.NotStarted },
                new ClinicalTrial { TrialId = "Test2", Title = "Trial 2", StartDate = new DateOnly(2025,02,06), EndDate = new DateOnly(2025,03,06), Participants = 15, Status = TrialStatus.Ongoing, Duration = 30 }
            };
            await _dbContext.ClinicalTrials.AddRangeAsync(trials);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectTrial()
        {
            // Arrange
            var trial = new ClinicalTrial { TrialId = "Test1", Title = "Trial 1", StartDate = new DateOnly(2025, 03, 06), Status = TrialStatus.NotStarted, Participants = 5 };
            await _dbContext.ClinicalTrials.AddAsync(trial);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync("Test1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test1", result.TrialId);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Act
            var result = await _repository.GetByIdAsync("NonExistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByStatusAsync_ShouldReturnTrialsWithGivenStatus()
        {
            // Arrange
            var trials = new List<ClinicalTrial>
            {
                new ClinicalTrial { TrialId = "Test1", Title = "Trial 1", StartDate = new DateOnly(2025,03,06), Participants = 50, Status = TrialStatus.NotStarted },
                new ClinicalTrial { TrialId = "Test2", Title = "Trial 2", StartDate = new DateOnly(2025,02,06), EndDate = new DateOnly(2025,03,06), Participants = 15, Status = TrialStatus.Ongoing, Duration = 30 }
            };
            await _dbContext.ClinicalTrials.AddRangeAsync(trials);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetByStatusAsync("NotStarted");

            // Assert
            Assert.Single(result);
            Assert.Equal(TrialStatus.NotStarted, result.First().Status);
        }

        [Fact]
        public async Task GetByStatusAsync_ShouldThrowRepositoryException_OnInvalidStatus()
        {
            // Act & Assert
            await Assert.ThrowsAsync<RepositoryException>(() => _repository.GetByStatusAsync("InvalidStatus"));
            _mockLogger.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
            ), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldThrowRepositoryException_OnException()
        {
            // Arrange
            _dbContext.Dispose(); // Simulate DB failure

            // Act & Assert
            await Assert.ThrowsAsync<RepositoryException>(() => _repository.GetAllAsync());
            _mockLogger.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
            ), Times.Once);
        }
    }
}

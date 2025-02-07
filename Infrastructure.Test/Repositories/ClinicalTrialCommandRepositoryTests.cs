using Application.Exceptions;
using Domain.Entities;
using Infrastructure.Persistance;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infrastructure.Test.Repositories
{
    public class ClinicalTrialCommandRepositoryTests
    {
        private readonly ClinicalTrialCommandRepository _repository;
        private readonly AppDbContext _dbContext;
        private readonly Mock<ILogger<ClinicalTrialCommandRepository>> _mockLogger;

        public ClinicalTrialCommandRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test
                .Options;

            _dbContext = new AppDbContext(options);
            _mockLogger = new Mock<ILogger<ClinicalTrialCommandRepository>>();
            _repository = new ClinicalTrialCommandRepository(_dbContext, _mockLogger.Object);
        }

        [Fact]
        public async Task AddTrialAsync_ShouldAddTrialAndReturnIt()
        {
            // Arrange
            var trial = new ClinicalTrial
            {
                TrialId = Guid.NewGuid().ToString(),
                Title = "Trial test add 1",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10),
                EndDate = null,
                Status = TrialStatus.NotStarted,
                Duration = null
            };

            // Act
            var result = await _repository.AddTrialAsync(trial);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(trial.Title, result.Title);
            Assert.Single(_dbContext.ClinicalTrials);
        }

        [Fact]
        public async Task UpdateTrialAsync_ShouldUpdateTrialAndReturnIt()
        {
            // Arrange
            var trial = new ClinicalTrial
            {
                TrialId = Guid.NewGuid().ToString(),
                Title = "Trial test update 1",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(15),
                Status = TrialStatus.Ongoing,
                Participants = It.IsAny<int>(),
                Duration = 30
            };
            _dbContext.ClinicalTrials.Add(trial);
            await _dbContext.SaveChangesAsync();

            // Act
            trial.Title = "Updated Trial";
            var result = await _repository.UpdateTrialAsync(trial);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Trial", result.Title);
        }

        [Fact]
        public async Task AddTrialAsync_ShouldThrowRepositoryException_OnConnectionCrash()
        {
            // Arrange
            var trial = new ClinicalTrial
            {
                TrialId = Guid.NewGuid().ToString(),
                Title = "Trial test add error 1",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = null,
                Status = TrialStatus.NotStarted,
                Participants = It.IsAny<int>(),
                Duration = null
            };
            _dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<RepositoryException>(() => _repository.AddTrialAsync(trial));
            _mockLogger.Verify(l =>
               l.Log(
                   LogLevel.Error,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((v, t) => true),
                   It.IsAny<Exception>(),
                   (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
               ),
               Times.Once);
        }

        [Fact]
        public async Task UpdateTrialAsync_ShouldThrowRepositoryException_OnConnectionCrash()
        {
            // Arrange
            var trial = new ClinicalTrial
            {
                TrialId = Guid.NewGuid().ToString(),
                Title = "Trial test update 1",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(1),
                Status = TrialStatus.Ongoing,
                Participants = It.IsAny<int>(),
                Duration = 30
            };
            _dbContext.ClinicalTrials.Add(trial);
            await _dbContext.SaveChangesAsync();
            _dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<RepositoryException>(() => _repository.UpdateTrialAsync(trial));
            _mockLogger.Verify(l =>
                l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
                ),
                Times.Once);
        }
    }
}

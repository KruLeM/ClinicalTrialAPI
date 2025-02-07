using System;
using System.Threading.Tasks;
using Application.Exceptions;
using Domain.Entities;
using Infrastructure.Persistance;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

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
                TrialId = "Test1",
                Title = "Trial test add 1",
                StartDate = new DateOnly(2025, 02, 07),
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
                TrialId = "Test1",
                Title = "Trial test update 1",
                StartDate = new DateOnly(2025, 02, 07),
                EndDate = new DateOnly(2025, 03, 07),
                Status = TrialStatus.Ongoing,
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
        public async Task AddTrialAsync_ShouldThrowRepositoryException_OnException()
        {
            // Arrange
            var trial = new ClinicalTrial
            {
                TrialId = "Test1",
                Title = "Trial test add error 1",
                StartDate = new DateOnly(2025, 02, 07),
                EndDate = null,
                Status = TrialStatus.NotStarted,
                Duration = null
            };
            _dbContext.Dispose(); // Simulate DB failure

            // Act & Assert
            await Assert.ThrowsAsync<RepositoryException>(() => _repository.AddTrialAsync(trial));
            _mockLogger.Verify(l => l.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTrialAsync_ShouldThrowRepositoryException_OnException()
        {
            // Arrange
            var trial = new ClinicalTrial
            {
                TrialId = "Test1",
                Title = "Trial test update 1",
                StartDate = new DateOnly(2025, 02, 07),
                EndDate = new DateOnly(2025, 03, 07),
                Status = TrialStatus.Ongoing,
                Duration = 30
            };
            _dbContext.ClinicalTrials.Add(trial);
            await _dbContext.SaveChangesAsync();
            _dbContext.Dispose(); // Simulate DB failure

            // Act & Assert
            await Assert.ThrowsAsync<RepositoryException>(() => _repository.UpdateTrialAsync(trial));
            _mockLogger.Verify(l => l.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        
    }
}

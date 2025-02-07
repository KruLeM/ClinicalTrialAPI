using Application.Commands;
using Application.Exceptions;
using Application.Handlers.CommandHandlers;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Test.Handlers.CommandHandlers
{
    public class AddTrialHandlerTests
    {
        private readonly Mock<ICommandRepository<ClinicalTrial>> _mockCommandRepository;
        private readonly Mock<IQueryRepository<ClinicalTrial>> _mockQueryRepository;
        private readonly Mock<ILogger<AddTrialHandler>> _mockLogger;
        private readonly AddTrialHandler _handler;

        public AddTrialHandlerTests()
        {
            _mockCommandRepository = new Mock<ICommandRepository<ClinicalTrial>>();
            _mockQueryRepository = new Mock<IQueryRepository<ClinicalTrial>>();
            _mockLogger = new Mock<ILogger<AddTrialHandler>>();
            _handler = new AddTrialHandler(
                _mockCommandRepository.Object,
                _mockQueryRepository.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldAddTrialAndReturnDTO()
        {
            // Arrange
            var request = new AddTrialCommand(
                Guid.NewGuid().ToString(), 
                "Test Trial",
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5), 
                null, 
                It.IsAny<int>(), 
                "NotStarted"
            );
            _mockQueryRepository.Setup(repo => repo.GetByIdAsync(request.TrialId)).ReturnsAsync((ClinicalTrial)null);
            _mockCommandRepository.Setup(repo => repo.AddTrialAsync(It.IsAny<ClinicalTrial>())).ReturnsAsync(new ClinicalTrial { TrialId = request.TrialId, Title = request.Title });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.TrialId, result.TrialId);
        }

        [Fact]
        public async Task Handle_ShouldThrowTrialDataException_WhenTrialIdExists()
        {
            // Arrange
            var request = new AddTrialCommand(
                Guid.NewGuid().ToString(), 
                "Test Trial",
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5), 
                null, 
                It.IsAny<int>(), 
                "NotStarted"
            );
            _mockQueryRepository.Setup(repo => repo.GetByIdAsync(request.TrialId)).ReturnsAsync(new ClinicalTrial { TrialId = request.TrialId });

            // Act & Assert
            await Assert.ThrowsAsync<TrialDataException>(() => _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowTrialDataException_OnInvalidEndDate()
        {
            // Arrange
            var request = new AddTrialCommand(
                Guid.NewGuid().ToString(), 
                "Test Trial",
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5),
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-5), 
                It.IsAny<int>(), 
                "NotStarted"
            );
            _mockQueryRepository.Setup(repo => repo.GetByIdAsync(request.TrialId)).ReturnsAsync((ClinicalTrial)null);

            // Act & Assert
            await Assert.ThrowsAsync<TrialDataException>(() => _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_OnRepositoryFailure()
        {
            // Arrange
            var request = new AddTrialCommand(
                Guid.NewGuid().ToString(), 
                "Test Trial",
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1), 
                null, 
                It.IsAny<int>(),
                "NotStarted"
            );
            _mockQueryRepository.Setup(repo => repo.GetByIdAsync(request.TrialId)).ReturnsAsync((ClinicalTrial)null);
            _mockCommandRepository.Setup(repo => repo.AddTrialAsync(It.IsAny<ClinicalTrial>())).ThrowsAsync(new RepositoryException("DB Failure"));

            // Act & Assert
            await Assert.ThrowsAsync<RepositoryException>(() => _handler.Handle(request, CancellationToken.None));
        }
    }
}

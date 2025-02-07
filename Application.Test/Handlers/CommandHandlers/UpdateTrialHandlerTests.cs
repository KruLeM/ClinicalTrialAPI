using Application.Commands;
using Application.Exceptions;
using Application.Handlers.CommandHandlers;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Test.Handlers.CommandHandlers
{
    public class UpdateTrialHandlerTests
    {
        private readonly Mock<ICommandRepository<ClinicalTrial>> _mockCommandRepository;
        private readonly Mock<IQueryRepository<ClinicalTrial>> _mockQueryRepository;
        private readonly Mock<ILogger<UpdateTrialHandler>> _mockLogger;
        private readonly UpdateTrialHandler _handler;

        public UpdateTrialHandlerTests()
        {
            _mockCommandRepository = new Mock<ICommandRepository<ClinicalTrial>>();
            _mockQueryRepository = new Mock<IQueryRepository<ClinicalTrial>>();
            _mockLogger = new Mock<ILogger<UpdateTrialHandler>>();

            _handler = new UpdateTrialHandler(
                _mockCommandRepository.Object,
                _mockQueryRepository.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldUpdateTrial()
        {
            // Arrange
            string trialId = Guid.NewGuid().ToString();
            var request = new UpdateTrialCommand
            (
                trialId,
                "Updated Trial",
                DateOnly.FromDateTime(DateTime.UtcNow),
                DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(2)),
                100,
                "Ongoing"
            );

            var existingTrial = new ClinicalTrial { TrialId = trialId, Title = "Old Trial" };
            _mockQueryRepository.Setup(r => r.GetByIdAsync(request.TrialId)).ReturnsAsync(existingTrial);
            _mockCommandRepository.Setup(r => r.UpdateTrialAsync(It.IsAny<ClinicalTrial>())).ReturnsAsync(existingTrial);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.TrialId, result.TrialId);
            _mockCommandRepository.Verify(r => r.UpdateTrialAsync(It.IsAny<ClinicalTrial>()), Times.Once);
        }


        [Fact]
        public async Task Handle_NonExistentTrial_ThrowsTrialDataException()
        {
            // Arrange
            var updateCommand = new UpdateTrialCommand(
                null,
                "Updated Trial",
                DateOnly.FromDateTime(DateTime.UtcNow),
                DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(2)),
                100,
                "Ongoing"
            );
            _mockQueryRepository.Setup(repo => repo.GetByIdAsync(updateCommand.TrialId)).ReturnsAsync((ClinicalTrial)null);

            // Act & Assert
            await Assert.ThrowsAsync<TrialDataException>(() => _handler.Handle(updateCommand, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_RepositoryException_ThrowsException()
        {
            // Arrange
            var trialId = Guid.NewGuid().ToString();
            var updateCommand = new UpdateTrialCommand(
                 trialId,
                "Updated Trial",
                DateOnly.FromDateTime(DateTime.UtcNow),
                DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(2)),
                100,
                "Ongoing"
            );
            _mockQueryRepository.Setup(repo => repo.GetByIdAsync(trialId)).ThrowsAsync(new RepositoryException("DB error"));

            // Act & Assert
            await Assert.ThrowsAsync<RepositoryException>(() => _handler.Handle(updateCommand, CancellationToken.None));
        }
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using WorkoutTrackerApi.Application;
using WorkoutTrackerApi.Domain;
using WorkoutTrackerApi.Infrastructure;
using Xunit;

namespace WorkoutTrackerAPI.Tests;

public class PlanServiceTests {
    private readonly Mock<IMapper> _mapperMock;
    private readonly WorkoutTrackerApiDbContext _contextInMemory;
    private readonly PlanService _service;

    public PlanServiceTests() {
        var options = new DbContextOptionsBuilder<WorkoutTrackerApiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _contextInMemory = new WorkoutTrackerApiDbContext(options);

        _mapperMock = new Mock<IMapper>();
        _service = new PlanService(_contextInMemory, _mapperMock.Object);
    }

    [Fact]
    public async Task GetOnePlan_ShouldReturnMappedPlanResponse_WhenPlanExists() {
        // Arange
        var cancellationToken = CancellationToken.None;
        var theOne = new Plan {
            Id = 1,
            Name = "Test_One",
            Description = "Test_One",
            StartTime = DateTime.UtcNow,
            Duration = new TimeSpan(0, 45, 0),
            UserId = 999,
        };

        _contextInMemory.PlanTable.Add(theOne);
        await _contextInMemory.SaveChangesAsync(cancellationToken);

        var shikomi = new PlanResponse {
            Name = "Test_One",
            Description = "Test_One",
            StartTime = DateTime.UtcNow,
            Duration = new TimeSpan(0, 45, 0),
        };

        _mapperMock.Setup(m => m.Map<PlanResponse>(It.IsAny<Plan>()))
            .Returns(shikomi);

        // Act
        var result = await _service.GetOnePlan(1, 999, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(shikomi.Name, result.Name);
        Assert.Equal(shikomi.Duration, result.Duration);

        _mapperMock.Verify(m => m.Map<PlanResponse>(It.IsAny<Plan>()), Times.Once);
        
    }
}
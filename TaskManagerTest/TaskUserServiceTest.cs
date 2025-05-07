using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TaskManager_API.Services;
using TaskManager_API.Models.Domain;
using TaskManager_API.Data;
using Microsoft.Extensions.Configuration;
using Moq;

public class TaskUserServiceTests
{
    private TaskContext GetTaskContext()
    {
        var options = new DbContextOptionsBuilder<TaskContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new TaskContext(options, new ConfigurationBuilder().Build());

        context.Tasks.Add(new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Sample Task",
            Description = "Some Description",
            AssignedUserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Role = "Developer",
            CreatedDate = DateTime.UtcNow
        });

        context.SaveChanges();
        return context;
    }

    private UserContext GetUserContext()
    {
        var options = new DbContextOptionsBuilder<UserContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new UserContext(options, new ConfigurationBuilder().Build());

        context.Users.Add(new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Nicolas",
            LastName = "Gonzalez",
            Email = "nicolas@test.com",
            UserImageURL = "image.jpg",
            Role = "Admin"
        });

        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task GetAllTasksAsync_ReturnsMappedTasks()
    {
        var taskContext = GetTaskContext();
        var userContext = GetUserContext();
        var logger = new Mock<ILogger<TaskUserService>>().Object;

        var service = new TaskUserService(taskContext, userContext, logger);

        var result = await service.GetAllTasksAsync();

        Assert.Single(result);
        var dto = result.First();
        Assert.Equal("Nicolas", dto.UserName);
        Assert.Equal("Gonzalez", dto.UserLastName);
        Assert.Equal("nicolas@test.com", dto.UserEmail);
    }
}
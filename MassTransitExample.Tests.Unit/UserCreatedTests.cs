using AutoFixture.Xunit2;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransitExample.Tests.Unit;

public class UserCreatedTests
{
    [Theory]
    [AutoData]
    public async Task GivenUserCreatedEventIsSend_WhenProperlyEventIsHandled_ThenUserIsCreated(Guid userId)
    {
        // Start the test harness
        await using var provider = new ServiceCollection()
            .AddDbContext<ChatContext>(options => options.UseInMemoryDatabase("TestChatDatabase"))
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<UserCreatedMessageHandler>();
            })
            .BuildServiceProvider(true);
        
        using (var scope = provider.CreateScope())
        {
            var chatContext = scope.ServiceProvider.GetRequiredService<ChatContext>();

            var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();
            await harness.Start();

            try
            {
                await harness.Bus.Publish<UserCreated>(new
                {
                    Id = userId
                });

                Assert.True(await harness.Consumed.Any<UserCreated>());

                // Assert

                await using (chatContext)
                {
                    // Verify changes in the database
                    var entity = await chatContext.Users.FirstOrDefaultAsync( /* Add conditions to find the entity */);
                    entity.Should().NotBeNull();
                    // Add additional assertions to verify the state of the entity or other changes in the database
                }
            }
            finally
            {
                // Clean up
                await harness.Stop();
            }
        }
    }
}

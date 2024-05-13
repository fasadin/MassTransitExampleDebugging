using System.Diagnostics;
using AutoFixture.Xunit2;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace MassTransitExample.Tests.Unit;

public class UserCreatedTests
{
    readonly TestOutputTextWriter _output;
    private readonly StringWriter _consoleOutput;

    public UserCreatedTests(ITestOutputHelper output)
    {
        _output = new TestOutputTextWriter(output);
        _consoleOutput = new StringWriter();
        Console.SetOut(_consoleOutput);
    }
    
    [Theory]
    [AutoData]
    public async Task GivenUserCreatedEventIsSend_WhenProperlyEventIsHandled_ThenUserIsCreated(Guid userId)
    {
        // Start the test harness
        await using var provider = new ServiceCollection()
            .AddTelemetryListener(_output)
            .AddLogging(builder =>
            {
                builder
                    .AddFilter(level => level >= LogLevel.Trace) 
                    .AddConsole();
            })
            .AddDbContext<ChatContext>(options => options.UseInMemoryDatabase("TestChatDatabase"))
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<UserCreatedMessageHandler>();
            })
            .AddScoped<IUserDataProvider, UserDataProvider>()
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

                var result = await harness.Consumed.Any<UserCreated>(x => x.Exception == null);
                result.Should().BeTrue();

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
                _output.WriteLine(_consoleOutput.ToString());
            }
        }
    }
}

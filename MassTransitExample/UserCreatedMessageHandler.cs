using MassTransit;

namespace MassTransitExample;

public class UserCreatedMessageHandler : IUserCreatedMessageHandler
{
    private readonly IUserDataProvider _userDataProvider;

    public UserCreatedMessageHandler(IUserDataProvider userDataProvider) => _userDataProvider = userDataProvider;

    public async Task Consume(ConsumeContext<UserCreated> context) =>
        await _userDataProvider.CreateUser(context.Message.Id);
}

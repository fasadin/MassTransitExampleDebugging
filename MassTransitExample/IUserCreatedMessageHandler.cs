using MassTransit;

namespace MassTransitExample;

public interface IUserCreatedMessageHandler : IConsumer<UserCreated> { }

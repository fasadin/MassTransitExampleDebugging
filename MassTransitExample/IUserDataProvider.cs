namespace MassTransitExample;

public interface IUserDataProvider
{
    Task CreateUser(Guid messageId);
}

namespace MassTransitExample;

public class UserDataProvider : IUserDataProvider
{
    private readonly ChatContext _context;

    public UserDataProvider(ChatContext context) => _context = context;

    public async Task CreateUser(Guid id)
    {
        await _context.Users.AddAsync(new User { Id = id });

        await _context.SaveChangesAsync();
    }
}

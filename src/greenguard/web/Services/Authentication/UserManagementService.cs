using Marten;
using web.Store;

namespace web.Services.Authentication;

public class UserManagementService
{
    private readonly IDocumentSession _session;

    public UserManagementService(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<User?> Exists(string username)
    {
        var user = await _session.Query<User>().Where(u => u.Username == username)
            .FirstOrDefaultAsync();

        return user;
    }

    public static bool PasswordMatch(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    public async Task<User?> Create(string username, string password)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Username = username,
            PasswordHash = passwordHash,
        };

        _session.Store(user);
        await _session.SaveChangesAsync();

        return user;
    }
}
using System.Text;
using System.Web;
using System.IO;
using System.Collections.Specialized;

namespace SimpleMDB;

public class MockUserService : IUserService
{
    private IUserRepository userRepository;

    public MockUserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
        _= Create(new User(0, "Admin", "AdminMan413", "", Roles.ADMIN));
    }

    public async Task<Result<PagedResult<User>>> ReadAll(int page, int size)
    {
        var pageResult = await userRepository.ReadAll(page, size);

        var result = (pageResult == null) ?
            new Result<PagedResult<User>>(new Exception("No result found.")) :
            new Result<PagedResult<User>>(pageResult);

        return result;
    }

    public async Task<Result<User>> Create(User newUser)
    {
        if (string.IsNullOrWhiteSpace(newUser.Role))
        {
            newUser.Role = Roles.USER;
        }
        if (string.IsNullOrWhiteSpace(newUser.Username))
        {
            return new Result<User>(new Exception("Username cannot be empty"));
        }
        else if (newUser.Username.Length > 16)
        {
            return new Result<User>(new Exception("Username cannot be more than 16 characters."));
        }

        else if (await userRepository.GetUserByUsername(newUser.Username) != null)
        {
            return new Result<User>(new Exception("Username already taken. Choose another Username"));
        }

        if (string.IsNullOrWhiteSpace(newUser.Password))
        {
            return new Result<User>(new Exception("Password cannot be empty"));
        }
        else if (newUser.Password.Length < 8)
        {
            return new Result<User>(new Exception("Password cannot be less than 8 characters."));
        }

        if (!Roles.IsValid(newUser.Role))
        {
            return new Result<User>(new Exception("Role cannot be empty"));
        }

        newUser.Salt = Path.GetRandomFileName();
        newUser.Password = Encode(newUser.Password + newUser.Salt);

        User? createdUser = await userRepository.Create(newUser);

        var result = (createdUser == null) ?
            new Result<User>(new Exception("User could not be created")) :
            new Result<User>(createdUser);

        return result;
    }

    public async Task<Result<User>> Read(int id)
    {
        User? user = await userRepository.Read(id);

        var result = (user == null) ?
            new Result<User>(new Exception("User could not be read")) :
            new Result<User>(user);

        return result;
    }

    public async Task<Result<User>> Update(int id, User newUser)
    {
        if (string.IsNullOrWhiteSpace(newUser.Role))
        {
            newUser.Role = Roles.USER;
        }
        if (string.IsNullOrWhiteSpace(newUser.Username))
        {
            return new Result<User>(new Exception("Username cannot be empty"));
        }
        else if (newUser.Username.Length > 16)
        {
            return new Result<User>(new Exception("Username cannot be more than 16 characters."));
        }

        else if (await userRepository.GetUserByUsername(newUser.Username) != null)
        {
            return new Result<User>(new Exception("Username already taken. Choose another Username"));
        }

        if (string.IsNullOrWhiteSpace(newUser.Password))
        {
            return new Result<User>(new Exception("Password cannot be empty"));
        }
        else if (newUser.Password.Length < 8)
        {
            return new Result<User>(new Exception("Password cannot be less than 8 characters."));
        }

        if (!Roles.IsValid(newUser.Role))
        {
            return new Result<User>(new Exception("Role cannot be empty"));
        }

        newUser.Salt = Path.GetRandomFileName();
        newUser.Password = Encode(newUser.Password + newUser.Salt);
        User? user = await userRepository.Update(id, newUser);

        var result = (user == null) ?
            new Result<User>(new Exception("User could not be updated")) :
            new Result<User>(user);

        return result;
    }

    public async Task<Result<User>> Delete(int id)
    {
        User? user = await userRepository.Delete(id);

        var result = (user == null) ?
            new Result<User>(new Exception("User could not be deleted")) :
            new Result<User>(user);

        return result;
    }

    public async Task<Result<string>> GetToken(string username, string password)
    {
        User? user = await userRepository.GetUserByUsername(username);

        if (user != null && string.Equals(user.Password, Encode(password + user.Salt)))
        {
            return new Result<string>(Encode($"username={user.Username}&role={user.Role}&expires={DateTime.Now.AddMinutes(60)}"));
        }
        else
        {
            return new Result<string>(new Exception("Invalid username or password."));
        }
    }

public Task<Result<NameValueCollection>> ValidateToken(string token)
{
    try
    {
        string decoded = Decode(token);
        var parts = HttpUtility.ParseQueryString(decoded);

        string? username = parts["username"];
        string? role = parts["role"];
        string? expiresStr = parts["expires"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(expiresStr))
        {
            return Task.FromResult(new Result<NameValueCollection>(new Exception("Invalid token format")));
        }

        if (!DateTime.TryParse(expiresStr, out DateTime expires) || DateTime.Now > expires)
        {
            return Task.FromResult(new Result<NameValueCollection>(new Exception("Token has expired")));
        }

        return Task.FromResult(new Result<NameValueCollection>(parts));
    }
    catch (Exception ex)
    {
        return Task.FromResult(new Result<NameValueCollection>(new Exception("Invalid token: " + ex.Message)));
    }
}


    public static string Encode(string plaintext)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(plaintext));
    }
    public static string Decode(string cyphertext)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(cyphertext));
    }
} 

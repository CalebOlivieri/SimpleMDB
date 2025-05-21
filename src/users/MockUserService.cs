namespace SimpleMDB;

public class MockUserService : IUserService
{
    private IUserRepository userRepository;

    public MockUserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
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
        if (string.IsNullOrWhiteSpace(newUser.Username))
        {
            return new Result<User>(new Exception("Username cannot be empty"));
        }
       else if (newUser.Username.Length > 16)
        {
            return new Result<User>(new Exception("Username cannot be more than 16 characters."));
        }
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
}

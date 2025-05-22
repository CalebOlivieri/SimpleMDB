namespace SimpleMDB;

public class MockActorService : IActorService
{
    private IActorRepository actorRepository;

    public MockActorService(IActorRepository actorRepository)
    {
        this.actorRepository = actorRepository;
    }

    public async Task<Result<PagedResult<Actor>>> ReadAll(int page, int size)
    {
        var pageResult = await actorRepository.ReadAll(page, size);

        var result = (pageResult == null) ?
            new Result<PagedResult<Actor>>(new Exception("No result found.")) :
            new Result<PagedResult<Actor>>(pageResult);

        return result;
    }

    public async Task<Result<Actor>> Create(Actor newActor)
    {
        if (string.IsNullOrWhiteSpace(newActor.FirstName))
        {
            return new Result<Actor>(new Exception("First name cannot be empty"));
        }
       else if (newActor.FirstName.Length > 16)
        {
            return new Result<Actor>(new Exception("First name cannot be more than 16 characters."));
        }
        else if (string.IsNullOrWhiteSpace(newActor.LastName))
        {
            return new Result<Actor>(new Exception("Last name cannot be empty"));
        }
       else if (newActor.LastName.Length > 16)
        {
            return new Result<Actor>(new Exception(" name cannot be more than 16 characters."));
        }
        Actor? createdActor = await actorRepository.Create(newActor);

        var result = (createdActor == null) ?
            new Result<Actor>(new Exception("Actor could not be created")) :
            new Result<Actor>(createdActor);

        return result;
    }

    public async Task<Result<Actor>> Read(int id)
    {
        Actor? actor = await actorRepository.Read(id);

        var result = (actor == null) ?
            new Result<Actor>(new Exception("Actor could not be read")) :
            new Result<Actor>(actor);

        return result;
    }

    public async Task<Result<Actor>> Update(int id, Actor newActor)
    {
        Actor? actor = await actorRepository.Update(id, newActor);

        var result = (actor == null) ?
            new Result<Actor>(new Exception("Actor could not be updated")) :
            new Result<Actor>(actor);

        return result;
    }

    public async Task<Result<Actor>> Delete(int id)
    {
        Actor? actor = await actorRepository.Delete(id);

        var result = (actor == null) ?
            new Result<Actor>(new Exception("Actor could not be deleted")) :
            new Result<Actor>(actor);

        return await Task.FromResult(result);
    }
}

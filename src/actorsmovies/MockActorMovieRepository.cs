
namespace SimpleMDB;

public class MockActorMovieRepository : IActorMovieRepository
{
   private IActorRepository actorRepository;
    private IMovieRepository movieRepository;
    private List<ActorMovie> actorMovies;
    private int idCount;

    public MockActorMovieRepository(IActorRepository actorRepository, IMovieRepository movieRepository)
    {
        this.actorRepository = actorRepository;
        this.movieRepository = movieRepository;
        actorMovies = new List<ActorMovie>();
        idCount = 0;
        Random r = new Random();

        for (int aid = 0; aid < 100; aid++)
        {
            int count = r.Next(100);
            for (int j = 0; j < count; j++)
            {
                int mid = r.Next(100);
                    actorMovies.Add(new ActorMovie(idCount++, aid, mid, "Jose"));
                
            }
        }  
    }

    public async Task<PagedResult<(ActorMovie, Movie)>> ReadAllMoviesByActor(int actorId, int page, int size)
    {
        List<ActorMovie> ams = actorMovies.FindAll((am) => am.ActorId == actorId);
        List<(ActorMovie, Movie)> movies = [];

        foreach (var am in ams)
        {
            var movie = (await movieRepository.Read(am.MovieId))!;
            movies.Add((am, movie));
        }
        int totalCount = movies.Count;
        int start = Math.Clamp((page - 1) * size, 0, totalCount);
        int length = Math.Clamp(size, 0, totalCount - start);
        List<(ActorMovie, Movie)> values = movies.Slice(start, length);
        var pagedResult = new PagedResult<(ActorMovie, Movie)>(values, totalCount);

        return await Task.FromResult(pagedResult);
    }
    

    public async Task<PagedResult<(ActorMovie, Actor)>> ReadAllActorsByMovie(int movieId, int page, int size)
    {
        List<ActorMovie> ams = actorMovies.FindAll((am) => am.MovieId == movieId);
        List<(ActorMovie, Actor)> actors = [];

        foreach (var am in ams)
        {
            var actor = (await actorRepository.Read(am.ActorId))!;
            actors.Add((am, actor));
        }

        int totalCount = actors.Count;
        int start = Math.Clamp((page - 1) * size, 0, totalCount);
        int length = Math.Clamp(size, 0, totalCount - start);
        List<(ActorMovie, Actor)> values = actors.Slice(start, length);
        var pagedResult = new PagedResult<(ActorMovie, Actor)>(values, totalCount);

        return await Task.FromResult(pagedResult);
    }

    public async Task<List<Actor>> ReadAllActors()
    {
        var pagedResult = await actorRepository.ReadAll(1, int.MaxValue);
        return pagedResult.Values;
    }

    public async Task<List<Movie>> ReadAllMovies()
    {
        var pagedResult = await movieRepository.ReadAll(1, int.MaxValue);
        return pagedResult.Values;
    }

    public async Task Create(int actorId, int movieId, string roleName)
    {
        var actorMovie = new ActorMovie(idCount++, actorId, movieId, roleName);
        actorMovies.Add(actorMovie);
        await Task.CompletedTask;
    }

    public async Task Delete(int id)
    {
        var actorMovie = actorMovies.FirstOrDefault(am => am.Id == id);
        if (actorMovie != null)
        {
            actorMovies.Remove(actorMovie);
        }
        await Task.CompletedTask;
    }
}

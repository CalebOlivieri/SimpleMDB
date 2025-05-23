namespace SimpleMDB;

public interface IActorMovieRepository
{
    public Task<PagedResult<(ActorMovie, Movie)>> ReadAllMoviesByActor(int actorId, int page, int size);
    public Task<PagedResult<(ActorMovie, Actor)>> ReadAllActorsByMovie(int movieId, int page, int size);
    public Task<List<Actor>> ReadAllActors();
    public Task<List<Movie>> ReadAllMovies();
    
    // CHANGE THIS LINE: It MUST return Task<ActorMovie>
    public Task<ActorMovie> Create(int actorId, int movieId, string roleName); 
    
    // CHANGE THIS LINE: It MUST return Task<ActorMovie?>
    public Task<ActorMovie?> Delete(int actorMovieId);
}
namespace SimpleMDB;

public class MockActorMovieService : IActorMovieService 
{
    private IActorMovieRepository actorMovieRepository;

    public MockActorMovieService(IActorMovieRepository actorMovieRepository)
    {
        this.actorMovieRepository = actorMovieRepository;
    }

    public async Task<Result<PagedResult<(ActorMovie, Movie)>>> ReadAllMoviesByActor(int actorId, int page, int size)
    {
        var pagedResult = await actorMovieRepository.ReadAllMoviesByActor(actorId, page, size);

        var result = (pagedResult == null) ?
            new Result<PagedResult<(ActorMovie, Movie)>>(new Exception("No movies found for the given actor.")) :
            new Result<PagedResult<(ActorMovie, Movie)>>(pagedResult);

        return await Task.FromResult(result);
    }

    public async Task<Result<PagedResult<(ActorMovie, Actor)>>> ReadAllActorsByMovie(int movieId, int page, int size)
    {
        var pagedResult = await actorMovieRepository.ReadAllActorsByMovie(movieId, page, size);

        var result = (pagedResult == null) ?
            new Result<PagedResult<(ActorMovie, Actor)>>(new Exception("No actors found for the given movie.")) :
            new Result<PagedResult<(ActorMovie, Actor)>>(pagedResult);

        return await Task.FromResult(result);
    }

    public async Task<Result<List<Actor>>> ReadAllActors()
    {
        var actors = await actorMovieRepository.ReadAllActors();

        var result = (actors == null) ?
            new Result<List<Actor>>(new Exception("No actors found.")) :
            new Result<List<Actor>>(actors);

        return result;
    }

    public async Task<Result<List<Movie>>> ReadAllMovies()
    {
        var movies = await actorMovieRepository.ReadAllMovies();

        var result = (movies == null) ?
            new Result<List<Movie>>(new Exception("No movies found.")) :
            new Result<List<Movie>>(movies);

        return result;
    }

    public async Task<Result<ActorMovie>> Create(int actorId, int movieId, string roleName)
    {
        // El repositorio MockActorMovieRepository.Create no devuelve un ActorMovie.
        // Si necesitas devolver el ActorMovie creado, MockActorMovieRepository.Create debería devolverlo.
        // Por ahora, asumiremos que la operación fue exitosa si no hay errores,
        // y podrías querer buscar el ActorMovie después o modificar el repositorio para devolverlo.
        try
        {
            await actorMovieRepository.Create(actorId, movieId, roleName);
            // Si el repositorio no devuelve el objeto creado, no podemos devolverlo aquí directamente.
            // Para fines de este ejercicio, si el 'await' no lanza una excepción, asumimos éxito.
            // Considera que el MockActorMovieRepository no tiene un método 'Read' para ActorMovie,
            // por lo que no podemos recuperarlo aquí. Se necesita un objeto ActorMovie de ejemplo.
            // Para que este método compile y sea lógico, necesitamos un ActorMovie de retorno.
            // Crearemos un objeto ActorMovie de ejemplo para el retorno exitoso.
            return new Result<ActorMovie>(new ActorMovie(0, actorId, movieId, roleName)); // Id 0 o manejar cómo se asigna el ID.
        }
        catch (Exception ex)
        {
            return new Result<ActorMovie>(ex);
        }
    }

    public async Task<Result<ActorMovie>> Delete(int id)
    {
        // Similar al Create, el repositorio MockActorMovieRepository.Delete no devuelve el ActorMovie eliminado.
        // Si necesitas devolver el ActorMovie eliminado, MockActorMovieRepository.Delete debería devolverlo.
        try
        {
            // Antes de intentar eliminar, intentamos "leer" un ActorMovie que se eliminará
            // para poder devolverlo en el Result. Si el MockActorMovieRepository no tiene un Read(int)
            // para ActorMovie, necesitaríamos simularlo o que el repositorio lo devuelva.
            // Como el MockActorMovieRepository no tiene un método Read para ActorMovie directamente,
            // y su Delete solo elimina, para cumplir con el `Result<ActorMovie>` necesitamos
            // una forma de obtener un `ActorMovie` que represente el que se eliminó.
            // En un escenario real, `Delete` devolvería el objeto eliminado o al menos un `bool`
            // que indique el éxito. Aquí, para que compile y sea coherente, necesitamos un objeto `ActorMovie`
            // de ejemplo. Si el repositorio lanza una excepción, la capturamos.

            // Por ahora, para resolver el error de compilación y mantener la estructura,
            // si el delete es "exitoso" (no lanza excepción), devolvemos un ActorMovie dummy.
            // En un sistema real, el repositorio podría devolver el objeto eliminado.
            await actorMovieRepository.Delete(id);
            return new Result<ActorMovie>(new ActorMovie(id, 0, 0, "Deleted Role")); // Objeto dummy
        }
        catch (Exception ex)
        {
            return new Result<ActorMovie>(ex);
        }
    }
}
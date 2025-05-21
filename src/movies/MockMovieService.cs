namespace SimpleMDB;

public class MockMovieService : IMovieService
{
    private IMovieRepository movieRepository;

    public MockMovieService(IMovieRepository movieRepository)
    {
        this.movieRepository = movieRepository;
    }

    public async Task<Result<PagedResult<Movie>>> ReadAll(int page, int size)
    {
        var pageResult = await movieRepository.ReadAll(page, size);

        var result = (pageResult == null) ?
            new Result<PagedResult<Movie>>(new Exception("No result found.")) :
            new Result<PagedResult<Movie>>(pageResult);

        return result;
    }

    public async Task<Result<Movie>> Create(Movie newMovie)
    {
        if (string.IsNullOrWhiteSpace(newMovie.Title))
        {
            return new Result<Movie>(new Exception("Title cannot be empty"));
        }
       else if (newMovie.Title.Length > 256)
        {
            return new Result<Movie>(new Exception("First name cannot be more than 256 characters."));
        }
       else if (newMovie.Year > DateTime.Now.Year)
        {
            return new Result<Movie>(new Exception("Year cannot be in the future."));
        }
        Movie? createdMovie = await movieRepository.Create(newMovie);

        var result = (createdMovie == null) ?
            new Result<Movie>(new Exception("Movie could not be created")) :
            new Result<Movie>(createdMovie);

        return result;
    }

    public async Task<Result<Movie>> Read(int id)
    {
        Movie? movie = await movieRepository.Read(id);

        var result = (movie == null) ?
            new Result<Movie>(new Exception("Movie could not be read")) :
            new Result<Movie>(movie);

        return result;
    }

    public async Task<Result<Movie>> Update(int id, Movie newMovie)
    {
        Movie? movie = await movieRepository.Update(id, newMovie);

        var result = (movie == null) ?
            new Result<Movie>(new Exception("Movie could not be updated")) :
            new Result<Movie>(movie);

        return result;
    }

    public async Task<Result<Movie>> Delete(int id)
    {
        Movie? movie = await movieRepository.Delete(id);

        var result = (movie == null) ?
            new Result<Movie>(new Exception("Movie could not be deleted")) :
            new Result<Movie>(movie);

        return result;
    }
}

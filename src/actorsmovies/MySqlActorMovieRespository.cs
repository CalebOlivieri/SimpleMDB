using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace SimpleMDB;

public class MySqlActorMovieRepository : IActorMovieRepository
{
    private string connectionString;

    public MySqlActorMovieRepository(string connectionString)
    {
        this.connectionString = connectionString;
        // Si descomentas Init(), asegúrate de que use VARCHAR en lugar de NVARCHAR.
        // Init(); 
    }

    private void Init()
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();

        cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS ActorsMovies 
        (
        id int AUTO_INCREMENT PRIMARY KEY,
        actorId int NOT NULL,
        movieId int NOT NULL,
        rolename VARCHAR(64), -- Corrected: Changed NVARCHAR to VARCHAR
        FOREIGN KEY(actorId) REFERENCES Actors(id) ON DELETE CASCADE,
        FOREIGN KEY(movieId) REFERENCES Movies(id) ON DELETE CASCADE
        )
        ";

        cmd.ExecuteNonQuery();
    }

    public MySqlConnection OpenDb()
    {
        var dbc = new MySqlConnection(connectionString);
        dbc.Open();
        return dbc;
    }

    public async Task<PagedResult<(ActorMovie, Movie)>> ReadAllMoviesByActor(int actorId, int page, int size)
    {
        using var dbc = OpenDb();

        using var countCmd = dbc.CreateCommand();
        countCmd.CommandText = @"
            SELECT COUNT(*) FROM ActorsMovies as am 
            JOIN Movies as m ON am.movieId = m.id 
            WHERE am.actorId = @actorId
        ";
        countCmd.Parameters.AddWithValue("@actorId", actorId); 

        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
            SELECT am.id, am.actorId, am.movieId, am.rolename, 
            m.id, m.title, m.year, m.description, m.rating
            FROM ActorsMovies as am 
            JOIN Movies as m ON am.movieId = m.id 
            WHERE am.actorId = @actorId
            LIMIT @offset, @limit
            ";
        cmd.Parameters.AddWithValue("@actorId", actorId);
        cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
        cmd.Parameters.AddWithValue("@limit", size);

        using var rows = await cmd.ExecuteReaderAsync();

        var amms = new List<(ActorMovie, Movie)>();

        while (await rows.ReadAsync())
        {
            ActorMovie am = new ActorMovie(
               rows.GetInt32(0),    // ActorsMovies.id
               rows.GetInt32(1),    // ActorsMovies.actorId
               rows.GetInt32(2),    // ActorsMovies.movieId
               rows.GetString(3));  // ActorsMovies.rolename

            Movie m = new Movie(
               rows.GetInt32(4),    // Movies.id
               rows.GetString(5),   // Movies.title
               rows.GetInt32(6),    // Movies.year
               rows.GetString(7),   // Movies.description
               rows.GetFloat(8)     // Movies.rating
            );

            amms.Add((am, m));
        }

        return new PagedResult<(ActorMovie, Movie)>(amms, totalCount);
    }

    public async Task<PagedResult<(ActorMovie, Actor)>> ReadAllActorsByMovie(int movieId, int page, int size)
    {
        using var dbc = OpenDb();

        using var countCmd = dbc.CreateCommand();
        countCmd.CommandText = @"
            SELECT COUNT(*) FROM ActorsMovies as am 
            JOIN Actors as a ON am.actorId = a.id 
            WHERE am.movieId = @movieId
        ";
        countCmd.Parameters.AddWithValue("@movieId", movieId); 

        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
            SELECT am.id, am.actorId, am.movieId, am.rolename, 
                   a.id, a.firstname, a.lastname, a.bio, a.rating
            FROM ActorsMovies as am 
            JOIN Actors as a ON am.actorId = a.id 
            WHERE am.movieId = @movieId
            LIMIT @offset, @limit
            ";
        cmd.Parameters.AddWithValue("@movieId", movieId); 
        cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
        cmd.Parameters.AddWithValue("@limit", size);

        using var rows = await cmd.ExecuteReaderAsync();

        var amas = new List<(ActorMovie, Actor)>();

        while (await rows.ReadAsync())
        {
            ActorMovie am = new ActorMovie(
               rows.GetInt32(0),    // ActorsMovies.id
               rows.GetInt32(1),    // ActorsMovies.actorId
               rows.GetInt32(2),    // ActorsMovies.movieId
               rows.GetString(3));  // ActorsMovies.rolename

            Actor a = new Actor(
               rows.GetInt32(4),    // Actors.id
               rows.GetString(5),   // Actors.firstname
               rows.GetString(6),   // Actors.lastname
               rows.GetString(7),   // Actors.bio
               rows.GetFloat(8)     // Actors.rating
            );

            amas.Add((am, a));
        }

        return new PagedResult<(ActorMovie, Actor)>(amas, totalCount);
    }

    public async Task<List<Actor>> ReadAllActors()
    {
        using var dbc = OpenDb();

        using var countCmd = dbc.CreateCommand();
        countCmd.CommandText = @"
        SELECT COUNT(*) FROM Actors 
        ";
        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync()); 

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
        SELECT * FROM Actors
        ";

        using var rows = await cmd.ExecuteReaderAsync();

        var actors = new List<Actor>();

        while (await rows.ReadAsync())
        {
            Actor a = new Actor(
                 rows.GetInt32(0),    // Actors.id
                 rows.GetString(1),   // Actors.firstname
                 rows.GetString(2),   // Actors.lastname
                 rows.GetString(3),   // Actors.bio
                 rows.GetFloat(4)     // Actors.rating
            );
            actors.Add(a);
        }
        return actors;
    }

    public async Task<List<Movie>> ReadAllMovies()
    {
        using var dbc = OpenDb();

        using var countCmd = dbc.CreateCommand();
        countCmd.CommandText = @"
        SELECT COUNT(*) FROM Movies 
        ";
        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync()); 

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
        SELECT * FROM Movies
        ";

        using var rows = await cmd.ExecuteReaderAsync();

        var movies = new List<Movie>();

        while (await rows.ReadAsync())
        {
            Movie movie = new Movie(
                 rows.GetInt32(0),    // Movies.id
                 rows.GetString(1),   // Movies.title
                 rows.GetInt32(2),    // Movies.year
                 rows.GetString(3),   // Movies.description
                 rows.GetFloat(4)     // Movies.rating
            );
            movies.Add(movie);
        }
        return movies;
    }

    public async Task<ActorMovie> Create(int actorId, int movieId, string roleName)
    {
        using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
        INSERT INTO ActorsMovies (actorId, movieId, rolename) 
        VALUES (@actorId, @movieId, @rolename);
        SELECT LAST_INSERT_ID();
        ";
        cmd.Parameters.AddWithValue("@actorId", actorId);
        cmd.Parameters.AddWithValue("@movieId", movieId);
        cmd.Parameters.AddWithValue("@rolename", roleName);

        int id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        var actorMovie = new ActorMovie(id, actorId, movieId, roleName);

        return actorMovie;
    }

    public async Task<ActorMovie?> Delete(int id)
    {
        using var dbc = OpenDb();

        var actorMovieToDelete = await Read(id);

        if (actorMovieToDelete == null)
        {
            return null; 
        }

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
        DELETE FROM ActorsMovies WHERE id = @id; 
        ";
        cmd.Parameters.AddWithValue("@id", id);

        int rowsAffected = await cmd.ExecuteNonQueryAsync();

        return (rowsAffected > 0) ? actorMovieToDelete : null;
    }

    public async Task<ActorMovie?> Read(int id)
    {
        using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
        SELECT * FROM ActorsMovies WHERE id = @id 
        ";
        cmd.Parameters.AddWithValue("@id", id);

        using var rows = await cmd.ExecuteReaderAsync();

        if (await rows.ReadAsync())
        {
            return new ActorMovie(
                 rows.GetInt32(0),    // ActorsMovies.id
                 rows.GetInt32(1),    // ActorsMovies.actorId
                 rows.GetInt32(2),    // ActorsMovies.movieId
                 rows.GetString(3)    // ActorsMovies.rolename
            );
        }

        return null;
    }
}
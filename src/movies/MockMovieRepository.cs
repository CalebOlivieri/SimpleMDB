using SimpleMDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SimpleMDB;

public class MockMovieRepository : IMovieRepository
{
    private List<Movie> movies;
    private int idCount;

    public MockMovieRepository()
    {
        movies = new List<Movie>();
        idCount = 1;

        var titles = new string[]
        {
            "The Last Horizon", "Silent Echo", "Crimson Sky", "Whispers in the Wind", "Echoes of Time",
            "Broken Dreams", "Golden Hour", "Shadow Realm", "Rise of the Brave", "Forgotten Tales",
            "Endless Journey", "Frozen Flame", "Soulbound", "The Final Stand", "Midnight Chase",
            "Fading Memories", "Lost in Light", "Dawn of Fate", "Glass Kingdom", "Wings of Ash",
            "Sacred Ground", "Hidden Truths", "Nocturne", "Burning Skies", "The Hollow Crown",
            "Twilight Sands", "Eternal Rain", "Cursed Paths", "The Iron Vow", "Echo Valley",
            "Moonlit War", "Velvet Chains", "Blood and Dust", "Frostbite", "Veil of Secrets",
            "The Fire Within", "Dragon's Call", "Shifting Tides", "Dead Man's Gold", "The Ninth Gate",
            "Wicked Game", "Kingdom's Edge", "Silent Reign", "The Glass Mirror", "Titan's Fall",
            "Maze of Bones", "Lone Survivor", "Shadowlight", "Hearts Ablaze", "Stormbreak"
        };

        var descriptionsStart = new string[]
        {
            "An epic adventure about",
            "A thrilling mystery involving",
            "A dramatic tale of",
            "A futuristic story centered on",
            "A heartwarming story of",
            "A dark and intense journey through"
        };

        var descriptionsMiddle = new string[]
        {
            "a forgotten civilization",
            "an unexpected hero",
            "the collapse of society",
            "a magical relic",
            "a dangerous love affair",
            "a parallel universe"
        };

        var descriptionsEnd = new string[]
        {
            "that will change everything.",
            "in a race against time.",
            "with secrets long buried.",
            "revealing shocking truths.",
            "under the shadow of destiny.",
            "that no one can escape."
        };

        var random = new Random();

        for (int i = 0; i < 100; i++)
        {
            var title = titles[random.Next(titles.Length)] + " " + random.Next(1, 100);
            var year = random.Next(1980, 2025);
            var description = $"{descriptionsStart[random.Next(descriptionsStart.Length)]} {descriptionsMiddle[random.Next(descriptionsMiddle.Length)]} {descriptionsEnd[random.Next(descriptionsEnd.Length)]}";
            var rating = (float)Math.Round(random.NextDouble() * 10.0, 1);

            movies.Add(new Movie(idCount++, title, year, description, rating));
        }
    }

    public async Task<PagedResult<Movie>> ReadAll(int page, int size)
    {
        int totalCount = movies.Count;
        int start = Math.Clamp((page - 1) * size, 0, totalCount);
        int length = Math.Clamp(size, 0, totalCount - start);
        List<Movie> values = movies.GetRange(start, length);
        var pagedResult = new PagedResult<Movie>(values, totalCount);
        return await Task.FromResult(pagedResult);
    }

    public async Task<Movie?> Create(Movie movie)
    {
        movie.Id = idCount++;
        movies.Add(movie);
        return await Task.FromResult(movie);
    }

    public async Task<Movie?> Read(int id)
    {
        Movie? movie = movies.FirstOrDefault(m => m.Id == id);
        return await Task.FromResult(movie);
    }

    public async Task<Movie?> Update(int id, Movie newMovie)
    {
        Movie? movie = movies.FirstOrDefault(m => m.Id == id);

        if (movie != null)
        {
            movie.Title = newMovie.Title;
            movie.Year = newMovie.Year;
            movie.Description = newMovie.Description;
            movie.Rating = newMovie.Rating;
        }

        return await Task.FromResult(movie);
    }

    public async Task<Movie?> Delete(int id)
    {
        Movie? movie = movies.FirstOrDefault(m => m.Id == id);

        if (movie != null)
        {
            movies.Remove(movie);
        }

        return await Task.FromResult(movie);
    }
}

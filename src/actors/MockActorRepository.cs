using SimpleMDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // Make sure System is included for Random, Math, etc.

namespace SimpleMDB;

public class MockActorRepository : IActorRepository
{
    private List<Actor> actors;
    private int idCount;

    public MockActorRepository()
    {
        actors = new List<Actor>(); // Initialize the class-level list
        idCount = 1; // Start IDs from 1 or 0, depending on preference. Using 1 here for clarity.

        var firstNames = new string[]
        {
            "Emma", "Liam", "Olivia", "Noah", "Ava", "Elijah", "Isabella", "James", "Sophia", "Benjamin",
            "Mia", "Lucas", "Charlotte", "Mason", "Amelia", "Logan", "Harper", "Ethan", "Evelyn", "Jacob",
            "Abigail", "Michael", "Emily", "Alexander", "Elizabeth", "Daniel", "Mila", "Henry", "Ella", "Jackson",
            "Scarlett", "Sebastian", "Victoria", "Aiden", "Aria", "Matthew", "Grace", "Samuel", "Chloe", "David",
            "Camila", "Joseph", "Penelope", "Carter", "Layla", "Owen", "Riley", "Wyatt", "Zoey", "John",
            "Nora", "Lily", "Luke", "Eleanor", "Jayden", "Hannah", "Dylan", "Lillian", "Grayson", "Addison",
            "Levi", "Aubrey", "Isaac", "Ellie", "Gabriel", "Stella", "Julian", "Natalie", "Mateo", "Zoe",
            "Anthony", "Leah", "Jaxon", "Hazel", "Lincoln", "Violet", "Joshua", "Aurora", "Christopher", "Savannah",
            "Andrew", "Audrey", "Theodore", "Brooklyn", "Caleb", "Bella", "Ryan", "Claire", "Asher", "Skylar",
            "Nathan", "Lucy", "Thomas", "Anna", "Leo", "Caroline", "Isaiah", "Genesis", "Charles", "Aaron"
        };

        var lastNames = new string[]
        {
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
            "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
            "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
            "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
            "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts",
            "Gomez", "Phillips", "Evans", "Turner", "Diaz", "Parker", "Cruz", "Edwards", "Collins", "Reyes",
            "Stewart", "Morris", "Morales", "Murphy", "Cook", "Rogers", "Gutierrez", "Ortiz", "Morgan", "Cooper",
            "Peterson", "Bailey", "Reed", "Kelly", "Howard", "Ramos", "Kim", "Cox", "Ward", "Richardson",
            "Watson", "Brooks", "Chavez", "Wood", "James", "Bennett", "Gray", "Mendoza", "Ruiz", "Hughes",
            "Price", "Alvarez", "Castillo", "Sanders", "Patel", "Myers", "Long", "Ross", "Foster", "Jimenez"
        };

        var bioStarts = new string[]
        {
            "A passionate actor with a love for",
            "An award-winning performer known for",
            "A newcomer to the industry with a focus on",
            "A classically trained actor specialized in",
            "A natural talent with experience in",
            "A creative soul driven by"
        };

        var bioThemes = new string[]
        {
            "dramatic storytelling",
            "method acting and realism",
            "comedic timing and improvisation",
            "historical character roles",
            "sci-fi and fantasy projects",
            "stage and television work"
        };

        var bioAchievements = new string[]
        {
            "who has starred in numerous indie films.",
            "featured in international film festivals.",
            "with a growing fanbase across the globe.",
            "who is constantly evolving their craft.",
            "known for their powerful on-screen presence.",
            "making waves in both TV and cinema."
        };

        var random = new Random();

        for (int i = 0; i < 100; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var bio = $"{bioStarts[random.Next(bioStarts.Length)]} {bioThemes[random.Next(bioThemes.Length)]} {bioAchievements[random.Next(bioAchievements.Length)]}";
            // Generate a random rating between 0.0 and 10.0, rounded to one decimal place
            var rating = (float)Math.Round(random.NextDouble() * 10.0, 1);

            // Correctly call the Actor constructor with an ID
            actors.Add(new Actor(idCount++, firstName, lastName, bio, rating));
        }

        // Removed the problematic user-related loop entirely, as it doesn't belong here.
        // Random r = new Random();
        // foreach (var actorname in actornames)
        // {
        //     var pass = Path.GetRandomFileName();
        //     var salt = Path.GetRandomFileName();
        //     var role = Roles.ROLES[r.Next(Roles.ROLES.Length)];
        //     Actor actor = new Actor(idCount++, actorname, pass, salt, role);
        //     actors.Add(actor);
        // }
    }

    public async Task<PagedResult<Actor>> ReadAll(int page, int size)
    {
        int totalCount = actors.Count;
        // Ensure start and length are within valid bounds. Clamp is helpful.
        int start = Math.Clamp((page - 1) * size, 0, totalCount);
        int length = Math.Clamp(size, 0, totalCount - start); // Adjust length to not go past totalCount

        // If start index is beyond totalCount, or length is negative, GetRange will throw.
        // The Math.Clamp ensures this, but it's good to consider edge cases.
        List<Actor> values = actors.GetRange(start, length);
        var pagedResult = new PagedResult<Actor>(values, totalCount);
        return await Task.FromResult(pagedResult);
    }

    public async Task<Actor?> Create(Actor actor)
    {
        // Assign a new ID before adding
        actor.Id = idCount++;
        actors.Add(actor);
        return await Task.FromResult(actor);
    }

    public async Task<Actor?> Read(int id)
    {
        Actor? actor = actors.FirstOrDefault(u => u.Id == id);
        return await Task.FromResult(actor);
    }

    public async Task<Actor?> Update(int id, Actor newActor)
    {
        Actor? actor = actors.FirstOrDefault(u => u.Id == id);

        if (actor != null)
        {
            // Update only the relevant actor properties based on the Actor class
            actor.FirstName = newActor.FirstName;
            actor.LastName = newActor.LastName;
            actor.Bio = newActor.Bio;
            actor.Rating = newActor.Rating;
            // Id is usually not updated via the update method; it's the identifier.
        }

        return await Task.FromResult(actor);
    }

    public async Task<Actor?> Delete(int id)
    {
        Actor? actor = actors.FirstOrDefault(u => u.Id == id);

        if (actor != null)
        {
            actors.Remove(actor);
        }

        return await Task.FromResult(actor);
    }
}
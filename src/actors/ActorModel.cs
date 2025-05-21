namespace SimpleMDB;

using SimpleMDB;

public class Actor
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Bio { get; set; }
    public float Rating { get; set; }

    public Actor(int id = 0, string firstname = "", string lastname = "", string bio = "", float rating = 0)
    {
        Id = id;
        FirstName = firstname;
        LastName = lastname;
        Bio = bio;
        Rating = rating;
    }

    public override string ToString()
    {
        return $"Actor[Id={Id}, FirstName={FirstName}, LastName={LastName}, Bio{Bio}, Rating={Rating}]";
    }
}
using System.Collections;
using System.Collections.Specialized;
using System.Net;

namespace SimpleMDB;

public class ActorMovieController
{
    private IActorMovieService actorMovieService;
    private IActorService actorService;
    private IMovieService movieService;

    public ActorMovieController(IActorMovieService actorMovieService, IActorService actorService, IMovieService movieService)
    {
        this.actorMovieService = actorMovieService;
        this.actorService = actorService;
        this.movieService = movieService;
    }

    // Get /actors/movie?aid=1&page=1&size=5
    public async Task ViewAllMoviesByActor(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int aid = int.TryParse(req.QueryString["aid"], out int a) ? a : 1;
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 1;

        var result1 = await actorService.Read(aid);
        var result2 = await actorMovieService.ReadAllMoviesByActor(aid, page, size);
        if (result1.IsValid && result2.IsValid)
        {
            var actor = result1.Value!;
            var pagedResult = result2.Value!;
            var movies = pagedResult.Values;
            int totalCount = pagedResult.TotalCount;

            string html = ActorMovieHtmlTemplates.ViewAllMoviesByActor(actor, movies, totalCount, page, size);
            string content = HtmlTemplates.Base("SimpleMDB", "View All Movies By Actor Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
        }
        else
        {
            string error = result1.IsValid ? "" : result1.Error!.Message;
            error += result2.IsValid ? "" : result2.Error!.Message;

            // Corrige la asignación del mensaje de error, asegurándote de que no sea null
            HttpUtils.AddOptions(options, "redirect", "message", !string.IsNullOrEmpty(error) ? error : "An unexpected error occurred while fetching movies by actor.");
            await HttpUtils.Redirect(req, res, options, "/");
        }
    }

    // Get /movies/actor?mid=1&page=1&size=5
    public async Task ViewAllActorsByMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : 1; // Cambiado de 'aid' a 'mid'
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 1;

        var result1 = await movieService.Read(mid); // Usar movieService.Read para obtener la película
        var result2 = await actorMovieService.ReadAllActorsByMovie(mid, page, size); // Usar mid para obtener actores por película

        if (result1.IsValid && result2.IsValid)
        {
            var movie = result1.Value!; // Ahora es un objeto Movie
            var pagedResult = result2.Value!;
            var actors = pagedResult.Values; // Obtener la lista de actores
            int totalCount = pagedResult.TotalCount;

            // Necesitarás un método en ActorMovieHtmlTemplates para esto, similar a ViewAllMoviesByActor
            string html = ActorMovieHtmlTemplates.ViewAllActorsByMovie(movie, actors, totalCount, page, size);
            string content = HtmlTemplates.Base("SimpleMDB", "View All Actors By Movie Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
        }
        else
        {
            string error = result1.IsValid ? "" : result1.Error!.Message;
            error += result2.IsValid ? "" : result2.Error!.Message;

            HttpUtils.AddOptions(options, "redirect", "message", !string.IsNullOrEmpty(error) ? error : "An unexpected error occurred while fetching actors by movie.");
            await HttpUtils.Redirect(req, res, options, "/");
        }
    }

    // Get /actors/movie/add?aid=1
    public async Task AddMoviesByActorGET(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int aid = int.TryParse(req.QueryString["aid"], out int a) ? a : 1;

        var result1 = await actorService.Read(aid);
        var result2 = await actorMovieService.ReadAllMovies();

        if (result1.IsValid && result2.IsValid)
        {
            var actor = result1.Value!;
            var movies = result2.Value!;

            string html = ActorMovieHtmlTemplates.AddMoviesByActor(actor, movies);
            string content = HtmlTemplates.Base("SimpleMDB", "Add Movies By Actor Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
        }
        else
        {
            string error = result1.IsValid ? "" : result1.Error!.Message;
            error += result2.IsValid ? "" : result2.Error!.Message;

            HttpUtils.AddOptions(options, "redirect", "message", error);
            await HttpUtils.Redirect(req, res, options, "/");
        }
    }

    // Get /movie/actor/add?aid=1
    public async Task AddActorsByMovieGET(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int mid = int.TryParse(req.QueryString["mid"], out int a) ? a : -1;

        var result1 = await movieService.Read(mid);
        var result2 = await actorMovieService.ReadAllActors();

        if (result1.IsValid && result2.IsValid)
        {
            var movie = result1.Value!;
            var actors = result2.Value!;

            string html = ActorMovieHtmlTemplates.AddActorsByMovie(movie, actors);
            string content = HtmlTemplates.Base("SimpleMDB", "Add Actors By Movie Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
        }
        else
        {
            string error = result1.IsValid ? "" : result1.Error!.Message;
            error += result2.IsValid ? "" : result2.Error!.Message;

            HttpUtils.AddOptions(options, "redirect", "message", error);
            await HttpUtils.Redirect(req, res, options, "/");
        }
    }
    // POST /actors/movie/add
    public async Task AddMoviesByActorPOST(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? [];
        var aid = int.TryParse(formData["aid"], out int a) ? a : 1;
        var mid = int.TryParse(formData["mid"], out int m) ? m : 1;
        var rolename = formData["rolename"] ?? "Lol";

        var result = await actorMovieService.Create(aid, mid, rolename);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "ActorMovie added succesfully!");

            await HttpUtils.Redirect(req, res, options, $"/actor/movie?aid={aid}");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect", formData);

            await HttpUtils.Redirect(req, res, options, $"/actor/movie/add?aid={aid}");
        }
    }
    // POST /movie/actor/add
    public async Task AddActorsByMoviePOST(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? [];
        var mid = int.TryParse(formData["mid"], out int a) ? a : 1;
        var aid = int.TryParse(formData["aid"], out int m) ? m : 1;
        var rolename = formData["rolename"] ?? "Lol";

        var result = await actorMovieService.Create(aid, mid, rolename);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "ActorMovie added succesfully!");

            await HttpUtils.Redirect(req, res, options, $"/movie/actor?mid={mid}");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect", formData);

            await HttpUtils.Redirect(req, res, options, $"/movie/actor/add?mid={mid}");
        }
    }
    // POST /actor/movie/remove?aid=1amid=1
    public async Task RemoveMoviesByActorPOST(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int amid = int.TryParse(req.QueryString["amid"], out int a) ? a : 1;

        Result<ActorMovie> result = await actorMovieService.Delete(amid);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "ActorMovie removed successfully!");
            await HttpUtils.Redirect(req, res, options, $"/actor/movie?aid={result.Value!.ActorId}");

        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, $"/actors");
        }
    }
      // POST /movie/actor/remove?amid=1
    public async Task RemoveActorsByMoviePOST(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int amid = int.TryParse(req.QueryString["amid"], out int a) ? a : 1;

        Result<ActorMovie> result = await actorMovieService.Delete(amid);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "ActorMovie removed successfully!");
            await HttpUtils.Redirect(req, res, options, $"/movie/actor?mid={result.Value!.MovieId}");

        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, $"/movies");
        }
    }
}
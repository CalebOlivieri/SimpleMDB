using System.Collections;
using System.Net;
using System.Text;

namespace SimpleMDB;

public class App
{
    private HttpListener server;
    private HttpRouter router;
    private int requestId;

    public App()
    {
        string host = "http://127.0.0.1:8080/";
        server = new HttpListener();
        server.Prefixes.Add(host);
        requestId = 0;

        Console.WriteLine("Server Listening on..." + host);

        var userRepository = new MockUserRepository();
        var userService = new MockUserService(userRepository);
        var authController = new AuthController(userService);
        var userController = new UserController(userService);

        //var actorRepository = new MockActorRepository();
        var actorRepository = new MySqlActorRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=MakroSimpNobara1!;");
        var actorService = new MockActorService(actorRepository);
        var actorController = new ActorController(actorService);

        //var movieRepository = new MockMovieRepository();
        var movieRepository = new MySqlMovieRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=MakroSimpNobara1!;");
        var movieService = new MockMovieService(movieRepository);
        var movieController = new MovieController(movieService);

        //var actorMovieRepository = new MockActorMovieRepository(actorRepository, movieRepository);
        var actorMovieRepository = new MySqlActorMovieRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=MakroSimpNobara1!;");
        var actorMovieService = new MockActorMovieService(actorMovieRepository);
        var actorMovieController = new ActorMovieController(actorMovieService, actorService, movieService);

        router = new HttpRouter();
        router.Use(HttpUtils.ServeStaticFile);
        router.Use(HttpUtils.ReadRequestFormData);


        router.AddGet("/", authController.LandingPageGet);
        router.AddGet("/register", authController.RegisterGet);
        router.AddPost("/register", authController.RegisterPost);
        router.AddGet("/login", authController.LoginGet);
        router.AddPost("/login", authController.LoginPost);
        router.AddPost("/logout", authController.LogoutPost);

        router.AddGet("/users", authController.CheckAdmin, userController.ViewAllUsersGet);
        router.AddGet("/user/add", authController.CheckAdmin, userController.AddUserGet);
        router.AddPost("/user/add", authController.CheckAdmin, userController.AddUserPost);
        router.AddGet("/user/view", authController.CheckAdmin, userController.ViewUserGet);
        router.AddGet("/user/edit", authController.CheckAdmin, userController.EditUserGet);
        router.AddPost("/user/edit", authController.CheckAdmin, userController.EditUserPost);
        router.AddPost("/user/remove", authController.CheckAdmin, userController.RemoveUserPost);

        router.AddGet("/actors", actorController.ViewAllActorsGet);
        router.AddGet("/actor/add", authController.CheckAuth, actorController.AddActorGet);
        router.AddPost("/actor/add", authController.CheckAuth, actorController.AddActorPost);
        router.AddGet("/actor/view", authController.CheckAuth, actorController.ViewActorGet);
        router.AddGet("/actor/edit", authController.CheckAuth, actorController.EditActorGet);
        router.AddPost("/actor/edit", authController.CheckAuth, actorController.EditActorPost);
        router.AddPost("/actor/remove", authController.CheckAuth, actorController.RemoveActorPost);

        router.AddGet("/movies", movieController.ViewAllMoviesGet);
        router.AddGet("/movie/add", authController.CheckAuth, movieController.AddMovieGet);
        router.AddPost("/movie/add", authController.CheckAuth, movieController.AddMoviePost);
        router.AddGet("/movie/view", authController.CheckAuth, movieController.ViewMovieGet);
        router.AddGet("/movie/edit", authController.CheckAuth, movieController.EditMovieGet);
        router.AddPost("/movie/edit", authController.CheckAuth, movieController.EditMoviePost);
        router.AddPost("/movie/remove", authController.CheckAuth, movieController.RemoveMoviePost);

        router.AddGet("/actor/movie", authController.CheckAuth, actorMovieController.ViewAllMoviesByActor);
        router.AddGet("/actor/movie/add", authController.CheckAuth, actorMovieController.AddMoviesByActorGET);
        router.AddPost("/actor/movie/add", authController.CheckAuth, actorMovieController.AddMoviesByActorPOST);
        router.AddPost("/actor/movie/remove", authController.CheckAuth, actorMovieController.RemoveMoviesByActorPOST);

        router.AddGet("/movie/actor", authController.CheckAuth, actorMovieController.ViewAllActorsByMovie);
        router.AddGet("/movie/actor/add", authController.CheckAuth, actorMovieController.AddActorsByMovieGET);
        router.AddPost("/movie/actor/add", authController.CheckAuth, actorMovieController.AddActorsByMoviePOST);
        router.AddPost("/movie/actor/remove", authController.CheckAuth, actorMovieController.RemoveActorsByMoviePOST); 
        
    }

    public async Task Start()
    {
        server.Start();
        Console.WriteLine("Server started. Waiting for requests...");

        while (server.IsListening)
        {
            var ctx = await server.GetContextAsync(); 
            _ = HandleContextAsync(ctx); 
        }
    }

    public void Stop()
    {
        if (server.IsListening)
        {
            server.Stop();
            Console.WriteLine("Server stopped.");
        }
        server.Close();
        Console.WriteLine("Server closed.");
    }

    private async Task HandleContextAsync(HttpListenerContext ctx)
    {
        var req = ctx.Request;
        var res = ctx.Response;
        var options = new Hashtable();

        var rid = req.Headers["X-Request-Id"] ?? requestId.ToString().PadLeft(6, '0');
        var method = req.HttpMethod;
        var rawUrl = req.RawUrl;
        var remoteEndPoint = req.RemoteEndPoint;
        res.StatusCode = HttpRouter.RESPONSE_NOT_SENT_YET;
        DateTime startTime = DateTime.UtcNow;
        requestId++;

        try
        {
            await router.Handle(req, res, options);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);

            if (res.StatusCode == HttpRouter.RESPONSE_NOT_SENT_YET)
            {

                string content = Environment.GetEnvironmentVariable("DEVELOPMENT_MODE") != "Production" ?
                     ex.ToString() : "An unexpected error occurred."; 
                string html = HtmlTemplates.Base("SimpleMDB", "Error Page", content, "");
                await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.InternalServerError, html);
            }
        }
        finally
        {
            if (res.StatusCode == HttpRouter.RESPONSE_NOT_SENT_YET)
            {
                
                string html = HtmlTemplates.Base("SimpleMDB", "Not Found Page", "Resource was not found.", "");
                await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.NotFound, html);
            }

            TimeSpan elapsedTime = DateTime.UtcNow - startTime;

            Console.WriteLine($"Request {rid}: {method} {rawUrl} from {remoteEndPoint} --> {res.StatusCode} ({res.ContentLength64} bytes) [{res.ContentType}]in {elapsedTime.TotalMilliseconds}ms");
           
        }
    }
}
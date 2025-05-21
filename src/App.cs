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

        var actorRepository = new MockActorRepository();
        var actorService = new MockActorService(actorRepository);
        var actorController = new ActorController(actorService);

        var movieRepository = new MockMovieRepository();
        var movieService = new MockMovieService(movieRepository);
        var movieController = new MovieController(movieService);


        router = new HttpRouter();
        router.Use(HttpUtils.ServeStaticFile);
        router.Use(HttpUtils.ReadRequestFormData);


        router.AddGet("/", authController.LandingPageGet);

        router.AddGet("/users", userController.ViewAllUsersGet);
        router.AddGet("/user/add", userController.AddUserGet);
        router.AddPost("/user/add", userController.AddUserPost);
        router.AddGet("/user/view", userController.ViewUserGet);
        router.AddGet("/user/edit", userController.EditUserGet);
        router.AddPost("/user/edit", userController.EditUserPost);
        router.AddPost("/user/remove", userController.RemoveUserPost);

        router.AddGet("/actors", actorController.ViewAllActorsGet);
        router.AddGet("/actor/add", actorController.AddActorGet);
        router.AddPost("/actor/add", actorController.AddActorPost);
        router.AddGet("/actor/view", actorController.ViewActorGet);
        router.AddGet("/actor/edit", actorController.EditActorGet);
        router.AddPost("/actor/edit", actorController.EditActorPost);
        router.AddPost("/actor/remove", actorController.RemoveActorPost);
        
        router.AddGet("/movies", movieController.ViewAllMoviesGet);
        router.AddGet("/movie/add", movieController.AddMovieGet);
        router.AddPost("/movie/add", movieController.AddMoviePost);
        router.AddGet("/movie/view", movieController.ViewMovieGet);
        router.AddGet("/movie/edit", movieController.EditMovieGet);
        router.AddPost("/movie/edit", movieController.EditMoviePost);
        router.AddPost("/movie/remove", movieController.RemoveMoviePost);
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
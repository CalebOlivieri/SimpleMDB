using System.Collections;
using System.Net;
using System.Text;

namespace SimpleMDB;

public class App
{
    private HttpListener server;
    private HttpRouter router;

    public App()
    {
        string host = "http://127.0.0.1:8080/";
        server = new HttpListener();
        server.Prefixes.Add(host);

        Console.WriteLine("Server Listening on..." + host);

        var userRepository = new MockUserRepository();
        var userService = new MockUserService(userRepository);
        var authController = new AuthController(userService);
        var userController = new UserController(userService);

        router = new HttpRouter();
        router.Use(HttpUtils.ReadRequestFormData);

        router.AddGet("/", authController.LandingPageGet);
        router.AddGet("/users", userController.ViewAllGet);
        router.AddGet("/user/add", userController.AddGet);
        router.AddPost("/user/add", userController.AddPost);
        router.AddGet("/user/view", userController.ViewGet);
        router.AddGet("/user/edit", userController.EditGet);
        router.AddPost("/user/edit", userController.EditPost);
        router.AddGet("/user/remove", userController.RemoveGet);
    }

    public async Task Start()
    {
        server.Start();

        while (server.IsListening)
        {
            var ctx = server.GetContext();
            await HandleContextAsync(ctx);
        }
    }

    public void Stop()
    {
        server.Stop();
        server.Close();
    }

    private async Task HandleContextAsync(HttpListenerContext ctx)
    {
        var req = ctx.Request;
        var res = ctx.Response;
        var options = new Hashtable();

        await router.Handle(req, res, options);
    }
}

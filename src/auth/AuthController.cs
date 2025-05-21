using System.Collections;
using System.Net;
using System.Text;

namespace SimpleMDB;

public class AuthController
{
    private IUserService userService;

    public AuthController(IUserService userService)
    {
        this.userService = userService;
    }

    public async Task LandingPageGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        // Define a message (can be dynamic if needed, e.g., from query string)
        string message = ""; // Or req.QueryString["message"] ?? ""; if you want to pass messages to the landing page

        string htmlContent = $@"
        <nav>
        <ul>
        <li><a href=""/register"">Register</a></li>
        <li><a href=""/login"">Login</a></li>
        <li><a href=""/logout"">Logout</a></li>
        <li><a href=""/users"">Users</a></li>
        <li><a href=""/actors"">Actors</a></li>
        <li><a href=""/movies"">Movies</a></li>
        </ul>
        </nav>
        ";

        
        string content = HtmlTemplates.Base("SimpleMDB", "Landing Page", htmlContent, message);

        
        await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
    }
}
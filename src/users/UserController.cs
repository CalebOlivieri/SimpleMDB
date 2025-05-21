using System.Collections;
using System.Collections.Specialized;
using System.Net;

namespace SimpleMDB;

public class UserController
{
    private IUserService userService;

    public UserController(IUserService userService)
    {
        this.userService = userService;
    }

    // GET /users
    public async Task ViewAllUsersGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

        Result<PagedResult<User>> result = await userService.ReadAll(page, size);

        if (result.IsValid)
        {
            PagedResult<User> pagedResult = result.Value!;
            List<User> users = pagedResult.Values;
            int userCount = pagedResult.TotalCount;

            string html = UserHtmlTemplates.ViewAllUsersGet(users, userCount, page, size);
            string content = HtmlTemplates.Base("SimpleMDB", "User View All Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
        }
        else
        {
            string errorMessage = result.Error?.Message ?? "Error loading users.";
            string errorHtml = HtmlTemplates.Base("SimpleMDB", "Error", errorMessage, ""); // Use Base template for error
            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.InternalServerError, errorHtml);
        }
    }

    // GET /user/add
    public async Task AddUserGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string username = req.QueryString["username"] ?? "";
        string role = req.QueryString["role"] ?? "";
        string message = req.QueryString["message"] ?? "";

        string html = UserHtmlTemplates.AddUserGet(username, role);
        string content = HtmlTemplates.Base("SimpleMDB", "Users Add Page", html, message);

        await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
    }

    // POST /user/add
    public async Task AddUserPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? new NameValueCollection();

        string username = formData["username"] ?? "";
        string password = formData["password"] ?? "";
        string role = formData["role"] ?? "";

        User newUser = new User(0, username, password, "", role);
        Result<User> result = await userService.Create(newUser);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "User added succesfully!");
            
            await HttpUtils.Redirect(req, res, options, "/users");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect", "username", username);
            HttpUtils.AddOptions(options, "redirect", "role", role);

            await HttpUtils.Redirect(req, res, options, "/user/add");
        }
    }

    // GET /user/view?uid=1
    public async Task ViewUserGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 1;

        Result<User> result = await userService.Read(uid);

        if (result.IsValid)
        {
            User user = result.Value!;
            // Corrected method name: UserHtmlTemplates.ViewGet
            string html = UserHtmlTemplates.ViewUserGet(user);
            string content = HtmlTemplates.Base("SimpleMDB", "Users View Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, "/users");
        }
    }

    // GET /user/edit?uid=1
    public async Task EditUserGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";
        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 1;

        Result<User> result = await userService.Read(uid);

        if (result.IsValid)
        {
            User user = result.Value!;
            string html = UserHtmlTemplates.EditUserGet(uid, user);
            string content = HtmlTemplates.Base("SimpleMDB", "Users Edit Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, "/users");
        }
    }

    // POST /user/edit?uid=1
    public async Task EditUserPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? new NameValueCollection();
        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 0;
 
        string username = formData["username"] ?? "";
        string password = formData["password"] ?? "";
        string role = formData["role"] ?? "";

        User updatedUser = new User(uid, username, password, "", role); 

        Result<User> result = await userService.Update(uid, updatedUser);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "User edited successfully!");
            await HttpUtils.Redirect(req, res, options, "/users");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, "/user/edit");
        }
    }

    // POST /user/remove?uid=1
    public async Task RemoveUserPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : 1;
       
        Result<User> result = await userService.Delete(uid);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "User removed successfully!");
            await HttpUtils.Redirect(req, res, options, "/users");

        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, "/user/edit");
        }
    }
}
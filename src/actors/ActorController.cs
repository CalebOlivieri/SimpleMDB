using System.Collections;
using System.Collections.Specialized;
using System.Net;
using SimpleMDB;


namespace SimpleMDB;

public class ActorController
{
    private IActorService actorService;

    public ActorController(IActorService actorService)
    {
        this.actorService = actorService;
    }

    // GET /actors
    public async Task ViewAllActorsGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

        Result<PagedResult<Actor>> result = await actorService.ReadAll(page, size);

        if (result.IsValid)
        {
            PagedResult<Actor> pagedResult = result.Value!;
            List<Actor> actors = pagedResult.Values;
            int actorCount = pagedResult.TotalCount;

            string html = ActorHtmlTemplates.ViewAllActorsGet(actors, actorCount, page, size);
            string content = HtmlTemplates.Base("SimpleMDB", "Actor View All Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
        }
        else
        {
            string errorMessage = result.Error?.Message ?? "Error loading actors.";
            string errorHtml = HtmlTemplates.Base("SimpleMDB", "Error", errorMessage, ""); // Use Base template for error
            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.InternalServerError, errorHtml);
        }
    }

    // GET /actor/add
    public async Task AddActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string firstname = req.QueryString["firstname"] ?? "";
        string lastname = req.QueryString["lastname"] ?? "";
        string bio = req.QueryString["bio"] ?? "";
        string rating = req.QueryString["rating"] ?? "";
        string message = req.QueryString["message"] ?? "";

        string html = ActorHtmlTemplates.AddActorGet(firstname, lastname, bio, rating);
        string content = HtmlTemplates.Base("SimpleMDB", "Actors Add Page", html, message);

        await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
    }

    // POST /actor/add
    public async Task AddActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? new NameValueCollection();

        string firstname = formData["firstname"] ?? "";
        string lastname = formData["lastname"] ?? "";
        string bio = req.QueryString["bio"] ?? "";
        float rating = float.TryParse(formData["rating"], out float r) ? r : 5F;

        Actor newActor = new Actor(0, firstname, lastname, "", rating);
        Result<Actor> result = await actorService.Create(newActor);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Actor added succesfully!");
            
            await HttpUtils.Redirect(req, res, options, "/actors");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect", formData);

            await HttpUtils.Redirect(req, res, options, "/actor/add");
        }
    }

    // GET /actor/view?aid=1
    public async Task ViewActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : 1;

        Result<Actor> result = await actorService.Read(aid);

        if (result.IsValid)
        {
            Actor actor = result.Value!;
            // Corrected method name: ActorHtmlTemplates.ViewGet
            string html = ActorHtmlTemplates.ViewActorGet(actor);
            string content = HtmlTemplates.Base("SimpleMDB", "Actors View Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, "/actors");
        }
    }

    // GET /actor/edit?aid=1
    public async Task EditActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";
        int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : 1;

        Result<Actor> result = await actorService.Read(aid);

        if (result.IsValid)
        {
            Actor actor = result.Value!;
          
            string html = ActorHtmlTemplates.EditActorGet(aid, actor);
            string content = HtmlTemplates.Base("SimpleMDB", "Actors Edit Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, content);
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, "/actors");
        }
    }

    // POST /actor/edit?aid=1
    public async Task EditActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? new NameValueCollection();
        int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : 0;
 
        string firstname = formData["firstname"] ?? "";
        string lastname = formData["lastname"] ?? "";
        string bio = formData["bio"] ?? "";
        float rating = float.TryParse(formData["rating"], out float r) ? r : 5F;

        Actor updatedActor = new Actor(aid, firstname, lastname, bio, rating); 

        Result<Actor> result = await actorService.Update(aid, updatedActor);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Actor edited successfully!");
            await HttpUtils.Redirect(req, res, options, "/actors");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);

            await HttpUtils.Redirect(req, res, options, "/actor/edit");
        }
    }

    // POST /actor/remove?aid=1
    public async Task RemoveActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : 1;
       
        Result<Actor> result = await actorService.Delete(aid);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Actor removed successfully!");
            await HttpUtils.Redirect(req, res, options, "/actors");

        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, "/actor/edit");
        }
    }
}
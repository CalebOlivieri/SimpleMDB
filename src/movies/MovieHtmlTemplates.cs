using System.Collections;
using System.Net;
using System.Text;
using SimpleMDB;


namespace SimpleMDB;

public class MovieHtmlTemplates
{
    public static string ViewAllMoviesGet(List<Movie> movies, int movieCount, int page, int size)
    {
        int pageCount = (int)Math.Ceiling((double)movieCount / size);
        int prevPage = page > 1 ? page - 1 : 1;
        int nextPage = page < pageCount ? page + 1 : pageCount;

        string rows = "";

        foreach (var movie in movies)
        {
            rows += @$"
                <tr>
                    <td>{movie.Id}</td>
                    <td>{movie.Title}</td>
                    <td>{movie.Year}</td>
                    <td>{movie.Description}</td>
                    <td>{movie.Rating}</td>
                    <td><a href=""/movie/view?mid={movie.Id}"">View</a></td>
                    <td><a href=""/movie/edit?mid={movie.Id}"">Edit</a></td>
                    <td><form action=""/movie/remove?mid={movie.Id}"" method=""POST"" onsubmit=""return confirm('Are you sure that you want to delete this movie?')"">
                        <input type =""submit"" value=""Remove"">
                       </form>
                      </td>
                </tr>";
        }

         string pDisable = (page > 1).ToString().ToLower();
         string nDisable = (page < pageCount).ToString().ToLower();

        string html = $@"
            <div class=""add"">
               <a href=""/movie/add"">Add new Movie</a>
             </div>
            <table class=""viewall"">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Title</th>
                        <th>Year</th>
                        <th>Description</th>
                        <th>Rating</th>
                        <th>View</th>
                        <th>Edit</th>
                        <th>Remove</th>
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
            </table>
            <div class=""pagination""> 
                <a href=""?page=1&size={size}"" onclick=""return {pDisable};"">First</a>
                <a href=""?page={prevPage}&size={size}"" onclick=""return {pDisable};"">Prev</a>
                <span>{page}/{pageCount}</span>
                <a href=""?page={nextPage}&size={size}"" onclick=""return {nDisable};"">Next</a>
                <a href=""?page={pageCount}&size={size}"" onclick=""return {nDisable};"">Last</a>
            </div>
            ";

        return html;
    }

    public static string AddMovieGet(string title, int year, string description, string rating) 
    {

        string html = $@"
        <form class=""addform"" action=""/movie/add"" method=""POST"">
            <label for=""title"">Title</label>
            <input id=""title"" name=""title"" type=""text"" placeholder=""Title"" value=""{title}"">
            <label for=""year"">Year</label>
            <input id=""year"" name=""year"" type=""number"" min=""1888"" max=""{DateTime.Now.Year}"" step=""1""placeholder=""Year"" value=""{year}"">
            <label for=""description"">Description</label>
            <input id=""description"" name=""description"" type=""text"" placeholder=""Description"" value=""{description}"">
            <label for=""rating"">Rating</label>
            <input id=""rating"" name=""rating"" type=""number"" min=""0"" max=""10"" step=""0.1"" value=""{rating}"">
            <input type=""submit"" value=""Add"">
        </form>
        ";

        return html;
    }

    public static string ViewMovieGet(Movie movie)
    {
        string html = $@"
            <table class=""view"">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Title</th>
                        <th>Year</th>
                        <th>Description</th>
                        <th>Rating</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                    <td>{movie.Id}</td>
                    <td>{movie.Title}</td>
                    <td>{movie.Year}</td>
                    <td>{movie.Description}</td>
                    <td>{movie.Rating}</td>
                </tr>
                </tbody>
            </table>
        ";
        return html;
    }

    public static string EditMovieGet(int mid, Movie movie)
    {

        string html = $@"
        <form class=""editform"" action=""/movie/edit?mid={mid}"" method=""POST"">
            <label for=""title"">Title</label>
            <input id=""title"" name=""title"" type=""text"" placeholder=""Title"" value = ""{movie.Title}"">
            <label for=""year"">Year</label>
            <input id=""year"" name=""year"" type=""number"" min=""1888"" max=""{DateTime.Now.Year}"" step=""1""placeholder=""Year"" value=""{movie.Year}"">
            <label for=""description"">Description</label>
            <input id=""description"" name=""description"" type=""text"" placeholder=""Description"" value=""{movie.Description}"">
            <label for=""rating"">Rating</label>
            <input id=""rating"" name=""rating"" type=""number"" min=""0"" max=""10"" step=""0.1"" value=""{movie.Rating}"">
            <input type=""submit"" value=""Edit"">
        </form>
        ";
        return html;
    }
}
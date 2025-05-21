using System.Collections;
using System.Net;
using System.Text;

namespace SimpleMDB;

public class UserHtmlTemplates
{
        public static string ViewAllUsersGet(List<User> users, int userCount, int page, int size)
    {
        int pageCount = (int)Math.Ceiling((double)userCount / size);
        int prevPage = page > 1 ? page - 1 : 1;
        int nextPage = page < pageCount ? page + 1 : pageCount;

        string rows = "";

        foreach (var user in users)
        {
            rows += @$"
                <tr>
                    <td>{user.Id}</td>
                    <td>{user.Username}</td>
                    <td>{user.Password}</td>
                    <td>{user.Salt}</td>
                    <td>{user.Role}</td>
                    <td><a href=""/user/view?uid={user.Id}"">View</a></td>
                    <td><a href=""/user/edit?uid={user.Id}"">Edit</a></td>
                    <td><form action=""/user/remove?uid={user.Id}"" method=""POST"" onsubmit=""return confirm('Are you sure that you want to delete this user?')"">
                        <input type =""submit"" value=""Remove"">
                       </form>
                     </td>
                </tr>";
        }

        string html = $@"
            <div class=""add"">
             <a href=""/user/add"">Add new User</a>
             </div>
            <table class=""viewall"">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Username</th>
                        <th>Password</th>
                        <th>Salt</th>
                        <th>Role</th>
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
                <a href=""?page=1&size={size}"">First</a>
                <a href=""?page={prevPage}&size={size}"">Prev</a>
                <span>{page}/{pageCount}</span>
                <a href=""?page={nextPage}&size={size}"">Next</a>
                <a href=""?page={pageCount}&size={size}"">Last</a>
            </div>
            ";

        return html;
    }

    public static string AddUserGet(string username, string role)
    {
        string roles = "";

        // Assuming Roles.ROLES exists and is a collection of strings
        foreach (var r in Roles.ROLES)
        {
            string selected = r == role ? " selected" : "";
            roles += $@"<option value=""{r}""{selected}>{r}</option>";
        }

        string html = $@"
        <form class=""addform"" action=""/user/add"" method=""POST"">
            <label for=""username"">Username</label>
            <input id=""username"" name=""username"" type=""text"" placeholder=""Username"" value=""{username}"">
            <label for=""password"">Password</label>
            <input id=""password"" name=""password"" type=""password"" placeholder=""Password"">
            <label for=""role"">Role</label>
            <select id=""role"" name=""role"">
                {roles}
            </select>
            <input type=""submit"" value=""Add"">
        </form>
        ";

        return html;
    }

    public static string ViewUserGet(User user)
    {
        string html = $@"
            <table class=""view"">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Username</th>
                        <th>Password</th>
                        <th>Salt</th>
                        <th>Role</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                    <td>{user.Id}</td>
                    <td>{user.Username}</td>
                    <td>{user.Password}</td>
                    <td>{user.Salt}</td>
                    <td>{user.Role}</td>
                </tr>
                </tbody>
            </table>
        ";
        return html;
    }

    public static string EditUserGet(int uid, User user)
    {
        string roles = "";

        // Assuming Roles.ROLES exists and is a collection of strings
        foreach (var role in Roles.ROLES)
        {
            string selected = (role == user.Role) ? " selected" : "";
            roles += $@"<option value=""{role}""{selected}>{role}</option>"; // Corrected: added selected attribute
        }

        string html = $@"
        <form class=""editform"" action=""/user/edit?uid={uid}"" method=""POST"">
            <label for=""username"">Username</label>
            <input id=""username"" name=""username"" type=""text"" placeholder=""Username"" value = ""{user.Username}"">
            <label for=""password"">Password</label>
            <input id=""password"" name=""password"" type=""password"" placeholder=""Password""> {{/* Usually you don't pre-fill password for security */}}
            <label for=""role"">Role</label>
            <select id=""role"" name=""role"">
                {roles}
            </select>
            <input type=""submit"" value=""Edit"">
        </form>
        ";
        return html;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LStu.Models;
using System.Web.Security;
using LStu.Utils;

namespace LStu.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                UserDTO currentUser = data.UserDTO
                         .Where(u => u.Login_ID == this.User.Identity.Name)
                         .Single();

                string sql = "SELECT b.* FROM dbo.T_PERMISSION a left join dbo.T_RESOURCE b on a.Resource_Code=b.Code WHERE a.Role_Code='" + currentUser.Role_Code + "' ORDER BY b.Sort ASC";
                var items = data.ExecuteQuery<ResourceDTO>(sql).ToList();

                ViewData["authorities"] = items;
            }

            return View();
        }
    }
}

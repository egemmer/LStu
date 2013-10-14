using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LStu.Controllers
{
    public class ConstantController : Controller
    {

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

    }
}

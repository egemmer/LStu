using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LStu.Models;
using LStu.Utils;

namespace LStu.Controllers
{
    public class PermissionController : Controller
    {

        [Authorize]
        public ActionResult List()
        {
            List<ResourceDTO> items = null;

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                items = data.ResourceDTO.ToList();
            }

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult HasPermissions()
        {
            string roleCode = Request.Params["roleCode"];

            if (Const.IsNullOrEmpty(roleCode))
            {
                return View();
            }
            else
            {
                using (DataClassesDataContext data = new DataClassesDataContext())
                {
                    var items = data.PermissionDTO.Where(u => u.Role_Code == roleCode).ToList();

                    return Json(items, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Authorize()
        {
            string roleCode = Request.Params["roleCode"];
            string resourceCodes = Request.Params["resourceCodes"];

            //if (roleCode == "Administrator")
            //{
            //    return Json(new { success = false, message = "系统管理员不允许修改。" }, JsonRequestBehavior.AllowGet);
            //}

            if (Const.IsNullOrEmpty(roleCode))
            {
                return Json(new { success = false, message = "参数错误。" }, JsonRequestBehavior.AllowGet);
            }

            if (resourceCodes == null) resourceCodes = "";

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                var items = data.PermissionDTO.Where(u => u.Role_Code == roleCode).ToList();
                data.PermissionDTO.DeleteAllOnSubmit(items);

                string[] splits = resourceCodes.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var s in splits)
                {
                    data.PermissionDTO.InsertOnSubmit(new PermissionDTO() { Role_Code = roleCode, Resource_Code = s });
                }

                data.SubmitChanges();
            }


            return Json(new { success = true, message = string.Empty }, JsonRequestBehavior.AllowGet);
        }
    }
}

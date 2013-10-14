using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LStu.Models;
using LStu.Utils;

namespace LStu.Controllers
{
    public class RoleController : Controller
    {
        //
        // GET: /Role/

        public ActionResult Index()
        {
            return View();
        }
        
        [Authorize]
        public ActionResult List()
        {
            ActionResult retval = null;
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                retval = Json(data.RoleDTO.ToList(), JsonRequestBehavior.AllowGet);
            }

            return retval;
        }

        [HttpPost]
        [Authorize]
        public ActionResult Create(RoleDTO entity)
        {
            int errors = 0;
            string message = string.Empty;

            if (Const.IsNullOrEmpty(entity.Name))
            {
                errors++;
                message = "角色名称不能为空。<br/>";
            }

            if (Const.IsNullOrEmpty(entity.Code))
            {
                errors++;
                message = "角色编码不能为空。<br/>";
            }

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                if (data.RoleDTO.Where(u => u.Code == entity.Code).Count() > 0)
                {
                    errors++;
                    message = entity + " 角色已经存在。<br/>";
                }

                if (errors == 0)
                {
                    data.RoleDTO.InsertOnSubmit(entity);
                    data.SubmitChanges();
                }
            }

            return Json(new { success = (errors == 0), message = message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Update(RoleDTO entity)
        {
            if (entity.Code == "Administrator")
            {
                return Json(new { success = false, message = "系统管理员不能修改。<br/>" }, JsonRequestBehavior.AllowGet);
            }

            int errors = 0;
            string message = string.Empty;

            if (Const.IsNullOrEmpty(entity.Name))
            {
                errors++;
                message = "角色名称不能为空。<br/>";
            }

            if (Const.IsNullOrEmpty(entity.Code))
            {
                errors++;
                message = "角色编码不能为空。<br/>";
            }

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                var items = data.RoleDTO.Where(u => u.Code == entity.Code)
                    .ToList();

                if (items.Count == 0)
                {
                    errors++;
                    message = entity.Name + " 角色不存在。<br/>";
                }

                if (errors == 0)
                {
                    var e = items.First();

                    e.Name = entity.Name;
                    e.Description = entity.Description;

                    data.SubmitChanges();
                }
            }

            return Json(new { success = (errors == 0), message = message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Delete()
        {
            var code = Request.Params["code"];

            if (Const.IsNullOrEmpty(code))
            {
                return Json(new { success = false, message = "code 参数错误。" }, JsonRequestBehavior.AllowGet);
            }

            if (code == "Administrator")
            {
                return Json(new { success = false, message = "系统管理员不能修改。<br/>" }, JsonRequestBehavior.AllowGet);
            }

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                var count = data.UserDTO.Where(u => u.Role_Code == code).Count();

                if (count > 0)
                {
                    return Json(new { success = false, message = "用户正在使用这个角色，请用户换角色再删除。" }, JsonRequestBehavior.AllowGet);
                }

                data.ExecuteCommand("DELETE FROM T_ROLE WHERE CODE=@p0", code);
            }

            return Json(new { success = true, message = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Permissions()
        {
            var code = Request.Params["code"];


            return null;
        }
    }
}

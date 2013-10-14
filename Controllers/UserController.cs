using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using LStu.Models;
using LStu.Utils;

namespace LStu.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        [Authorize]
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
                string sql = "SELECT A.Login_ID, A.Username, A.Department_ID, A.Role_Code, B.Name AS Department_Name, C.Code AS Role_Code, C.Name AS Role_Name FROM dbo.T_USER A LEFT JOIN dbo.T_DEPARTMENT B ON A.DEPARTMENT_ID=B.ID LEFT JOIN dbo.T_ROLE C ON A.ROLE_CODE=C.CODE ORDER BY A.Login_ID ASC";
                var items = data.ExecuteQuery<UserModel>(sql).ToList();
                retval = Json(items, JsonRequestBehavior.AllowGet);
            }

            return retval;
        }



        [HttpPost]
        [Authorize]
        public ActionResult Create(UserDTO entity)
        {
            int errors = 0;
            string message = string.Empty;

            if (Const.IsNullOrEmpty(entity.Login_ID))
            {
                errors++;
                message = "帐号名不能为空。<br/>";
            }

            if (Const.IsNullOrEmpty(entity.Username))
            {
                errors++;
                message = "用户名不能为空。<br/>";
            }

            if (Const.IsNullOrEmpty(entity.Role_Code)) entity.Role_Code = "Guest";

            if (entity.Department_ID <= 0) entity.Department_ID = 1;

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                if (data.UserDTO.Where(u => u.Login_ID == entity.Login_ID).Count() > 0)
                {
                    errors++;
                    message = entity.Login_ID + " 帐号已经存在。<br/>";
                }

                if (errors == 0)
                {
                    entity.Password = "123456a";
                    data.UserDTO.InsertOnSubmit(entity);
                    data.SubmitChanges();
                }
            }

            return Json(new {success = (errors == 0), message = message}, JsonRequestBehavior.AllowGet);
        } 

        [HttpPost]
        [Authorize]
        public ActionResult Update(UserDTO entity)
        {
            try
            {
                if (entity.Login_ID == "Admin")
                {
                    return Json(new { success = false, message = "Admin 帐号不能修改。<br/>" }, JsonRequestBehavior.AllowGet);
                }

                int errors = 0;
                string message = string.Empty;

                if (Const.IsNullOrEmpty(entity.Login_ID))
                {
                    errors++;
                    message = "帐号不能为空。<br/>";
                }

                if (Const.IsNullOrEmpty(entity.Username))
                {
                    errors++;
                    message = "用户名不能为空。<br/>";
                }

                if (Const.IsNullOrEmpty(entity.Role_Code)) entity.Role_Code = "Guest";

                if (entity.Department_ID <= 0) entity.Department_ID = 1;

                using (DataClassesDataContext data = new DataClassesDataContext())
                {
                    var items = data.UserDTO.Where(u => u.Login_ID == entity.Login_ID)
                        .ToList();

                    if (items.Count == 0)
                    {
                        errors++;
                        message = entity.Login_ID + " 帐号不存在。<br/>";
                    }

                    if (errors == 0)
                    {
                        var e = items.First();

                        e.Login_ID = entity.Login_ID;
                        e.Department_ID = entity.Department_ID;
                        e.Role_Code = entity.Role_Code;
                        e.Username = entity.Username;

                        data.SubmitChanges();
                    }
                }

                return Json(new { success = (errors == 0), message = message }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /User/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete()
        {
            var loginID = Request.Params["Login_ID"];

            if(Const.IsNullOrEmpty(loginID)) {
                return Json(new { success = false, message = "Login_ID 参数错误。" }, JsonRequestBehavior.AllowGet);
            }

            if (loginID == "Admin")
            {
                return Json(new { success = false, message = "Admin 帐号不能删除。" }, JsonRequestBehavior.AllowGet);
            }

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                var items = data.UserDTO.Where(u => u.Login_ID == loginID).ToList();
                if (items.Count > 0)
                {
                    var entity = items.First();
                    data.UserDTO.DeleteOnSubmit(entity);
                    data.SubmitChanges();
                }
            }

            return Json(new { success = true, message = string.Empty }, JsonRequestBehavior.AllowGet);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LStu.Models;
using LStu.Utils;

namespace LStu.Controllers
{
    public class DepartmentController : Controller
    {
        //
        // GET: /Department/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Create(DepartmentDTO entity)
        {
            int errors = 0;
            string message = string.Empty;

            if (Const.IsNullOrEmpty(entity.Name))
            {
                errors++;
                message = "部门名称不能为空。<br/>";
            }

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                if (data.DepartmentDTO.Where(u => u.ID == entity.ID).Count() > 0)
                {
                    errors++;
                    message = entity + " 部门已经存在。<br/>";
                }

                if (errors == 0)
                {
                    data.DepartmentDTO.InsertOnSubmit(entity);
                    data.SubmitChanges();
                }
            }

            return Json(new { success = (errors == 0), message = message }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult List()
        {
            ActionResult retval = null;
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                 retval = Json(data.DepartmentDTO.ToList(), JsonRequestBehavior.AllowGet);
            }

            return retval;
        }

        [HttpPost]
        [Authorize]
        public ActionResult Update(DepartmentDTO entity)
        {
            try
            {
                if (entity.ID == 1)
                {
                    return Json(new { success = false, message = "默认部门不能修改。<br/>" }, JsonRequestBehavior.AllowGet);
                }

                int errors = 0;
                string message = string.Empty;

                if (Const.IsNullOrEmpty(entity.Name))
                {
                    errors++;
                    message = "部门名称不能为空。<br/>";
                }

                using (DataClassesDataContext data = new DataClassesDataContext())
                {
                    var items = data.DepartmentDTO.Where(u => u.ID == entity.ID)
                        .ToList();

                    if (items.Count == 0)
                    {
                        errors++;
                        message = entity.Name + " 部门不存在。<br/>";
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
            catch
            {
                return View();
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Delete()
        {
            var Department_ID = Request.Params["Department_ID"];

            if (Const.IsNullOrEmpty(Department_ID))
            {
                return Json(new { success = false, message = "Department_ID 参数错误。" }, JsonRequestBehavior.AllowGet);
            }

            var id = Convert.ToInt64(Department_ID);

            if (id == 1)
            {
                return Json(new { success = false, message = "默认部门不能删除。" }, JsonRequestBehavior.AllowGet);
            }

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                var items = data.DepartmentDTO.Where(u => u.ID == id).ToList();
                if (items.Count > 0)
                {
                    var entity = items.First();
                    data.DepartmentDTO.DeleteOnSubmit(entity);
                    data.SubmitChanges();
                }
            }

            return Json(new { success = true, message = string.Empty }, JsonRequestBehavior.AllowGet);
        }

    }
}

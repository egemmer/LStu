using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LStu.Models;
using LStu.Utils;
using System.Configuration;

namespace LStu.Controllers
{
    public class ProjectController : Controller
    {
        //
        // GET: /Project/

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult List()
        {
            List<ProjectDTO> items;
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                items = data.ProjectDTO.ToList();
            }

            return Json(items.Select(u => new {
                ID = u.ID,
                Project_Code = u.Project_Code,
                Project_Name = u.Project_Name,
                Project_Date = u.Project_Date,
                Business_Owner = u.Business_Owner}).ToList(), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Create(ProjectDTO entity)
        {

            if (Const.IsNullOrEmpty(entity.Project_Code) && Const.IsNullOrEmpty(entity.Project_Name))
            {
                return Json(new
                {
                    success = false,
                    errors = "E001",
                    message = "项目编码、项目名称不允许空值。"
                }, "text/html", JsonRequestBehavior.AllowGet);
            }

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                if (data.ProjectDTO.Where(u => u.Project_Code == entity.Project_Code).Count() > 0)
                {
                    return Json(new
                    {
                        success = false,
                        errors = "E002",
                        message = "项目编码已经存在。"
                    }, "text/html", JsonRequestBehavior.AllowGet);
                }

                var directory = ConfigurationManager.AppSettings["Repository"];
                directory += entity.Project_Code;
                Const.Direcotry(directory);

                entity.Project_Date = Const.StripSubfix(entity.Project_Code);
                data.ProjectDTO.InsertOnSubmit(entity);
                data.SubmitChanges();
            }

            return Json(new
            {
                success = true,
                errors = "B000",
                message = "操作已成功。"
            }, "text/html", JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Update(ProjectDTO entity)
        {
            try
            {
                int errors = 0;
                string message = string.Empty;

                if (Const.IsNullOrEmpty(entity.Project_Code))
                {
                    errors++;
                    message = "项目编码不能为空。<br/>";
                }

                if (Const.IsNullOrEmpty(entity.Project_Name))
                {
                    errors++;
                    message = "项目名称不能为空。<br/>";
                }

                using (DataClassesDataContext data = new DataClassesDataContext())
                {
                    var items = data.ProjectDTO.Where(u => u.Project_Code == entity.Project_Code)
                        .ToList();

                    if (items.Count == 0)
                    {
                        errors++;
                        message = entity.Project_Code + " 项目不存在。<br/>";
                    }

                    if (errors == 0)
                    {
                        var e = items.First();

                        e.Project_Name = entity.Project_Name;
                        e.Project_Date = Const.StripSubfix(entity.Project_Code);
                        e.Business_Owner = entity.Business_Owner;

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

        [Authorize]
        public ActionResult Delete()
        {
            var id = Request.Params["ID"];

            if (!Const.IsNullOrEmpty(id))
            {
                using (DataClassesDataContext data = new DataClassesDataContext())
                {
                    var items = data.ProjectDTO.Where(t => t.ID == Convert.ToInt32(id)).ToList();
                    if (items.Count > 0)
                    {
                        var entity = items.First();
                        data.ProjectDTO.DeleteOnSubmit(entity);
                        data.SubmitChanges();

                        var filepath = ConfigurationManager.AppSettings["Repository"] + entity.Project_Code;
                        System.IO.Directory.Delete(filepath);
                    }
                }
            }

            return Json(new { success = true });
        }
    }
}

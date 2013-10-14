using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LStu.Utils;
using LStu.Models;

namespace LStu.Controllers
{
    public class ProjectStageController : Controller
    {

        [Authorize]
        public ActionResult List()
        {
            List<ProjectStageDTO> items;
            
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                items = data.ProjectStageDTO.OrderBy(t => t.Name).ToList();
            }

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Create(ProjectStageDTO entity)
        {
            var errors = "B000";
            string message = string.Empty;

            if (Const.IsNullOrEmpty(entity.Name))
            {
                errors = "B001";
                message = "项目阶段名称不能为空。";
            }

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                if (data.ProjectStageDTO.Where(u => u.Name == entity.Name).Count() > 0)
                {
                    errors = "B002";
                    message = "项目阶段已经存在。";
                }

                if (errors == "B000")
                {
                    data.ProjectStageDTO.InsertOnSubmit(entity);
                    data.SubmitChanges();
                }
            }

            return Json(new { success = (errors == "B000"), errors = errors, message = message }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Delete()
        {
            var id = Request.Params["ID"];

            var errors = "B000";
            var message = "操作成功";

            if (!Const.IsNullOrEmpty(id))
            {
                using (DataClassesDataContext data = new DataClassesDataContext())
                {
                    var items = data.ProjectStageDTO.Where(t => t.ID == Convert.ToInt32(id)).ToList();
                    if (items.Count > 0)
                    {
                        var entity = items.First();

                        var profiles = data.ProfileDTO.Where(t => t.Project_Stage == entity.Name).Count();

                        if (profiles > 0)
                        {
                            errors = "B001";
                            message = "该项目阶段已包含文档, 不允删除";
                        }
                        else
                        {
                           
                            data.ProjectStageDTO.DeleteOnSubmit(entity);
                            data.SubmitChanges();
                        }
                    }
                }
            }

            var retval = new
            {
                success = (errors == "B000"),
                errors = errors,
                message = message
            };

            return Json(retval, "text/html", JsonRequestBehavior.AllowGet);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LStu.Models;
using System.Web.Routing;
using LStu.Utils;

namespace LStu.Controllers
{
    public class ArchiveController : Controller
    {
        private IBarcodeService BarcodeService { get; set; }
        private IProfileTemporaryService ProfileTemporaryService { get; set; }
        private IProjectService ProjectService { get; set; }
        private IProjectStageService ProjectStageService { get; set; }
        private IProjectSubjectService ProjectSubjectService { get; set; }

        //
        // GET: /Archive/

        protected override void Initialize(RequestContext requestContext)
        {
            if (ProfileTemporaryService == null) { ProfileTemporaryService = new ProfileTemporaryService(); }
            if (BarcodeService == null) { BarcodeService = new BarcodeService(); }
            if (ProjectService == null) { ProjectService = new ProjectService(); }
            if (ProjectStageService == null) { ProjectStageService = new ProjectStageService(); }
            if (ProjectSubjectService == null) { ProjectSubjectService = new ProjectSubjectService(); }

            base.Initialize(requestContext);
        }

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Find()
        {
            string barcode = Request.Params["barcode"];

            List<ProfileTemporaryDTO> items = new List<ProfileTemporaryDTO>();

            var e  = ProfileTemporaryService.GetProfileTemporary(barcode);

            if (e != null)
            {
                items.Add(e);
            }

            return Json(items.Select(u => new {
                Barcode = u.Barcode,
                Project_Code = u.Project_Code,
                Project_Stage = u.Project_Stage.Replace(" ", ""),
                Project_Name = u.Project_Name,
                Project_Subject = u.Project_Subject.Replace(" ", ""),
                File_Name = u.File_Name,
                Diagram_Code = u.Diagram_Code,
                Diagram_Name = u.Diagram_Name,
                Diagram_Version = u.Diagram_Version,
                Diagram_Scale = u.Diagram_Scale,
                Created_Date = u.Created_Date.ToString("yyyy-MM-dd"),
                Plotter = u.Plotter
            }), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult IsExisted()
        {
            string barcode = Request.Params["barcode"];

            List<ProfileTemporaryDTO> items = new List<ProfileTemporaryDTO>();

            var e = ProfileTemporaryService.GetProfileTemporary(barcode);

            return Json(new { success = true, existed = e != null }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult List()
        {
            string str = Request.Params["items"];
            
            List<ProfileTemporaryDTO> items = new List<ProfileTemporaryDTO>();

            if (Const.NotEmpty(str))
            {
                using (DataClassesDataContext data = new DataClassesDataContext())
                {
                    var splits = str.Split(';');
                    var results = (from e in data.ProfileTemporaryDTO select e).Where(t => splits.Contains(t.Barcode)).ToList();
                    items.AddRange(results);
                }
            }

            return Json(items.Select(u => new
            {
                Barcode = u.Barcode,
                Project_Code = u.Project_Code,
                Project_Stage = u.Project_Stage.Replace(" ", ""),
                Project_Name = u.Project_Name,
                Project_Subject = u.Project_Subject.Replace(" ", ""),
                File_Name = u.File_Name,
                Diagram_Code = u.Diagram_Code,
                Diagram_Name = u.Diagram_Name,
                Diagram_Version = u.Diagram_Version,
                Diagram_Scale = u.Diagram_Scale,
                Created_Date = u.Created_Date.ToString("yyyy-MM-dd"),
                Plotter = u.Plotter
            }), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Execute()
        {
            var barcode = Request.Params["barcode"];
            var handler = Request.Params["handler"];
            var archiveOption = Request.Params["archiveOption"];

            if (Const.IsNullOrEmpty(barcode))
            {
                var result = new { errors = "E001", message = "条形码不能为空。" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            ProfileTemporaryDTO entity = ProfileTemporaryService.GetProfileTemporary(barcode);

            if (entity == null)
            {
                var result = new { errors = "B001", message = "无预备归档记录。" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (BarcodeService.IsExists(barcode))
            {
                var result = new { errors = "B002", message = "条形码已存在。" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (Const.IsNullOrEmpty(entity.Project_Code))
            {
                var result = new { errors = "B003", message = "项目编码不能为空。" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (Const.IsNullOrEmpty(entity.Project_Name))
            {
                var result = new { errors = "B004", message = "项目名称不能为空。" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (handler == "B008")
            {
                using (DataClassesDataContext data = new DataClassesDataContext())
                {
                    var newEntity = new ProjectDTO()
                    {
                        Project_Code = entity.Project_Code,
                        Project_Name = entity.Project_Name,
                        Project_Date = Const.StripSubfix(entity.Project_Code),
                        Business_Owner = entity.Business_Owner
                    };
                    data.ProjectDTO.InsertOnSubmit(newEntity);
                    data.SubmitChanges();
                }
            }

            if (Const.IsNullOrEmpty(entity.Project_Stage))
            {
                var result = new { errors = "B005", message = "项目阶段不能为空。" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (Const.IsNullOrEmpty(entity.Project_Subject))
            {
                var result = new { errors = "B006", message = "项目专业不能为空。" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (Const.IsNullOrEmpty(entity.File_Name))
            {
                var result = new { errors = "B007", message = "文件名不能为空。" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (!ProjectService.IsExists(entity.Project_Code))
            {
                var result = new { errors = "B008", message = "项目编码不存在。" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (!ProjectStageService.IsExists(entity.Project_Stage.Replace(" ", "")))
            {
                var result = new { errors = "B009", message = "项目阶段不存在。" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (!ProjectSubjectService.IsExists(entity.Project_Subject.Replace(" ", "")))
            {
                var result = new { errors = "B010", message = "项目专业不存在。" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            int status = ProfileTemporaryService.Archive(barcode, false);

            if (status == 2)
            {
                var result = new { errors = "B011", message = "试图访问磁盘上不存在的文件失败。" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else if (status == 3)
            {
                var result = new { errors = "B012", message = "当找不到文件或目录。" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else if (status == 4)
            {
                var result = new { errors = "B013", message = "当调用的方法不受支持，或试图读取、查找<br/>或写入不支持调用功能的流。" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else if (status == 5)
            {
                var result = new { errors = "B014", message = "发生 I/O 错误。" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(new {errors = "B000", message = "已成功。"}, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Update(ProfileTemporaryDTO update)
        {
            if (Const.IsNullOrEmpty(update.Project_Code))
            {
                return Json(new
                {
                    success = false,
                    errors = "B001",
                    message = "项目编码必须填写."
                }, "text/html", JsonRequestBehavior.AllowGet);
            }

            if (Const.IsNullOrEmpty(update.Project_Code))
            {
                return Json(new
                {
                    success = false,
                    errors = "B002",
                    message = "项目名称必须填写."
                }, "text/html", JsonRequestBehavior.AllowGet);
            }

            if (Const.IsNullOrEmpty(update.Project_Stage))
            {
                return Json(new
                {
                    success = false,
                    errors = "B003",
                    message = "项目阶段必须填写."
                }, "text/html", JsonRequestBehavior.AllowGet);
            }

            if (Const.IsNullOrEmpty(update.Project_Subject))
            {
                return Json(new
                {
                    success = false,
                    errors = "B004",
                    message = "项目专业必须填写."
                }, "text/html", JsonRequestBehavior.AllowGet);
            }

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                List<ProfileTemporaryDTO> items = data.ProfileTemporaryDTO
                    .Where(u => u.Barcode == update.Barcode)
                    .ToList();

                if (items.Count > 0)
                {
                    ProfileTemporaryDTO entity = items.First();

                    entity.Diagram_Name = update.Diagram_Name;
                    entity.Diagram_Code = update.Diagram_Code;
                    entity.Diagram_Version = update.Diagram_Version;
                    entity.Diagram_Scale = update.Diagram_Scale;

                    entity.Project_Code = update.Project_Code;
                    entity.Project_Name = update.Project_Name;
                    entity.Project_Stage = update.Project_Stage.Replace(" ", "");
                    entity.Project_Subject = update.Project_Subject.Replace(" ", "");
                    entity.Project_Date = Const.StripSubfix(update.Project_Code);

                    entity.Business_Owner = update.Business_Owner;
                    entity.File_Name = update.File_Name;
                    data.SubmitChanges();
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        errors = "B008",
                        message = "无预备归档记录."
                    }, "text/html", JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new
            {
                success = true,
                errors = "B000",
                message = "操作已成功."
            }, "text/html", JsonRequestBehavior.AllowGet);
        }

    }
}

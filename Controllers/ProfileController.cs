using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LStu.Models;
using System.Web.Routing;
using LStu.Utils;
using System.Configuration;
using System.IO;
using LStu.Core;

namespace LStu.Controllers
{
    public class ProfileController : Controller
    {
        private IProfileService ProfileService { get; set; }
        private IBarcodeService BarcodeService { get; set; }
        private IProjectService ProjectService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (ProfileService == null) { ProfileService = new ProfileService(); }
            if (BarcodeService == null) { BarcodeService = new BarcodeService(); }
            if (ProjectService == null) { ProjectService = new ProjectService(); }
            base.Initialize(requestContext);
        }
        //
        // GET: /Profile/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Profile/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Profile/Create

        [Authorize]
        [HandleError(View="Error")]
        public ActionResult Create(ProfileDTO profile)
        {
            bool success = false;

            var handler = Request.Params["handler"];

            if (Const.IsNullOrEmpty(profile.Project_Code))
            {
                var result = new { errors = "B003", message = "项目编码不能为空。" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var uploadFile = Request.Files["UploadFile"];
            
            string filepath = ConfigurationManager.AppSettings["Repository"];
            filepath += profile.Project_Code + @"\";
            filepath += profile.Project_Stage + @"\";
            filepath += profile.Project_Subject + @"\";

            Const.Direcotry(filepath);

            string barcode = BarcodeService.Create();
            string filename = barcode + "@" + Const.GetSimpleFileName(uploadFile.FileName);
            filepath += filename;

            try
            {
                uploadFile.SaveAs(filepath);

                var extension = Const.GetExtension(filepath);
                if (extension.ToLower() == ".dwg")
                {
                    // 转换PDF文件
                    var pdfFileName = Const.RemoveExtension(filepath) + ".pdf";
                    DwgDirectXManager.Instance.ConvertToPDF(new InputOutputParam()
                    {
                        dwgFileName = filepath,
                        pdfFileName = pdfFileName
                    });
                }

                profile.Barcode = barcode;
                profile.File_Name = filename;
                profile.Created_Date = DateTime.Now;
                profile.Plotter = User.Identity.Name;

                ProfileService.Save(profile);
                success = true;
            }
            catch
            {
                System.IO.File.Delete(filepath);
            }

            return Json(new { success = success }, "text/html");
        }

        public ActionResult IsExisted(string projectCode)
        {
            if (Const.IsNullOrEmpty(projectCode))
            {
                throw new ArgumentNullException();
            }

            bool existed = false;

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
               existed = data.ProjectDTO.Where(t => t.Project_Code == projectCode).Count() > 0;
            }

            return Json(new { success = true, existed = existed }, "text/html");
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            ViewDataDictionary ViewData = new ViewDataDictionary();
            ViewData.Add("message", filterContext.Exception.Message);
            filterContext.Result = new ViewResult { 
                ViewName = "Error",
                ViewData = ViewData,
                TempData = filterContext.Controller.TempData
            };
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}

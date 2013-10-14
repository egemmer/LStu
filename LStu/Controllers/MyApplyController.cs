using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LStu.Models;
using LStu.Utils;
using System.Configuration;
using System.IO;

namespace LStu.Controllers
{
    public class MyApplyController : Controller
    {
        //
        // GET: /MyApply/

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult List()
        {
            var status = Request.Params["status"];
            var loginID = User.Identity.Name;
            int startIndex = Convert.ToInt32(Request.Params["start"]);
            int maxResults = Convert.ToInt32(Request.Params["limit"]);

            List<AuditDTO> items = null;
            var count = 0;
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                IQueryable<AuditDTO> query = null;
                if (Const.IsNullOrEmpty(status)) status = "0"; // 默认查询等待审核

                query = (from e in data.AuditDTO select e).Where(t => t.Status == Convert.ToInt32(status) && t.Login_ID == loginID);
                
                count = query.Count();
                items = query.OrderByDescending(t => t.Apply_Date).Skip(startIndex).Take(maxResults).ToList();
            }

            var result = new
            {
                count = count,
                items = items.Select(t => new
                {
                    ID = t.ID,
                    Project_Code = t.Project_Code,
                    Project_Name = t.Project_Name,
                    Project_Stage = t.Project_Stage,
                    Project_Subject = t.Project_Subject,
                    Barcode = t.Barcode,
                    Diagram_Name = t.Diagram_Name,
                    Login_ID = t.Login_ID,
                    File_Name = t.File_Name,
                    Reason = t.Reason,
                    Status = t.Status,
                    Audit_Date = t.Audit_Date.HasValue ? t.Audit_Date.ToString() : null,
                    Apply_Date = t.Apply_Date.HasValue ? t.Apply_Date.ToString() : null,
                    StatusDescription = Const.GetStatusDesc(t.Status)
                }).ToList()
            };

            return Json(result, "text/html", JsonRequestBehavior.AllowGet);
        }


        public ActionResult Download()
        {
            string id = Request.Params["id"];
            if (Const.IsNullOrEmpty(id))
            {
                ViewData["errors"] = "B001";
                ViewData["message"] = "参数错误";
                return View();
            }
            else
            {
                using (DataClassesDataContext data = new DataClassesDataContext())
                {
                    var items = data.AuditDTO.Where(t => t.ID == Convert.ToInt32(id)).ToList();
                    if (items.Count > 0)
                    {
                        var entity = items.First();
                        if (entity.Status == 1)
                        {
                            if (Const.NotEmpty(entity.File_Name)
                                && Const.NotEmpty(entity.Project_Subject)
                                && Const.NotEmpty(entity.Project_Stage)
                                && Const.NotEmpty(entity.Project_Code))
                            {
                                var filepath = ConfigurationManager.AppSettings["Repository"] + @"\";
                                filepath += entity.Project_Code + @"\";
                                filepath += entity.Project_Stage + @"\";
                                filepath += entity.Project_Subject + @"\";
                                filepath += entity.File_Name;

                                var contentType = "application/octet-stream";
                                var extension = Const.GetExtension(entity.File_Name);
                                switch (extension)
                                {
                                    case ".dwg": contentType = "application/acad"; break;
                                    case ".xls": contentType = "application/vnd.ms-excel"; break;
                                    case ".xlsx": contentType = "application/vnd.ms-excel"; break;
                                    case ".doc": contentType = "application/msword"; break;
                                }

                                return File(filepath, contentType, entity.File_Name);
                            }
                            else 
                            {
                                var contentType = "application/x-zip-compressed";

                                var filepath = ConfigurationManager.AppSettings["Repository"];
                                filepath += entity.Project_Code + @"\";
                                if (Const.NotEmpty(entity.Project_Stage))
                                {
                                    filepath += entity.Project_Stage + @"\";
                                }
                                if (Const.NotEmpty(entity.Project_Subject))
                                {
                                    filepath += entity.Project_Subject + @"\";
                                }

                                MemoryStream stream = new MemoryStream();
                                if (!Const.CompressDirectory(filepath, stream))
                                {
                                    ViewData["errors"] = "B004";
                                    ViewData["message"] = "压缩失败，可能目录不存在。";
                                    return View();
                                }
                                byte[] buffer = stream.ToArray();
                                return File(buffer, contentType, DateTime.Now + ".zip");
                            }
                        }
                        else
                        {
                            ViewData["errors"] = "B003";
                            ViewData["message"] = "申请状态有误。";
                            return View();
                        }
                    }
                    else
                    {
                        ViewData["errors"] = "B002";
                        ViewData["message"] = "没有找到申请记录。";
                        return View();
                    }
                }
            }
        }
    }
}

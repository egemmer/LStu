using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LStu.Models;
using System.Web.Routing;
using System.Configuration;
using System.IO;
using LStu.Utils;
using System.Collections;
using LStu.Core;

namespace LStu.Controllers
{
    public class SearchController : Controller
    {
        private IProjectService ProjectService { get; set; }
        private IProfileService ProfileService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (ProjectService == null) { ProjectService = new ProjectService(); }
            if (ProfileService == null) { ProfileService = new ProfileService(); }

            base.Initialize(requestContext);
        }
        //
        // GET: /Search/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            List<ProfileDTO> data = null;

            string at = Request.Params["at"];

            if (Const.NotEmpty(at) && at == "F")
            {
                return FuzzyQuery();
            }

            string subject = Request.Params["subject"];
            string stage = Request.Params["stage"];
            string code = Request.Params["code"];
            int startIndex = Convert.ToInt32(Request.Params["start"]);
            int maxResults = Convert.ToInt32(Request.Params["limit"]);

            int count = ProfileService.Count(code, stage, subject);
            data = ProfileService.List(code, stage, subject, startIndex, maxResults);

            var result = new
            {
                count = count,
                items = data.Select(u => new
                {
                    Barcode = u.Barcode,
                    Project_Code = u.Project_Code,
                    Project_Stage = u.Project_Stage,
                    Project_Name = u.Project_Name,
                    Project_Subject = u.Project_Subject,
                    Diagram_Code = u.Diagram_Code,
                    Diagram_Name = u.Diagram_Name,
                    Diagram_Version = u.Diagram_Version,
                    Diagram_Scale = u.Diagram_Scale,
                    File_Name = u.File_Name,
                    Created_Date = u.Created_Date.ToString("yyyy-MM-dd"),
                    Plotter = u.Plotter
                }).ToList()
            };

            return Json(result, "text/html",JsonRequestBehavior.AllowGet);
        }

        public ActionResult FuzzyQuery()
        {
            List<ProfileDTO> rows = null;

            string code = Request.Params["code"];
            string name = Request.Params["name"];
            int startIndex = Convert.ToInt32(Request.Params["start"]);
            int maxResults = Convert.ToInt32(Request.Params["limit"]);

            int count;
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                var query = from e in data.ProfileDTO select e;
                if (Const.NotEmpty(code))
                {
                    query = query.Where(t => t.Project_Code.Contains(code));
                }

                if(Const.NotEmpty(name))
                {
                    query = query.Where(t => t.Project_Name.Contains(name));
                }

                query = query.OrderByDescending(t => t.Created_Date);

                count = query.Count();
                rows = query.Skip(startIndex).Take(maxResults).ToList();
            }

            var result = new
            {
                count = count,
                items = rows.Select(u => new
                {
                    Barcode = u.Barcode,
                    Project_Code = u.Project_Code,
                    Project_Stage = u.Project_Stage,
                    Project_Name = u.Project_Name,
                    Project_Subject = u.Project_Subject,
                    Diagram_Code = u.Diagram_Code,
                    Diagram_Name = u.Diagram_Name,
                    Diagram_Version = u.Diagram_Version,
                    Diagram_Scale = u.Diagram_Scale,
                    File_Name = u.File_Name,
                    Created_Date = u.Created_Date.ToString("yyyy-MM-dd"),
                    Plotter = u.Plotter
                }).ToList()
            };

            return Json(result, "text/html", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Childrens(TreeNodeModel clicked)
        {
            List<TreeNodeModel> nodes = new List<TreeNodeModel>();

            var id = clicked.id;

            if (id == "root")
            {
                using (DataClassesDataContext data = new DataClassesDataContext())
                {
                    var items = (from e in data.ProjectDTO select e.Project_Date).Distinct().OrderByDescending(t => t);

                    foreach (string e in items)
                    {
                        nodes.Add(new TreeNodeModel()
                        {
                            id = e,
                            text = e,
                            leaf = false,
                            Order = 1,
                            cls = "folder",
                            Year = e,
                            Project_Code = "",
                            Project_Name = "",
                            Project_Stage = "",
                            Project_Subject = ""
                        });
                    }
                }
            }
            else if (Const.IsNullOrEmpty(clicked.Project_Code) && Const.IsNullOrEmpty(clicked.Project_Stage) && Const.IsNullOrEmpty(clicked.Project_Subject))
            {
                var items = ProjectService.List().Where(t => t.Project_Date == clicked.Year).OrderByDescending(t => t.Project_Date);

                foreach (ProjectDTO e in items)
                {
                    nodes.Add(new TreeNodeModel() {
                        id = Convert.ToString(e.ID), 
                        text = e.Project_Code, 
                        leaf = false,
                        Order = 2,
                        cls = "folder",
                        Year = clicked.Year,
                        Project_Code = e.Project_Code, 
                        Project_Name = e.Project_Name, 
                        Project_Stage = "",
                        Project_Subject = ""
                    });
                }
            }
            else if (Const.IsNullOrEmpty(clicked.Project_Stage) && Const.IsNullOrEmpty(clicked.Project_Subject))
            {
                using (DataClassesDataContext data = new DataClassesDataContext())
                {
                    var items = data.ProjectStageDTO.OrderByDescending(t => t.Name);

                    foreach (ProjectStageDTO e in items)
                    {
                        nodes.Add(new TreeNodeModel()
                        {
                            id = clicked.id + "-" + Convert.ToString(e.ID),
                            text = e.Name,
                            leaf = false,
                            Order = 3,
                            cls = "folder",
                            Year = clicked.Year,
                            Project_Code = clicked.Project_Code,
                            Project_Name = clicked.Project_Name,
                            Project_Stage = e.Name,
                            Project_Subject = ""
                        });
                    }
                }
            }
            else if (Const.NotEmpty(clicked.Project_Stage) && Const.IsNullOrEmpty(clicked.Project_Subject))
            {
                using (DataClassesDataContext data = new DataClassesDataContext())
                {
                    var items = data.ProjectSubjectDTO.OrderByDescending(t => t.Name);

                    foreach (ProjectSubjectDTO e in items)
                    {
                        nodes.Add(new TreeNodeModel() {
                            id = clicked.id + "-" + Convert.ToString(e.ID),
                            text = e.Name,
                            leaf = true,
                            Order = 4,
                            cls = "file",
                            Year = clicked.Year,
                            Project_Code = clicked.Project_Code,
                            Project_Name = clicked.Project_Name,
                            Project_Stage = clicked.Project_Stage,
                            Project_Subject = e.Name
                        });
                    }
                }
            }

            return Json(nodes, "text/html", JsonRequestBehavior.AllowGet);
        }

        public ActionResult StageRecords()
        {
            List<ProjectStageDTO> items = null;
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                items = data.ProjectStageDTO.ToList();
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SubjectRecords()
        {
            List<ProjectSubjectDTO> items = null;
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                items = data.ProjectSubjectDTO.ToList();
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Preview()
        {
            string barcode = Request.Params["barcode"];

            ProfileDTO e = ProfileService.GetProfile(barcode);
            if (e == null)
            {
                ViewData["message"] = "数据库中,没有找到归档文件信息.";
                return View();
            }

            var filename = e.File_Name;
            var extension = Const.GetExtension(filename);
            if (extension == ".dwg")
            {
                filename = Const.RemoveExtension(filename) + ".pdf";
                extension = ".pdf";
            }

            string filepath = ConfigurationManager.AppSettings["Repository"];
            filepath += e.Project_Code + @"\";
            filepath += e.Project_Stage + @"\";
            filepath += e.Project_Subject + @"\";
            filepath += filename;

            if (!System.IO.File.Exists(filepath))
            {
                ViewData["message"] = filename + " 下载文件不存在. ";
                return View();
            }

            var contentType = "application/octet-stream";
            switch (extension)
            {
                //case ".dwg": contentType = "application/acad"; break;
                case ".xls": contentType = "application/vnd.ms-excel"; break;
                case ".xlsx": contentType = "application/vnd.ms-excel"; break;
                case ".doc": contentType = "application/msword"; break;
                case ".pdf": contentType = "application/pdf"; break;
            }

            return File(filepath, contentType, filename);
        }

        [Authorize]
        public ActionResult FindAuditors()
        {
            ActionResult retval = null;
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                var sql = "select t.* from T_User t, T_Permission r where t.Role_Code=r.Role_Code and r.Resource_Code='Audit'";
                var items = data.ExecuteQuery<UserModel>(sql).ToList();
                retval = Json(items, JsonRequestBehavior.AllowGet);
            }

            return retval;
        }

        [Authorize]
        public ActionResult Apply(AuditDTO row)
        {
            var errors = "";
            string message = "";

            row.Login_ID = User.Identity.Name;
            row.Status = 0; // 初始化状态

            if (Const.IsNullOrEmpty(row.Project_Code))
            {
                errors = "B001";
                message = "项目编码不能为空。";
            }
            else if (Const.IsNullOrEmpty(row.Project_Name))
            {
                errors = "B002";
                message = "项目名称不能为空。";
            }
            else if (Const.IsNullOrEmpty(row.Reason))
            {
                errors = "B003";
                message = "申请理由不能为空。";
            }
            else
            {
                using (DataClassesDataContext data = new DataClassesDataContext())
                {
                    row.Apply_Date = DateTime.Now;
                    data.AuditDTO.InsertOnSubmit(row);
                    data.SubmitChanges();
                }

                errors = "B000";
                message = "申请借阅成功。";
            }

            var responseContext = new
            {
                success = (errors == "B000"),
                errors = errors,
                message = message
            };

            return Json(responseContext, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult ApplyForBatch()
        {
            string reason = Request.Params["reason"];
            string items = Request.Params["items"];
            string auditor = Request.Params["Audit_User_ID"];

            if (Const.IsNullOrEmpty(reason))
            {
                return Json(new
                {
                    success = false,
                    errors = "B001",
                    message = "reason 参数错误."
                }, JsonRequestBehavior.AllowGet);
            }

            if (Const.IsNullOrEmpty(items))
            {
                return Json(new
                {
                    success = false,
                    errors = "B002",
                    message = "items 参数错误."
                }, JsonRequestBehavior.AllowGet);
            }

            if (Const.IsNullOrEmpty(auditor))
            {
                return Json(new
                {
                    success = false,
                    errors = "B003",
                    message = "auditor 参数错误."
                }, JsonRequestBehavior.AllowGet);
            }

            string[] barcodes = items.Split(';');
            try
            {
                using (DataClassesDataContext data = new DataClassesDataContext())
                {
                    var rows = data.ProfileDTO.Where(t => barcodes.Contains(t.Barcode));

                    foreach (ProfileDTO e in rows)
                    {
                        AuditDTO audit = new AuditDTO
                        {
                            Login_ID = User.Identity.Name,
                            Status = 0, // 初始化状态
                            Apply_Date = DateTime.Now,
                            Reason = reason,
                            Project_Code = e.Project_Code,
                            Project_Name = e.Project_Name,
                            Project_Stage = e.Project_Stage,
                            Project_Subject = e.Project_Subject,
                            Barcode = e.Barcode,
                            File_Name = e.File_Name,
                            Audit_User_ID = auditor,
                            Diagram_Name = e.Diagram_Name
                        };

                        data.AuditDTO.InsertOnSubmit(audit);
                    }

                    data.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = false,
                    errors = "B004",
                    message = "批量申请借阅失败, 原因: " + e.Message
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                success = true,
                errors = "B000",
                message = "批量申请借阅成功."
            }, JsonRequestBehavior.AllowGet);
        }
    }
}

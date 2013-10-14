using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LStu.Models;
using LStu.Utils;

namespace LStu.Controllers
{
    public class AuditController : Controller
    {

        private IAuditService AuditService { get; set; }
        //
        // GET: /Audit/

        public ActionResult Index()
        {
            return View();
        }

        private void SetUsername(AuditDTO audit)
        {
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                var q = from u in data.UserDTO where u.Login_ID == audit.Login_ID select u;
                if (q.Count() > 0)
                {
                    audit.Username = q.First().Username;
                }
            }
        }

        [Authorize]
        public ActionResult List()
        {
            var status = Request.Params["status"];
            int startIndex = Convert.ToInt32(Request.Params["start"]);
            int maxResults = Convert.ToInt32(Request.Params["limit"]);

            List<AuditDTO> items;
            var count = 0;
            using (DataClassesDataContext data = new DataClassesDataContext())
            {

                var q = from s in data.AuditDTO select s;

                var identity = User.Identity.Name;
                if (Const.IsNullOrEmpty(status)) status = "0"; // 默认查询等待审核

                IQueryable<AuditDTO> query = (from e in data.AuditDTO select e).Where(t => t.Status == Convert.ToInt32(status) && t.Audit_User_ID == identity).OrderByDescending(t => t.Apply_Date);
                count = query.Count();
                items = query.Skip(startIndex).Take(maxResults).ToList();
            }

            items.ForEach(SetUsername);

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
                    Username = t.Username,
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

        public ActionResult Agree()
        {
            string[] items = Request.Params["items"].Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries);

            using(DataClassesDataContext data = new DataClassesDataContext())
            {
                var entities = data.AuditDTO.Where(u => items.Contains(u.ID.ToString())).ToList();

                foreach (var entity in entities)
                {
                    entity.Status = 1;
                    entity.Audit_Date = DateTime.Now;
                }

                data.SubmitChanges();
            }

            return Json(new {success=true, message=string.Empty}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Unagree()
        {
            string[] items = Request.Params["items"].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                var entities = data.AuditDTO.Where(u => items.Contains(u.ID.ToString())).ToList();

                foreach (var entity in entities)
                {
                    entity.Status = 2;
                    entity.Audit_Date = DateTime.Now;
                }

                data.SubmitChanges();
            }

            return Json(new { success = true, message = string.Empty }, JsonRequestBehavior.AllowGet);
        }
    }
}

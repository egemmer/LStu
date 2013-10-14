using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using LStu.Models;

namespace LStu.Controllers
{

    [HandleError]
    public class AccountController : Controller
    {

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            var requestBy = Request.Headers["Request-By"];

            if(requestBy == "Ext") {
                return Json(new { redirect = "/Home/Index", signOut = true }, JsonRequestBehavior.AllowGet);
            }

            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "提供的用户名或密码不正确。");
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }
        

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            var oldPassword = model.OldPassword;
            var newPassword = model.NewPassword;
            var confirmPassword = model.ConfirmPassword;
            var loginId = User.Identity.Name;

            var success = false;
            var message = "";
            if (newPassword != confirmPassword)
            {
                message = "密码不一致";
            }
            else
            {
                using (DataClassesDataContext data = new DataClassesDataContext())
                {
                    var user = data.UserDTO.Where(u => u.Login_ID == loginId && u.Password == oldPassword).FirstOrDefault();
                    if (user == null)
                    {
                        message = "原密码不正确";
                    }
                    else
                    {
                        success = true;
                        message = "密码修改成功";

                        user.Password = newPassword;
                        data.SubmitChanges();
                    }
                }
            }

            return Json(new { success = success, message = message }, JsonRequestBehavior.AllowGet);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

    }
}

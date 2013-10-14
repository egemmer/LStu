using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using System.Data.Objects;
using System.Configuration;
using System.Linq.Expressions;
using LStu.Utils;
using System.Data.SqlClient;
using LStu.Core;

namespace LStu.Models
{
    #region 模型
    public class TreeNodeModel
    {
        [Required]
        [DisplayName("标识")]
        public string id { get; set; }
        [Required]
        [DisplayName("等级")]
        public int Order { get; set; }
        [Required]
        [DisplayName("项目创建年份")]
        public string Year { get; set; }
        [Required]
        [DisplayName("项目编码")]
        public string Project_Code { get; set; }
        [Required]
        [DisplayName("项目名称")]
        public string Project_Name { get; set; }
        [Required]
        [DisplayName("项目阶段")]
        public string Project_Stage { get; set; }
        [Required]
        [DisplayName("项目专业")]
        public string Project_Subject { get; set; }
        [Required]
        [DisplayName("展示文本")]
        public string text { get; set; }
        [Required]
        [DisplayName("叶子节点")]
        public bool leaf { get; set; }
        [Required]
        [DisplayName("节点类型")]
        public string cls { get; set; }
    }

    public class UserModel
    {
        public string Login_ID { get; set; }
        public string Username { get; set; }
        public long Department_ID { get; set; }
        public string Department_Name { get; set; }
        public string Role_Code { get; set; }
        public string Role_Name { get; set; }
    }

    [PropertiesMustMatch("NewPassword", "ConfirmPassword", ErrorMessage = "新密码和确认密码不匹配。")]
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("当前密码")]
        public string OldPassword { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("新密码")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("确认新密码")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [DisplayName("用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("密码")]
        public string Password { get; set; }

        [DisplayName("记住我?")]
        public bool RememberMe { get; set; }
    }

    [PropertiesMustMatch("Password", "ConfirmPassword", ErrorMessage = "密码和确认密码不匹配。")]
    public class RegisterModel
    {
        [Required]
        [DisplayName("用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("电子邮件地址")]
        public string Email { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("密码")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("确认密码")]
        public string ConfirmPassword { get; set; }
    }
    #endregion

    #region Services

    public interface IBarcodeService
    {
        bool IsExists(string barcode);
        string Create();
    }

    public class BarcodeService : IBarcodeService
    {
        public bool IsExists(string barcode)
        {
            bool retval = false;
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                var count = data.ProfileDTO.Where(t => t.Barcode == barcode).Count();
                retval = count > 0;
            }
            return retval;
        }

        public string Create()
        {
            return "U" + DateTime.Now.ToString("yyyymmddhhMMss");
        }
    }

    public interface IProfileService
    {
        ProfileDTO Save(ProfileDTO entity);
        ProfileDTO GetProfile(string barcode);
        List<ProfileDTO> List(string code, string stage, string subject, int startIndex, int maxResults);
        int Count(string code, string stage, string subject);
    }

    public class ProfileService : IProfileService
    {
        public ProfileDTO Save(ProfileDTO entity)
        {
            if (GetProfile(entity.Barcode) == null)
            {
                using (DataClassesDataContext data = new DataClassesDataContext())
                {
                    data.ProfileDTO.InsertOnSubmit(entity);
                    data.SubmitChanges();
                }
            }

            return entity;
        }

        public ProfileDTO GetProfile(string barcode)
        {
            ProfileDTO retval = null;

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                var items = data.ProfileDTO.Where(t => t.Barcode == barcode).ToList();
                if (items.Count > 0)
                {
                    retval = items.First();
                }
            }

            return retval;
        }

        public List<ProfileDTO> List(string code, string stage, string subject, int startIndex, int maxResults)
        {
            List<ProfileDTO> retval = null;

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                if (Const.NotEmpty(code) && Const.IsNullOrEmpty(stage) && Const.IsNullOrEmpty(subject))
                {
                    retval = (from e in data.ProfileDTO select e).OrderByDescending(t => t.Created_Date).Where(t => t.Project_Code == code).Skip(startIndex).Take(maxResults).ToList();
                }
                else if (Const.NotEmpty(code) && Const.NotEmpty(stage) && Const.IsNullOrEmpty(subject))
                {
                    retval = (from e in data.ProfileDTO select e).OrderByDescending(t => t.Created_Date).Where(t => t.Project_Code == code && t.Project_Stage == stage).Skip(startIndex).Take(maxResults).ToList();
                }
                else if (Const.NotEmpty(code) && Const.NotEmpty(stage) && Const.NotEmpty(subject))
                {
                    retval = (from e in data.ProfileDTO select e).OrderByDescending(t => t.Created_Date).Where(t => t.Project_Code == code && t.Project_Stage == stage && t.Project_Subject == subject).Skip(startIndex).Take(maxResults).ToList();
                }
                else
                {
                    retval = (from e in data.ProfileDTO select e).OrderByDescending(t => t.Created_Date).Skip(startIndex).Take(maxResults).ToList();
                }
            }

            return retval;
        }

        public int Count(string code, string stage, string subject)
        {
            int retval = 0;

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                if (Const.NotEmpty(code) && Const.IsNullOrEmpty(stage) && Const.IsNullOrEmpty(subject))
                {
                    retval = (from e in data.ProfileDTO select e).Where(t => t.Project_Code == code).Count();
                }
                else if (Const.NotEmpty(code) && Const.NotEmpty(stage) && Const.IsNullOrEmpty(subject))
                {
                    retval = (from e in data.ProfileDTO select e).Where(t => t.Project_Code == code && t.Project_Stage == stage).Count();
                }
                else if (Const.NotEmpty(code) && Const.NotEmpty(stage) && Const.NotEmpty(subject))
                {
                    retval = (from e in data.ProfileDTO select e).Where(t => t.Project_Code == code && t.Project_Stage == stage && t.Project_Subject == subject).Count();
                }
                else
                {
                    retval = (from e in data.ProfileDTO select e).Count();
                }
            }

            return retval;
        }

    }

    public interface IProjectService
    {
        ProjectDTO Get(string projectCode);

        List<ProjectDTO> List();

        bool IsExists(string projectCode);
    }

    public class ProjectService : IProjectService
    {
        public ProjectDTO Get(string projectCode)
        {
            ProjectDTO retval = null;

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                var items = data.ProjectDTO.Where(t => t.Project_Code == projectCode).ToList();
                if (items.Count > 0)
                {
                    retval = items.First();
                }
            }

            return retval;
        }

        public List<ProjectDTO> List()
        {
            List<ProjectDTO> items = null;

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                items = data.ProjectDTO.ToList();
            }

            return items;
        }

        public bool IsExists(string projectCode)
        {
            bool retval = false;
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                retval = data.ProjectDTO.Where(t => t.Project_Code == projectCode).Count() > 0;
            }

            return retval;
        }
    }

    public interface IProjectStageService
    {
        bool IsExists(string name);
        void Add(string name);
    }

    public class ProjectStageService : IProjectStageService
    {

        public bool IsExists(string name)
        {
            bool retval = false;
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                retval = data.ProjectStageDTO.Where(t => t.Name == name).Count() > 0;
            }
            return retval;
        }

        public void Add(string name)
        {
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                data.ProjectStageDTO.InsertOnSubmit(new ProjectStageDTO() { Name = name });
                data.SubmitChanges();
            }
        }
    }

    public interface IProjectSubjectService
    {
        bool IsExists(string name);
        void Add(string name);
    }

    public class ProjectSubjectService : IProjectSubjectService
    {

        public bool IsExists(string name)
        {
            bool retval = false;
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                retval = data.ProjectSubjectDTO.Where(t => t.Name == name).Count() > 0;
            }
            return retval;
        }

        public void Add(string name)
        {
            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                data.ProjectSubjectDTO.InsertOnSubmit(new ProjectSubjectDTO() { Name = name });
                data.SubmitChanges();
            }
        }
    }

    public interface IProfileTemporaryService
    {

        ProfileTemporaryDTO GetProfileTemporary(string barcode);
        int Archive(string barcode, bool archiveOption);
    }

    public class ProfileTemporaryService : IProfileTemporaryService
    {
        public ProfileTemporaryDTO GetProfileTemporary(string barcode)
        {
            ProfileTemporaryDTO retval = null;
            using (DataClassesDataContext context = new DataClassesDataContext())
            {
                var items = context.ProfileTemporaryDTO.Where(c => c.Barcode == barcode).ToList();
                if (items.Count > 0)
                {
                    retval = items.First();
                }
            }

            return retval;
        }

        public int Archive(string barcode, bool archiveOption)
        {
            int retval = 0;
            var repository = ConfigurationManager.AppSettings["Repository"];
            var temporary = ConfigurationManager.AppSettings["Temporary"];

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                var items = data.ProfileTemporaryDTO.Where(u => u.Barcode == barcode).ToList();

                if (items.Count > 0)
                {
                    ProfileTemporaryDTO e = items.First();

                    try
                    {
                        string source = temporary + e.File_Name;

                        string directory = e.Project_Code + @"\" + e.Project_Stage + @"\" + e.Project_Subject + @"\";
                        string dest = repository + directory;

                        Const.Direcotry(dest);
                        e.File_Name = e.Barcode + '@' + e.File_Name; // 替换文件名, 使文件唯一存储在磁盘
                        dest += e.File_Name;

                        File.Copy(source, dest, true);
                        var extension = Const.GetExtension(dest);
                        if (extension.ToLower() == ".dwg")
                        {
                            // 转换PDF文件
                            var pdfFileName = Const.RemoveExtension(dest) + ".pdf";
                            DwgDirectXManager.Instance.ConvertToPDF(new InputOutputParam()
                            {
                                dwgFileName = dest,
                                pdfFileName = pdfFileName
                            });
                        }

                        retval = 1;
                    }
                    catch (FileNotFoundException)
                    {
                        retval = 2;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        retval = 3;
                    }
                    catch (NotSupportedException)
                    {
                        retval = 4;
                    }
                    catch (IOException)
                    {
                        retval = 5;
                    }

                    if (retval == 1)
                    {
                        data.ProfileDTO.InsertOnSubmit(new ProfileDTO()
                        {
                            Barcode = e.Barcode,
                            Project_Code = e.Project_Code,
                            Project_Stage = e.Project_Stage,
                            Project_Name = e.Project_Name,
                            Project_Subject = e.Project_Subject,
                            Diagram_Name = e.Diagram_Name,
                            Diagram_Code = e.Diagram_Code,
                            Diagram_Version = e.Diagram_Version,
                            Diagram_Scale = e.Diagram_Scale,
                            File_Name = e.File_Name,
                            Created_Date = DateTime.Now,
                            Plotter = e.Plotter
                        });

                        data.ProfileTemporaryDTO.DeleteOnSubmit(e);
                        data.SubmitChanges();
                    }
                }
            }

            return retval;
        }
    }

    public interface IAuditService
    {
        bool IsExists(AuditDTO row);
        bool Apply(AuditDTO row);
    }

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
    }

    public class AccountMembershipService : IMembershipService
    {


        public AccountMembershipService()
        {
        }

        public int MinPasswordLength
        {
            get
            {
                return 6;
            }
        }

        public bool ValidateUser(string loginID, string password)
        {
            if (String.IsNullOrEmpty(loginID)) throw new ArgumentException("值不能为 null 或为空。", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("值不能为 null 或为空。", "password");

            bool retval = false;

            using (DataClassesDataContext data = new DataClassesDataContext())
            {
                var items = data.UserDTO.Where(u => u.Login_ID == loginID && u.Password == password).ToList();
                retval = items.Count > 0;
            }

            return retval;
        }

        public bool ChangePassword(string loginID, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(loginID)) throw new ArgumentException("值不能为 null 或为空。", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("值不能为 null 或为空。", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("值不能为 null 或为空。", "newPassword");

            // 在某些出错情况下，基础 ChangePassword() 将引发异常，
            // 而不是返回 false。
            try
            {
                if (ValidateUser(loginID, oldPassword))
                {
                    using (DataClassesDataContext data = new DataClassesDataContext())
                    {
                        UserDTO currentUser = data.UserDTO.Where(u => u.Login_ID == loginID).Single();
                        currentUser.Password = newPassword;

                        data.SubmitChanges();
                    }

                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }
    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string loginID, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(loginID)) throw new ArgumentException("值不能为 null 或为空。", "userName");

            FormsAuthentication.SetAuthCookie(loginID, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }

    #endregion

    #region Validation
    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // 请参见 http://go.microsoft.com/fwlink/?LinkID=177550 以查看
            // 状态代码的完整列表。
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "用户名已存在。请另输入一个用户名。";

                case MembershipCreateStatus.DuplicateEmail:
                    return "已存在与该电子邮件地址对应的用户名。请另输入一个电子邮件地址。";

                case MembershipCreateStatus.InvalidPassword:
                    return "提供的密码无效。请输入有效的密码值。";

                case MembershipCreateStatus.InvalidEmail:
                    return "提供的电子邮件地址无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidAnswer:
                    return "提供的密码取回答案无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidQuestion:
                    return "提供的密码取回问题无效。请检查该值并重试。";

                case MembershipCreateStatus.InvalidUserName:
                    return "提供的用户名无效。请检查该值并重试。";

                case MembershipCreateStatus.ProviderError:
                    return "身份验证提供程序返回了错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";

                case MembershipCreateStatus.UserRejected:
                    return "已取消用户创建请求。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";

                default:
                    return "发生未知错误。请验证您的输入并重试。如果问题仍然存在，请与系统管理员联系。";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class PropertiesMustMatchAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' 和 '{1}' 不匹配。";
        private readonly object _typeId = new object();

        public PropertiesMustMatchAttribute(string originalProperty, string confirmProperty)
            : base(_defaultErrorMessage)
        {
            OriginalProperty = originalProperty;
            ConfirmProperty = confirmProperty;
        }

        public string ConfirmProperty { get; private set; }
        public string OriginalProperty { get; private set; }

        public override object TypeId
        {
            get
            {
                return _typeId;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                OriginalProperty, ConfirmProperty);
        }

        public override bool IsValid(object value)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
            object originalValue = properties.Find(OriginalProperty, true /* ignoreCase */).GetValue(value);
            object confirmValue = properties.Find(ConfirmProperty, true /* ignoreCase */).GetValue(value);
            return Object.Equals(originalValue, confirmValue);
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' 必须至少包含 {1} 个字符。";
        private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                name, _minCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }
    }
    #endregion

}

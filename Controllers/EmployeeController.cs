using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using log4net;
using MCS.Library.OGUPermission;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.ViewsModel.Employee;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 人员信息
    /// </summary>
    public class EmployeeController : ApiController
    {
        /// <summary>
        /// 接口实例
        /// </summary>
        Services.IUserTaskService _service;

        /// <summary>
        /// 通讯录缓存
        /// </summary>
        static Models.AddressBook.ContactsCollection contacts;


        /// <summary>
        /// 人员信息
        /// </summary>
        public EmployeeController(Services.IUserTaskService service)
        {
            _service = service;
        }

        #region 获取当前登录用户信息
        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetUserInfo()
        {
            var result = ControllerService.Run(() =>
            {
                var user = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, User.Identity.Name)[0];
                 
                // 发票数量
                var invoiceCount = Adapter.Invoice.InvoiceAdapter.Instance.GetList(user.ID);
                // 名片数量
                var businessCardCount = Adapter.BusinessCard.BusinessCardAdapter.Instance.GetCount(user.ID);
                // 出生日期
                var birthday = new ViewsModel.BaseViewIsValue<string>() { Value = null };
                // 入职日期
                var employedDate = new ViewsModel.BaseViewIsValue<string>() { Value = null };
                try
                {
                    var drEB = Services.EmployeeService.Instance.GetEmployedDateAndBirthday(user.ID);
                    if (drEB != null)
                    {
                        if (drEB["EmployedDate"] != DBNull.Value)
                        {
                            employedDate.Value = Convert.ToDateTime(drEB["EmployedDate"]).ToString("yyyy-MM-dd");
                        }
                        if (drEB["Birthday"] != DBNull.Value)
                        {
                            birthday.Value = Convert.ToDateTime(drEB["Birthday"]).ToString("yyyy-MM-dd");
                        }
                    }
                }
                catch
                {

                }
                //是否使用office365 邮箱
                bool O365 = false, isOpenEmail=false;
                DataTable office365 = UsersInfoExtendAdapter.Instance.GetUserOfficeEmail(user.LogOnName);
                if (office365.Rows.Count > 0)
                {
                    if (office365.Rows[0]["HomeMDB"] != null && office365.Rows[0]["HomeMDB"] != DBNull.Value)
                    {
                        O365 = false;
                    }
                    else
                    {
                        O365 = true;
                    }
                    if (office365.Rows[0]["Status"] != null && office365.Rows[0]["Status"] != DBNull.Value)
                    {
                        isOpenEmail = office365.Rows[0]["Status"].ToString() == "A";
                    }
                }

                var regex = new System.Text.RegularExpressions.Regex(@"\([^\)]*\)|（[^）]*）");

                var userInfo = new UserInfo()
                {
                    Photo = UserHeadPhotoService.GetUserHeadPhoto(user.ID),
                    Id = user.ID,
                    LogonName = user.LogOnName,
                    DisplayName = regex.Replace(user.DisplayName, ""),
                    FullPath = user.FullPath,
                    Position = user.Occupation,
                    Mp = user.Properties["MP"].ToString(),
                    OtherMp = user.Properties["OtherMP"].ToString(),
                    E_Mail = user.Properties["E_MAIL"].ToString(),
                    CompanyName = user.Properties["CompanyName"].ToString(),
                    DepartmentName = user.Properties["DepartmentName"].ToString(),
                    Birthday = birthday,
                    EmployedDate = employedDate,
                    IsEip = User.IsInRole(ConfigAppSetting.EIPACCESSER) || User.IsInRole(ConfigAppSetting.YBACCESSER),
                    InvoiceCount = invoiceCount,
                    O365 = O365,
                    IsOpenEmail= isOpenEmail,
                    BusinessCardCount = businessCardCount,
                };
                return userInfo;
            });
            return Ok(result);
        }
        /// <summary>
        /// 用户信息
        /// </summary>
        public class UserInfo
        {

            /// <summary>
            /// 头像
            /// </summary>
            public string Photo { get; set; }
            /// <summary>
            /// 用户编码
            /// </summary>
            public string Id { set; get; }
            /// <summary>
            /// 登录名
            /// </summary>
            public string LogonName { set; get; }
            /// <summary>
            /// 用户名称
            /// </summary>
            public string DisplayName { set; get; }
            /// <summary>
            /// 组织机构完整路径
            /// </summary>
            public string FullPath { set; get; }
            /// <summary>
            /// 职务
            /// </summary>
            public string Position { set; get; }
            /// <summary>
            /// 联系电话
            /// </summary>
            public string Mp { set; get; }
            /// <summary>
            /// 其他联系电话
            /// </summary>
            public string OtherMp { set; get; }
            /// <summary>
            /// 邮箱
            /// </summary>
            public string E_Mail { set; get; }
            /// <summary>
            /// 公司名称
            /// </summary>
            public string CompanyName { set; get; }
            /// <summary>
            /// 部门名称
            /// </summary>
            public string DepartmentName { set; get; }
            /// <summary>
            /// 出生日期
            /// </summary>
            public ViewsModel.BaseViewIsValue<string> Birthday { set; get; }
            /// <summary>
            /// 入职时间
            /// </summary>
            public ViewsModel.BaseViewIsValue<string> EmployedDate { set; get; }
            /// <summary>
            /// 是否具有EIP访问权限
            /// </summary>
            public bool IsEip { set; get; }
            /// <summary>
            /// 发票数量
            /// </summary>
            public int InvoiceCount { set; get; }
            /// <summary>
            /// 个人名片数量
            /// </summary>
            public int BusinessCardCount { set; get; }
            /// <summary>
            /// 是否已经操作过打卡提醒设置
            /// </summary>
            public bool IsUserOperationRecordPunchRemindSetting
            {
                get
                {
                    try
                    {
                        return Adapter.UserOperationRecord.UserOperationRecordAdapter.Instance.IsExist(ViewsModel.UserOperationRecord.UserOperationRecordModule.PunchRemindSetting.ToString(), Id);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            /// <summary>
            /// ProxyToken
            /// </summary>
            public string ProxyToken
            {
                get
                {
                    return ConfigurationManager.AppSettings["ProxyToken"];
                }
            }
            /// <summary>
            /// 是否是 office365 邮箱
            /// </summary>
            public bool O365 { get; set; }

             /// <summary>
             /// 邮箱是否可用 A 可用 D 不可用
             /// </summary>
            public bool IsOpenEmail { get; set; }

        }
        #endregion

        #region 获取发票及名片数量
        /// <summary>
        /// 获取发票及名片数量
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetProjectCount()
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;

                var invoiceCount = Adapter.Invoice.InvoiceAdapter.Instance.GetList(user.Id);
                var businessCardCount = 0;

                return new
                {
                    InvoiceCount = invoiceCount,
                    BusinessCardCount = businessCardCount
                };
            });
            return Ok(result);
        }
        #endregion

        #region 根据人员编码获取人员信息
        /// <summary>
        /// 根据人员编码获取人员信息
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetUserInfoByCodes(string userCodes)
        {
            var result = ControllerService.Run(() =>
            {
                var arrCode = userCodes.Split(',');
                var users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, arrCode);
                if (users.Count < 1)
                {
                    throw new Exception("人员编码错误，没有找到相关信息！");
                }
                var view = new List<EmployeeViewModel>();
                foreach (var user in users)
                {
                    view.Add(new EmployeeViewModel()
                    {
                        Code = user.ID,
                        EnName = user.LogOnName,
                        CnName = user.DisplayName,
                        Phone = user.Properties["MP"].ToString()
                    });
                }
                return view;
            });
            return Ok(result);
        }
        #endregion

        #region 根据流程编码获取待办人信息  
        /// <summary>
        /// 根据流程编码获取待办人信息
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetUserInfoByTask(string resourceId, string processId)
        {
            var result = ControllerService.Run(() =>
            {
                string sql = $"SELECT SEND_TO_USER FROM [WF].[USER_TASK] WHERE [RESOURCE_ID] ='{resourceId}' AND [PROCESS_ID] ='{processId}' AND [STATUS] = '1' AND([EXPIRE_TIME] IS NULL OR([EXPIRE_TIME] IS NOT NULL AND[EXPIRE_TIME] > GETDATE())); ";
                SqlDbHelper sqlhelp = new SqlDbHelper(ConfigurationManager.ConnectionStrings["MCS_WORKFLOW"].ConnectionString);
                DataTable dt = sqlhelp.ExecuteDataTable(sql);
                if (dt == null || dt.Rows.Count==0) {
                    throw new Exception("没有找到相关待办人信息！");
                }
                var userCodeList = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    if (row[0] != null) {
                        userCodeList.Add(row[0].ToString());
                    }
                }
                var users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, userCodeList.ToArray());
                if (users.Count < 1)
                {
                    throw new Exception("人员编码错误，没有找到相关信息！");
                }
                var view = new List<EmployeeViewModel>();
                foreach (var user in users)
                {
                    view.Add(new EmployeeViewModel()
                    {
                        Code = user.ID,
                        EnName = user.LogOnName,
                        CnName = user.DisplayName,
                        Phone = user.Properties["MP"].ToString()
                    });
                }
                return view;
            });
            return Ok(result);
        }
        #endregion

        #region 当前登陆人的信息 - PC
        /// <summary>
        /// 当前登陆人的信息 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult UserName()
        {
            OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, User.Identity.Name);

            UserYuanXin user = new UserYuanXin();
            user.Id = users[0].ID;
            user.DisplayName = users[0].DisplayName;
            return Ok(user);
        }
        /// <summary>
        /// 用户信息 ViewModel
        /// </summary>
        public class UserYuanXin
        {
            /// <summary>
            /// 用户编码
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// 用户名称
            /// </summary>
            public string DisplayName { get; set; }
        }
        #endregion

        #region 帐户管理 - 人员列表 - PC
        /// <summary>
        /// 帐户管理 - 人员列表 - PC
        /// </summary>
        [HttpPost]
        public IHttpActionResult GetEmployeeList(int pageSize, int pageIndex, GetEmployeeListSearchInfo searchInfo)
        {
            var result = ControllerService.Run(() =>
            {
                var _contacts = GetContacts();
                var _seagulls = new Models.AddressBook.SeagullUsersCollection();

                if (searchInfo.IsValid == 3)
                {
                    _seagulls = SeagullUsersAdapter.Instance.LoadUsers();
                }
                else if (searchInfo.IsValid == 0)
                {
                    // 未激活
                    _seagulls = Adapter.AddressBook.SeagullUsersAdapter.Instance.LoadUsers(false);
                }
                else
                {
                    if (searchInfo.Version == "全部")
                    {
                        // 已激活
                        _seagulls = Adapter.AddressBook.SeagullUsersAdapter.Instance.LoadUsers(true);
                    }
                    else
                    {
                        // 已激活 有版本
                        _seagulls = Adapter.AddressBook.SeagullUsersAdapter.Instance.LoadVersion(searchInfo.Version);
                    }

                }

                var fullPath = "";
                if (!string.IsNullOrWhiteSpace(searchInfo.DepartmentCode))
                {
                    fullPath = Adapter.AddressBook.ContactsAdapter.Instance.LoadByCode(searchInfo.DepartmentCode).FullPath;
                }

                var listFind = new List<Models.AddressBook.ContactsModel>();
                if (!string.IsNullOrWhiteSpace(searchInfo.DepartmentCode) && !string.IsNullOrWhiteSpace(searchInfo.Keyword))
                {
                    listFind = _contacts.FindAll(w => w.FullPath.StartsWith(fullPath) && (w.Logon_Name.Contains(searchInfo.Keyword) || w.DisplayName.Contains(searchInfo.Keyword) || w.Mail.Contains(searchInfo.Keyword) || w.MP.Contains(searchInfo.Keyword))).ToList();
                }
                else if (!string.IsNullOrWhiteSpace(searchInfo.DepartmentCode) && string.IsNullOrWhiteSpace(searchInfo.Keyword))
                {
                    listFind = _contacts.FindAll(w => w.FullPath.StartsWith(fullPath)).ToList();
                }
                else if (string.IsNullOrWhiteSpace(searchInfo.DepartmentCode) && !string.IsNullOrWhiteSpace(searchInfo.Keyword))
                {
                    listFind = _contacts.FindAll(w => w.Logon_Name.Contains(searchInfo.Keyword) || w.DisplayName.Contains(searchInfo.Keyword) || w.Mail.Contains(searchInfo.Keyword) || w.MP.Contains(searchInfo.Keyword)).ToList();
                }
                else
                {
                    listFind = _contacts.ToList();
                }
                var listJoin = listFind.Join(
                        _seagulls,
                        outer => outer.ObjectID,
                        inner => inner.UserId,
                        (i, o) => new EmployeeListViewModel()
                        {
                            Code = i.ObjectID,
                            CnName = i.DisplayName,
                            EnName = i.Logon_Name,
                            Email = i.Mail,
                            FullPath = i.FullPath,
                            IsValid = o.IsValid,
                            InviteCount = o.InviteCount,
                            Phone = i.MP,
                            AppVersion = o.AppVersion
                        });

                var count = listJoin.Count();
                if (searchInfo.Order)
                {
                    listJoin = listJoin.OrderBy(o => o.InviteCount);
                }
                else
                {
                    listJoin = listJoin.OrderByDescending(o => o.InviteCount);
                }
                pageIndex--;

                return new ViewsModel.BaseViewPage()
                {
                    DataCount = count,
                    PageCount = count % pageSize == 0 ? count / pageSize : count / pageSize + 1,
                    PageData = listJoin.Skip(pageSize * pageIndex).Take(pageSize)
                };
            });
            return Ok(result);
        }


        /// <summary>
        /// 搜索  为了使用缓存数据
        /// </summary>
        [HttpGet]
        public IHttpActionResult AddressBookQuery(string keyword)
        {
            var result = ControllerService.Run(() =>
            {
                if (string.IsNullOrEmpty(keyword)) throw new Exception("请输入关键字");
                var _contacts = GetContacts();
                var listFind = new List<ContactsModel>();
                listFind = _contacts.FindAll(w => w.Logon_Name.Contains(keyword) || w.DisplayName.Contains(keyword) || w.Mail.Contains(keyword) || w.MP.Contains(keyword)).ToList();
                return new
                {
                    QueryCondition = keyword,
                    QueryResul = listFind,
                    ResultShow = listFind.Count
                };
            });
            return Ok(result);
        }

        /// <summary>
        /// 检索信息
        /// </summary>
        public class GetEmployeeListSearchInfo
        {
            /// <summary>
            /// 是否激活
            /// </summary>
            public int IsValid { set; get; }
            /// <summary>
            /// 部门编码
            /// </summary>
            public string DepartmentCode { set; get; }
            /// <summary>
            /// 姓名和帐号关键字
            /// </summary>
            public string Keyword { set; get; }

            /// <summary>
            /// 版本
            /// </summary>
            public string Version { set; get; }
            /// <summary>
            /// 排序发送次数
            /// </summary>
            public bool Order { get; set; }

        }
        Models.AddressBook.ContactsCollection GetContacts()
        {
            if (contacts != null && contacts.Count > 0)
            {
                return contacts;
            }
            else
            {
               contacts = Adapter.AddressBook.ContactsAdapter.Instance.Load(w =>
                {
                    w.AppendItem("SchemaType", "Users");
                    w.AppendItem("FullPath", "机构人员\\远洋集团%", "LIKE");
                    w.AppendItem("IsDefault", true);
                }, o => o.AppendItem("InnerSort", MCS.Library.Data.Builder.FieldSortDirection.Ascending));
                return contacts;
            }
        }

        #endregion

        #region  通讯录数据每天同步的时候调用此接口去除缓存数据

        /// <summary>
        /// 清理缓存重新获取最新数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ClearAddressBookCache()
        {
            var result = ControllerService.Run(() =>
            {
                contacts = Adapter.AddressBook.ContactsAdapter.Instance.Load(w =>
                {
                    w.AppendItem("SchemaType", "Users");
                    w.AppendItem("FullPath", "机构人员\\远洋集团%", "LIKE");
                    w.AppendItem("IsDefault", true);
                }, o => o.AppendItem("InnerSort", MCS.Library.Data.Builder.FieldSortDirection.Ascending));
                return contacts.Count;
            });
            return Ok(result);
        }

        #endregion


        #region 发送邀请邮件
        /// <summary>
        /// 发送邀请邮件
        /// </summary>
        [HttpPost]
        public IHttpActionResult InviteEmail(string EmailCode, string UserCodes)
        {
            var result = ControllerService.Run(() =>
            {
                EmailTempleModel email = EmailTempleAdapter.Instance.Load(w => w.AppendItem("Code", EmailCode)).FirstOrDefault();
                var userCodeArr = UserCodes.Split(',');
                var fileStream = File.OpenRead(HttpRuntime.AppDomainAppPath + "/HtmlTemplate/employee_invite_email_v1.html");
                var streamReader = new StreamReader(fileStream);
                var template = streamReader.ReadToEnd();
                template = template.Replace("{{body}}", email.Emailbody);
                int count = 0;
                string emptyMail = "";
                foreach (var userCode in userCodeArr)
                {
                    if (!string.IsNullOrWhiteSpace(userCode))
                    {
                        var user =  ContactsAdapter.Instance.LoadUserByCode(userCode);
                        if (user != null && !string.IsNullOrWhiteSpace(user.Mail))
                        {
                            var mail = SeagullMailService.GetInstance();
                            mail.AddTo(new Dictionary<string, string>() { { user.Mail, user.DisplayName } });
                            mail.AddSubject(email.EmailTheme);
                            mail.AddBody(template, true);
                            mail.Send();
                            // 更新邀请次数
                             SeagullUsersAdapter.Instance.UpdateInviteCount(userCode);
                            var newInvited = new InvitedRecordModel();

                            //添加邀请记录
                            newInvited.Code = Guid.NewGuid().ToString();
                            newInvited.SendTime = DateTime.Now;
                            newInvited.SenderName = ((Seagull2Identity)User.Identity).DisplayName;
                            newInvited.SenderCode = ((Seagull2Identity)User.Identity).Id;
                            newInvited.SendContent = email.Emailtitle;
                            newInvited.RecipientCode = user.ObjectID;
                            newInvited.RecipientName = user.DisplayName;

                            InvitedRecordAdapter.Instance.Update(newInvited);
                        }
                        else
                        {
                            emptyMail = emptyMail + "  " + user.DisplayName;
                            count++;
                        }
                    }
                }
                ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "发送了通讯录-组织架构-邀请邮件");
                return emptyMail;
            });
            return Ok(result);
        }
        /// <summary>
        /// 大批量发送
        /// </summary>
        [HttpPost]
        public IHttpActionResult InviteEmail1(GetEmployeeListSearchInfo par)
        {
            string EmailCode = "", UserCodes = "";
            EmailCode = par.Keyword;
            UserCodes = par.Version;
            var result = ControllerService.Run(() =>
            {
                string emptyMail = "";
                if (string.IsNullOrEmpty(EmailCode) || string.IsNullOrEmpty(UserCodes))
                {
                    return emptyMail;
                }
                EmailTempleModel email = EmailTempleAdapter.Instance.Load(w => w.AppendItem("Code", EmailCode)).FirstOrDefault();

                //========




                var userCodeArr = UserCodes.Split(',');
                var fileStream = File.OpenRead(HttpRuntime.AppDomainAppPath + "/HtmlTemplate/employee_invite_email_v1.html");
                var streamReader = new StreamReader(fileStream);
                var template = streamReader.ReadToEnd();
                template = template.Replace("{{body}}", email.Emailbody);
                int count = 0;

                foreach (var userCode in userCodeArr)
                {
                    if (!string.IsNullOrWhiteSpace(userCode))
                    {
                        var user = Adapter.AddressBook.ContactsAdapter.Instance.LoadUserByCode(userCode);
                        if (user != null && !string.IsNullOrWhiteSpace(user.Mail))
                        {
                            var mail = SeagullMailService.GetInstance();
                            mail.AddTo(new Dictionary<string, string>() { { user.Mail, user.DisplayName } });
                            mail.AddSubject(email.EmailTheme);
                            mail.AddBody(template, true);
                            mail.Send();
                            // 更新邀请次数
                            Adapter.AddressBook.SeagullUsersAdapter.Instance.UpdateInviteCount(userCode);
                            var newInvited = new InvitedRecordModel();

                            //添加邀请记录
                            newInvited.Code = Guid.NewGuid().ToString();
                            newInvited.SendTime = DateTime.Now;
                            newInvited.SenderName = ((Seagull2Identity)User.Identity).DisplayName;
                            newInvited.SenderCode = ((Seagull2Identity)User.Identity).Id;
                            newInvited.SendContent = email.Emailtitle;
                            newInvited.RecipientCode = user.ObjectID;
                            newInvited.RecipientName = user.DisplayName;

                            InvitedRecordAdapter.Instance.Update(newInvited);
                        }
                        else
                        {
                            emptyMail = emptyMail + "  " + user.DisplayName;
                            count++;
                        }
                    }
                }
                return emptyMail;

            });
            return Ok(result);
        }
        #endregion

        #region 获取所有版本
        /// <summary>
        /// 获取所有版本
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetVersion()
        {
            var result = ControllerService.Run(() =>
            {
                return  SeagullUsersAdapter.Instance.GetVersion();
            });
            return Ok(result);
        }
        #endregion


    }
}
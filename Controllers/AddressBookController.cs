using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MCS.Library.OGUPermission;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.ViewsModel.AddressBook;
using Seagull2.YuanXin.AppApi.Adapter.ContactsLabel;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 通讯录
    /// </summary>
    public class AddressBookController : ApiController
    {
        #region 获取所有组织编码和名称列表
        /// <summary>
        /// 获取所有组织编码和名称列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetAllOrganizations()
        {
            var result = ControllerService.Run(() =>
            {
                return ContactsAdapter.Instance.GetAllOrganizations();
            });
            return Ok(result);
        }
        #endregion

        #region 获取通讯录第一级、第二级部门列表
        /// <summary>
        /// 获取通讯录第一级、第二级部门列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetDepartList()
        {
            var result = ControllerService.Run(() =>
            {
                var view = new List<object>();
                var list = ContactsAdapter.Instance.GetDepartList();
                var parents = list.Where(w => w.ParentID == "efb29cac-5321-495b-844b-ed239a844ada").ToList();
                parents.ForEach(parent =>
                {
                    var childrenView = new List<object>();
                    var children = list.Where(w => w.ParentID == parent.ObjectID).ToList();
                    children.ForEach(child =>
                    {
                        childrenView.Add(new { Index = child.ObjectID, Key = child.DisplayName, Value = new List<object>() });
                    });
                    view.Add(new { Index = parent.ObjectID, Key = parent.DisplayName, Value = childrenView });
                });
                return view;
            });
            return Ok(result);
        }
        #endregion

        #region 添加常用联系人
        /// <summary>
        /// 添加常用联系人
        /// </summary>
        [HttpPost]
        public IHttpActionResult AddContect(ContactModel model)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                var user = (Seagull2Identity)User.Identity;

                var contact = ContactAdapter.Instance.LoadByContactID(model.ContactID, user.Id);
                if (contact != null)
                {
                    throw new Exception("已经存在此联系人！");
                }
                model.ID = Guid.NewGuid().ToString();
                model.Creator = user.Id;
                model.CreateTime = DateTime.Now;
                model.ValidStatus = true;
                ContactAdapter.Instance.Update(model);
            });
            return Ok(result);
        }
        #endregion

        #region 删除常用联系人
        /// <summary>
        /// 删除常用联系人
        /// </summary>
        [HttpPost]
        public IHttpActionResult DelContectByID(ContactModel model)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                ContactAdapter.Instance.Delete(model.ContactID, user.Id);
            });
            return Ok(result);
        }
        #endregion

        #region 获取常用联系人列表
        /// <summary>
        /// 获取常用联系人列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetContactList()
        {
            var result = ControllerService.Run(() =>
            {
                var view = new List<UserInfoViewModel>();
                var userCodes = ContactAdapter.Instance.LoadByCreator(((Seagull2Identity)User.Identity).Id).Select(item => item.ContactID).ToList();
                if (userCodes.Count > 0)
                {
                    var users = ContactsAdapter.Instance.LoadListByUserCodeList(userCodes);
                    foreach (var user in users)
                    {
                        if (user.IsDefault == 1)
                        {
                            view.Add(new UserInfoViewModel()
                            {
                                ID = user.ObjectID,
                                DisplayName = user.DisplayName,
                                Name = user.Logon_Name,
                                ObjectType = user.SchemaType,
                                Email = user.Mail,
                                Phone = user.MP,
                                FullPath = user.FullPath
                            });
                        }
                    }
                }
                return view;
            });
            return Ok(result);
        }
        #endregion

        #region 人员详细信息
        /// <summary>
        /// 人员详细信息
        /// </summary>
        [HttpGet]
        public IHttpActionResult Info(string userId)
        {
            UserDetailModel result = new UserDetailModel();
            ContactsModel user =ContactsAdapter.Instance.LoadUserByCode(userId);
            UsersInfoExtend userExtend = UsersInfoExtendAdapter.Instance.GetUsersInfoExtendById(userId);
            result.DisplayName = user.DisplayName;
            result.ExtendEmail = user.Mail;
            result.FullPath = user.FullPath;
            result.ID = user.ObjectID;
            result.InnerEmail = user.Mail;
            result.LogonName = user.Logon_Name;
            result.Name = user.DisplayName;
            result.BIRTHDAY = userExtend.BIRTHDAY == DateTime.MinValue ? "" : userExtend.BIRTHDAY.ToString("yyyy-MM-dd");
            result.GENDER = userExtend.GENDER;
            result.MOBILE = userExtend.MOBILE;
            result.MOBILE2 = userExtend.MOBILE2;
            result.NATION = userExtend.NATION;
            result.OfficeTel = userExtend.OfficeTel;
            result.StartWorkTime = userExtend.StartWorkTime == DateTime.MinValue ? "" : userExtend.StartWorkTime.ToString("yyyy-MM-dd");
            ContactModel cm = new ContactModel();
            cm = ContactAdapter.Instance.LoadByContactID(user.ObjectID, ((Seagull2Identity)User.Identity).Id);
            if (cm == null)
            {
                result.IsContact = false;
            }
            else { result.IsContact = true; }
            return Ok(result);
        }

        /// <summary>
        /// 人员详细信息
        /// </summary>
        [HttpGet]
        public IHttpActionResult InfoPC(string userId, int pageIndex, int pageSize)
        {
            var resultData = ControllerService.Run(() =>
            {
                if (string.IsNullOrEmpty(userId)) throw new Exception("人员标识不能为空");
                var count = InvitedRecordAdapter.Instance.GetListByPage(userId);
                var table = InvitedRecordAdapter.Instance.GetListByPage(pageIndex, pageSize, userId);
                var lists = DataConvertHelper<InvitedRecordModel>.ConvertToList(table);

                UserDetailModel result = new UserDetailModel();
                ContactsCollection contacts = ContactsAdapter.Instance.Load(w =>
                { w.AppendItem("ObjectID", userId); });
                ContactsModel isDefault = contacts.Find(f => f.IsDefault == 1);
                var seagullUser = SeagullUsersAdapter.Instance.Load(p => p.AppendItem("UserId", userId)).FirstOrDefault();
                var userHeadUrl = UserHeadPhotoService.GetUserHeadPhoto(userId);
                if (isDefault != null && seagullUser != null)
                {
                    //人员信息
                    result.DisplayName = isDefault.DisplayName;
                    result.LogonName = isDefault.Logon_Name;
                    result.FullPath = isDefault.FullPath;
                    contacts.ForEach(f=> {
                        if (f.IsDefault == 0) {
                            result.FullPath = result.FullPath + "|" + f.FullPath;
                        }
                    });
                    result.MOBILE = isDefault.MP;
                    result.InnerEmail = isDefault.Mail;
                    // app 信息
                    result.ID = isDefault.ObjectID;
                    result.IsValid = seagullUser.IsValid;
                    result.InviteCount = seagullUser.InviteCount;
                    result.UserHeadUrl = userHeadUrl;
                }

                return new
                {
                    DataCount = count,
                    PageCount = count % pageSize == 0 ? count / pageSize : count / pageSize + 1,
                    Data = lists,
                    UserMessage = result
                };
            });
            return Ok(resultData);
        }



        #endregion

        #region 搜索
        /// <summary>
        /// 搜索
        /// </summary>
        [HttpGet]
        public IHttpActionResult Query(string userName, string pageIndex)
        {
            UserInfoQueryMode result = new UserInfoQueryMode();

            if (userName.IsEmptyOrNull())
            {
                result.QueryResult = new List<UsersInfoExtend>();
                result.ResultShow = 0;
                return Ok(result);
            }

            //判断userName是否为电话号码
            if (CommonService.IsHandset(userName))
            {
                userName = string.Format("+{0}{1}", "86", userName);
            }

            var collection = UsersInfoExtendAdapter.Instance.GetUsersInfoExtendCollectionByName(userName, Convert.ToInt32(pageIndex), 10);

            result.QueryCondition = userName;
            result.QueryResult = collection.ToList();
            result.ResultShow = result.QueryResult.Count;

            return Ok(result);
        }

        /// <summary>
        /// 搜索 2018-4-28 --lujt  
        /// </summary>
        [HttpGet]
        public IHttpActionResult Query(string searchContent, int pageIndex, int pageSize)
        {
            var result = ControllerService.Run(() =>
            {
                searchContent = searchContent.Replace(" ", "");
                UserInfoQueryMode model = new UserInfoQueryMode
                {
                    QueryCondition = searchContent
                };

                // 判断是否为电话号码
                if (CommonService.IsHandset(searchContent))
                { 
                    searchContent = string.Format("+{0}{1}", "86", searchContent);
                }
                // 处理拼音
                var regex = new System.Text.RegularExpressions.Regex(@"^([a-z]+\s)([a-z]+\s)*([a-z]+)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (regex.IsMatch(searchContent))
                {
                    var pinyin1 = "";
                    var pinyin2 = "";

                    string[] sheng = { "zh", "ch", "sh" };

                    var regexOne = new System.Text.RegularExpressions.Regex(@"[a-z]+\s?", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    var matches = regexOne.Matches(searchContent);
                    foreach (System.Text.RegularExpressions.Match match in matches)
                    {
                        var word = match.Value.Trim();
                        var isSheng = false;
                        for (var i = 0; i < sheng.Length; i++)
                        {
                            if (word.StartsWith(sheng[i]))
                            {
                                isSheng = true;
                            }
                        }
                        pinyin1 += word;
                        pinyin2 += (match.Index == 0 ? word : (isSheng ? word.Substring(0, 2) : word.Substring(0, 1)));
                    }
                    searchContent = "\"" + pinyin1 + "\" OR \"" + pinyin2 + "\"";
                }

                var collection = UsersInfoExtendAdapter.Instance.GetUsersInfoExtendCollectionByName(searchContent, pageIndex, pageSize);
                model.QueryResult = collection;
                model.ResultShow = collection.Count;
                return model;
            });
            return Ok(result);
        }
        #endregion

        #region 获取通讯录列表
        /// <summary>
        /// 获取通讯录列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult LoadChild(string parentId = "efb29cac-5321-495b-844b-ed239a844ada")
        {
            var result = ControllerService.Run(() =>
            {
                var view = ContactsAdapter.Instance.LoadChild(parentId).ToUserInfoViewModelList();
                return view;
            });
            return Ok(result);
        }
        #endregion

        /// <summary>
        /// 获取子集方法
        /// </summary>
        private List<UserInfoViewModel> GetChild(string parentId)
        {
            if (string.IsNullOrEmpty(parentId))
            {
                parentId = "efb29cac-5321-495b-844b-ed239a844ada";
            }
            System.Data.DataTable table = UsersInfoExtendAdapter.Instance.GetChildsBy(parentId);
            List<UserInfoViewModel> infos = UserInfoViewModel.fromTable(table);
            return infos;
        }

        #region 获取我的部门
        /// <summary>
        /// 获取我的部门
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetMyDepartment()
        {
            var result = ControllerService.Run<object>(() =>
            {
                var users = ContactsAdapter.Instance.LoadCollectionByObjectId(((Seagull2Identity)User.Identity).Id);
                if (users.Count > 1)
                {
                    // 当前用户存在于多个部门，返回部门列表
                    var departs = ContactsAdapter.Instance.LoadListByUserCodeList(users.Select(s => s.ParentID).ToList());
                    var list = new List<UserInfoViewModel>();
                    foreach (var depart in departs)
                    {
                        list.Add(new UserInfoViewModel()
                        {
                            ID = depart.ObjectID,
                            DisplayName = depart.DisplayName,
                            Name = depart.Logon_Name,
                            ObjectType = depart.SchemaType,
                            Email = string.Empty,
                            Phone = string.Empty,
                            FullPath = depart.FullPath
                        });
                    }
                    return list;
                }
                else
                {
                    // 当前用户只有一个部门，返回该部门下的所有成员
                    var user = users.SingleOrDefault();
                    if (user == null)
                    {
                        throw new Exception("暂无数据！");
                    }
                    var list = ContactsAdapter.Instance.LoadUsers(user.ParentID).ToUserInfoViewModelList();
                    return list;
                }
            });
            return Ok(result);
        }
        #endregion

        #region 加载所有组织
        /// <summary>
        /// 加载所有组织
        /// </summary>
        [HttpGet]
        public IHttpActionResult LoadOrganizations(string parentId = "")
        {
            if (string.IsNullOrEmpty(parentId))
            {
                parentId = "efb29cac-5321-495b-844b-ed239a844ada";
            }
            List<OguUserInfo> result = new List<OguUserInfo>();
            List<OguUserInfo> resultOgu = new List<OguUserInfo>();

            IOrganization parent = OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(SearchOUIDType.Guid, parentId).FirstOrDefault();

            if (parent != null)
            {
                var list = parent.Children;
                result.AddRange(from item in list
                                where item.ObjectType == SchemaType.Organizations
                                select new OguUserInfo
                                {
                                    DisplayName = item.DisplayName,
                                    ID = item.ID,
                                    Name = item.Name,
                                    ObjectType = item.ObjectType.ToString()
                                });

                foreach (var item in result)
                {
                    item.UserCount = 0;
                    IOrganization Children = OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(SearchOUIDType.Guid, item.ID).FirstOrDefault();
                    foreach (var childrenItem in Children.Children)
                    {
                        if (childrenItem.ObjectType == SchemaType.Users)
                        {
                            item.UserCount++;
                            item.IsHasUser = true;
                        }
                        else
                        {
                            item.IsHasUser = false;
                        }
                    }
                    resultOgu.Add(item);
                }
            }
            return Ok(resultOgu);
        }
        #endregion

        /// <summary>
        /// 根据userCode获取组织机构及部门人员信息
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetInfoByUserCode(string userCode)
        {
            var result = ControllerService.Run(() =>
            {
                var users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, userCode);
                if (users.Count < 1)
                {
                    throw new Exception("无法找到该用户");
                }

                OrganizationsInfo data = new OrganizationsInfo();
                var table = UsersInfoExtendAdapter.Instance.GetLoadSuperior(userCode);
                //上级所有的组织机构，截至远洋地产
                data.OrganList = DataConvertHelper<Organizations>.ConvertToList(table);
                //去除人员信息，只保留组织信息
                data.OrganList.Remove(data.OrganList[0]);
                //部门下的人员列表
                data.UserList = GetChild(data.OrganList[0].ObjectID);
                var fullPath = data.OrganList[0].DisPlayName;
                data.UserList.ForEach(m =>
                {
                    m.FullPath = fullPath;
                });
                return data;
            });
            return Ok(result);
        }

        #region 将组织机构、标签转换成人员
        /// <summary>
        /// 将组织机构、标签转换成人员
        /// </summary>
        [HttpPost]
        public IHttpActionResult ConvertToUser(List<ConvertToUserViewModel> post)
        {
            var result = ControllerService.Run(() =>
            {
                var view = new List<ConvertToUserResultViewModel>();
                post.ForEach(model =>
                {
                    switch (model.Type)
                    {
                        case ConvertToUserViewModel.DataType.Orgization:
                            {
                                var data = ContactsAdapter.Instance.LoadChildren(model.Code).Where(w => w.SchemaType == "Users").ToList();
                                data.ForEach(item =>
                                {

                                    view.Add(new ConvertToUserResultViewModel()
                                    {
                                        Code = item.ObjectID,
                                        Name = item.DisplayName,
                                        LogonName = item.Logon_Name,
                                        Email = item.Mail,
                                        DepartmentName = GetDepartmentName(item.FullPath),
                                    });
                                });
                                break;
                            }
                        case ConvertToUserViewModel.DataType.User:
                            {
                                var data = ContactsAdapter.Instance.LoadUserByCode(model.Code);
                                if (data != null)
                                {
                                    view.Add(new ConvertToUserResultViewModel()
                                    {
                                        Code = data.ObjectID,
                                        Name = data.DisplayName,
                                        LogonName = data.Logon_Name,
                                        Email = data.Mail,
                                        DepartmentName = GetDepartmentName(data.FullPath),
                                    });
                                }
                                break;
                            }
                        case ConvertToUserViewModel.DataType.Label:
                            {
                                var data = ContactsLabelPersonAdapter.Instance.GetPersonList(model.Code);
                                var userCodeList = data.Select(w => w.UserCode).ToList();
                                var dataCollection = ContactsAdapter.Instance.LoadListByUserCodeList(userCodeList);
                                dataCollection.ForEach(item =>
                                {
                                    view.Add(new ConvertToUserResultViewModel()
                                    {
                                        Code = item.ObjectID,
                                        Name = item.DisplayName,
                                        LogonName = item.Logon_Name,
                                        Email = item.Mail,
                                        DepartmentName = GetDepartmentName(item.FullPath),
                                    });
                                });
                                break;
                            }
                    }
                });
                return view.Where((x, i) => view.FindIndex(z => z.Code == x.Code) == i);
            });
            return Ok(result);
        }

        string GetDepartmentName(string fullPath)
        {
            try
            {
                var arr = fullPath.Split('\\');
                return arr[arr.Length - 2];
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion

        /// <summary>
        /// 通讯录搜索
        /// </summary>
        [HttpGet]
        public IHttpActionResult Search(string keyword)
        {
            var result = ControllerService.Run(() =>
            {
                var data = ContactsAdapter.Instance.SearchTopN(10, keyword).ToList();
                return data.Where((item, index) => data.FindIndex(z => z.ObjectID == item.ObjectID) == index);
            });
            return Ok(result);
        }

        /// <summary>
        /// 通讯录搜索
        /// </summary>
        [HttpGet]
        public IHttpActionResult Search(string keyword, int count)
        {
            var result = ControllerService.Run(() =>
            {
                var data = ContactsAdapter.Instance.SearchTopN(count, keyword).ToList();
                return data.Where((item, index) => data.FindIndex(z => z.ObjectID == item.ObjectID) == index);
            });
            return Ok(result);
        }










        /// <summary>
        /// 获取用户在IM上的ID
        /// </summary>
        [HttpPost]
        public IHttpActionResult GetUserID(string[] id)
        {
            Seagull2RelationModelCollection result = new Seagull2RelationModelCollection();
            result = Seagull2RelationAdapter.Instance.LoadById(id);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult AddEmailTemplet(EmailTempleModel email)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                var user = ((Seagull2Identity)User.Identity);
                email.Emailbody = HtmlHelper.ReplaceImgUrl(email.Emailbody);
                if (String.IsNullOrEmpty(email.Code))
                {
                    email.Code = Guid.NewGuid().ToString();
                    email.CreateTime = DateTime.Now;
                    email.Creator = user.Id;
                    email.Name = user.DisplayName;
                    email.ValidStatus = true;
                    EmailTempleAdapter.Instance.Update(email);
                }
                else
                {
                    var old = EmailTempleAdapter.Instance.Load(p => p.AppendItem("Code", email.Code)).FirstOrDefault();
                    old.Modifier = user.Id;
                    old.ModifyTime = DateTime.Now;
                    old.Emailbody = email.Emailbody;
                    old.EmailTheme = email.EmailTheme;
                    old.Emailtitle = email.Emailtitle;
                    EmailTempleAdapter.Instance.Update(old);
                }
                ControllerService.UploadLog(user.Id, "操作了工具-邮件模板管理-邮件模板");
            });
            return Ok(result);
        }


        #region 删除邮件模板
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult DeletEmailTemple(string Code)
        {
            var result = ControllerService.Run(() =>
            {
                //获取用户
                var user = (Seagull2Identity)User.Identity;
                //删除模板
                EmailTempleAdapter.Instance.Delete(m =>
                {
                    m.AppendItem("Code", Code);

                });
                ControllerService.UploadLog(user.Id, "删除了工具-邮件模板管理-邮件模板");
            });

            return Ok(result);
        }
        #endregion


        #region 获取模板列表
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetList(int pageIndex, int pageSize)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var count = EmailTempleAdapter.Instance.GetListByPage();
                //查询当前页数据
                var lists = EmailTempleAdapter.Instance.GetListByPage(pageIndex, pageSize);
                return new
                {
                    PageCount = count % pageSize == 0 ? count / pageSize : count / pageSize + 1,
                    Data = lists
                };
            });
            return Ok(result);
        }

        #endregion


    }
}
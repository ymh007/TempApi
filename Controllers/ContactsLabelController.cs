using MCS.Library.Data;
using MCS.Library.OGUPermission;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.ContactsLabel;
using Seagull2.YuanXin.AppApi.Models.ContactsLabel;
using Seagull2.YuanXin.AppApi.ViewsModel.ContactsLabel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 通讯录标签-Controller
    /// </summary>
    public class ContactsLabelController : ApiController
    {
        private readonly string sp = "|";
        private readonly char spl = '|';

        #region 新增/编辑标签
        /// <summary>
        /// 新增/编辑标签
        /// </summary>
        [HttpPost]
        public IHttpActionResult Save(ContactsLabelSaveViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                #region 参数校验
                if (model == null) { throw new Exception("参数不能为null"); }
                if (string.IsNullOrEmpty(model.Name)) { throw new Exception("标签名称不能为空"); }
                if (model.Persons.Count < 1) { throw new Exception("标签人员至少选择一个"); }
                var user = (Seagull2Identity)User.Identity;

                #endregion

                //获取人员信息
                var ids = new List<string>();
                model.Persons.ForEach(m =>
                {
                    ids.Add(m.UserCode);
                });
                ids = ids.Distinct().ToList();
                var users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, ids.ToArray());
                if (users.Count < 1) { throw new Exception("无有效的userCode"); }
                var userList = users.ToList().FindAll(m => m.IsSideline == false);
                //事务提交保存数据
                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    if (string.IsNullOrEmpty(model.Code))
                    {
                        var labelCollection = ContactsLabelAdapter.Instance.Load(m => m.AppendItem("Creator", user.Id));
                        labelCollection.ForEach(m =>
                        {
                            if (model.Name == m.Name)
                            {
                                throw new Exception("标签名已存在");
                            }
                        });
                        //保存标签信息
                        var labelInfo = new ContactsLabelModel
                        {
                            Code = Guid.NewGuid().ToString(),
                            Name = model.Name,
                            Creator = user.Id,
                            CreateTime = DateTime.Now,
                            ValidStatus = true
                        };
                        ContactsLabelAdapter.Instance.Update(labelInfo);
                        //保存标签人员
                        var coll = new ContactsLabelPersonCollection();
                        userList.ForEach(m =>
                        {
                            var item = new ContactsLabelPersonModel
                            {
                                Code = Guid.NewGuid().ToString(),
                                LabelCode = labelInfo.Code,
                                UserCode = m.ID,
                                UserName = m.DisplayName,
                                FullPath = m.FullPath,
                                Creator = user.Id,
                                CreateTime = DateTime.Now
                            };
                            coll.Add(item);
                        });
                        ContactsLabelPersonAdapter.Instance.ContactsLabelPersonInsert(coll);
                    }
                    else
                    {
                        var labelColl = ContactsLabelAdapter.Instance.Load(m => m.AppendItem("Code", model.Code).AppendItem("ValidStatus", true));
                        if (labelColl.Count < 1) { throw new Exception("无效的code"); }
                        //编辑标签信息
                        ContactsLabelAdapter.Instance.Update(new ContactsLabelModel
                        {
                            Code = model.Code,
                            Name = model.Name,
                            Creator = user.Id,
                            CreateTime = DateTime.Now,
                            ValidStatus = true
                        });
                        //删除之前的标签人员
                        ContactsLabelPersonAdapter.Instance.Delete(m => m.AppendItem("LabelCode", model.Code));
                        //保存标签人员
                        var coll = new ContactsLabelPersonCollection();
                        userList.ForEach(m =>
                        {
                            var item = new ContactsLabelPersonModel
                            {
                                Code = Guid.NewGuid().ToString(),
                                LabelCode = model.Code,
                                UserCode = m.ID,
                                UserName = m.DisplayName,
                                FullPath = m.FullPath,
                                Creator = user.Id,
                                CreateTime = DateTime.Now
                            };
                            coll.Add(item);
                        });
                        ContactsLabelPersonAdapter.Instance.ContactsLabelPersonInsert(coll);
                    }
                    //事务提交
                    scope.Complete();
                }
            });
            return Ok(result);
        }
        #endregion

        #region 删除标签
        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult DeleteByCode(string code)
        {
            var result = ControllerService.Run(() =>
            {
                if (string.IsNullOrEmpty(code)) { throw new Exception("参数不能为空"); }
                var labelColl = ContactsLabelAdapter.Instance.Load(m => m.AppendItem("Code", code));
                if (labelColl.Count < 1) { throw new Exception("无效的code"); }
                ContactsLabelAdapter.Instance.Delete(m => m.AppendItem("Code", code));
                ContactsLabelPersonAdapter.Instance.Delete(m => m.AppendItem("LabelCode", code));
            });
            return Ok(result);
        }
        #endregion

        #region 获取标签列表
        /// <summary>
        /// 获取标签列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetList()
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;

                var labelColl = ContactsLabelAdapter.Instance.GetList(user.Id);

                var usersColl = ContactsLabelPersonAdapter.Instance.GetListByCreator(user.Id);

                var view = new List<ContactsLabelListViewModel>();

                labelColl.ForEach(m =>
                {
                    List<ContactsLabelListPersonViewModel> personList = new List<ContactsLabelListPersonViewModel>();
                    usersColl.FindAll(o => o.LabelCode == m.Code).ToList().ForEach(o =>
                    {
                        var child = new ContactsLabelListPersonViewModel
                        {
                            ID = o.UserCode,
                            DisplayName = o.UserName,
                            FullPath = o.FullPath
                        };
                        personList.Add(child);
                    });
                    var item = new ContactsLabelListViewModel
                    {
                        Code = m.Code,
                        Name = m.Name,
                        List = personList
                    };
                    view.Add(item);
                });
                return view;
            });
            return Ok(result);
        }
        #endregion

        #region 获取用户的标签
        /// <summary>
        /// 获取用户标签
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetDetailsByCode(string userCode)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var collAll = ContactsLabelAdapter.Instance.Load(m => m.AppendItem("Creator", user.Id).AppendItem("LabelType", 0)).OrderByDescending(o => o.CreateTime).ToList();
                var isChoice = ContactsLabelAdapter.Instance.GetLabelInfoByUserCode(user.Id, userCode);
                LabelDetails model = new LabelDetails();
                List<LabelInfo> noChoice = new List<LabelInfo>();
                model.IsChoice = isChoice;
                collAll.ForEach(m =>
                {
                    var count = isChoice.FindAll(o => o.Code == m.Code).Count;
                    if (count < 1)
                    {
                        var item = new LabelInfo
                        {
                            Code = m.Code,
                            Name = m.Name
                        };
                        noChoice.Add(item);
                    }
                });
                model.NoChoice = noChoice;
                return model;
            });
            return Ok(result);
        }
        #endregion

        #region 设置用户标签
        /// <summary>
        /// 设置用户标签
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SeetingUserLabelInfo(SeetingLabelInfo model)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                //获取人员信息
                var users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, model.UserCode);
                if (users.Count < 1) { throw new Exception("无有效的userCode"); }
                var userInfo = users.ToList().Find(m => m.IsSideline == false);
                ContactsLabelPersonAdapter.Instance.Delete(m => m.AppendItem("Creator", user.Id).AppendItem("UserCode", userInfo.ID));
                if (model.Choice.Count > 0)
                {
                    model.Choice.ForEach(m =>
                    {
                        ContactsLabelPersonAdapter.Instance.Update(new ContactsLabelPersonModel
                        {
                            Code = Guid.NewGuid().ToString(),
                            LabelCode = m.Code,
                            UserName = userInfo.DisplayName,
                            UserCode = model.UserCode,
                            FullPath = userInfo.FullPath,
                            CreateTime = DateTime.Now,
                            Creator = user.Id
                        });
                    });
                }
            });
            return Ok(result);
        }
        #endregion


        #region 外部联系人操作

        //获取标签列表
        [HttpGet]
        public IHttpActionResult GetLabels()
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var lists = ContactsLabelAdapter.Instance.Load(s => s.AppendItem("Creator", user.Id).AppendItem("LabelType", 1));
                return lists;
            });
            return Ok(result);
        }

        // 添加标签
        [HttpPost]
        public IHttpActionResult AddLabel(query q)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                if (string.IsNullOrEmpty(q.label)) throw new Exception("标签不能为空！");
                ContactsLabelModel mode = new ContactsLabelModel()
                {
                    Code = Guid.NewGuid().ToString(),
                    Name = q.label,
                    CreateTime = DateTime.Now,
                    Creator = user.Id,
                    LabelType = 1,
                    ValidStatus = true
                };
                ContactsLabelAdapter.Instance.Update(mode);
                return mode;
            });
            return Ok(result);
        }

        //删除标签
        [HttpPost]
        public IHttpActionResult DeleteLabel(query q)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                ContactsLabelAdapter.Instance.Delete(w => w.AppendItem("Code", q.id));
                return ExternalContactAdapter.Instance.UpdateLabels(q.name.Trim(), user.Id);
            });
            return Ok(result);
        }


        //获取外部人员详情
        [HttpGet]
        public IHttpActionResult GetExtrContactDetail(string userID)
        {
            var result = ControllerService.Run(() =>
            {
                var model = ExternalContactAdapter.Instance.Load(w => w.AppendItem("Code", userID)).FirstOrDefault();
                if (model == null)
                {
                    throw new Exception("人员不存在了！");
                }
                if (!string.IsNullOrEmpty(model.Phone)) { model.Phones = model.Phone.Split(spl); }
                if (!string.IsNullOrEmpty(model.Email)) { model.Emails = model.Email.Split(spl); }
                model.Photo = string.IsNullOrEmpty(model.Photo) ? "" : FileService.DownloadFile(model.Photo);
                model.Card = string.IsNullOrEmpty(model.Card) ? "" : FileService.DownloadFile(model.Card);
                return model;
            });
            return Ok(result);
        }


        //获取外部人员 集合 v1
        [HttpPost]
        public IHttpActionResult GetListDatav1(query q)
        {
            var result = ControllerService.Run(() =>
            {
                List<string> labels = string.IsNullOrEmpty(q.label) ? new List<string>() : q.label.Split(',').ToList();
                var user = (Seagull2Identity)User.Identity;
                var lists = ExternalContactAdapter.Instance.GetCurrentUserList(user.Id, q.name, labels, q.index, q.psize);
                lists.ForEach(f =>
                {
                    if (!string.IsNullOrEmpty(f.Phone))
                    {
                        f.Phones = f.Phone.Split(spl);
                    }
                    if (!string.IsNullOrEmpty(f.Email))
                    {
                        f.Emails = f.Email.Split(spl);
                    }
                    f.PinYin = NPinyin.Pinyin.GetPinyin(f.Name.ToCharArray()[0]).FirstOrDefault().ToString().ToUpper();
                });
                List<object> groupData = new List<object>();
                lists.GroupBy(g => g.PinYin).OrderBy(o => o.Key).ToList().ForEach(p =>
                {
                    var o = new { PinYin = p.Key, data = new List<ExternalContact>() };
                    p.ToList().ForEach(x =>
                    {
                        o.data.Add(x);
                    });
                    groupData.Add(o);
                });
                return groupData;
            });
            return Ok(result);
        }

        //获取外部人员 集合
        [HttpPost]
        public IHttpActionResult GetListData(query q)
        {
            var result = ControllerService.Run(() =>
            {
                List<string> labels = string.IsNullOrEmpty(q.label) ? new List<string>() : q.label.Split(',').ToList();
                var user = (Seagull2Identity)User.Identity;
                var lists = ExternalContactAdapter.Instance.GetCurrentUserList(user.Id, q.name, labels, q.index, q.psize);
                lists.ForEach(f =>
                {
                    if (!string.IsNullOrEmpty(f.Phone))
                    {
                        f.Phones = f.Phone.Split(spl);
                    }
                    if (!string.IsNullOrEmpty(f.Email))
                    {
                        f.Emails = f.Email.Split(spl);
                    }
                });
                return lists;
            });
            return Ok(result);
        }


        //删除外部人员
        [HttpGet]
        public IHttpActionResult DelExtrContact(string code)
        {
            var result = ControllerService.Run(() =>
            {
                ExternalContactAdapter.Instance.Delete(w => w.AppendItem("Code", code));
            });
            return Ok(result);
        }
        //添加外部人员
        [HttpPost]
        public IHttpActionResult AddExContact(List<ExternalContact> data)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                ExternalContactCollection dbAllData = ExternalContactAdapter.Instance.GetCurrentUserList(user.Id);
                List<ExternalContact> addData = new List<ExternalContact>();
                List<ExternalContact> updateData = new List<ExternalContact>();
                List<string> tPhone = new List<string>();
                List<string> tEmail = new List<string>();
                data.ForEach(f =>
                {
                    tPhone = f.Phones.ToList();
                    tEmail = f.Emails.ToList();
                    if (!string.IsNullOrEmpty(f.Code))
                    {
                        f.Phone = string.Join(sp, tPhone.Distinct());
                        f.Email = string.Join(sp, tEmail.Distinct());
                        f.Creator = user.Id;
                        updateData.Add(f);
                    }
                    else
                    {
                        f.Code = Guid.NewGuid().ToString();
                        f.Creator = user.Id;
                        f.Phone = string.Join(sp, tPhone.Distinct());
                        f.Email = string.Join(sp, tEmail.Distinct());
                        addData.Add(f);
                    }
                });
                ExternalContactAdapter.Instance.AddContacts(addData);
                updateData.ForEach(u =>
                {
                    ExternalContactAdapter.Instance.Update(u);
                });
            });
            return Ok(result);
        }

        public class query
        {
            public int psize { get; set; }
            public int index { get; set; }
            public string name { get; set; }
            public string label { get; set; }
            public string id { get; set; }
        }

        #endregion





    }
}

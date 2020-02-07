using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Seagull2.YuanXin.AppApi.ViewsModel.ActivityNew;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.Adapter.ActivityNew;
using Seagull2.YuanXin.AppApi.Models.ActivityNew;
using Seagull2.Core.Models;
using System.Threading.Tasks;
using log4net;
using System.Reflection;
using System.Data;
using Seagull2.YuanXin.AppApi.Domain.YuanXinOfficeCommon;
using static Seagull2.YuanXin.AppApi.ViewsModel.YuanXinOfficeCommon.YuanXinOfficeCommon;
using static Seagull2.YuanXin.AppApi.Controllers.EmployeeController;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Models.Message;
using Seagull2.YuanXin.AppApi.Adapter.Message;
using Seagull2.YuanXin.AppApi.Common;

namespace Seagull2.YuanXin.AppApi.Controllers.ActivityNew
{
    /// <summary>
    /// 活动 API
    /// </summary>
    public class ActivityInfoController : ApiController
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 发布编辑活动
        /// <summary>
        /// 发布编辑活动
        /// </summary>
        [HttpPost]
        public IHttpActionResult Save(ActivitySaveViewModel post)
        {
            try
            {
                #region 基本信息验证
                if (post.StartTime < DateTime.Now)
                {
                    return Ok(new BaseView
                    {
                        State = false,
                        Message = "活动开始时间不能小于当前时间！"
                    });
                }
                if (post.StartTime >= post.EndTime)
                {
                    return Ok(new BaseView
                    {
                        State = false,
                        Message = "活动结束时间必须大于开始时间！"
                    });
                }
                if (post.ApplyEndTime > post.EndTime)
                {
                    return Ok(new BaseView
                    {
                        State = false,
                        Message = "活动报名截止时间不能大于活动结束时间！"
                    });
                }
                #endregion

                var user = (Seagull2Identity)User.Identity;

                #region 保存活动基本信息
                var info = new ActivityInfoModel();
                if (string.IsNullOrWhiteSpace(post.Code))
                {
                    info.Code = Guid.NewGuid().ToString();
                    info.Creator = user.Id;
                    info.CreateTime = DateTime.Now;
                }
                else
                {
                    info = ActivityInfoAdapter.Instance.Load(w => w.AppendItem("Code", post.Code)).SingleOrDefault();
                    if (info == null)
                    {
                        return Ok(new BaseView
                        {
                            State = false,
                            Message = "活动不存在！"
                        });
                    }
                    info.Modifier = user.Id;
                    info.ModifyTime = DateTime.Now;
                }
                info.Title = post.Title;
                info.Cover = post.Cover;
                info.StartTime = post.StartTime;
                info.EndTime = post.EndTime;
                info.IsOffline = post.IsOffline;
                info.OfflineAddress = post.OfflineAddress;
                info.Description = post.Description;
                info.Contact = post.Contact;
                info.IsApplyAll = post.IsApplyAll;
                info.ApplyEndTime = post.ApplyEndTime;
                info.ValidStatus = true;
                ActivityInfoAdapter.Instance.Update(info);
                #endregion

                #region 保存活动分类
                Task.Run(() =>
                {
                    try
                    {
                        ActivityCategoryRecordAdapter.Instance.Delete(w => w.AppendItem("ActivityCode", info.Code));
                        post.CategoryCodeList.ForEach(categoryCode =>
                        {
                            ActivityCategoryRecordAdapter.Instance.Update(new ActivityCategoryRecordModel()
                            {
                                Code = Guid.NewGuid().ToString(),
                                ActivityCode = info.Code,
                                ActivityCategoryCode = categoryCode,
                                Creator = user.Id,
                                CreateTime = DateTime.Now,
                                ValidStatus = true
                            });
                        });
                    }
                    catch (Exception e)
                    {
                        log.Error("保存活动分类异常：" + e.Message);
                    }
                });
                #endregion

                #region 保存活动报名范围原始设置
                Task.Run(() =>
                {
                    try
                    {
                        ActivityApplySetOrigAdapter.Instance.Delete(w => w.AppendItem("ActivityCode", info.Code));
                        if (!post.IsApplyAll)
                        {
                            post.ApplySetList.ForEach(item =>
                            {
                                ActivityApplySetOrigAdapter.Instance.Update(new Models.ActivityNew.ActivityApplySetOrigModel()
                                {
                                    Code = Guid.NewGuid().ToString(),
                                    ActivityCode = info.Code,
                                    SelectCode = item.Code,
                                    SelectType = item.Type.ToString(),
                                    Creator = user.Id,
                                    CreateTime = DateTime.Now,
                                    ValidStatus = true
                                });
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("保存活动报名范围设置异常：" + e.Message);
                    }
                });
                #endregion

                #region 保存活动报名范围设置
                Task.Run(() =>
                {
                    try
                    {
                        ActivityApplySetAdapter.Instance.Delete(w => w.AppendItem("ActivityCode", info.Code));
                        if (!post.IsApplyAll)
                        {
                            _UserList = new List<string>();
                            post.ApplySetList.ForEach(item =>
                            {
                                SaveApplySet(item, info.Code, user.Id);
                            });
                            ActivityApplySetAdapter.Instance.Insert(_UserList, info.Code, user.Id);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("保存活动报名范围设置异常：" + e.Message);
                    }
                });
                #endregion

                #region 消息提醒和推送
                if (post.IsApplyAll)
                {
                    // 全部人员可以报名

                    var contacts = new Models.AddressBook.ContactsCollection();
                    contacts = Adapter.AddressBook.ContactsAdapter.Instance.Load(w =>
                    {
                        w.AppendItem("SchemaType", "Users");
                        w.AppendItem("FullPath", "机构人员\\远洋集团%", "LIKE");
                        w.AppendItem("IsDefault", true);
                    });
                    #region 系统提醒
                    Task.Run(() =>
                    {
                        MessageCollection messColl = new MessageCollection();
                        List<string> idList = new List<string>();
                        foreach (var items in contacts)
                        {
                            MessageModel mess = new MessageModel()
                            {
                                Code = Guid.NewGuid().ToString(),
                                MeetingCode = info.Code,
                                MessageContent = $"收到新的活动{info.Title}，点击查看详情",
                                MessageStatusCode = EnumMessageStatus.New,
                                MessageTypeCode = "2",
                                MessageTitleCode = EnumMessageTitle.System,
                                ModuleType = EnumMessageModuleType.Activity.ToString(),
                                Creator = user.Id,
                                CreatorName = user.DisplayName,
                                ReceivePersonCode = items.ObjectID,
                                ReceivePersonName = items.DisplayName,
                                ReceivePersonMeetingTypeCode = "",
                                OverdueTime = DateTime.Now.AddDays(7),
                                ValidStatus = true,
                                CreateTime = DateTime.Now
                            };
                            messColl.Add(mess);
                        }
                        MessageAdapter.Instance.BatchInsert(messColl);
                    });
                    #endregion

                    #region 消息推送提醒
                    Task.Run(() =>
                    {
                        var activityPerson = new PushService.Model()
                        {
                            BusinessDesc = "移动办公后台发布编辑活动",
                            Title = "活动提醒",
                            Content = $"收到新的活动{info.Title},点击查看详情",
                            SendType = PushService.SendType.All,
                            Extras = new PushService.ModelExtras()
                            {
                                action = "Activity",
                                bags = info.Code
                            }
                        };
                        //消息推送（创建人）
                        PushService.Push(activityPerson, out string pushResult);
                    });
                    #endregion
                }
                else
                {
                    // 不是全部人员可以报名

                    #region 系统提醒
                    Task.Run(() =>
                    {
                        MessageCollection messColl = new MessageCollection();
                        List<string> idList = new List<string>();
                        DataFilter filter = new DataFilter();
                        var messageList = filter.FilterValidUsers(post.ApplySetList.Select(m => m.Code).ToList());
                        foreach (var items in messageList)
                        {
                            MessageModel mess = new MessageModel()
                            {
                                Code = Guid.NewGuid().ToString(),
                                MeetingCode = info.Code,
                                MessageContent = $"收到新的活动{info.Title},点击查看详情",
                                MessageStatusCode = EnumMessageStatus.New,
                                MessageTypeCode = "2",
                                MessageTitleCode = EnumMessageTitle.System,
                                ModuleType = EnumMessageModuleType.Activity.ToString(),
                                Creator = user.Id,
                                CreatorName = user.DisplayName,
                                ReceivePersonCode = items.ID,
                                ReceivePersonName = items.DisplayName,
                                ReceivePersonMeetingTypeCode = "",
                                OverdueTime = DateTime.Now.AddDays(7),
                                ValidStatus = true,
                                CreateTime = DateTime.Now
                            };
                            messColl.Add(mess);
                        }
                        MessageAdapter.Instance.BatchInsert(messColl);
                    });
                    #endregion

                    #region 消息推送提醒
                    Task.Run(() =>
                    {
                        var activityPerson = new PushService.Model()
                        {
                            BusinessDesc = "发布编辑活动",
                            Title = "活动提醒",
                            Content = $"收到新的活动{info.Title},点击查看详情",
                            SendType = PushService.SendType.All,
                            Ids = string.Join(",", post.ApplySetList.Select(m => m.Code)),
                            Extras = new PushService.ModelExtras()
                            {
                                action = "Activity",
                                bags = info.Code
                            }
                        };
                        //消息推送（创建人）
                        PushService.Push(activityPerson, out string pushResult);
                    });
                    #endregion
                }
                #endregion

                return Ok(new BaseView
                {
                    State = true,
                    Message = "success."
                });
            }
            catch (Exception e)
            {
                log.Error("保存活动异常：" + e.Message);
                return Ok(new BaseView
                {
                    State = false,
                    Message = "保存活动异常：" + e.Message
                });
            }
        }
        List<string> _UserList;
        /// <summary>
        /// 遍历存储范围信息
        /// </summary>
        void SaveApplySet(ActivityApplySetOrigSaveViewModel model, string activityCode, string creator)
        {
            if (model.Type == ViewsModel.ActivityNew.Type.Users)
            {
                if (!_UserList.Contains(model.Code))
                {
                    _UserList.Add(model.Code);
                }
            }
            if (model.Type == ViewsModel.ActivityNew.Type.Organizations)
            {
                var list = new List<ActivityApplySetOrigSaveViewModel>();
                var dt = Adapter.AddressBook.UsersInfoExtendAdapter.Instance.GetChildsBy(model.Code);
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    try
                    {
                        list.Add(new ActivityApplySetOrigSaveViewModel()
                        {
                            Code = dr["ID"].ToString(),
                            Type = (ViewsModel.ActivityNew.Type)System.Enum.Parse(typeof(ViewsModel.ActivityNew.Type), dr["ObjectType"].ToString())
                        });
                    }
                    catch (Exception e)
                    {
                        log.ErrorFormat("将DataRow转换为{0}对象失败，详情：{1}", typeof(ActivityApplySetOrigSaveViewModel).Name, e.Message);
                    }
                }
                list.ForEach(item =>
                {
                    SaveApplySet(item, activityCode, creator);
                });
            }
        }
        #endregion

        #region 全部活动
        /// <summary>
        /// 全部活动
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetListForAll(int pageSize, int pageIndex)
        {
            var userCode = ((Seagull2Identity)User.Identity).Id;

            var list = new List<AppActivityListForAllViewModel>();

            var dataCount = ActivityInfoAdapter.Instance.GetListForAll(userCode);
            var dataList = ActivityInfoAdapter.Instance.GetListForAll(pageSize, pageIndex, userCode);

            foreach (DataRow dr in dataList.Rows)
            {
                list.Add(new AppActivityListForAllViewModel()
                {
                    Code = dr["Code"].ToString(),
                    Title = dr["Title"].ToString(),
                    Cover = dr["Cover"].ToString(),
                    StartTime = Convert.ToDateTime(dr["StartTime"]),
                    EndTime = Convert.ToDateTime(dr["EndTime"]),
                    IsOffline = Convert.ToBoolean(dr["IsOffline"]),
                    OfflineAddress = dr["OfflineAddress"].ToString(),
                    FollowTime = dr["FollowTime"].ToString(),
                    CreateTime = Convert.ToDateTime(dr["CreateTime"])
                });
            }

            var data = new BaseViewPage()
            {
                DataCount = dataCount,
                PageCount = dataCount % pageSize == 0 ? dataCount / pageSize : dataCount / pageSize + 1,
                PageData = list
            };

            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
                Data = data
            });
        }
        #endregion

        #region 我参与的活动
        /// <summary>
        /// 我参与的活动
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetListForApply(int pageSize, int pageIndex)
        {
            var userCode = ((Seagull2Identity)User.Identity).Id;

            var list = new List<AppActivityListForApplyViewModel>();

            var dataCount = ActivityInfoAdapter.Instance.GetListForApply(userCode);
            var dataList = ActivityInfoAdapter.Instance.GetListForApply(pageSize, pageIndex, userCode);

            foreach (DataRow dr in dataList.Rows)
            {
                list.Add(new AppActivityListForApplyViewModel()
                {
                    Code = dr["Code"].ToString(),
                    Title = dr["Title"].ToString(),
                    Cover = dr["Cover"].ToString(),
                    StartTime = Convert.ToDateTime(dr["StartTime"]),
                    EndTime = Convert.ToDateTime(dr["EndTime"]),
                    IsOffline = Convert.ToBoolean(dr["IsOffline"]),
                    OfflineAddress = dr["OfflineAddress"].ToString(),
                    CreatorCode = dr["Creator"].ToString(),
                    Contact = dr["Contact"].ToString(),
                    CreateTime = Convert.ToDateTime(dr["CreateTime"])
                });
            }

            var data = new BaseViewPage()
            {
                DataCount = dataCount,
                PageCount = dataCount % pageSize == 0 ? dataCount / pageSize : dataCount / pageSize + 1,
                PageData = list
            };

            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
                Data = data
            });
        }
        #endregion

        #region 我发布的活动
        /// <summary>
        /// 我发布的活动
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetListForMy(int pageSize, int pageIndex)
        {
            var userCode = ((Seagull2Identity)User.Identity).Id;

            var list = new List<AppActivityListForMyViewModel>();

            var dataCount = ActivityInfoAdapter.Instance.GetListForMy(userCode);
            var dataList = ActivityInfoAdapter.Instance.GetListForMy(pageSize, pageIndex, userCode);

            foreach (DataRow dr in dataList.Rows)
            {
                list.Add(new AppActivityListForMyViewModel()
                {
                    Code = dr["Code"].ToString(),
                    Title = dr["Title"].ToString(),
                    Cover = dr["Cover"].ToString(),
                    StartTime = Convert.ToDateTime(dr["StartTime"]),
                    EndTime = Convert.ToDateTime(dr["EndTime"]),
                    IsOffline = Convert.ToBoolean(dr["IsOffline"]),
                    OfflineAddress = dr["OfflineAddress"].ToString(),
                    CreateTime = Convert.ToDateTime(dr["CreateTime"])
                });
            }

            var data = new BaseViewPage()
            {
                DataCount = dataCount,
                PageCount = dataCount % pageSize == 0 ? dataCount / pageSize : dataCount / pageSize + 1,
                PageData = list
            };

            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
                Data = data
            });
        }
        #endregion

        #region 获取活动详情
        /// <summary>
        /// 获取活动详情
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetDetail(string activityCode)
        {
            var user = (Seagull2Identity)User.Identity;

            var model = ActivityInfoAdapter.Instance.Load(w => w.AppendItem("Code", activityCode)).SingleOrDefault();
            if (model == null)
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = "没有找到该活动！"
                });
            }

            var data = new AppActivityInfoViewModel(user.Id)
            {
                Code = model.Code,
                Title = model.Title,
                Cover = model.Cover,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                IsOffline = model.IsOffline,
                OfflineAddress = model.OfflineAddress,
                Description = model.Description,
                CreatorCode = model.Creator,
                Contact = model.Contact,
                CreateTime = model.CreateTime,
                ApplyEndTime = model.ApplyEndTime
            };

            // 增加浏览次数
            ActivityRecordAdapter.Instance.Update(new ActivityRecordModel()
            {
                Code = Guid.NewGuid().ToString(),
                ActivityCode = activityCode,
                UserCode = user.Id,
                Type = ActivityRecordType.View,
                Creator = user.Id,
                CreateTime = DateTime.Now,
                ValidStatus = true
            });

            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
                Data = data
            });
        }
        #endregion

        #region 获取活动统计信息及报名人员列表
        /// <summary>
        /// 获取活动统计信息及报名人员列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetDetailStatistics(string activityCode)
        {
            var user = (Seagull2Identity)User.Identity;

            var model = ActivityInfoAdapter.Instance.Load(w => w.AppendItem("Code", activityCode)).SingleOrDefault();
            if (model == null)
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = "没有找到该活动！"
                });
            }

            var data = new AppActivityInfoStatisticsViewModel()
            {
                ActivityCode = activityCode
            };

            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
                Data = data
            });
        }
        #endregion

        // PC端

        #region 获取活动列表
        /// <summary>
        /// 获取活动列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetListForPC(int pageSize, int pageIndex, string title = "")
        {
            var dataCount = ActivityInfoAdapter.Instance.GetList(title);
            var dataList = ActivityInfoAdapter.Instance.GetList(pageSize, pageIndex, title);

            var view = new List<ActivityListForPC>();
            dataList.ForEach(item =>
            {
                view.Add(new ActivityListForPC()
                {
                    Code = item.Code,
                    Title = item.Title,
                    Cover = item.Cover,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime,
                    IsOffline = item.IsOffline,
                    OfflineAddress = item.OfflineAddress,
                    CreatorCode = item.Creator,
                    CreateTime = item.CreateTime
                });
            });

            var data = new BaseViewPage()
            {
                DataCount = dataCount,
                PageCount = dataCount % pageSize == 0 ? dataCount / pageSize : dataCount / pageSize + 1,
                PageData = view
            };

            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
                Data = data
            });
        }
        #endregion

        #region 删除活动
        /// <summary>
        /// 删除活动
        /// </summary>
        [HttpGet]
        public IHttpActionResult Delete(string code)
        {
            var model = ActivityInfoAdapter.Instance.Load(m => m.AppendItem("Code", code)).SingleOrDefault();
            if (model == null)
            {
                return Ok(new BaseView
                {
                    State = false,
                    Message = "not find."
                });
            }
            ActivityInfoAdapter.Instance.Delete(w => w.AppendItem("Code", code));
            ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "删除了应用管理-活动" + model.Title);
            return Ok(new BaseView
            {
                State = true,
                Message = "删除成功！"
            });
        }
        #endregion
    }
}
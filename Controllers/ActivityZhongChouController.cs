using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Xml;
using log4net;
using MCS.Library.Core;
using System.Web.Http.Description;
using Newtonsoft.Json.Linq;
using Seagull2.YuanXin.AppApi.ZhongChouApi.Models;
using Seagull2.YuanXin.AppApi.ZhongChouApi.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.ViewData;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using MCS.Library.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.BaseEntity;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using MobileBusiness.Common.Data;
using MCS.Web.Library.Script;
using Seagull2.YuanXin.AppApi.ZhongChou.Data.Models;
using Seagull2.YuanXin.AppApi.ViewsModel.Contacts;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("ActivityZhongChou")]
    public class ActivityZhongChouController : ControllerBase
    {
        /// <summary>
        /// 客服列表
        /// </summary>
        [HttpGet]
        [Route("CustomerService")]
        public string CustomerService()
        {
            List<ContactsViewModel> userList = new List<ContactsViewModel>();
            string[] loginNameList = ConfigurationManager.AppSettings["CustomerService"].Split(',');
            loginNameList.ForEach(loginName =>
            {
                var item  = ContactsAdapter.Instance.LoadByLoginName(loginName);
                var model = new ContactsViewModel
                {
                    ObjectID = item.ObjectID,
                    ParentID = item.ParentID,
                    LOGIN_Name = item.Logon_Name,
                    DisplayName = item.DisplayName,
                    FullPath = item.FullPath,
                    Mail = item.Mail,
                    WP = item.WP,
                    MP = item.MP,
                    GlobalSort = item.GlobalSort,
                };
                userList.Add(model);
            });
            return JsonConvert.SerializeObject(userList, JsonHelper.GetDefaultJsonSettings());
        }

        #region Home

        /// <summary>
        /// 获取全部活动列表
        /// </summary>
        /// <param name="city">城市</param>
        /// <returns>App首页视图数据</returns>
        [HttpGet]
        [Route("Home/Index")]
        [ResponseType(typeof(AppIndexView))]
        public IHttpActionResult AppIndex(int pageIndex,int pageSize,string city = "")
        {
            AppIndexView appIndexView = new AppIndexView();

            var topProjectDataQuery = new TopProjectViewDataQuery
            {
                City = city,
                OrderByClause = "tp.StartTime desc"
            };
            int totalCount = 0;
            int startRowIndex = (pageIndex - 1) * pageSize;
            //暂时没有用到以下代码 注释时间：2017.9.20
            //appIndexView.TopProjectCollection = topProjectDataQuery.Query(startRowIndex, pageSize, ref totalCount);//默认最多10个置顶项目
            //var result = appIndexView.TopProjectCollection.ToListDataView(pageIndex, pageSize, totalCount);
            var activityDataQuery = new CaseProjecttViewDataQuery
            {
                ProjectType = ProjectTypeEnum.Online.ToString("D") + "," + ProjectTypeEnum.Anchang.ToString("D"),
                AuditStatus = ZhongChouData.Enums.AuditStatus.Success.ToString("D"),
                City = city,
                OrderByClause = "state asc,FocusNo DESC,ModifyTime DESC"
            };

            appIndexView.ActivityCollection = activityDataQuery.Query(startRowIndex, pageSize, ref totalCount);
            var results = appIndexView.ActivityCollection.ToListDataView(pageIndex, pageSize, totalCount);
            //var zcProjectDataQuery = new ZcProjectViewDataQuery()
            //{
            //    Type = "2,3,4,5",
            //    AuditStatus = ZhongChouData.Enums.AuditStatus.Success.ToString("D"),
            //    City = city,
            //    OrderByClause = "[state] asc, ModifyTime desc"
            //};

            //appIndexView.NewestZcProjectCollection = zcProjectDataQuery.Query(0, 6, ref totalCount);

            //var topicsDataQuery = new TopicsViewDataQuery()
            //{
            //    OrderByClause = "p.FocusNo DESC,p.ModifyTime DESC",
            //    City = city,
            //    IsAll = false
            //};
            //appIndexView.TopicsCollection = topicsDataQuery.Query(0, 6, ref totalCount);

            //var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = appIndexView };

            return Ok(new ViewsModel.BaseView()
            {
                State = true,
                Message = "success.",
                Data = results
            });

        }

        #endregion


        #region 活动信息

        /// <summary>
        /// 获取热门活动列表
        /// </summary>
        /// <param name="tag">活动标签</param>
        /// <param name="type">活动类型</param>
        /// <param name="city">活动城市</param>
        /// <param name="status">排序</param>
        /// <param name="times">时间范围内</param>
        /// <param name="sort">排序 ModifyTime最新发布 SupportNo报名最多</param>
        /// <param name="pageIndex">页码索引</param>
        /// <returns>活动视图数据</returns>
        [HttpGet]
        [Route("Activity/Activitys")]
        [ResponseType(typeof(ListView))]
        public IHttpActionResult GetActivitys(string title = "", string tag = "", string type = "", string city = "", string status = "", string times = "", string sort = "", int pageIndex = 1)
        {
            //移动办公活动列表：只能看到线下（发布范围为0）和线上活动

            int pageindex = pageIndex;
            int pagesize = 10;
            var dataQuery = new CaseProjecttViewDataQuery
            {
                Title = title,
                ProjectTagCode = tag,
                City = city,
                ProjectType = !string.IsNullOrEmpty(type) ? type : (ProjectTypeEnum.Anchang.ToString("D") + "," + ProjectTypeEnum.Online.ToString("D")),
                AuditStatus = ZhongChouData.Enums.AuditStatus.Success.ToString("D"),
                ProjectState = status,
                Time = times,
                OrderByClause = !string.IsNullOrEmpty(sort) ? ("[state] asc," + sort + " DESC") : "[state] asc,ModifyTime DESC"
            };
            int totalCount = 0;
            int startRowIndex = (pageIndex - 1) * pagesize;
            var queryResult = dataQuery.Query(startRowIndex, pagesize, ref totalCount);
            var result = new ListView(pagesize, totalCount, pageIndex) { ListData = queryResult };
            //var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = result };
            return Ok(new ViewsModel.ViewModelBaseList
            {
                State = true,
                Message = "success.",
                Data = result
            });
        }

        /// <summary>
        /// 获取活动详情
        /// </summary>
        /// <param name="projectCode">项目编码</param>
        /// <param name="userCode">用户编码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Activity/Detail")]
        [ResponseType(typeof(ActivityView))]
        public IHttpActionResult GetDetail(string projectCode, string userCode)
        {
            var result = new ActivityView(projectCode, userCode);
            return Ok(new ViewsModel.BaseView() {
                State = true,
                Message = "sucess",
                Data = result
            });
        }

        /// <summary>
        /// 我关注的活动
        /// </summary>
        /// <param name="userCode">用户编码</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>关注的活动视图数据</returns>
        [HttpGet]
        [Route("My/MyFocusActivity")]
        [ResponseType(typeof(ListView))]
        public string GetMyFocusActivity(string userCode, int pageIndex = 1)
        {
            //准备查询条件
            int pagesize = 10;
            var dataQuery = new ActivityFocusViewDataQuery { FocusCreator = userCode };

            //得到查询结果
            int totalCount = 0;
            int startRowIndex = (pageIndex - 1) * pagesize;
            var queryResult = dataQuery.Query(startRowIndex, pagesize, ref totalCount);

            var result = new ListView(pagesize, totalCount) { ListData = queryResult };
            var apiresult = new { Data = result };

            return JsonConvert.SerializeObject(apiresult, JsonHelper.GetDefaultJsonSettings());
        }
        /// <summary>
        /// 用户关注
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("My/AddFocus")]
        [ResponseType(typeof(ApiResult))]
        public string AddFocus([FromBody]FocusActionParameters data)
        {
            var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Error, Data = "" };
            try
            {
                var userCode = data.userCode;
                var projectCode = data.projectCode;


                //是否关注
                var isFocus = false;
                if (userCode.IsNullOrEmpty() || projectCode.IsNullOrEmpty())
                {
                    apiresult.ErrorMsg = "用户编码或项目编码不能为空";
                    return JsonConvert.SerializeObject(apiresult);
                }

                var point = 0;
                //用户是否是首次关注
                var isFirstFocus = false;
                if (UserPointAdapter.Instance.IsCompleteNewUserTaks(userCode, SourceType.FollowCrowdfund))
                {
                    isFirstFocus = true;
                }

                var userFocus = UserFocusAdapter.Instance.Load(userCode, projectCode);
                var project = ProjectAdapter.Instance.LoadByCode(projectCode);
                userFocus.IsNull(() =>
                {
                    UserFocus focus = new UserFocus()
                    {
                        Code = UuidHelper.NewUuidString(),
                        ProjectCode = projectCode,
                        Creator = userCode,
                        CreateTime = DateTime.Now
                    };
                    switch (project.Type)
                    {
                        case ProjectTypeEnum.Anchang: focus.Type = UserFocusType.Anchang; break;
                        case ProjectTypeEnum.Coupon: focus.Type = UserFocusType.Coupon; break;
                        case ProjectTypeEnum.Online: focus.Type = UserFocusType.Online; break;
                        case ProjectTypeEnum.ZaiShou: focus.Type = UserFocusType.ZaiShou; break;
                        default: focus.Type = UserFocusType.None; break;
                    }
                    //如果不是首次关注
                    if (isFirstFocus == false)
                    {
                        var userPoint = UserPointAdapter.Instance.SetPoint(userCode, SourceType.FollowCrowdfund, "FollowCrowdfund");
                        point = userPoint.ChangeNo;
                    }
                    UserFocusAdapter.Instance.Update(focus);
                    ProjectAdapter.Instance.SetIncFocusNo(projectCode);
                    isFocus = true;
                });

                apiresult.Code = (int)ResultCodeEnum.Success;
                apiresult.Data = new { point = point, isFocus = isFocus };
            }
            catch (Exception e)
            {
                Log.WriteLog(e.Message);
                Log.WriteLog(e.StackTrace);
                throw e;
            }

            return JsonConvert.SerializeObject(apiresult);
        }

        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="data"></param>
        [HttpPost]
        [Route("My/CancleFocus")]
        public void CancleFocus([FromBody]FocusActionParameters data)
        {
            //
            var userCode = data.userCode;
            var projectCode = data.projectCode;
            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                //更新到数据库
                UserFocusAdapter.Instance.Delete(userCode, projectCode);
                ProjectAdapter.Instance.SetDecFocusNo(projectCode);
                scope.Complete();
            }

        }

        /// <summary>
        /// [案场活动]报名用户列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Activity/GetEnrollUserList")]
        [ResponseType(typeof(ListView))]
        public string GetEnrollUserList(string projectCode, int pageIndex = 1)
        {
            //准备查询条件
            int pageindex = Convert.ToInt32(pageIndex);
            int pagesize = 10;
            var dataQuery = new OrderEnrollViewDataQuery
            {
                ProjectCode = projectCode,
                IsValid = true,
                OrderStatus = OrderStatus.Anchang_Enrolled.GetHashCode().ToString() + "," + OrderStatus.Anchang_Signed.GetHashCode().ToString() + "," + OrderStatus.Anchang_Evaluated.GetHashCode().ToString(),
                Type = OrderType.Anchang
            };
            //得到查询结果
            int totalCount = 0;
            int startRowIndex = 0;
            var queryResult = dataQuery.Query(0, 1000, ref totalCount);

            queryResult.ForEach(order =>
            {
                var oldStatus = order.Status;
                if (oldStatus == OrderStatus.Anchang_NotEnroll)
                {
                    if (order.Total == 0)
                    {
                        order.Status = OrderStatus.Anchang_Enrolled;
                    }
                    else
                    {
                        TransactionInfo model = TransactionInfoAdapter.Instance.GetByObjectID(order.Code);
                        if (model != null && model.TradeState == 1)
                        {
                            order.Status = OrderStatus.Anchang_Enrolled;
                        }
                    }
                }
            });
            OrderEnrollViewDataCollection myColl = new OrderEnrollViewDataCollection();
            for (var i = 0; i < queryResult.Count; i++)
            {
                if (i + 1 >= (pageIndex - 1) * pagesize && i + 1 < pageindex * pagesize)
                {
                    myColl.Add(queryResult[i]);
                }
            }

            var result = new ListView(pagesize, queryResult.Count, pageIndex) { ListData = myColl };
            var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = result };
            return JsonConvert.SerializeObject(apiresult, JsonHelper.GetDefaultJsonSettings());
        }

        /// <summary>
        /// 获得直播信息
        /// </summary>
        /// <param name="projectCode">项目编码</param>        
        /// <returns></returns>
        [HttpGet]
        [Route("Activity/GetVideo")]
        [ResponseType(typeof(VideoView))]
        public string GetVideo(string projectCode)
        {
            var result = new VideoView(projectCode);
            var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = result };
            return JsonConvert.SerializeObject(apiresult, JsonHelper.GetDefaultJsonSettings());
        }

        #endregion

        #region 活动订单

        /// <summary>
        /// 获取案场活动订单
        /// </summary>
        /// <param name="userCode">用户编码</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Activity/CaseActivityOrders")]
        [ResponseType(typeof(ListView))]
        public string GetCaseActivityOrders(string userCode, int pageIndex = 1)
        {
            //准备查询条件           
            int pagesize = 10;
            var dataQuery = new OrderViewDataQuery { OrderCreator = userCode, IsValid = true, ProjectType = ProjectTypeEnum.Anchang };
            //得到查询结果
            int totalCount = 0;
            int startRowIndex = (pageIndex - 1) * pagesize;
            var queryResult = dataQuery.Query(startRowIndex, pagesize, ref totalCount);

            queryResult.ForEach(order =>
            {
                var oldStatus = order.Status;
                if (oldStatus == OrderStatus.Anchang_NotEnroll)
                {
                    if (order.Total == 0)
                    {
                        order.Status = OrderStatus.Anchang_Enrolled;
                    }
                    else
                    {
                        TransactionInfo model = TransactionInfoAdapter.Instance.GetByObjectID(order.Code);
                        if (model != null && model.TradeState == 1)
                        {
                            order.Status = OrderStatus.Anchang_Enrolled;
                        }
                    }
                }
            });

            var result = new ListView(pagesize, totalCount, pageIndex) { ListData = queryResult };
            var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = result };
            return JsonConvert.SerializeObject(apiresult, JsonHelper.GetDefaultJsonSettings());
        }

        /// <summary>
        /// 案场活动提交订单（获取提交页面视图数据）
        /// </summary>
        /// <param name="projectCode">项目编码</param>
        /// <param name="userCode">用户编码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Activity/SubmitOrder")]
        [ResponseType(typeof(SubmitOrderView_CaseActivity))]
        public string SubmitOrder(string userCode, string projectCode)
        {
            var result = new SubmitOrderView_CaseActivity(projectCode, userCode);
            result.UserInfo = ContactsAdapter.Instance.LoadByCode(userCode);
            var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = result };
            return JsonConvert.SerializeObject(apiresult, JsonHelper.GetDefaultJsonSettings());
        }

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="formData"></param>
        /// <returns>返回订单编码</returns>
        [HttpPost]
        [Route("Activity/SubmitOrder")]
        public IHttpActionResult SubmitOrder([FromBody]OrderForm formData)
        {
            //Log.WriteLog("报名参数：" + JsonConvert.SerializeObject(formData));
            var order = formData.ToOrder(OrderType.Anchang);
            ActivityWorks work = ActivityWorksAdapter.Instance.LoadByCreator(formData.ProjectCode, formData.Creator);
            if (work != null)
            {
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = "fail."
                });
            }
            var eventInfo = ActivityEventAdapter.Instance.LoadByCode(order.SubProjectCode);
            var orderAddress = formData.ToOrderAddress();
            var project = ProjectAdapter.Instance.LoadByCode(formData.ProjectCode);
            eventInfo.EnrollNo += formData.GoodsCount;

            var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = order.Code };
            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                try
                {
                    OrderAdapter.Instance.Update(order);
                    ActivityEventAdapter.Instance.Update(eventInfo);
                    OrderAddressAdapter.Instance.Update(orderAddress);
                    if (order.Status == OrderStatus.Anchang_Enrolled)
                    {
                        ProjectAdapter.Instance.SetIncSupportNo(order.ProjectCode);
                        //增加好友动态
                        NewsFeedAdapter.Instance.AddOneFeed(NewsFeedAction.Enlist, order.ProjectCode, order.Creator, "", 0, order.Code);
                        MessagerHelp.SendOrderMessage(project, order, true);
                    }
                }
                catch (Exception e)
                {
                    apiresult.Code = (int)ResultCodeEnum.Error;
                    apiresult.Data = e.Message + "|" + e.StackTrace;
                    Log.WriteLog("报名失败：" + e.Message + "|" + e.StackTrace);
                }
                scope.Complete();
            }

            return Ok(new ViewsModel.BaseView()
            {
                State = true,
                Message = "success.",
                Data = apiresult
            });
        }

        /// <summary>
        /// 获取案场活动订单详情
        /// </summary>
        /// <param name="orderCode">订单编码</param>
        /// <returns>订单详情视图数据</returns>
        [HttpGet]
        [Route("Activity/GetOrderDetails")]
        [ResponseType(typeof(OrderDetailView))]
        public string GetOrderDetails(string orderCode)
        {
            var orderDetailView = new OrderView_Case(orderCode);

            var order = orderDetailView.Order;
            if (order != null)
            {
                var oldStatus = order.Status;
                if (oldStatus == OrderStatus.Anchang_NotEnroll)
                {
                    if (order.Total == 0)
                    {
                        order.Status = OrderStatus.Anchang_Enrolled;
                    }
                    else
                    {
                        TransactionInfo model = TransactionInfoAdapter.Instance.GetByObjectID(order.Code);
                        if (model != null && model.TradeState == 1)
                        {
                            order.Status = OrderStatus.Anchang_Enrolled;
                        }
                    }
                }
            }

            var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = orderDetailView };
            return JsonConvert.SerializeObject(apiresult, JsonHelper.GetDefaultJsonSettings());
        }
        [HttpGet]
        [Route("Activity/GetCaseActivityOrderDetail")]
        public string GetCaseActivityOrderDetail(string orderCode)
        {
            var order = CaseOrderAdapter.Instance.LoadByCode(orderCode);

            if (order != null)
            {
                var oldStatus = order.Status;
                if (oldStatus == OrderStatus.Anchang_NotEnroll)
                {
                    if (order.Total == 0)
                    {
                        order.Status = OrderStatus.Anchang_Enrolled;
                    }
                    else
                    {
                        TransactionInfo model = TransactionInfoAdapter.Instance.GetByObjectID(order.Code);
                        if (model != null && model.TradeState == 1)
                        {
                            order.Status = OrderStatus.Anchang_Enrolled;
                        }
                    }
                }
            }

            var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = order };
            return JsonConvert.SerializeObject(apiresult, JsonHelper.GetDefaultJsonSettings());
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="orderCode">订单编码</param>
        /// <returns></returns>
        [Route("CancelOrder")]
        [HttpPost]
        public void CancelOrder(string orderCode)
        {
            var order = OrderAdapter.Instance.LoadByCode(orderCode);
            order.IsValid = false;

            var eventInfo = ActivityEventAdapter.Instance.LoadByCode(order.SubProjectCode);
            if (eventInfo != null)
                eventInfo.EnrollNo -= order.GoodsCount;

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                OrderAdapter.Instance.Update(order);
                if (eventInfo != null)
                    ActivityEventAdapter.Instance.Update(eventInfo);
                scope.Complete();
            }

        }

        private static readonly object locker = new object();


        #endregion

        #region 活动作品

        /// <summary>
        /// 获取参与作品
        /// </summary>
        /// <param name="projectCode">项目编码</param>
        /// <param name="userCode">用户编码</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>作品列表视图数据</returns>
        [HttpGet]
        [Route("Activity/ActivityWorks")]
        [ResponseType(typeof(ListView))]
        public string GetActivityWorks(string projectCode, string userCode, bool isCreated = false, int pageIndex = 1)
        {
            //准备查询条件
            int pagesize = 10;
            var dataQuery = isCreated ? new ActivityWorksViewDataDataQuery
            {
                ProjectCode = projectCode,
                UserCode = userCode,
                OrderByClause = "CreateTime desc"
            } : new ActivityWorksViewDataDataQuery
            {
                ProjectCode = projectCode,
                OrderByClause = "CreateTime desc"
            };
            List<string> VotedActivityWorkCodes = ActivityWorksAdapter.Instance.LoadVoted(projectCode, userCode);
            //得到查询结果
            int totalCount = 0;
            int startRowIndex = (pageIndex - 1) * pagesize;
            var queryResult = dataQuery.Query(startRowIndex, pagesize, ref totalCount);
            if (VotedActivityWorkCodes != null && VotedActivityWorkCodes.Count > 0)
            {
                queryResult.Where(o => VotedActivityWorkCodes.Contains(o.Code)).All(x => x.IsVote = true);
            }
            if (userCode.IsNotWhiteSpace())
            {
                var vote = UserVoteAdapter.Instance.Load(userCode, projectCode);
                vote.IsNotNull(v =>
                {
                    foreach (var item in queryResult)
                    {
                        if (item.Code == vote.ActivityWorksCode)
                            item.IsVote = true;
                    }
                });
            }
            var result = new ListView(pagesize, totalCount, pageIndex) { ListData = queryResult };
            var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = result };
            return JsonConvert.SerializeObject(apiresult, JsonHelper.GetDefaultJsonSettings());
        }

        /// <summary>
        /// 获取作品详情
        /// </summary>        
        /// <param name="workCode">作品编码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Activity/GetWorkDetail")]
        [ResponseType(typeof(ActivityWorks))]
        public string GetWorkDetail(string workCode)
        {
            ActivityWorks work = ActivityWorksAdapter.Instance.LoadByCode(workCode);
            OnlineProject project = OnlineProjectAdapter.Instance.LoadByCode(work.ProjectCode);
            var result = new { work = work, project = project };
            var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = result };
            return JsonConvert.SerializeObject(apiresult, JsonHelper.GetDefaultJsonSettings());
        }

        /// <summary>
        /// 发布作品
        /// </summary>
        /// <param name="formdata">作品表单实体</param>
        [HttpPost]
        [Route("Activity/PublishWorks")]
        public void PublishWorks([FromBody]ActivityWorksForm formdata)
        {
            var attachments = new AttachmentCollection();
            var works = formdata.ToActivityWorks(out attachments);
            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                ActivityWorksAdapter.Instance.Update(works);
                //附件入库&上传附件
                AttachmentAdapter.Instance.UpdateCollectionWithVersioned(works.Code, attachments);
                //作品+1
                ProjectAdapter.Instance.SetIncSupportNo(works.ProjectCode);
                scope.Complete();
            }
        }

        /// <summary>
        /// 作品投票
        /// </summary>
        /// <param name="formData"></param>
        [HttpPost]
        [Route("Activity/Vote")]
        public void WorksVote([FromBody]UserVoteForm formData)
        {
            var userVote = formData.ToUserVote();

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                UserVoteAdapter.Instance.Update(userVote);

                ActivityWorksAdapter.Instance.SetIncVoteCount(formData.ActivityWorksCode);

                scope.Complete();
            }
        }

        /// <summary>
        /// 获奖作品
        /// </summary>
        /// <param name="projectCode">用户编码</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Activity/WorksAwards")]
        [ResponseType(typeof(ActivityWorks))]
        public string GetWorksAwards(string projectCode, int pageIndex = 1)
        {
            int pagesize = 10;
            var dataQuery = new WorksAwardsViewDataQuery
            {
                ProjectCode = projectCode,
            };

            //得到查询结果
            int totalCount = 0;
            int startRowIndex = (pageIndex - 1) * pagesize;
            var queryResult = dataQuery.Query(startRowIndex, pagesize, ref totalCount);

            var result = new ListView(pagesize, totalCount) { ListData = queryResult };

            var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = result };

            return JsonConvert.SerializeObject(apiresult, JsonHelper.GetDefaultJsonSettings());
        }
        /// <summary>
        /// 删除上传作品
        /// </summary>
        /// <param name="workCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Activity/DeleteWork")]
        public string DeleteWork(string workCode)
        {
            var result = new object();
            var work = ActivityWorksAdapter.Instance.LoadByCode(workCode);
            if (work == null)
            {
                result = new { result = false, err = "该作品已删除" };
            }
            else
            {
                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    NewsFeedAdapter.Instance.DeleteOneFeed(NewsFeedAction.Join, work.ProjectCode, work.Creator, work.Code);
                    if (!ZhongChouApi.Common.CommonHelper.IsTestUser(work.Creator) || ZhongChouApi.Common.CommonHelper.IsTestModel())
                    {
                        ProjectAdapter.Instance.SetDecSupportNo(work.ProjectCode);
                    }
                    ActivityWorksAdapter.Instance.DeleteByCodes(workCode);
                    scope.Complete();
                }
                result = new { result = true, err = "作品删除成功" };
            }
            var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = result };

            return JsonConvert.SerializeObject(apiresult, JsonHelper.GetDefaultJsonSettings());
        }
        #endregion


        #region 发布话题和回复话题相关
        /// <summary>
        /// 发表/回复话题
        /// </summary>
        /// <param name="commentForm">评论实体</param>               
        /// <returns></returns>
        [HttpPost]
        [Route("Comment/UpdateComment")]
        public string UpdateComment([FromBody]CommentForm commentForm)
        {
            Log.WriteLog("发表评论参数：" + JsonConvert.SerializeObject(commentForm));

            var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Error };
            try
            {
                UserComment comment = commentForm.ToComment();
                Project project = null;
                var isCompleteDiscussTaks = false;
                var point = 0;
                if (comment.ProjectCode.IsNotEmpty() && comment.ParentCode.IsNullOrEmpty())
                {
                    project = ProjectAdapter.Instance.LoadByCode(commentForm.projectCode);
                    project.CommentNo++;
                }
                if (UserPointAdapter.Instance.IsCompleteTodayTaks(commentForm.usercode, SourceType.Discuss))
                    isCompleteDiscussTaks = true;

                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    UserCommentAdapter.Instance.Update(comment);

                    if (project != null)
                    { ProjectAdapter.Instance.Update(project); }
                    else
                    {
                        UserComment uct = UserCommentAdapter.Instance.LoadByCode(comment.ParentCode);
                        MessagerHelp.PushCommentMessage(uct.Creator, commentForm.content);
                    }
                    NewsFeedAdapter.Instance.AddOneFeed(NewsFeedAction.Topic, commentForm.projectCode, commentForm.usercode, commentForm.content, 0, comment.Code);
                    if (isCompleteDiscussTaks == false)
                    {
                        var userPoint = UserPointAdapter.Instance.SetPoint(commentForm.usercode, SourceType.Discuss, "Discuss");
                        point = userPoint.ChangeNo;
                    }
                    if (comment.WorksCode.IsNotEmpty())
                    {
                        var work = ActivityWorksAdapter.Instance.LoadByCode(comment.WorksCode);
                        work.CommentNo += 1;
                        if (work != null)
                        {
                            ActivityWorksAdapter.Instance.Update(work);
                        }
                    }
                    scope.Complete();
                }


                apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = new { result = comment, point = point } };
            }
            catch (Exception e)
            {
                Log.WriteLog(e.Message);
                Log.WriteLog(e.StackTrace);
                apiresult.Data = e.Message;
            }

            return JsonConvert.SerializeObject(apiresult, JsonHelper.GetDefaultJsonSettings());
        }
        /// <summary>
        /// 获得话题列表
        /// </summary>
        /// <param name="projectCode"></param>
        /// <param name="workCode">非作品不提供</param>
        /// <param name="pageIndex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Comment/GetComment")]
        [ResponseType(typeof(ListView))]
        public String GetComment(string projectCode, string workCode = "", int pageIndex = 1, int pagesize = 10)
        {
            //准备查询条件
            int pageindex = Convert.ToInt32(pageIndex);

            var dataQuery = new CommentViewDataDataQuery
            {
                ProjectCode = projectCode,
                WorksCode = workCode,
                OrderByClause = "CreateTime desc"
            };

            //得到查询结果
            int totalCount = 0;
            int startRowIndex = (pageindex - 1) * pagesize;
            var queryResult = dataQuery.Query(startRowIndex, pagesize, ref totalCount);
            var result = new { List = new ListView(pagesize, totalCount, pageIndex) { ListData = queryResult } };
            var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = new { result = result } };
            return JsonConvert.SerializeObject(apiresult, JsonHelper.GetDefaultJsonSettings());
        }
        /// <summary>
        /// 获得话题及回复
        /// </summary>
        /// <param name="projectCode"></param>
        /// <param name="workCode"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Comment/GetReComment")]
        [ResponseType(typeof(ListView))]
        public String GetReComment(string projectCode, string workCode = "", int pageIndex = 1, int pagesize = 10)
        {
            //准备查询条件
            int pageindex = Convert.ToInt32(pageIndex);

            var dataQuery = new CommentViewDataDataQuery
            {
                ProjectCode = projectCode,
                WorksCode = workCode,
                OrderByClause = "CreateTime desc"
            };

            //得到查询结果
            int totalCount = 0;
            int startRowIndex = (pageindex - 1) * pagesize;
            var queryResult = dataQuery.Query(startRowIndex, pagesize, ref totalCount);
            var result = new { List = new ListView(pagesize, totalCount, pageIndex) { ListData = queryResult } };
            var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = new { result = result } };
            return JsonConvert.SerializeObject(apiresult, JsonHelper.GetDefaultJsonSettings());
        }

        #endregion

        #region 活动的评价相关
        /// <summary>
        /// 获得活动--评价列表
        /// </summary>
        /// <param name="projectCode"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Comment/GetEvaluation")]
        [ResponseType(typeof(ListView))]
        public string GetEvaluation(string projectCode, int pageIndex = 1, int pagesize = 10)
        {
            var dataQuery = new EvaluationViewDataDataQuery()
            {
                ProjectCode = projectCode
            };
            int totalCount = 0;
            int startRowIndex = (pageIndex - 1) * pagesize;
            var queryResult = dataQuery.Query(startRowIndex, pagesize, ref totalCount);
            var result = new { List = new ListView(pagesize, totalCount, pageIndex) { ListData = queryResult } };
            var apiresult = new ApiResult { Code = (int)ResultCodeEnum.Success, Data = result };
            return JsonConvert.SerializeObject(apiresult, JsonHelper.GetDefaultJsonSettings());
        }
        /// <summary>
        /// 案场--活动评价
        /// </summary>
        /// <param name="formdata"></param>
        [HttpPost]
        [Route("UserEvaluation/Evaluate")]
        public void Evaluate([FromBody]UserEvaluationForm formdata)
        {
            AttachmentCollection attachments = new AttachmentCollection();
            var userEvaluation = formdata.ToUserEvaluation(out attachments);

            OrderStatus status = OrderStatus.None;

            var order = OrderAdapter.Instance.LoadByCode(userEvaluation.OrderCode);

            if (formdata.Type == EvaluationType.Anchang.ToString("D"))
            {
                status = OrderStatus.Anchang_Evaluated;
            }
            using (TransactionScope ts = TransactionScopeFactory.Create())
            {
                OrderAdapter.Instance.UpdateOrderStatus(userEvaluation.OrderCode, status);
                UserEvaluationAdapter.Instance.Update(userEvaluation);
                if (!ZhongChouApi.Common.CommonHelper.IsTestUser(order.Creator) || ZhongChouApi.Common.CommonHelper.IsTestModel())
                {
                    ProjectAdapter.Instance.SetIncEvaluationUserTotal(order.ProjectCode);
                    ProjectAdapter.Instance.SetIncEvaluationScoreTotal(order.ProjectCode, (int)userEvaluation.Score);
                }
                if (attachments != null && attachments.Count > 0)
                {
                    //附件入库&上传附件
                    AttachmentAdapter.Instance.UpdateCollectionWithVersioned(userEvaluation.Code, attachments);
                    //上传附件到OSS
                    ZhongChouApi.Common.CommonHelper.UploadAttachments(formdata.Attachments);
                }
                MessagerHelp.SendOrderMessageToBusiness(order);

                ts.Complete();
            }
        }
        #endregion


        #region PC接口
        /// <summary>
        /// 案场活动列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadProjectListAnChang")]
        public string LoadProjectListAnChang(string title = "", int pageIndex = 1, int pageSize = 10)
        {
            var dataQuery = new CaseProjecttViewDataQuery();

            dataQuery.Title = title;
            dataQuery.ProjectType = ((int)ProjectTypeEnum.Anchang).ToString();
            dataQuery.AuditStatus = "1,2,3";
            dataQuery.OrderByClause = "CreateTime DESC,ModifyTime DESC";

            //得到查询结果
            int totalCount = 0;
            int startRowIndex = (pageIndex - 1) * pageSize;
            var queryResult = dataQuery.Query(startRowIndex, pageSize, ref totalCount);
            queryResult.ForEach(data =>
            {
                if (!data.CoverImg.Contains("http://"))
                {
                    data.CoverImg = ConfigurationManager.AppSettings["GetProjectFileUrl"] + data.CoverImg;
                }
            });

            //转化查询结果后输出
            ListDataView result = queryResult.ToListDataView(pageIndex, pageSize, totalCount);

            return JSONSerializerExecute.Serialize(result);
        }
        /// <summary>
        /// 征集活动列表
        /// </summary>
        [HttpGet]
        [Route("LoadProjectListZhengJi")]
        public string LoadProjectListZhengJi(string title = "", int pageIndex = 1, int pageSize = 10)
        {
            var dataQuery = new OnlineProjectViewDataQuery();

            dataQuery.Title = title;
            dataQuery.ProjectType = ((int)ProjectTypeEnum.Online).ToString();
            dataQuery.AuditStatus = "1,2,3";
            dataQuery.OrderByClause = "CreateTime DESC,ModifyTime DESC";

            //得到查询结果
            int totalCount = 0;
            int startRowIndex = (pageIndex - 1) * pageSize;
            var queryResult = dataQuery.Query(startRowIndex, pageSize, ref totalCount);
            queryResult.ForEach(data =>
            {
                if (!data.CoverImg.Contains("http://"))
                {
                    data.CoverImg = ConfigurationManager.AppSettings["GetProjectFileUrl"] + data.CoverImg;
                }
            });

            //转化查询结果后输出
            ListDataView result = queryResult.ToListDataView(pageIndex, pageSize, totalCount);

            return JSONSerializerExecute.Serialize(result);
        }
        /// <summary>
        /// 案场活动详情
        /// </summary>
        [HttpGet]
        [Route("GetProjectDetailsAnChang")]
        public string GetProjectDetailsAnChang(string code)
        {
            var project = CaseProjectAdapter.Instance.LoadByCode(code);

            #region 场次
            var activityEventColl = ActivityEventAdapter.Instance.LoadByProjectCode(code);

            var starTimeList = activityEventColl.Select(o => o.StartTime).Distinct().ToList();
            List<object> objList = new List<object>();
            //Dictionary<DateTime, List<ActivityEvent>> dicData = new Dictionary<DateTime, List<ActivityEvent>>();
            starTimeList.ForEach(p =>
            {
                var list = activityEventColl.FindAll(f => f.StartTime == p).ToList();

                objList.Add(new { key = p, keyStr = p.ToString("yyyy-MM-dd HH:mm"), value = list });
                //dicData.Add(p, list);
            });

            #endregion

            var result = new { project = project, activityEvents = project.ActivityEventCollection, ApplyUserInfo = project.ApplyUserInfo, userComments = project.UserCommentList, opinions = project.OpinionCollection, dicData = objList, UserEvaluation = project.UserEvaluation };
            if (!result.project.CoverImg.Contains("http://"))
            {
                result.project.CoverImg = ConfigurationManager.AppSettings["GetProjectFileUrl"] + result.project.CoverImg;
            }
            return JsonConvert.SerializeObject(result);
        }
        /// <summary>
        /// 征集活动详情
        /// </summary>
        [HttpGet]
        [Route("GetProjectDetailsZhengJi")]
        public string GetProjectDetailsZhengJi(string code)
        {
            var project = OnlineProjectAdapter.Instance.LoadByCode(code);

            #region 参与作品查询
            var dataQuery = new ActivityWorksViewDataDataQuery();
            dataQuery.ProjectCode = code;
            dataQuery.OrderByClause = "CreateTime DESC";
            int total = 0;
            ActivityWorksViewDataCollection coll = dataQuery.Query(0, 100, ref total);
            #endregion

            var result = new { project = project, activityWorksViewDataColl = coll };

            return JSONSerializerExecute.Serialize(result);
        }
        /// <summary>
        /// 删除活动
        /// </summary>
        /// <param name="data">活动编码code</param>
        /// <returns></returns>
        [HttpPost]
        [Route("DeleteProject")]
        public string DeleteProject(dynamic data)
        {
            string code = data.code;
            ProjectAdapter.Instance.DeleteByCode(code, false);
            return "OK";
        }

        /// <summary>
        /// 审核众筹项目
        /// </summary>
        [HttpPost]
        [Route("AuditProject")]
        public string AuditProject(dynamic data)
        {
            //data = new {
            //    projectCode="",
            //    auditResult="",
            //    auditResultContent=""
            //};

            //准备参数
            string projectCode = data.projectCode;
            int auditStatus = int.Parse((string)data.auditResult);
            var opinion = new Opinion
            {
                Code = Guid.NewGuid().ToString(),
                Type = OpinionType.Project,
                ResourceID = projectCode,
                Content = data.auditResultContent,
                Creator = CurrentUserCode,
                CreatorName = CurrentUser.DiaplayName,
                CreateTime = DateTime.Now,
                IsValid = true
            };

            var project = ProjectAdapter.Instance.LoadByCode(projectCode);

            //更新项目状态
            ProjectAdapter.Instance.AuditProject(auditStatus, projectCode);

            //更新审核意见
            OpinionAdapter.Instance.Update(opinion);

            //更新项目总个数
            if (auditStatus == Convert.ToInt32(ZhongChouData.Enums.AuditStatus.Success))
            {
                ProjectTypeAdapter.Instance.SetIncTotalProjectNo(project.Type.ToString("D"));
            }


            var result = new { result = "ok" };

            return JSONSerializerExecute.Serialize(result);
        }

        /// <summary>
        /// 增加分享人数
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("IncreaseShareNo")]
        public void IncreaseShareNo(string projectCode)
        {
            ProjectAdapter.Instance.SetIncShareNo(projectCode);
        }


        #endregion
    }
}

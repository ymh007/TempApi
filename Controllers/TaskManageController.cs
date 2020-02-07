using log4net;
using MCS.Library.Data;
using MCS.Library.OGUPermission;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Task;
using Seagull2.YuanXin.AppApi.Domain.TaskManage;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Models.TaskManage;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;
using static Seagull2.YuanXin.AppApi.Domain.TaskManage.TaskSubject;
using static Seagull2.YuanXin.AppApi.ViewsModel.TaskManage.TaskManageViewModel;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 任务管理控制器
    /// </summary>
    public class TaskManageController : ApiController
    {
        static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 新增编辑任务
        /// <summary>
        /// 新增编辑任务
        /// </summary>
        [HttpPost]
        public IHttpActionResult Save(SaveViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                var identity = (Seagull2Identity)User.Identity;

                var domain = new TaskDomain();
                var subject = new TaskSubject();
                domain.IsValidOfPanretCode(model.ParentCode);

                var userList = new List<IUser>();
                if (model.CopyPersons != null && model.CopyPersons.Count > 0)
                {
                    userList = domain.FilterInvalid(model);
                }

                // 新增
                if (string.IsNullOrEmpty(model.Code))
                {
                    var taskModel = new TaskModel();

                    using (TransactionScope scope = TransactionScopeFactory.Create())
                    {
                        domain.InsertTaskInfo(identity, model, out TaskModel taskInfo);
                        taskModel = taskInfo;
                        if (userList.Count > 0)
                        {
                            domain.InsertCopyPerson(identity, userList, taskInfo.Code);
                        }
                        scope.Complete();
                    }
                    Task.Run(() =>
                    {
                        subject.Add(taskModel, identity, userList);
                    });
                }
                // 编辑
                else
                {
                    var formerModel = domain.IsValidOfTaskCode(model.Code);
                    var currentModel = domain.StructureTaskModel(model, formerModel);
                    using (TransactionScope scope = TransactionScopeFactory.Create())
                    {
                        TaskApapter.Instance.Update(currentModel);
                        if (userList.Count > 0)
                        {
                            domain.InsertCopyPerson(identity, userList, currentModel.Code);
                        }
                        domain.DletePersonByTaskCodeAndUserCode(currentModel.Code, currentModel.Executor);
                        domain.DeleteTaskHide(currentModel.Code);
                        scope.Complete();
                    }
                    Task.Run(() =>
                    {
                        subject.Edit(formerModel, currentModel, identity, userList);
                    });
                }
            });
            return Ok(result);
        }
        #endregion

        #region Redis发送通知
        /// <summary>
        /// Redis发送通知
        /// </summary>
        /// <param name="post">RedisViewModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost, AllowAnonymous]
        public IHttpActionResult PushRemind(RedisViewModel post)
        {
            var result = ControllerService.Run(() =>
            {
                TaskDomain domain = new TaskDomain(); //定义业务领域
                var taskInfo = domain.IsValidOfTaskCode(post.Code);
                var pushMessage = domain.GetPushContent(taskInfo.TitleContent);
                domain.TaskRedisMessagePush(taskInfo, pushMessage, post.RemindStr);
                domain.TaskRedisPushService(taskInfo, pushMessage, post.RemindStr);
            });
            return Ok(result);
        }
        #endregion

        #region 取消任务
        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="code">任务code</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult TaskCancel(string code)
        {
            var result = ControllerService.Run(() =>
            {
                var subject = new TaskSubject();//实例化主题
                TaskDomain domain = new TaskDomain(); //定义业务领域
                var user = (Seagull2Identity)User.Identity;//获取当前登录用户
                var taskInfo = domain.IsVaildOfTaskCode(user, code);
                var taskColl = domain.GetAllTaskColl(code);
                subject.Cancel(taskInfo.Code, user, taskColl);
            });
            return Ok(result);
        }
        #endregion

        #region 获取任务评论列表
        /// <summary>
        /// 获取任务评论列表
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="code">任务编码</param>
        /// <param name="type">0：全部消息；1：评论</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        public IHttpActionResult TaskMessageList(int pageIndex, int pageSize, string code, EnumTaskMessageType type)
        {
            var result = ControllerService.Run(() =>
            {
                var domain = new TaskDomain();
                var list = domain.SelectMessageList(pageIndex, pageSize, code, (int)type);
                return list;
            });
            return Ok(result);
        }
        #endregion

        #region  新增任务评论消息
        /// <summary>
        /// 新增任务评论消息
        /// </summary>
        /// <param name="model">新增任务评论消息ViewModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        public IHttpActionResult TaskAddMessage(TaskAddMessageViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                var domain = new TaskDomain();
                var user = (Seagull2Identity)User.Identity;
                domain.SaveTaskCommentMessage(model, user);
            });
            return Ok(result);
        }

        #endregion

        #region 获取任务列表
        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="type">0：未完成;1:完成；2：我创建的；3：我执行的；4：抄送我的</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        public IHttpActionResult GetTaskList(int pageIndex, int pageSize, EnumTaskType type)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var domain = new TaskDomain();
                return domain.GetTaskList(pageIndex, pageSize, (int)type, user);
            });
            return Ok(result);
        }
        #endregion

        #region 获取任务详情
        /// <summary>
        /// 获取任务详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetTaskDetails(string code)
        {
            var result = ControllerService.Run(() =>
            {
                var domain = new TaskDomain();
                return domain.GetTaskDetails(code);
            });
            return Ok(result);
        }

        #endregion

        #region 完成或重新打开任务
        /// <summary>
        /// 完成或重新打开任务
        /// </summary>
        /// <param name="code">任务编码</param>
        /// <param name="state">未完成，完成</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ChangeTaskState(string code, EnumTaskCompletion state)
        {
            var result = ControllerService.Run(() =>
            {
                var domain = new TaskDomain();
                var user = (Seagull2Identity)User.Identity;
                domain.ChangeTaskState(user, code, state);
                domain.DeleteTaskHide(code);
            });
            return Ok(result);
        }
        #endregion

        #region 任务搜索
        /// <summary>
        /// 任务搜索
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="searchText">输入内容</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        public IHttpActionResult TsakSearch(int pageIndex, int pageSize, string searchText)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var domain = new TaskDomain();
                return domain.TaskSearch(pageIndex, pageSize, user.Id, searchText);
            });
            return Ok(result);
        }
        #endregion

        #region 任务列表移除任务
        /// <summary>
        /// 任务列表移除任务
        /// </summary>
        /// <param name="code">任务编码</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        public IHttpActionResult RemoveTask(string code)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var domain = new TaskDomain();
                var currentTaskInfo = domain.IsValidOfTaskCode(code);
                domain.RemoveTask(code, user);
            });
            return Ok(result);
        }
        #endregion

        #region 抄送人员列表
        /// <summary>
        /// 根据任务编码获取抄送人员列表
        /// </summary>
        /// <param name="code">任务编码</param>
        /// <returns></returns>
        public IHttpActionResult GetCopyPersonList(string code)
        {
            var result = ControllerService.Run(() =>
            {
                var domain = new TaskDomain();
                return domain.GetCopyPersonList(code);
            });
            return Ok(result);
        }
        #endregion
    }
}
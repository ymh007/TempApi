using MCS.Library.OGUPermission;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Models.TaskManage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Seagull2.YuanXin.AppApi.Domain.TaskManage.TaskObserver;

namespace Seagull2.YuanXin.AppApi.Domain.TaskManage
{
    /// <summary>
    /// 任务主题
    /// </summary>
    public class TaskSubject : IObserver
    {
        public Action<TaskModel, Seagull2Identity, List<IUser>> AddAction;
        public Action<TaskModel, TaskModel, Seagull2Identity, List<IUser>> EditAction;
        public Action<string, Seagull2Identity, TaskCollection> CancelAction;
        public Action<Seagull2Identity, TaskModel, EnumTaskCompletion> ChangeTaskStateAction;
        /// <summary>
        /// 观察者模式，实现新增逻辑下系统提醒，推送，记录，操作redis
        /// </summary>
        public void Add(TaskModel model, Seagull2Identity user, List<IUser> userList)
        {
            Task.Run(() =>
            {
                var message = new MessageObserver();
                var push = new PushMessageObserver();
                var record = new OperationRecordObserver();
                var redis = new ReidsOperaObserver();
                AddAction += message.Add;
                AddAction += push.Add;
                AddAction += record.Add;
                AddAction += redis.Add;
                AddAction?.Invoke(model, user, userList);
            });
        }

        /// <summary>
        /// 观察者模式，实现新增逻辑下系统提醒，推送，记录，操作redis
        /// </summary>
        public void Edit(TaskModel formerModel, TaskModel currentModel, Seagull2Identity user, List<IUser> userList)
        {
            Task.Run(() =>
            {
                var message = new MessageObserver();
                var push = new PushMessageObserver();
                var record = new OperationRecordObserver();
                var redis = new ReidsOperaObserver();
                EditAction += message.Edit;
                EditAction += push.Edit;
                EditAction += record.Edit;
                EditAction += redis.Edit;
                EditAction?.Invoke(formerModel, currentModel, user, userList);
            });
        }

        /// <summary>
        /// 取消
        /// </summary>
        public void Cancel(string code, Seagull2Identity user, TaskCollection taskColl)
        {
            Task.Run(() =>
            {
                var dataBase = new CancelDataBaseObserver();
                var message = new CancelMessageObserver();
                var push = new CancelPushObserver();
                var redis = new CancelRedisObserver();
                CancelAction += dataBase.Cancel;
                CancelAction += message.Cancel;
                CancelAction += push.Cancel;
                CancelAction += redis.Cancel;
                CancelAction?.Invoke(code, user, taskColl);
            });
        }

        /// <summary>
        /// 修改任务状态
        /// </summary>
        public void ChangeTaskState(Seagull2Identity user, TaskModel model, EnumTaskCompletion state)
        {
            Task.Run(() =>
            {
                var message = new ChangeTaskMessageObserver();
                var push = new ChangeTaskPushObserver();
                var record = new ChangeTaskRecordObserver();
                ChangeTaskStateAction += message.Change;
                ChangeTaskStateAction += push.Change;
                ChangeTaskStateAction += record.Change;
                ChangeTaskStateAction?.Invoke(user, model, state);
            });
        }
    }
}
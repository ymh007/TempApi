using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using log4net;
using MCS.Library.OGUPermission;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Message;
using Seagull2.YuanXin.AppApi.Common;
using Seagull2.YuanXin.AppApi.Models.Message;
using Seagull2.YuanXin.AppApi.ViewsModel.YuanXinOfficeCommon;
using static Seagull2.YuanXin.AppApi.ViewsModel.YuanXinOfficeCommon.YuanXinOfficeCommon;

namespace Seagull2.YuanXin.AppApi.Domain.YuanXinOfficeCommon
{
	/// <summary>
	/// 推送业务类
	/// </summary>
	public class PushDomain
	{
		private static List<IUser> Users;

		static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public void Push(PushMessageBody post)
		{
			try
			{
				var dataFilter = new DataFilter();
				Users = dataFilter.FilterValidUsers(post.UserList); //根据userCode过滤无效人员
				if (Users.Count < 1)
				{
					throw new Exception("无有效的推送人员");
				}
				AbstractFactory stystemPush = new StystemPush();
				AbstractFactory stystemMessage = new StystemMessage();
				if (post.PushType == 0)
				{
					stystemPush.Push(post, Users);
					stystemMessage.Push(post, Users);
					return;
				}
				if (post.PushType == 1)
				{
					stystemPush.Push(post, Users);
					return;
				}
				if (post.PushType == 2)
				{
					stystemMessage.Push(post, Users);
					return;
				}
			}
			catch (Exception ex)
			{
				log.Error(ex.ToString());
			}
		}
	}

	/// <summary>
	/// 系统推送对象 - 系统推送
	/// </summary>
	public class StystemPush : AbstractFactory
	{
		public override void Push(PushMessageBody post, List<IUser> users)
		{
			var pushModelCopy = new PushService.Model()
			{
                BusinessDesc="移动端后台管理",
				Title = post.Title,
				Content = post.Content,
				SendType = PushService.SendType.Person,
				Ids = string.Join(",", users.Select(m => m.ID).ToArray()),
				Extras = new PushService.ModelExtras()
				{
					action = post.MoudleType.ToString(),
                    bags = post.MoudleType == Enum.EnumMessageModuleType.TimeManage ? Newtonsoft.Json.JsonConvert.SerializeObject(new { UserCode = post.Code, CreateTime = DateTime.Now }) : post.Code
				}
			};
			string pushResult = string.Empty;
			bool isPushCopy = PushService.Push(pushModelCopy, out pushResult);
		}
	}

	/// <summary>
	/// 系统提醒对象 - 系统提醒
	/// </summary>
	public class StystemMessage : AbstractFactory
	{
		public override void Push(PushMessageBody post, List<IUser> users)
		{
			var messColl = new MessageCollection();
			users.ForEach(m =>
			{
				messColl.Add(new MessageModel
				{
					Code = Guid.NewGuid().ToString(),
					MeetingCode = post.Code,
					MessageContent = post.Content,
					MessageStatusCode = EnumMessageStatus.New,
					MessageTypeCode = "2",
					MessageTitleCode = post.TitleType,
					ModuleType = post.MoudleType.ToString(),
					Creator = post.Creaotr,
					CreatorName = post.CreatorName,
					ReceivePersonCode = m.ID,
					ReceivePersonName = m.DisplayName,
					ReceivePersonMeetingTypeCode = "",
					OverdueTime = DateTime.Now.AddDays(7),
					ValidStatus = true,
					CreateTime = DateTime.Now
				});

			});
			//系统提醒
			MessageAdapter.Instance.MessageBatchInsert(messColl);
		}
	}

	/// <summary>
	/// 抽象工厂类 提供公共的推送接口
	/// </summary>
	public abstract class AbstractFactory
	{
		public abstract void Push(PushMessageBody post, List<IUser> users);
	}
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MCS.Library.Data.Builder;
using MCS.Library.OGUPermission;
using MCS.Library.WcfExtensions;
using Newtonsoft.Json;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Services;
using Seagull2.YuanXin.AppApi.Services.Meeting;
using SinoOcean.Seagull2.Framework.Contracts;
using SOS2.Meeting.MobileSite.Models;
using MD = SinoOcean.Seagull2.Framework.MasterData;
using MVC = System.Web.Mvc;
using TD = SinoOcean.Seagull2.TransactionData.Meeting;

namespace Seagull2.YuanXin.AppApi.Controllers
{
	/// <summary>
	/// 预定会议室控制器
	/// </summary>
	public class MeetingController : ControllerBase
	{
		public static async Task<Seagull2UserBase> getCurrentUser()
		{
			Seagull2UserBase uu = new Seagull2UserBase();
			return uu;
		}

		public IUser CurrentUserIUser
		{
			get
			{
				MeetingController mc = new MeetingController();
				OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, CurrentUser.LoginName);
				return users[0];
			}
		}

		#region 加载最新一条数据
		/// <summary>
		/// 加载最新一条数据
		/// </summary>
		[HttpGet]
		public Task<IHttpActionResult> LoadByYuanxinCreatorCode()
		{
			TD.Meetings meetings = TD.MeetingAdapter.Instance.LoadByCreatorCode(((Seagull2Identity)User.Identity).Id);
			return Task.FromResult<IHttpActionResult>(Ok(meetings));
		}
		#endregion

		#region 加载部门列表
		/// <summary>
		/// 加载部门列表
		/// </summary>
		[HttpGet]
		public IHttpActionResult Index()
		{
			HelpClass hc = new HelpClass();
			try
			{
				IEnumerable<MVC.SelectListItem> sliIEList = new MeetingRoomUseInfoCollection().CorporationList;
				hc.obj1 = sliIEList;
				IUser cuUser = CurrentUserIUser;
				var index = 0;
				foreach (var cor in sliIEList)
				{
					if (cuUser.Parent.FullPath.Contains(cor.Text))
					{
						hc.obj2 = index;
						break;
					}
					index++;
				}
			}
			catch (Exception e)
			{
				Log.WriteLog(e.Message);
				Log.WriteLog(e.StackTrace);

			}

			return Ok(hc);
		}
		#endregion

		#region 查询会议室
		/// <summary>
		/// 查询会议室
		/// </summary>
		[HttpGet]
		public IHttpActionResult ChangeForm(DateTime searchDate, DateTime searchBeginTime, DateTime searchEndTime, string departmentCode, int pageIndex, int pageSize)
		{
			//会议时间以8:00--20:00为一天
			searchBeginTime = searchDate.AddHours(searchBeginTime.Hour).AddMinutes(searchBeginTime.Minute);
			if (searchBeginTime < searchDate.AddHours(8))
			{
				searchBeginTime = searchDate.AddHours(8);
			}
			searchEndTime = searchDate.AddHours(searchEndTime.Hour).AddMinutes(searchEndTime.Minute);
			if (searchEndTime > searchDate.AddHours(20))
			{
				searchEndTime = searchDate.AddHours(20);
			}
			//搜索相差时间
			TimeSpan ts = searchEndTime.Subtract(searchBeginTime);
			double totalMinutes = ts.TotalMinutes;//720

			yxMeetingRoomCollection mrColl = new yxMeetingRoomCollection();
			try
			{
				mrColl = yxMeetingRoomAdatper.Instance.SearchMyRoomByPageIndexAndTime(departmentCode, pageIndex, pageSize, searchBeginTime, searchEndTime);
				mrColl.ForEach(mr =>
				{
					mr.Remark = ((totalMinutes - mr.mtRoomTime) / 60).ToString("0.0");//-390
					if (mr.mtRoomTime >= totalMinutes)
					{
						mr.Remark = "忙碌中";
					}
					else if (mr.mtRoomTime == 0)
					{
						mr.Remark = "空闲中";
					}
					else
					{
						mr.Remark = "空闲" + mr.Remark + "小时";
					}
					mr.Meeting_Equipment.ForEach(me =>
					{
						if (me.MeetingSupplyTypeCode == MD.MeetingSupplyTypeEnum.Flower)
						{
							me.MeetingCode = "zhuohua.png";
						}
						else if (me.MeetingSupplyTypeCode == MD.MeetingSupplyTypeEnum.Fruit)
						{
							me.MeetingCode = "shuiguo.png";
						}
						else if (me.MeetingSupplyTypeCode == MD.MeetingSupplyTypeEnum.Phone)
						{
							me.MeetingCode = "dianhua.png";
						}
						else if (me.MeetingSupplyTypeCode == MD.MeetingSupplyTypeEnum.Projector)
						{
							me.MeetingCode = "touying.png";
						}
						else if (me.MeetingSupplyTypeCode == MD.MeetingSupplyTypeEnum.Video)
						{
							me.MeetingCode = "shipin.png";
						}
					});
				});
			}
			catch (Exception e)
			{
				Log.WriteLog(e.Message);
				Log.WriteLog(e.StackTrace);
			}
			return Ok(mrColl);
		}
		#endregion

		#region 开始预定初始化数据加载
		/// <summary>
		/// 开始预定初始化数据加载
		/// </summary>
		[HttpGet]
		public async Task<IHttpActionResult> BeginSchedule()
		{
			HelpClass helpClass = new HelpClass();
			//记账公司
			MD.NcCorporationCollection ncCorporationColl = MD.NcCorporationAdapter.Instance.GetNcCorporations();
			helpClass.obj1 = await getCurrentUser();

			IUser cuUser = CurrentUserIUser;


			var list = MeetingsYuanXinAdapter.LoadByCreaterAndCnCode(cuUser.ID).ToList();
			var code = list[0].NcCompanyCode;

			for (int i = 0; i < ncCorporationColl.Count; i++)
			{
				if (code == ncCorporationColl[i].Code)
				{
					helpClass.obj4 = i;
				}
			}

			helpClass.obj2 = ncCorporationColl;
			helpClass.obj3 = MD.CostCenterEntityAdapter.Instance.GetCostCentersByNcCorp(ncCorporationColl[helpClass.obj4].Code);

			return Ok(helpClass);
		}
		#endregion

		#region 人员查询
		/// <summary>
		/// 人员查询（单人支持模糊查询，最多展示10条）
		/// </summary>
		/// <param name="loginName">登录名</param>
		[HttpGet]
		public IHttpActionResult userCheck(string loginName)
		{
			List<Seagull2UserBase> userList = new List<Seagull2UserBase>();
			UserCollection ucColl = UserAdapter.getUserByLoginNameOrDisplayName(loginName.Trim());
			for (int i = 0; i < ucColl.Count; i++)
			{
				if (userList.Count < 10)
				{
					userList.Add(new Seagull2UserBase { Id = ucColl[i].Code, LogonName = ucColl[i].LogonName, DisplayName = ucColl[i].DisplayName, FullPath = ucColl[i].All_Path_Name });
				}
			}
			return Ok(userList);
		}
		#endregion

		#region 预定会议室
		/// <summary>
		/// 预定会议室
		/// </summary>
		[HttpPost]
		public Task<IHttpActionResult> SubmitMeeting(SubmitMeetingPost post)
		{
			try
			{
				log.Info("预定会议室：" + JsonConvert.SerializeObject(post));

				var user = Adapter.AddressBook.ContactsAdapter.Instance.LoadByCode(((Seagull2Identity)User.Identity).Id);

				string serviceUrl = ConfigurationManager.AppSettings["LaunchMeetingService"];

				var endpoint = new EndpointAddress(serviceUrl);

				using (var factory = new WfClientChannelFactory<ILaunchMeeting>(endpoint))
				{
					#region[会议信息]
					MeetingServiceContract launchMeetings = new MeetingServiceContract();
					launchMeetings.ID = Guid.NewGuid().ToString();
					launchMeetings.creatorID = user.ObjectID;
					launchMeetings.creatorName = user.DisplayName;
					launchMeetings.Subject = post.meetName;
					launchMeetings.MeetingType = MD.MeetingTypeEnum.Manager;
					launchMeetings.StartTime = Convert.ToDateTime(post.searchDate + " " + post.searchBeginTimeStr + ":00");
					launchMeetings.EndTime = Convert.ToDateTime(post.searchDate + " " + post.searchEndTimeStr + ":00");
					launchMeetings.Importance = MD.MeetingImportanceEnum.Ordinary;
					launchMeetings.IfMeetingMinutes = false;
					launchMeetings.CreateTime = DateTime.Now;
					launchMeetings.ModeratorID = post.zhchrUserCode;    //会议主持人
					launchMeetings.ContactPersonID = user.ObjectID;     //会议联系人
					launchMeetings.ServerCode = post.fuwuUserCode;      //会议服务人
					launchMeetings.ORGID = user.ParentID;               //会议发起人业务组织机构
					launchMeetings.MeetingCompanyCode = post.meetCompanyCode;       //会议公司
					launchMeetings.MeetingRoomLocation = post.meetRoomName;     //会议地点
					launchMeetings.MeetingRoomLocationCode = post.meetRoomCode; //会议室code
					#endregion

					#region[参会人信息]
					launchMeetings.MeetingMenCodeList = post.canhuiUserCodeColl;
					#endregion

					#region[会议议题]
					launchMeetings.MeetingTopic = post.meetContent;
					#endregion

					#region[会议提醒方式]
					launchMeetings.MeetingsRemindWayList = ((int)MD.MeetingRemindWayEnum.Message).ToString() + ";" + ((int)MD.MeetingRemindWayEnum.Email).ToString() + ";";
					#endregion

					#region[会议使用设备]
					launchMeetings.meetingSupplyTypesList = string.Empty;
					#endregion

					//launchMeetings.CostCenterCode = post.costCenterCode;
					//launchMeetings.NCCompanyCode = post.companyCode;
					launchMeetings.ContactPersonPhoneCode = user.MP;
					launchMeetings.SupplyRemark = string.Empty;
					launchMeetings.Remark = string.Empty;

					ILaunchMeeting service = factory.CreateChannel();
					var tasks = new List<Task>();
					tasks.Add(Task.Run(() =>
					{
						LaunchMeetingReturn _LaunchMeetingReturn = service.GetMeetingService(launchMeetings);
						log.Info("预定会议室：" + JsonConvert.SerializeObject(_LaunchMeetingReturn));
					}));
					tasks.Add(Task.Run(() =>
					{
						#region [添加消息提醒]
						try
						{
							MessageForMeetingYuanXin ms = new MessageForMeetingYuanXin();
							ms.Creator = user.ObjectID;
							ms.CreatorName = user.DisplayName;
							ms.MeetingCode = launchMeetings.ID;
							ms.MessageContent = string.Format("您 {0}至{1}在{2}有主题为“{3}”的会议，请准时参加。",
														launchMeetings.StartTime.ToString("yyyy-MM-dd HH:mm"),
														launchMeetings.EndTime.ToString("yyyy-MM-dd HH:mm"),
														launchMeetings.MeetingRoomLocation,
														launchMeetings.Subject);
							ms.OverdueTime = launchMeetings.StartTime;
							var userCodeList = MeetingService.GetPeople(launchMeetings);
							var people = new List<MessageForMeetingYuanXinPeople>();
							foreach (string code in userCodeList)
							{
								people.Add(new MessageForMeetingYuanXinPeople() { UserCode = code });
							}
							ms.ReceivePerson = people;
							ms.MessageTitleCode = EnumMessageTitle.MeetingRequest;
							Adapter.Message.MessageAdapter.Instance.AddMessage(ms);
						}
						catch (Exception e)
						{
							log.Error("发送提醒消息异常：" + e.ToString());
						}
						#endregion
					}));
					Task.WaitAll(tasks.ToArray());
					return Task.FromResult<IHttpActionResult>(Ok(new ViewsModel.ViewModelBaseNull()
					{
						State = true,
						Message = "会议室预订成功！"
					}));
				}
			}
			catch (Exception e)
			{
				log.Error("创建预定会议室信息失败：" + e.ToString());
				return Task.FromResult<IHttpActionResult>(Ok(new ViewsModel.ViewModelBaseNull()
				{
					State = false,
					Message = e.Message
				}));
			}
		}
		/// <summary>
		/// 预定会议室POST参数类
		/// </summary>
		public class SubmitMeetingPost
		{
			/// <summary>
			/// 会议标题
			/// </summary>
			public string meetName { get; set; }
			/// <summary>
			/// 会议日期
			/// </summary>
			public string searchDate { get; set; }
			/// <summary>
			/// 开始时间
			/// </summary>
			public string searchBeginTimeStr { get; set; }
			/// <summary>
			/// 结束时间
			/// </summary>
			public string searchEndTimeStr { get; set; }
			/// <summary>
			/// 会议公司编码
			/// </summary>
			public string meetCompanyCode { set; get; }
			/// <summary>
			/// 会议室编码
			/// </summary>
			public string meetRoomCode { get; set; }
			/// <summary>
			/// 会议室名称
			/// </summary>
			public string meetRoomName { set; get; }
			/// <summary>
			/// 主持人编码
			/// </summary>
			public string zhchrUserCode { get; set; }
			/// <summary>
			/// 服务人编码
			/// </summary>
			public string fuwuUserCode { get; set; }
			/// <summary>
			/// 参会人编码列表（Code1; Code2; Code3; ...）
			/// </summary>
			public string canhuiUserCodeColl { get; set; }
			/// <summary>
			/// 会议内容
			/// </summary>
			public string meetContent { get; set; }
		}
		#endregion

		#region 选择会议室时间段请求
		/// <summary>
		/// 选择会议室时间段请求
		/// </summary>
		[HttpGet]
		public IHttpActionResult selectMeetingRoomSubmit(string meetingRoomCode, DateTime beginTime, DateTime endTime)
		{
			TD.MeetingsCollection searchMeetingColl = MeetingsYuanXinAdapter.getAllByRoomCodeAndBetweenTime(meetingRoomCode, beginTime, endTime);
			DateTime date = DateTime.Now;
			if (date < beginTime)
			{
				date = beginTime.AddHours(8);
			}
			List<SelectMeetingClass> meetingTimeColl = new List<SelectMeetingClass>();
			searchMeetingColl.ForEach(m =>
			{

				#region 当前元素有跨天会议预定
				if (DateTime.Compare(m.StartTime.Date, m.EndTime.Date) != 0)
				{
					#region 当前服务器时间大于数据开始时间小于结束时间
					if (date > m.StartTime && date < m.EndTime)
					{
						#region 查询结束时间大于数据结束时间
						if (endTime > m.EndTime)
						{
							if (date.Minute > 30)
							{
								meetingTimeColl.Add(new SelectMeetingClass
								{
									startTime = date.Date.AddHours(date.Hour).AddMinutes(30).ToString("yyyy-MM-dd HH:mm"),
									endTime = date.Date.ToString("yyyy-MM-dd 20:00"),
									isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
								});
							}
							else
							{
								if (DateTime.Compare(date.Date, m.EndTime.Date) != 0)
								{
									meetingTimeColl.Add(new SelectMeetingClass
									{
										startTime = date.Date.AddHours(date.Hour).ToString("yyyy-MM-dd HH:mm"),
										endTime = date.Date.ToString("yyyy-MM-dd 20:00"),
										isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
									});
								}
							}
							if ((m.EndTime - m.StartTime).Days >= 2 && m.StartTime < m.EndTime)
							{
								for (int i = 1; i < (m.EndTime.Date - date.Date).Days; i++)
								{
									meetingTimeColl.Add(new SelectMeetingClass
									{
										startTime = date.AddDays(i).ToString("yyyy-MM-dd 8:00"),
										endTime = date.AddDays(i).ToString("yyyy-MM-dd 20:00"),
										isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
									});
								}
							}

							meetingTimeColl.Add(new SelectMeetingClass
							{
								startTime = m.EndTime.ToString("yyyy-MM-dd 8:00"),
								endTime = m.EndTime.ToString("yyyy-MM-dd HH:mm"),
								isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
							});

						}
						#endregion

						#region 查询结束时间小于数据结束时间
						else
						{
							if (date.Minute > 30)
							{
								meetingTimeColl.Add(new SelectMeetingClass
								{
									startTime = date.Date.AddHours(date.Hour).AddMinutes(30).ToString("yyyy-MM-dd HH:mm"),
									endTime = date.Date.ToString("yyyy-MM-dd 20:00"),
									isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
								});
							}
							else
							{
								meetingTimeColl.Add(new SelectMeetingClass
								{
									startTime = date.Date.AddHours(date.Hour).ToString("yyyy-MM-dd HH:mm"),
									endTime = date.Date.ToString("yyyy-MM-dd 20:00"),
									isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
								});
							}
							if ((endTime.Date - date.Date).Days >= 2)
							{
								for (int i = 1; i < (endTime.Date - date.Date).Days; i++)
								{
									meetingTimeColl.Add(new SelectMeetingClass
									{
										startTime = date.AddDays(i).ToString("yyyy-MM-dd 8:00"),
										endTime = date.AddDays(i).ToString("yyyy-MM-dd 20:00"),
										isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
									});
								}
							}
							meetingTimeColl.Add(new SelectMeetingClass
							{
								startTime = endTime.ToString("yyyy-MM-dd 8:00"),
								endTime = endTime.ToString("yyyy-MM-dd 20:00"),
								isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
							});
						}
						#endregion

					}
					#endregion

					#region 当前服务器时间小于数据开始时间
					if (date < m.StartTime)
					{
						//判断查询结束时间是否小于数据结束时间
						if (endTime > m.EndTime)
						{
							meetingTimeColl.Add(new SelectMeetingClass
							{
								startTime = m.StartTime.ToString("yyyy-MM-dd  HH:mm"),
								endTime = m.StartTime.ToString("yyyy-MM-dd 20:00"),
								isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
							});
							if ((m.EndTime - m.StartTime).Days >= 2)
							{
								for (int i = 1; i < (m.EndTime - m.StartTime).Days; i++)
								{
									meetingTimeColl.Add(new SelectMeetingClass
									{
										startTime = m.StartTime.AddDays(i).ToString("yyyy-MM-dd 8:00"),
										endTime = m.StartTime.AddDays(i).ToString("yyyy-MM-dd 20:00"),
										isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
									});
								}
							}
							meetingTimeColl.Add(new SelectMeetingClass
							{
								startTime = m.EndTime.ToString("yyyy-MM-dd 8:00"),
								endTime = m.EndTime.ToString("yyyy-MM-dd HH:mm"),
								isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
							});
						}
						else
						{
							meetingTimeColl.Add(new SelectMeetingClass
							{
								startTime = m.StartTime.ToString("yyyy-MM-dd  HH:mm"),
								endTime = m.StartTime.ToString("yyyy-MM-dd 20:00"),
								isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
							});
							if ((endTime.Date - m.StartTime.Date).Days >= 2)
							{
								for (int i = 1; i < (endTime.Date - date.Date).Days; i++)
								{
									meetingTimeColl.Add(new SelectMeetingClass
									{
										startTime = m.StartTime.AddDays(i).ToString("yyyy-MM-dd 8:00"),
										endTime = m.StartTime.AddDays(i).ToString("yyyy-MM-dd 20:00"),
										isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
									});
								}
							}
							meetingTimeColl.Add(new SelectMeetingClass
							{
								startTime = endTime.ToString("yyyy-MM-dd 8:00"),
								endTime = endTime.ToString("yyyy-MM-dd 20:00"),
								isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
							});
						}
					}
					#endregion
				}
				#endregion

				#region 当前会议预定没有跨天预定
				else
				{
					#region 当前服务器时间大于数据开始时间小于结束时间
					if (date > m.StartTime && date < m.EndTime)
					{
						if (date.Minute > 30)
						{
							meetingTimeColl.Add(new SelectMeetingClass
							{
								startTime = date.Date.AddHours(date.Hour).AddMinutes(30).ToString(),
								endTime = m.EndTime.ToString("yyyy-MM-dd HH:mm"),
								isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
							});
						}
						else
						{
							meetingTimeColl.Add(new SelectMeetingClass
							{
								startTime = date.Date.AddHours(date.Hour).ToString(),
								endTime = m.EndTime.ToString("yyyy-MM-dd HH:mm"),
								isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
							});
						}
					}
					#endregion

					#region 当前服务器时间小于数据开始时间且查询EndTime大于StartTime
					if (date < m.StartTime && endTime > m.StartTime)
					{
						meetingTimeColl.Add(new SelectMeetingClass
						{
							startTime = m.StartTime.ToString("yyyy-MM-dd HH:mm"),
							endTime = m.EndTime.ToString("yyyy-MM-dd HH:mm"),
							isMyMeeting = m.Creator.LogOnName == User.Identity.Name ? true : false
						});

					}
					#endregion
				}
				#endregion
			});
			return Ok(meetingTimeColl);
		}
		#endregion

		#region 查询我某天的会议
		/// <summary>
		/// 查询我某天的会议
		/// </summary>
		[HttpGet]
		public IHttpActionResult MyMeeting(DateTime currentTime)
		{
			try
			{
				var myCode = ((Seagull2Identity)User.Identity).Id;
				string startTime = currentTime.ToString("yyyy-MM-dd 00:00:00");
				string endTime = currentTime.ToString("yyyy-MM-dd 23:59:59");

				WhereSqlClauseBuilder whereAnd = new WhereSqlClauseBuilder();
				whereAnd.AppendItem("s.StartTime", startTime, ">=");
				whereAnd.AppendItem("s.EndTime", endTime, "<=");

				//查询当天所有的会议
				TD.MeetingsCollection meetings = TD.MeetingAdapter.Instance.loadByWhereName(whereAnd.ToSqlString(TSqlBuilder.Instance));

				//返回我的会议
				TD.MeetingsCollection meetingsCollection = new TD.MeetingsCollection();
				foreach (TD.Meetings items in meetings)
				{
					if (items.Moderator.ID == myCode)
					{
						//会议主持人
						meetingsCollection.Add(items);
					}
					else if (items.ContactPerson.ID == myCode)
					{
						//会议联系人(预订人)
						meetingsCollection.Add(items);
					}
					else
					{
						//参会人
						foreach (TD.MeetingInvitedPeople server in items.MeetingMen)
						{
							if (server.AttendeeCode == myCode)
							{
								meetingsCollection.Add(items);
							}
						}
					}
				}

				/*
                var view = new ViewsModel.Meeting.MeetingModel();
                foreach (var item in meetingsCollection)
                {
                    view.MeetingCode = item.ID;
                    view.Subject = item.Subject;
                    view.Content = item.MeetingTopics[0].CnName;
                    view.StartTime = item.StartTime.ToString("yyyy-MM-dd HH:mm");
                    view.EndTime = item.EndTime.ToString("yyyy-MM-dd HH:mm");
                    view.CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm");
                    view.CreatorCode = item.Creator.ID;
                    view.CreatorName = item.Creator.DisplayName;
                    view.MeetingRoomName = item.MeetingRooms[0].MeetingRoomName;
                    view.MeetingMan=
                }*/

				log.Info(JsonConvert.SerializeObject(meetingsCollection));

				return Ok(new ViewsModel.ViewModelBaseList() { State = true, Message = "success.", Data = meetingsCollection });
				//return Ok(meetingsCollection);
			}
			catch (Exception e)
			{
				log.Error("获取我的会议异常：" + e);
				return Ok(new ViewsModel.ViewModelBaseNull() { State = false, Message = e.Message });
			}
		}
		#endregion

		#region 取最新一条数据的CreatorDepartment
		/// <summary>
		/// 取最新一条数据的CreatorDepartment
		/// </summary>
		[HttpGet]
		public Task<IHttpActionResult> LoadByCreatorCode()
		{
			TD.Meetings meetings = TD.MeetingAdapter.Instance.LoadByCreatorCode(CurrentUserCode);
			return Task.FromResult<IHttpActionResult>(Ok(meetings));
		}
		#endregion

		#region 根据Code查询会议详情
		/// <summary>
		/// 根据Code查询会议详情
		/// </summary>
		[HttpGet]
		public IHttpActionResult MeetingInformation(string code)
		{
			try
			{
				MeetingServiceContract meetings = MeetingService.GetByCode(code);
				if (meetings == null)
				{
					log.Error("根据Code查询会议详情异常：没有获取到预定信息。");
					return Ok(new ViewsModel.ViewModelBaseNull() { State = false, Message = "没有获取到预定信息" });
				}

				//所有人员（创建人、主持人、服务人、参会人）
				var usersCode = MeetingService.GetPeople(meetings);
				var users = new List<ViewsModel.Meeting.MeetingModel.Man>();
				foreach (string userCode in usersCode)
				{
					users.Add(new ViewsModel.Meeting.MeetingModel.Man() { Code = userCode });
				}

				var view = new ViewsModel.Meeting.MeetingModel()
				{
					MeetingCode = meetings.ID,
					Subject = meetings.Subject,
					Content = meetings.MeetingTopic,
					StartTime = meetings.StartTime.ToString("yyyy-MM-dd HH:mm"),
					EndTime = meetings.EndTime.ToString("yyyy-MM-dd HH:mm"),
					CreateTime = meetings.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
					CreatorCode = meetings.creatorID,
					CreatorName = meetings.creatorName,
					MeetingRoomName = meetings.MeetingRoomLocation,
					MeetingMan = users
				};

				return Ok(new ViewsModel.ViewModelBaseList() { State = true, Message = "success", Data = view });
			}
			catch (Exception e)
			{
				log.Error("根据Code查询会议详情异常：" + e);
				return Ok(new ViewsModel.ViewModelBaseNull() { State = false, Message = e.Message });
			}
		}
		#endregion

		#region 取消会议室预定
		/// <summary>
		/// 取消会议室预定
		/// </summary>
		[HttpGet]
		public Task<IHttpActionResult> DeleteYuanXinMeetingsByCode(string code)
		{
			try
			{
				string serviceUrl = ConfigurationManager.AppSettings["LaunchMeetingService"];

				var endpoint = new EndpointAddress(serviceUrl);
				using (var factory = new WfClientChannelFactory<ILaunchMeeting>(endpoint))
				{
					ILaunchMeeting service = factory.CreateChannel();
					LaunchMeetingReturn _LaunchMeetingReturn = service.DeleteMeetingService(code);
					if (_LaunchMeetingReturn.isSuccess)
					{
						return Task.FromResult<IHttpActionResult>(Ok(new ViewsModel.ViewModelBaseNull()
						{
							State = true,
							Message = "取消会议预定成功！"
						}));
					}
					else
					{
						return Task.FromResult<IHttpActionResult>(Ok(new ViewsModel.ViewModelBaseNull()
						{
							State = false,
							Message = _LaunchMeetingReturn.ValidateInfo
						}));
					}
				}
			}
			catch (Exception e)
			{

				log.Error("取消会议室预定失败：" + e.ToString());
				return Task.FromResult<IHttpActionResult>(Ok(new ViewsModel.ViewModelBaseNull()
				{
					State = false,
					Message = e.Message
				}));
			}
		}
		#endregion

		#region 获取默认的NC公司
		/// <summary>
		/// 获取默认的NC公司
		/// </summary>
		[HttpGet]
		public Task<IHttpActionResult> LoadDefaultCompany()
		{
			try
			{
				string fileds = "NcCompanyCode";
				List<string> Nccode = TD.MeetingAdapter.Instance.LoadByCreatorCode2(CurrentUserCode, fileds);
				MD.NcCorporationCollection ncCorporationColl = new MD.NcCorporationCollection();
				if (Nccode.Count != 0)
				{
					foreach (string item in Nccode)
					{
						if (item != "undefined")
						{
							if (ncCorporationColl.Count <= 4)
							{
								MD.NcCorporation ncCorporation = MD.NcCorporationAdapter.Instance.GetNcCorporation(item);
								ncCorporationColl.Add(ncCorporation);
							}
						}
					}
				}
				return Task.FromResult<IHttpActionResult>(Ok(ncCorporationColl));
			}
			catch (Exception ex)
			{

				Log.WriteLog(ex.Message);
				Log.WriteLog(ex.StackTrace);
				throw ex;
			}
		}
		#endregion

		#region 获取所有的NC公司
		/// <summary>
		/// 获取所有的NC公司
		/// </summary>
		[HttpGet]
		public Task<IHttpActionResult> LoadNcCompany()
		{
			try
			{
				MD.NcCorporationCollection ncCorporationColl = MD.NcCorporationAdapter.Instance.GetNcCorporations();
				return Task.FromResult<IHttpActionResult>(Ok(ncCorporationColl));
			}
			catch (Exception ex)
			{

				Log.WriteLog(ex.Message);
				Log.WriteLog(ex.StackTrace);
				throw ex;
			}
		}
		#endregion

		#region 根据记账公司获取成本中心集合
		/// <summary>
		/// 根据记账公司获取成本中心集合
		/// </summary>
		[HttpGet]
		public Task<IHttpActionResult> GetCostCenterEntityColl()
		{
			try
			{
				string ncCorporationCode = HttpContext.Current.Request["ncCorporationCode"];
				//成本中心
				IList<MD.CostCenterEntity> cList = MD.CostCenterEntityAdapter.Instance.GetCostCentersByNcCorp(ncCorporationCode);
				return Task.FromResult<IHttpActionResult>(Ok(cList));
			}
			catch (Exception ex)
			{

				Log.WriteLog(ex.Message);
				Log.WriteLog(ex.StackTrace);
				throw ex;
			}
		}
		#endregion

		#region 获取默认的成本中心
		/// <summary>
		/// 获取默认的成本中心
		/// </summary>
		[HttpGet]
		public Task<IHttpActionResult> LoadGetCostCenterMeeting()
		{
			try
			{
				string ncCorporationCode = HttpContext.Current.Request["ncCorporationCode"];
				string fileds = string.Format("CostCenterCode");
				List<string> Nccode = TD.MeetingAdapter.Instance.LoadByCreatorCode2(CurrentUserCode, fileds, ncCorporationCode);

				MD.CostCenterEntityCollection costCenterEntityCollection = new MD.CostCenterEntityCollection();

				if (Nccode.Count != 0)
				{
					foreach (string item in Nccode)
					{
						if (item != "undefined")
						{
							if (costCenterEntityCollection.Count <= 4)
							{
								MD.CostCenterEntity costCenterEntity = MD.CostCenterEntityAdapter.Instance.GetCostCenter(item);
								costCenterEntityCollection.Add(costCenterEntity);
							}
						}
					}
				}
				return Task.FromResult<IHttpActionResult>(Ok(costCenterEntityCollection));
			}
			catch (Exception ex)
			{

				Log.WriteLog(ex.Message);
				Log.WriteLog(ex.StackTrace);
				throw ex;
			}
		}
		#endregion

		#region getMondayTime
		/// <summary>
		/// getMondayTime
		/// </summary>
		[HttpGet]
		public IHttpActionResult getMondayTime()
		{
			DateTime dt = DateTime.Now;
			DateTime startWeek = dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d"))); //本周周一
			var startMonday = startWeek.ToString("yyyy-MM-dd 01:00");
			List<SelectMeetingClass> meetingTimeColl = new List<SelectMeetingClass>();
			meetingTimeColl.Add(new SelectMeetingClass
			{
				mondayTime = startMonday
			});
			return Ok(meetingTimeColl);
		}
		#endregion

		#region 辅助类
		public class HelpClass
		{
			public object obj1 { get; set; }
			public object obj2 { get; set; }
			public object obj3 { get; set; }
			public int obj4 { get; set; }
		}



		public class SelectMeetingClass
		{
			public string startTime { get; set; }
			public string endTime { get; set; }

			public string mondayTime { get; set; }
			public bool isMyMeeting { get; set; }
		}
		#endregion
	}
}
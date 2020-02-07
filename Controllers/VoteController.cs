using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.OGUPermission;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Message;
using Seagull2.YuanXin.AppApi.Adapter.Vote;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Models.Message;
using Seagull2.YuanXin.AppApi.Models.Vote;
using Seagull2.YuanXin.AppApi.ViewsModel.Vote;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 投票问卷 Controller
    /// </summary>
    public class VoteController : ApiController
    {
        #region 创建投票问卷
        /// <summary>
        /// 创建投票问卷
        /// </summary>
        [HttpPost]
        public IHttpActionResult CreateVote(CreateViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                var identity = (Seagull2Identity)User.Identity;

                var voteInfo = new VoteInfoModel();
                var quesColl = new VoteQuestionCollcetion();
                var optionColl = new VoteOptionCollection();

                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    // 基本信息
                    voteInfo = new VoteInfoModel
                    {
                        Code = Guid.NewGuid().ToString(),
                        Title = model.VoteInfo.Title,
                        Describe = model.VoteInfo.Describe,
                        EndTime = model.VoteInfo.EndTime,
                        IsShowPoll = model.VoteInfo.IsShowPoll,
                        IsShowResult = model.VoteInfo.IsShowResult,
                        VoteType = model.VoteInfo.VoteType,
                        Creator = identity.Id,
                        CreateTime = DateTime.Now,
                        Modifier = identity.Id,
                        ModifyTime = DateTime.Now,
                        ValidStatus = true
                    };
                    VoteInfoAdapter.Instance.Update(voteInfo);

                    // 人员范围
                    VotePersonAdapter.Instance.BatchInsert(model.Person, voteInfo.Code, identity.Id);

                    // 管理员
                    model.ManagerList?.ForEach(item =>
                    {
                        VoteManagerAdapter.Instance.Add(voteInfo.Code, item.ManagerCode, item.ManagerDisplayName, identity.Id);
                    });

                    // 题目
                    model.Question.ForEach(m =>
                    {
                        var VoteQuestion = new VoteQuestionModel
                        {
                            Code = Guid.NewGuid().ToString(),
                            VoteCode = voteInfo.Code,
                            Title = m.Title,
                            QuestionType = m.QuestionType,
                            MinChoice = m.MinChoice,
                            MaxChoice = m.MaxChoice,
                            Sort = m.Sort,
                            Creator = identity.Id,
                            CreateTime = DateTime.Now,
                            Modifier = identity.Id,
                            ModifyTime = DateTime.Now,
                            ValidStatus = true
                        };
                        quesColl.Add(VoteQuestion);

                        // 选项
                        m.Option?.ForEach(o =>
                        {
                            var VoteOption = new VoteOptionModel
                            {
                                VoteCode = voteInfo.Code,
                                Code = Guid.NewGuid().ToString(),
                                QuestionCode = VoteQuestion.Code,
                                Name = o.Name,
                                Sort = o.Sort,
                                IsFill = o.IsFill,
                                Creator = identity.Id,
                                CreateTime = DateTime.Now,
                                Modifier = identity.Id,
                                ModifyTime = DateTime.Now,
                                ValidStatus = true
                            };
                            optionColl.Add(VoteOption);
                        });
                    });

                    //批量插入问题
                    VoteQuestionAdapter.Instance.BatchInsert(quesColl);

                    //批量插入选项
                    VoteOptionAdapter.Instance.BatchInsert(optionColl);

                    //事务提交
                    scope.Complete();
                }

                #region 消息提醒
                Task.Run(() =>
                {
                    try
                    {
                        var messageColl = new MessageCollection();
                        foreach (var items in model.Person)
                        {
                            var message = new MessageModel()
                            {
                                Code = Guid.NewGuid().ToString(),
                                MeetingCode = voteInfo.Code,
                                MessageContent = model.VoteInfo.VoteType == 0 ? "收到一个新的投票，点击进入详情" : "收到一个新的问卷，点击进入详情",
                                MessageStatusCode = EnumMessageStatus.New,
                                MessageTypeCode = "2",
                                MessageTitleCode = EnumMessageTitle.System,
                                ModuleType = EnumMessageModuleType.Vote.ToString(),
                                Creator = identity.Id,
                                CreatorName = identity.DisplayName,
                                ReceivePersonCode = items.UserCode,
                                ReceivePersonName = items.UserName,
                                ReceivePersonMeetingTypeCode = "",
                                OverdueTime = DateTime.Now.AddDays(7),
                                ValidStatus = true,
                                CreateTime = DateTime.Now
                            };
                            messageColl.Add(message);
                        }
                        MessageAdapter.Instance.BatchInsert(messageColl);
                    }
                    catch (Exception e)
                    {
                        Log.WriteLog(e.Message);
                    }
                });
                #endregion

                #region 推送
                Task.Run(() =>
                {
                    try
                    {
                        var votePerson = new PushService.Model()
                        {
                            BusinessDesc= "创建投票问卷",
                            Title = "投票问卷",
                            Content = model.VoteInfo.VoteType == 1 ? "收到一个新的投票，点击进入详情" : "收到一个新的问卷，点击进入详情",
                            SendType = PushService.SendType.Person,
                            Ids = string.Join(",", model.Person.Select(m => m.UserCode)),
                            Extras = new PushService.ModelExtras()
                            {
                                action = "Vote",
                                bags = voteInfo.Code
                            }
                        };
                        PushService.Push(votePerson, out string pushResult);
                    }
                    catch (Exception e)
                    {
                        Log.WriteLog(e.Message);
                    }
                });
                #endregion
            });
            return Ok(result);
        }
        #endregion

        #region 提交投票问卷结果
        /// <summary>
        /// 提交投票问卷结果
        /// </summary>
        [HttpPost]
        public IHttpActionResult CastVote(CastVoteViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                #region 数据校验
                if (string.IsNullOrEmpty(model.VoteCode) || model.CastQuestion == null)
                {
                    throw new Exception("参数不能为空");
                }

                var user = (Seagull2Identity)User.Identity;
                var personColl = VotePersonAdapter.Instance.Load(m => m.AppendItem("UserCode", user.Id).AppendItem("VoteCode", model.VoteCode));
                if (personColl.Count == 0)
                {
                    throw new Exception("您的投票信息不存在");
                }

                var info = VoteInfoAdapter.Instance.Load(m => { m.AppendItem("Code", model.VoteCode); }).FirstOrDefault();
                if (info == null)
                {
                    throw new Exception("无效的VoteCode");
                }

                if (info.EndTime < DateTime.Now)
                {
                    throw new Exception("当前投票已经过期");
                }
                #endregion

                #region 判断是否已投

                var record = VoteRecordAdapter.Instance.Load(m => m.AppendItem("VoteCode", model.VoteCode).AppendItem("Creator", user.Id)).FirstOrDefault();
                if (record != null) { throw new Exception("当前用户已经投过票"); }
                #endregion

                #region 保存投票
                List<VoteRecordModel> list = new List<VoteRecordModel>();
                model.CastQuestion.ForEach(m =>
                {
                    m.CastOption.ForEach(i =>
                    {
                        list.Add(new VoteRecordModel
                        {
                            Code = Guid.NewGuid().ToString(),
                            VoteCode = model.VoteCode,
                            QuestionCode = m.Code,
                            OptionCode = i.OptionCode,
                            FillContent = i.FillContent,
                            UserName = user.DisplayName,
                            Creator = user.Id,
                            CreateTime = DateTime.Now,
                            Modifier = user.Id,
                            ModifyTime = DateTime.Now,
                            ValidStatus = true
                        });
                    });
                });
                VoteRecordAdapter.Instance.BatchInsertVote(list);
                #endregion
            });
            return Ok(result);
        }
        #endregion

        #region 获取我创建的列表
        /// <summary>
        /// 获取我创建的列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetVoteListByCtreated(int pageIndex, int pageSize)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var table = VoteInfoAdapter.Instance.GetCreatedList(pageIndex, pageSize, user.Id);
                var list = DataConvertHelper<VoteListByCreatedViewModel>.ConvertToList(table);
                if (list.Count > 0)
                {
                    var persons = VotePersonAdapter.Instance.Load(m => m.AppendItem("VoteCode", list.FirstOrDefault().Code));
                    list.ForEach(m =>
                    {
                        m.IsPerson = persons.FindAll(o => o.Creator == user.Id).Count > 0 ? true : false;
                    });
                }
                return list;
            });
            return Ok(result);
        }
        #endregion

        #region 获取我参与的列表
        /// <summary>
        /// 获取我参与的列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetVoteListByParticipates(int pageIndex, int pageSize)
        {
            var result = ControllerService.Run(() =>
            {
                var identity = (Seagull2Identity)User.Identity;

                var table = VoteInfoAdapter.Instance.GetParticipatesList(pageIndex, pageSize, identity.Id);

                return DataConvertHelper<VoteListByApplyViewModel>.ConvertToList(table);
            });
            return Ok(result);
        }
        #endregion

        #region 投票问卷详情
        /// <summary>
        /// 投票问卷详情
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetVoteDetails(string voteCode)
        {
            var result = ControllerService.Run(() =>
            {
                var identity = (Seagull2Identity)User.Identity;

                var info = VoteInfoAdapter.Instance.Load(m => m.AppendItem("Code", voteCode)).FirstOrDefault();
                if (info == null)
                {
                    throw new Exception("投票问卷信息不存在！");
                }
                if (info.Creator != identity.Id)
                {
                    var person = VotePersonAdapter.Instance.Load(m => m.AppendItem("UserCode", identity.Id).AppendItem("VoteCode", info.Code));
                    var manager = VoteManagerAdapter.Instance.Load(w => w.AppendItem("ManagerCode", identity.Id).AppendItem("VoteCode", info.Code));
                    if (person.Count <= 0 && manager.Count <= 0)
                    {
                        throw new Exception("您没有权限访问此内容！");
                    }
                }

                var question = VoteQuestionAdapter.Instance.Load(m => m.AppendItem("VoteCode", voteCode)).OrderBy(o => o.Sort).ToList();
                var option = VoteOptionAdapter.Instance.Load(m => m.AppendItem("VoteCode", voteCode));
                var record = VoteRecordAdapter.Instance.Load(m => m.AppendItem("VoteCode", voteCode));

                // 投票问卷基本信息
                VoteDetailsViewModel model = new VoteDetailsViewModel();
                model.Code = info.Code;
                model.Title = info.Title;
                model.Describe = info.Describe;
                model.IsCast = record.FindAll(m => m.Creator == identity.Id).Count > 0 ? true : false;
                if (info.EndTime < DateTime.Now)
                {
                    model.IsOverdue = true;
                }
                else
                {
                    model.IsOverdue = false;
                }
                model.IsShowPoll = info.IsShowPoll;
                model.VoteType = info.VoteType;
                model.IsShowResult = info.IsShowResult;
                model.IsCreator = info.Creator == identity.Id ? true : false;
                model.EndTime = info.EndTime.ToString("yyyy-MM-dd HH:mm");
                model.Question = new List<QuestionDetailsViewModel>();
                question.ForEach(m =>
                {
                    var listOption = new List<OptionDetailsViewModel>();
                    var optionItems = option.FindAll(z => z.QuestionCode == m.Code).OrderBy(n => n.Sort).ToList();
                    var crea = record.FindAll(t => t.QuestionCode == m.Code);
                    //获取选项集合
                    optionItems.ForEach(o =>
                    {
                        var count = record.FindAll(t => t.OptionCode == o.Code);
                        var choice = record.FindAll(t => t.OptionCode == o.Code && t.Creator == identity.Id);
                        double percentage = 0;
                        if (count.Count == 0 || crea.Count == 0)
                        {
                            percentage = 0;
                        }
                        else
                        {
                            percentage = (double)count.Count / crea.Count;
                        }
                        listOption.Add(new OptionDetailsViewModel
                        {
                            Code = o.Code,
                            Name = o.Name,
                            Sort = o.Sort,
                            IsFill = o.IsFill,
                            FillContent = choice.Count > 0 ? choice[0].FillContent : string.Empty,
                            CastCount = count.Count,
                            Percentage = percentage,
                            IsChoice = choice.Count > 0 ? true : false
                        });
                    });
                    //获取问题集合
                    var quesItem = new QuestionDetailsViewModel
                    {
                        Code = m.Code,
                        Title = m.Title,
                        Sort = m.Sort,
                        QuestionType = m.QuestionType,
                        MinChoice = m.MinChoice,
                        MaxChoice = m.MaxChoice,
                        CastCount = crea.Count,
                        OptionDetails = listOption,
                        Answer = string.Empty
                    };
                    if (quesItem.QuestionType == 2)
                    {
                        var recordFind = record.FindAll(f => f.QuestionCode == quesItem.Code && f.Creator == identity.Id).FirstOrDefault();
                        quesItem.Answer = recordFind == null ? string.Empty : recordFind.FillContent;
                    }
                    model.Question.Add(quesItem);
                });
                return model;
            });
            return Ok(result);
        }
        #endregion

        #region 删除我参与的投票
        /// <summary>
        /// 删除我参与的投票
        /// </summary>
        [HttpPost]
        public IHttpActionResult DeleteVoteByParticipates(dynamic post)
        {
            var result = ControllerService.Run(() =>
            {
                var voteCode = post.voteCode.ToString();
                if (string.IsNullOrEmpty(voteCode)) { throw new Exception("参数为空或null"); }
                var user = (Seagull2Identity)User.Identity;
                var personColl = VotePersonAdapter.Instance.Load(m => m.AppendItem("UserCode", user.Id));
                if (personColl.Count == 0) { throw new Exception("您不属于投票范围"); }
                var person = VotePersonAdapter.Instance.Load(m => m.AppendItem("UserCode", user.Id).AppendItem("VoteCode", voteCode)).FirstOrDefault();
                if (person == null) { throw new Exception("无法找到该人员"); }
                person.ValidStatus = false;
                VotePersonAdapter.Instance.Update(person);
            });
            return Ok(result);
        }
        #endregion

        #region 删除我创建的投票问卷
        /// <summary>
        /// 删除我创建的投票问卷
        /// </summary>
        [HttpPost]
        public IHttpActionResult DeleteVoteByCreated(dynamic post)
        {
            var result = ControllerService.Run(() =>
            {
                var voteCode = post.voteCode.ToString();
                if (string.IsNullOrEmpty(voteCode))
                {
                    throw new Exception("参数不能为空！");
                }

                var identity = (Seagull2Identity)User.Identity;

                var votes = VoteInfoAdapter.Instance.Load(m => m.AppendItem("Creator", identity.Id).AppendItem("Code", voteCode));
                if (votes.Count > 0)
                {
                    VoteInfoAdapter.Instance.DeleteVoteByCreated(voteCode);
                }
                else
                {
                    var managers = VoteManagerAdapter.Instance.Load(w => w.AppendItem("VoteCode", voteCode).AppendItem("ManagerCode", identity.Id));
                    if (managers.Count > 0)
                    {
                        VoteInfoAdapter.Instance.DeleteVoteByCreated(voteCode);
                    }
                    else
                    {
                        throw new Exception("无投票数据或无权限执行此操作！");
                    }
                }
            });
            return Ok(result);
        }
        #endregion

        #region 设置
        /// <summary>
        /// 设置
        /// </summary>
        [HttpPost]
        public IHttpActionResult Settings(Seetings model)
        {
            var result = ControllerService.Run(() =>
            {
                var identity = (Seagull2Identity)User.Identity;

                // 数据校验
                var info = VoteInfoAdapter.Instance.Load(m => m.AppendItem("Code", model.VoteCode)).FirstOrDefault();
                if (info == null)
                {
                    throw new Exception("无效的编码！");
                }
                if (info.Creator != identity.Id)
                {
                    throw new Exception("您没有权限执行此操作！");
                }

                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    // 保存投票信息
                    info.IsShowPoll = model.IsShowPoll;
                    info.IsShowResult = model.IsShowResult;
                    info.EndTime = model.EndTime;
                    info.Modifier = identity.Id;
                    info.ModifyTime = DateTime.Now;
                    VoteInfoAdapter.Instance.Update(info);

                    // 人员范围
                    var deleteList = new List<string>();
                    var addList = new List<CreatePersonViewModel>();
                    var oldPerson = VotePersonAdapter.Instance.Load(m => m.AppendItem("VoteCode", model.VoteCode));
                    oldPerson.ForEach(m =>
                    {
                        var person = model.Person.FindAll(o => o.UserCode == m.UserCode).FirstOrDefault();
                        if (person == null)
                        {
                            deleteList.Add(m.UserCode);
                        }
                    });
                    InSqlClauseBuilder inSql = new InSqlClauseBuilder();
                    inSql.AppendItem(deleteList.ToArray());
                    if (deleteList.Count > 0)
                    {
                        VotePersonAdapter.Instance.Delete(where => where.AppendItem("VoteCode", info.Code).AppendItem("UserCode", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true));
                        VoteRecordAdapter.Instance.Delete(where => where.AppendItem("VoteCode", info.Code).AppendItem("Creator", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true));
                    }
                    model.Person.ForEach(m =>
                    {
                        var count = oldPerson.FindAll(o => o.UserCode == m.UserCode).Count;
                        if (count == 0)
                        {
                            addList.Add(m);
                        }
                    });
                    model.Person = addList;
                    VotePersonAdapter.Instance.BatchInsert(model.Person, model.VoteCode, identity.Id);

                    // 管理员
                    VoteManagerAdapter.Instance.Delete(w => w.AppendItem("VoteCode", model.VoteCode));
                    model.ManagerList?.ForEach(item =>
                    {
                        VoteManagerAdapter.Instance.Add(model.VoteCode, item.ManagerCode, item.ManagerDisplayName, identity.Id);
                    });

                    #region 系统消息提醒
                    Task.Run(() =>
                    {
                        try
                        {
                            MessageCollection messColl = new MessageCollection();
                            List<string> idList = new List<string>();
                            foreach (var items in model.Person)
                            {
                                MessageModel mess = new MessageModel()
                                {
                                    Code = Guid.NewGuid().ToString(),
                                    MeetingCode = info.Code,
                                    MessageContent = string.Format("收到一个新的投票，点击进入投票详情"),
                                    MessageStatusCode = EnumMessageStatus.New,
                                    MessageTypeCode = "2",
                                    MessageTitleCode = EnumMessageTitle.System,
                                    ModuleType = "Vote",
                                    Creator = identity.Id,
                                    CreatorName = identity.DisplayName,
                                    ReceivePersonCode = items.UserCode,
                                    ReceivePersonName = items.UserName,
                                    ReceivePersonMeetingTypeCode = "",
                                    OverdueTime = DateTime.Now.AddDays(7),
                                    ValidStatus = true,
                                    CreateTime = DateTime.Now
                                };
                                idList.Add(items.UserCode);
                                messColl.Add(mess);
                            }
                            MessageAdapter.Instance.BatchInsert(messColl);
                        }
                        catch (Exception e)
                        {
                            Log.WriteLog(e.Message);
                        }
                    });
                    #endregion

                    //事务提交
                    scope.Complete();
                }

            });
            return Ok(result);
        }
        #endregion

        #region 发送系统提醒
        /// <summary>
        /// 发送系统提醒
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult PushStystemMessage(PushSystemMessage model)
        {
            var result = ControllerService.Run(() =>
            {
                var identity = (Seagull2Identity)User.Identity;

                var info = VoteInfoAdapter.Instance.Load(m => m.AppendItem("Code", model.VoteCode)).FirstOrDefault();
                if (info == null)
                {
                    throw new Exception("无效的VoteCode");
                }

                if (info.Creator != identity.Id)
                {
                    var manager = VoteManagerAdapter.Instance.Load(w => w.AppendItem("VoteCode", info.Code).AppendItem("ManagerCode", identity.Id));
                    if (manager.Count < 1)
                    {
                        throw new Exception("无权限执行此操作");
                    }
                }

                #region 系统消息提醒
                MessageCollection messColl = new MessageCollection();
                List<string> idList = new List<string>();
                var personColl = VotePersonAdapter.Instance.Load(m => m.AppendItem("VoteCode", info.Code).AppendItem("ValidStatus", false));
                var pushContent = string.Empty;
                if (info.VoteType == 1)
                {
                    pushContent = string.Format("您的问卷{0}还没有进行提交噢~，点击进入详情", info.Title);
                }
                else
                {
                    pushContent = string.Format("您的投票{0}还没有进行投票噢~，点击进入详情", info.Title);
                }

                foreach (var items in model.Person)
                {
                    var person = personColl.FindAll(m => m.UserCode == items.UserCode);
                    if (person.Count > 0)
                    {
                        person.FirstOrDefault().ValidStatus = true;
                        VotePersonAdapter.Instance.Update(person.FirstOrDefault());
                    }
                    MessageModel mess = new MessageModel()
                    {
                        Code = Guid.NewGuid().ToString(),
                        MeetingCode = info.Code,
                        MessageContent = pushContent,
                        MessageStatusCode = EnumMessageStatus.New,
                        MessageTypeCode = "2",
                        MessageTitleCode = EnumMessageTitle.System,
                        ModuleType = "Vote",
                        Creator = identity.Id,
                        CreatorName = identity.DisplayName,
                        ReceivePersonCode = items.UserCode,
                        ReceivePersonName = items.UserName,
                        ReceivePersonMeetingTypeCode = "",
                        OverdueTime = DateTime.Now.AddDays(7),
                        ValidStatus = true,
                        CreateTime = DateTime.Now,
                    };
                    idList.Add(items.UserCode);
                    messColl.Add(mess);
                }

                MessageAdapter.Instance.BatchInsert(messColl);
                #endregion

                #region 推送
                var votePerson = new PushService.Model()
                {
                    BusinessDesc= "创建投票问卷",
                    Title = "投票问卷",
                    Content = pushContent,
                    SendType = PushService.SendType.Person,
                    Ids = string.Join(",", model.Person.Select(m => m.UserCode)),
                    Extras = new PushService.ModelExtras()
                    {
                        action = "Vote",
                        bags = info.Code
                    }
                };
                PushService.Push(votePerson, out string pushResult);
                #endregion

            });
            return Ok(result);
        }

        #endregion

        #region 获取投票人员
        /// <summary>
        /// 获取投票人员
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetPersonList(string voteCode)
        {
            var result = ControllerService.Run(() =>
            {
                var allPerson = VotePersonAdapter.Instance.Load(m => m.AppendItem("VoteCode", voteCode));

                var table = VoteRecordAdapter.Instance.GetCastPerson(voteCode);
                var list = DataConvertHelper<CreatePersonViewModel>.ConvertToList(table);
                //构造返回数据
                PersonList model = new PersonList();
                List<PersonInfo> CastPerson = new List<PersonInfo>();
                List<PersonInfo> NoCastPerson = new List<PersonInfo>();
                //遍历已经
                list.ForEach(m =>
                {
                    //UserHeadPhotoService
                    //构造已投人员数据
                    CastPerson.Add(new PersonInfo
                    {
                        UserCode = m.UserCode,
                        UserName = m.UserName,
                        HeadPhoto = UserHeadPhotoService.GetUserHeadPhoto(m.UserCode)
                    });
                    //构造未投人员数据
                    var item = allPerson.Find(o => o.UserCode == m.UserCode);
                    if (item != null)
                    {
                        allPerson.Remove(o => o.UserCode == m.UserCode);
                    }
                });
                //构造未投人员数据
                allPerson.ForEach(m =>
                {
                    NoCastPerson.Add(new PersonInfo
                    {
                        UserCode = m.UserCode,
                        UserName = m.UserName,
                        HeadPhoto = UserHeadPhotoService.GetUserHeadPhoto(m.UserCode)
                    });

                });
                model.CastPerson = CastPerson;
                model.NoCastPerson = NoCastPerson;
                return model;
            });
            return Ok(result);
        }
        #endregion

        #region 数据导出 - 发送邮箱
        /// <summary>
        /// 数据导出 - 发送邮箱
        /// </summary>
        [HttpGet]
        public IHttpActionResult DataExport(string voteCode, string email)
        {
            var result = ControllerService.Run(() =>
            {
                var identity = (Seagull2Identity)User.Identity;

                var info = VoteInfoAdapter.Instance.Load(m => m.AppendItem("Code", voteCode)).FirstOrDefault();
                if (info == null)
                {
                    throw new Exception("投票问卷编码错误！");
                }
                if (info.Creator != identity.Id)
                {
                    var manager = VoteManagerAdapter.Instance.Load(w => w.AppendItem("VoteCode", info.Code).AppendItem("ManagerCode", identity.Id));
                    if (manager.Count < 1)
                    {
                        throw new Exception("无权限执行此操作！");
                    }
                }

                var records = VoteRecordAdapter.Instance.Load(m => m.AppendItem("VoteCode", voteCode));
                if (records.Count < 1)
                {
                    throw new Exception("暂无数据！");
                }

                var questions = VoteQuestionAdapter.Instance.Load(w => w.AppendItem("VoteCode", voteCode), o => o.AppendItem("Sort", FieldSortDirection.Ascending));
                var options = VoteOptionAdapter.Instance.Load(w => w.AppendItem("VoteCode", voteCode));

                var userRecords = records.GroupBy(g => g.Creator).ToList();

                var userCodes = userRecords.Select(f => f.Key).ToList();
                var users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, userCodes.ToArray());

                // 构建表头
                var table = new DataTable();
                table.Columns.Add("姓名", typeof(string));
                table.Columns.Add("域账号", typeof(string));
                questions.ForEach(m =>
                {
                    table.Columns.Add(m.Title, typeof(string));
                });

                // 填充数据
                userRecords.ForEach(userRecord =>
                {
                    var user = users.Find(w => w.ID == userRecord.Key);
                    if (user == null)
                    {
                        return;
                    }

                    var dr = table.NewRow();
                    dr["姓名"] = user.DisplayName;
                    dr["域账号"] = user.LogOnName;

                    questions.ForEach(question =>
                    {
                        var answer = "";

                        var _records = userRecord.ToList().Where(w => w.QuestionCode == question.Code).ToList();
                        if (_records.Count < 1)
                        {
                            return;
                        }

                        // 单选
                        if (question.QuestionType == 0)
                        {
                            var _record = _records.First();
                            var _option = options.SingleOrDefault(w => w.Code == _record.OptionCode);
                            if (_option == null)
                            {
                                return;
                            }
                            if (_option.IsFill)
                            {
                                answer = $"{_option.Name}：{_record.FillContent}";
                            }
                            else
                            {
                                answer = _option.Name;
                            }
                        }
                        // 多选
                        if (question.QuestionType == 1)
                        {
                            var _answers = new List<string>();
                            _records.ForEach(_record =>
                            {
                                var _option = options.SingleOrDefault(w => w.Code == _record.OptionCode);
                                if (_option == null)
                                {
                                    return;
                                }
                                if (_option.IsFill)
                                {
                                    _answers.Add($"{_option.Name}：{_record.FillContent}");
                                }
                                else
                                {
                                    _answers.Add(_option.Name);
                                }
                            });
                            answer = string.Join("；", _answers);
                        }
                        // 填空
                        if (question.QuestionType == 2)
                        {
                            answer = _records.First().FillContent;
                        }

                        dr[question.Title] = answer;
                    });

                    table.Rows.Add(dr);
                });

                var filePath = HttpRuntime.AppDomainAppPath + string.Format(@"Resources\Vote\{0}\{1}.xls", info.Code, info.Title);
                Common.ExcelService.DtToExcel(table, filePath, info.Title);

                // 发送邮件
                Task.Run(() =>
                {
                    var mail = SeagullMailService.GetInstance();
                    mail.AddSubject(info.Title + "报表");
                    mail.AddAttachments(new List<string> { filePath });
                    mail.AddTo(new Dictionary<string, string>() { { email, identity.DisplayName } });
                    mail.Send();
                });
            });
            return Ok(result);
        }
        #endregion

        #region 获取当前设置
        /// <summary>
        /// 获取当前设置
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetSeetings(string voteCode)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                //数据校验
                var info = VoteInfoAdapter.Instance.Load(m => m.AppendItem("Code", voteCode).AppendItem("Creator", user.Id)).FirstOrDefault();
                if (info == null) { throw new Exception("无效的VoteCode"); }
                if (info.Creator != user.Id) { throw new Exception("无权限执行此操作"); }
                var person = VotePersonAdapter.Instance.Load(m => m.AppendItem("VoteCode", info.Code));
                GetSeetings list = new GetSeetings();
                List<CreatePersonViewModel> personList = new List<CreatePersonViewModel>();
                SeetingsVoteInfo infoModel = new SeetingsVoteInfo
                {
                    Title = info.Title,
                    IsShowPoll = info.IsShowPoll,
                    IsShowResult = info.IsShowResult,
                    EndTime = info.EndTime.ToString("yyyy-MM-dd HH:mm")
                };
                if (person.Count > 0)
                {
                    person.ForEach(m =>
                    {
                        var personItem = new CreatePersonViewModel
                        {
                            UserCode = m.UserCode,
                            UserName = m.UserName
                        };
                        personList.Add(personItem);
                    });
                }
                var managerList = new List<CreateVoteManagerViewModel>();
                VoteManagerAdapter.Instance.Load(w => w.AppendItem("VoteCode", voteCode)).ForEach(item =>
                {
                    managerList.Add(new CreateVoteManagerViewModel
                    {
                        ManagerCode = item.ManagerCode,
                        ManagerDisplayName = item.ManagerDisplayName,
                    });
                });
                list.VoteInfo = infoModel;
                list.PersonList = personList;
                list.ManagerList = managerList;
                return list;
            });
            return Ok(result);
        }
        #endregion

        #region 获取填空详细信息
        [HttpGet]
        public IHttpActionResult GetCompletionInfo(string voteCode, string questionCode, string optionCode = "")
        {
            var result = ControllerService.Run(() =>
            {
                var record = VoteRecordAdapter.Instance.Load(m =>
                {
                    m.AppendItem("VoteCode", voteCode);
                    m.AppendItem("QuestionCode", questionCode);
                    if (!string.IsNullOrWhiteSpace(optionCode))
                    {
                        m.AppendItem("OptionCode", optionCode);
                    }
                }).OrderByDescending(o => o.CreateTime);
                return record.Select(s => s.FillContent);
            });
            return Ok(result);
        }
        #endregion
    }
}
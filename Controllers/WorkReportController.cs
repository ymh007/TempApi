using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Adapter.Message;
using Seagull2.YuanXin.AppApi.Adapter.ShareFile;
using Seagull2.YuanXin.AppApi.Adapter.WorkReport;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.Models.Message;
using Seagull2.YuanXin.AppApi.Models.ShareFile;
using Seagull2.YuanXin.AppApi.Models.WorkReport;
using Seagull2.YuanXin.AppApi.ViewsModel.WorkReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static Seagull2.YuanXin.AppApi.ViewsModel.WorkReport.WorkReportListDetailViewModel;
using static Seagull2.YuanXin.AppApi.ViewsModel.WorkReport.WorkReportTemplateViewModel;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 工作汇报接口
    /// </summary>
    public class WorkReportController : ApiController
    {
        #region 新增或编辑模板
        /// <summary>
        /// 新增或编辑模板
        /// </summary>
        [HttpPost]
        public IHttpActionResult WorkReportTemplateEdit(WorkReportTemplateViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                var identity = (Seagull2Identity)User.Identity;

                if (string.IsNullOrEmpty(model.Code))
                {
                    // 新增
                    var code = Guid.NewGuid().ToString();
                    var WorkReportTemplate = new WorkReportTemplateModel
                    {
                        Code = code,
                        Title = model.Title,
                        IsSystem = false,
                        Creator = identity.Id,
                        CreateTime = DateTime.Now,
                        ValidStatus = true,
                    };
                    WorkReportTemplateAdapter.Instance.Update(WorkReportTemplate);

                    model.ChildList.ForEach(item =>
                    {
                        var WorkReportField = new WorkReportFieldModel
                        {
                            Code = Guid.NewGuid().ToString(),
                            Name = item.Title,
                            Sort = item.Sort,
                            TemplateCode = code,
                            Creator = identity.Id,
                            CreateTime = DateTime.Now,
                            ValidStatus = true,
                        };
                        WorkReportFieldAdapter.Instance.Update(WorkReportField);
                    });
                }
                else
                {
                    // 编辑
                    var WorkReportTemplate = WorkReportTemplateAdapter.Instance.Load(w => w.AppendItem("Code", model.Code)).FirstOrDefault();
                    if (WorkReportTemplate == null)
                    {
                        throw new Exception("编码错误!");
                    }

                    WorkReportTemplate.Title = model.Title;
                    WorkReportTemplate.Modifier = identity.Id;
                    WorkReportTemplate.ModifyTime = DateTime.Now;
                    WorkReportTemplateAdapter.Instance.Update(WorkReportTemplate);

                    WorkReportFieldAdapter.Instance.Delete(m =>
                    {
                        m.AppendItem("TemplateCode", model.Code);
                        m.AppendItem("[Creator]", identity.Id);
                    });

                    model.ChildList.ForEach(item =>
                    {
                        var WorkReportField = new WorkReportFieldModel
                        {
                            Code = Guid.NewGuid().ToString(),
                            Name = item.Title,
                            Sort = item.Sort,
                            TemplateCode = model.Code,
                            Creator = identity.Id,
                            CreateTime = DateTime.Now,
                            ValidStatus = true,
                        };
                        WorkReportFieldAdapter.Instance.Update(WorkReportField);
                    });
                }
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
                var count = WorkReportTemplateAdapter.Instance.GetListByPage(user.Id);
                //查询当前页数据
                var table = WorkReportTemplateAdapter.Instance.GetListByPage(pageIndex, pageSize, user.Id);
                var lists = DataConvertHelper<WorkReportTemplateModel>.ConvertToList(table);

                var checkList = new List<WorkReportTemplateViewModel>();
                lists.ForEach(m =>
                {
                    var userMenuList = WorkReportFieldAdapter.Instance.Load(w =>
                    {
                        w.AppendItem("TemplateCode", m.Code);
                        w.AppendItem("Creator", user.Id);
                    }).OrderBy(o =>
                    {
                        return o.Sort;
                    }).ToList();

                    //格式化模板字符
                    var chidList = new List<ChildListItem>();
                    userMenuList.ForEach((e =>
                    {
                        var ItemMenu = new ChildListItem
                        {
                            Code = e.Code,
                            Title = e.Name,
                            Sort = e.Sort
                        };
                        chidList.Add(ItemMenu);

                    }));


                    var item = new WorkReportTemplateViewModel
                    {
                        Code = m.Code,
                        Title = m.Title,
                        IsSystem = m.IsSystem,
                        ChildList = chidList
                    };
                    checkList.Add(item);

                });

                return new
                {
                    PageCount = count % pageSize == 0 ? count / pageSize : count / pageSize + 1,
                    Data = checkList
                };




            });
            return Ok(result);
        }

        #endregion

        #region 删除模板
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult DeleWorkReportTemplate(string Code)
        {
            var result = ControllerService.Run(() =>
            {
                //获取用户
                var user = (Seagull2Identity)User.Identity;
                //删除模板
                WorkReportTemplateAdapter.Instance.Delete(m =>
                {
                    m.AppendItem("Code", Code);

                });
                // 删除模板字符
                WorkReportFieldAdapter.Instance.Delete(m =>
                {
                    m.AppendItem("TemplateCode", Code);


                });



            });

            return Ok(result);
        }
        #endregion

        #region 新增汇报
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult WorkReportAdd(WorkReportDetailViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                //获取用户
                var user = (Seagull2Identity)User.Identity;

                //新增详情
                var reportCode = Guid.NewGuid().ToString();

                //系统推送
                var messageList = new MessageCollection();
                //获取接收人list
                var ReceiverList = ContactsAdapter.Instance.LoadListByUserCodeList(model.Receiver).ToList();
                ReceiverList = ReceiverList.Where((x, i) => ReceiverList.FindIndex(z => z.ObjectID == x.ObjectID) == i).ToList();

                List<ContactsModel> nonReapetMenu = new List<ContactsModel>();
                foreach (ContactsModel item in ReceiverList)
                {
                    if (nonReapetMenu.Exists(x => x.ObjectID == item.ObjectID) == false)
                    {
                        nonReapetMenu.Add(item);
                    }
                }
                ReceiverList = nonReapetMenu;

                //接收人详情
                string ReceiverDetails = "";
                ReceiverList.ForEach(item =>
                {
                    ReceiverDetails = ReceiverDetails + item.DisplayName + "、";
                });


                //获取抄送人list
                var CopyPersonList = ContactsAdapter.Instance.LoadListByUserCodeList(model.CopyPerSon).ToList();
                CopyPersonList = CopyPersonList.Where((x, i) => CopyPersonList.FindIndex(z => z.ObjectID == x.ObjectID) == i).ToList();

                List<ContactsModel> nonReapetMenus = new List<ContactsModel>();
                foreach (ContactsModel item in CopyPersonList)
                {
                    if (nonReapetMenu.Exists(x => x.ObjectID == item.ObjectID) == false)
                    {
                        nonReapetMenus.Add(item);
                    }
                }
                CopyPersonList = nonReapetMenus;
                //抄送人详情
                string CopyDetails = "";
                CopyPersonList.ForEach(item =>
                {
                    CopyDetails = CopyDetails + item.DisplayName + "、";
                });

                //新增发出人数据
                var WorkReportDetails = new WorkReportDetailsModel
                {
                    Code = Guid.NewGuid().ToString(),
                    ReportCode = reportCode,
                    Title = model.Title,
                    Mark = model.Mark,
                    ReceiverDetails = ReceiverDetails,//接收人详情
                    CopyDetails = CopyDetails,//抄送人详情
                    IsRead = false,
                    IsOriginal = true,//是否原件
                    IsSender = true,//是否发出者需要判断
                    ForwardTitle = "",//原创转发标题需要组织数据
                    CreateName = user.DisplayName,
                    ReceiveName = user.DisplayName,//接收人名称需要组织数据
                    ReceiveCode = user.Id,//接收人编码需要组织数据
                    Creator = user.Id,
                    CreateTime = DateTime.Now,
                    ValidStatus = true,
                };
                WorkReportDetailsAdapter.Instance.Update(WorkReportDetails);


                //接收人数据添加
                ReceiverList.ForEach(item =>
                {
                    var code = Guid.NewGuid().ToString();
                    //接受人推送
                    var message = new MessageModel
                    {
                        Code = Guid.NewGuid().ToString(),
                        MeetingCode = code,
                        MessageContent = "您收到" + user.DisplayName + "的工作汇报，请注意查收",
                        MessageStatusCode = EnumMessageStatus.New,
                        MessageTypeCode = "2",
                        MessageTitleCode = EnumMessageTitle.System,
                        ModuleType = EnumMessageModuleType.WorkReport.ToString(),
                        Creator = user.Id,
                        CreatorName = user.DisplayName,
                        ReceivePersonCode = item.ObjectID,
                        ReceivePersonName = item.DisplayName,
                        ReceivePersonMeetingTypeCode = "",
                        OverdueTime = DateTime.Now.AddDays(7),
                        ValidStatus = true,
                        CreateTime = DateTime.Now

                    };
                    messageList.Add(message);


                    var ReceiverWorkReportDetails = new WorkReportDetailsModel
                    {
                        Code = code,
                        ReportCode = reportCode,
                        Title = model.Title,
                        Mark = model.Mark,
                        ReceiverDetails = ReceiverDetails,//接收人详情
                        CopyDetails = CopyDetails,//抄送人详情
                        IsRead = false,
                        IsOriginal = true,//是否原件
                        IsSender = false,//是否发出者需要判断
                        ForwardTitle = "",//原创转发标题需要组织数据
                        CreateName = user.DisplayName,
                        ReceiveName = item.DisplayName,//接收人名称需要组织数据
                        ReceiveCode = item.ObjectID,//接收人编码需要组织数据
                        Creator = user.Id,
                        CreateTime = DateTime.Now,
                        ValidStatus = true,
                    };
                    WorkReportDetailsAdapter.Instance.Update(ReceiverWorkReportDetails);

                });
                //抄送人详情
                CopyPersonList.ForEach(item =>
                {
                    var code = Guid.NewGuid().ToString();

                    //抄送人推送
                    var message = new MessageModel
                    {
                        Code = Guid.NewGuid().ToString(),
                        MeetingCode = code,
                        MessageContent = "您收到" + user.DisplayName + "的工作汇报，请注意查收",
                        MessageStatusCode = EnumMessageStatus.New,
                        MessageTypeCode = "2",
                        MessageTitleCode = EnumMessageTitle.System,
                        ModuleType = EnumMessageModuleType.WorkReport.ToString(),
                        Creator = user.Id,
                        CreatorName = user.DisplayName,
                        ReceivePersonCode = item.ObjectID,
                        ReceivePersonName = item.DisplayName,
                        ReceivePersonMeetingTypeCode = "",
                        OverdueTime = DateTime.Now.AddDays(7),
                        ValidStatus = true,
                        CreateTime = DateTime.Now

                    };
                    messageList.Add(message);

                    var CopyPersonWorkReportDetails = new WorkReportDetailsModel
                    {
                        Code = code,
                        ReportCode = reportCode,
                        Title = model.Title,
                        Mark = model.Mark,
                        ReceiverDetails = ReceiverDetails,//接收人详情
                        CopyDetails = CopyDetails,//抄送人详情
                        IsRead = false,
                        IsOriginal = true,//是否原件
                        IsSender = false,//是否发出者需要判断
                        ForwardTitle = "",//原创转发标题需要组织数据
                        CreateName = user.DisplayName,
                        ReceiveName = item.DisplayName,//接收人名称需要组织数据
                        ReceiveCode = item.ObjectID,//接收人编码需要组织数据
                        Creator = user.Id,
                        CreateTime = DateTime.Now,
                        ValidStatus = true,
                    };
                    WorkReportDetailsAdapter.Instance.Update(CopyPersonWorkReportDetails);

                });

                //在内容表里面添加数据
                model.ChildList.ForEach(e =>
                {
                    var content = new WorkReportContentModel
                    {
                        Code = Guid.NewGuid().ToString(),
                        ReportCode = reportCode,
                        Title = e.Title,
                        Content = e.Content,
                        Sort = e.Sort,
                        Creator = user.Id,
                        CreateTime = DateTime.Now,
                        ValidStatus = true,
                    };
                    WorkReportContentAdapter.Instance.Update(content);
                });

                //在图片表里面增加数据
                int num = 0;
                model.ImageList.ForEach((n) =>
                {
                    num++;
                    var content = new ShareFileModel
                    {
                        Code = Guid.NewGuid().ToString(),
                        Module = "WorkReport",
                        TargetCode = reportCode,
                        Type = "image",
                        Path = n,
                        Sort = num,
                        Name = "",
                        Creator = user.Id,
                        CreateTime = DateTime.Now,
                        ValidStatus = true,
                    };
                    ShareFileAdapter.Instance.Update(content);
                });


                // 发推送 发消息
                if (messageList.Count > 0)
                {
                    MessageAdapter.Instance.BatchInsert(messageList);
                    var pushModel = new PushService.Model()
                    {
                        BusinessDesc = "工作汇报",
                        Title = "系统提醒",
                        Content = messageList.FirstOrDefault().MessageContent,
                        SendType = PushService.SendType.Person,
                        Ids = string.Join(",", messageList.Select(m => m.ReceivePersonCode).ToArray()),
                        Extras = new PushService.ModelExtras()
                        {
                            action = EnumMessageModuleType.WorkReport.ToString().ToLower(),
                            bags = messageList.FirstOrDefault().MeetingCode
                        }
                    };
                    string pushResult = string.Empty;
                    bool isPushCopy = PushService.Push(pushModel, out pushResult);
                }



            });
            return Ok(result);
        }
        #endregion

        #region 工作汇报列表
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isSender"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetWorkReportList(int pageIndex, int pageSize, bool isSender)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var count = WorkReportDetailsAdapter.Instance.GetWorkReportListByPage(user.Id, isSender);
                //查询当前页数据
                var table = WorkReportDetailsAdapter.Instance.GetWorkReportListByPage(pageIndex, pageSize, user.Id, isSender);
                var lists = DataConvertHelper<WorkReportDetailsModel>.ConvertToList(table);
                var checkList = new List<WorkReportListDetailViewModel>();

                lists.ForEach(m =>
                {

                    //获取详情内容
                    var ChildListList = WorkReportContentAdapter.Instance.Load(w =>
                    {
                        w.AppendItem("ReportCode", m.ReportCode);

                    }).OrderBy(o =>
                    {
                        return o.Sort;
                    }).ToList();

                    var chidList = new List<ChildListItems>();
                    ChildListList.ForEach(e =>
                    {
                        //格式化详情内容
                        var ItemMenu = new ChildListItems
                        {
                            Title = e.Title,
                            Content = e.Content
                        };
                        chidList.Add(ItemMenu);
                    });

                    //获取图片
                    var imageList = ShareFileAdapter.Instance.Load(w =>
                    {
                        w.AppendItem("TargetCode", m.ReportCode);

                    }).OrderBy(o =>
                    {
                        return o.Sort;
                    }).ToList();

                    //格式化图片
                    var ImageLists = new List<string>();

                    imageList.ForEach(n =>
                    {
                        //格式化详情内容
                        ImageLists.Add(n.Path);

                    });
                    var item = new WorkReportListDetailViewModel
                    {
                        Code = m.Code,
                        Title = m.Title,
                        SenderName = m.CreateName,
                        CreateTime = m.CreateTime,
                        Mark = m.Mark,
                        ReceiverDetails = m.ReceiverDetails,
                        CopyDetails = m.CopyDetails,
                        ForwardDetails = m.ForwardDetails,
                        ForwardTitle = m.ForwardTitle,
                        IsOriginal = m.IsOriginal,
                        IsRead = m.IsRead,
                        IsSender = m.IsSender,
                        ChildList = chidList,
                        ImageList = ImageLists,

                    };
                    checkList.Add(item);


                });

                return new
                {
                    PageCount = count % pageSize == 0 ? count / pageSize : count / pageSize + 1,
                    Data = checkList
                };




            });
            return Ok(result);
        }

        #endregion

        #region 导出工作汇报列表
        /// <summary>
        /// 导出工作汇报发邮件
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ExportWorkReport(string email, DateTime start, DateTime end)
        {
            var result = ControllerService.Run(() =>
            {
                if (string.IsNullOrEmpty(email)) throw new Exception("邮箱不能为空！");
                var user = (Seagull2Identity)User.Identity;
                DataTable sourceList = WorkReportDetailsAdapter.Instance.ExportWorkReport(user.Id, start, end);
                WorkReportContentCollection contents = WorkReportContentAdapter.Instance.GetContents(start, end);
                if (sourceList != null && sourceList.Rows.Count > 0)
                {
                    sourceList.Columns.Add("Content", typeof(string));
                    sourceList.Columns["Content"].SetOrdinal(7);
                    string contentStr = "";
                    foreach (DataRow row in sourceList.Rows)
                    {
                        contents.FindAll(f => f.ReportCode == row["ReportCode"].ToString()).ToList().ForEach(f =>
                        {
                            contentStr = contentStr + f.Title + " : " + f.Content + Environment.NewLine;
                        });
                        row["Content"] = contentStr;
                        contentStr = "";
                    }
                }
                else
                {
                    throw new Exception("没有可以导出的数据！");
                }
                string title = user.DisplayName + start.ToString("yyyy-MM-dd") + "到" + end.ToString("yyyy-MM-dd") + "的工作汇报";
                var filePath = HttpRuntime.AppDomainAppPath + string.Format(@"Resources\workReport\{0}.xlsx", title);
                List<string> columns = new List<string>() { "发送人", "接受人", "抄送人", "类型", "内容", "备注", "时间" };
                Common.ExcelService.GenerateExcel(sourceList, filePath, title, columns);
                // 发送邮件
                Task.Run(() =>
                {
                    var mail = SeagullMailService.GetInstance();
                    mail.AddSubject(title);
                    mail.AddAttachments(new List<string> { filePath });
                    mail.AddTo(new Dictionary<string, string>() { { email, user.DisplayName } });
                    mail.Send();
                });

            });
            return Ok(result);
        }

        #endregion

        #region 删除工作汇报
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult DeleWorkReport(string Code)
        {
            var result = ControllerService.Run(() =>
            {

                //删除模板
                WorkReportDetailsAdapter.Instance.Delete(m =>
                {
                    m.AppendItem("Code", Code);

                });

            });

            return Ok(result);
        }


        #endregion

        #region 改变汇报状态
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ChangeWorkReportStatus(string Code)
        {
            var result = ControllerService.Run(() =>
            {
                //获取用户
                var user = (Seagull2Identity)User.Identity;

                //编辑模板
                var WorkReport = WorkReportDetailsAdapter.Instance.Load(w => w.AppendItem("Code", Code)).FirstOrDefault();
                if (WorkReport == null)
                {
                    throw new Exception("编码错误!");
                }

                WorkReport.IsRead = true;
                WorkReport.Modifier = user.Id;
                WorkReport.ModifyTime = DateTime.Now;
                WorkReportDetailsAdapter.Instance.Update(WorkReport);


            });

            return Ok(result);
        }
        #endregion

        #region 工作汇报转发
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult WorkReportForward(ForwardModel model)
        {
            var result = ControllerService.Run(() =>
            {
                //获取用户
                var user = (Seagull2Identity)User.Identity;


                //获取当前code的数据
                var WorkReport = WorkReportDetailsAdapter.Instance.Load(w => w.AppendItem("Code", model.Code)).FirstOrDefault();
                if (WorkReport == null)
                {
                    throw new Exception("编码错误!");
                }

                //系统推送
                var messageList = new MessageCollection();
                //获取接收人list
                var ReceiverList = ContactsAdapter.Instance.LoadListByUserCodeList(model.Receiver).ToList();
                List<ContactsModel> nonReapetMenu = new List<ContactsModel>();
                foreach (ContactsModel item in ReceiverList)
                {
                    if (nonReapetMenu.Exists(x => x.ObjectID == item.ObjectID) == false)
                    {
                        nonReapetMenu.Add(item);
                    }
                }
                ReceiverList = nonReapetMenu;
                //item.ObjectID
                //转发人详情
                string ForwardDetails = "";
                ReceiverList.ForEach(item =>
                {
                    ForwardDetails = ForwardDetails + item.DisplayName + "、";
                });



                //新增发出人数据
                var WorkReportDetails = new WorkReportDetailsModel
                {
                    Code = Guid.NewGuid().ToString(),
                    ReportCode = WorkReport.ReportCode,
                    Title = WorkReport.Title,
                    Mark = WorkReport.Mark,
                    ReceiverDetails = WorkReport.ReceiverDetails,//接收人详情
                    CopyDetails = WorkReport.CopyDetails,//抄送人详情
                    ForwardDetails = ForwardDetails,
                    IsRead = false,
                    IsOriginal = false,//是否原件
                    IsSender = true,//是否发出者需要判断
                    ForwardTitle = "",//原创转发标题需要组织数据
                    CreateName = user.DisplayName,
                    ReceiveName = user.DisplayName,//接收人名称需要组织数据
                    ReceiveCode = user.Id,//接收人编码需要组织数据
                    Creator = user.Id,
                    CreateTime = DateTime.Now,
                    ValidStatus = true,
                };
                WorkReportDetailsAdapter.Instance.Update(WorkReportDetails);


                //接收人数据添加
                ReceiverList.ForEach(item =>
                {
                    var code = Guid.NewGuid().ToString();
                    //接受人推送
                    var message = new MessageModel
                    {
                        Code = Guid.NewGuid().ToString(),
                        MeetingCode = code,
                        MessageContent = "您收到" + user.DisplayName + "转发的工作汇报，请注意查收",
                        MessageStatusCode = EnumMessageStatus.New,
                        MessageTypeCode = "2",
                        MessageTitleCode = EnumMessageTitle.System,
                        ModuleType = EnumMessageModuleType.WorkReport.ToString(),
                        Creator = user.Id,
                        CreatorName = user.DisplayName,
                        ReceivePersonCode = item.ObjectID,
                        ReceivePersonName = item.DisplayName,
                        ReceivePersonMeetingTypeCode = "",
                        OverdueTime = DateTime.Now.AddDays(7),
                        ValidStatus = true,
                        CreateTime = DateTime.Now

                    };
                    messageList.Add(message);


                    var ReceiverWorkReportDetails = new WorkReportDetailsModel
                    {
                        Code = code,
                        ReportCode = WorkReport.ReportCode,
                        Title = WorkReport.Title,
                        Mark = WorkReport.Mark,
                        ReceiverDetails = WorkReport.ReceiverDetails,//接收人详情
                        CopyDetails = WorkReport.CopyDetails,//抄送人详情
                        ForwardDetails = ForwardDetails,
                        IsRead = false,
                        IsOriginal = false,//是否原件
                        IsSender = false,//是否发出者需要判断
                        ForwardTitle = "",//原创转发标题需要组织数据
                        CreateName = user.DisplayName,
                        ReceiveName = item.DisplayName,//接收人名称需要组织数据
                        ReceiveCode = item.ObjectID,//接收人编码需要组织数据
                        Creator = user.Id,
                        CreateTime = DateTime.Now,
                        ValidStatus = true,
                    };
                    WorkReportDetailsAdapter.Instance.Update(ReceiverWorkReportDetails);

                });


                MessageAdapter.Instance.BatchInsert(messageList);

            });
            return Ok(result);
        }

        #endregion

        #region 工作汇报详情
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetWorkReportDetails(string Code)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;

                //根据code获取当前数据
                var WorkReport = WorkReportDetailsAdapter.Instance.Load(w => w.AppendItem("Code", Code)).FirstOrDefault();
                if (WorkReport == null)
                {
                    throw new Exception("编码错误!");
                }

                //获取详情内容
                var ChildListList = WorkReportContentAdapter.Instance.Load(w =>
                {
                    w.AppendItem("ReportCode", WorkReport.ReportCode);

                }).OrderBy(o =>
                {
                    return o.Sort;
                }).ToList();

                var chidList = new List<ChildListItems>();
                ChildListList.ForEach(e =>
                {
                    //格式化详情内容
                    var ItemMenu = new ChildListItems
                    {
                        Title = e.Title,
                        Content = e.Content
                    };
                    chidList.Add(ItemMenu);
                });

                //获取图片
                var imageList = ShareFileAdapter.Instance.Load(w =>
                {
                    w.AppendItem("TargetCode", WorkReport.ReportCode);

                }).OrderBy(o =>
                {
                    return o.Sort;
                }).ToList();

                //格式化图片
                var ImageLists = new List<string>();

                imageList.ForEach(n =>
                {
                    //格式化详情内容
                    ImageLists.Add(n.Path);

                });


                var item = new WorkReportListDetailViewModel
                {
                    Code = WorkReport.Code,
                    Title = WorkReport.Title,
                    SenderName = WorkReport.CreateName,
                    CreateTime = WorkReport.CreateTime,
                    Mark = WorkReport.Mark,
                    ReceiverDetails = WorkReport.ReceiverDetails,
                    CopyDetails = WorkReport.CopyDetails,
                    ForwardDetails = WorkReport.ForwardDetails,
                    ForwardTitle = WorkReport.ForwardTitle,
                    IsOriginal = WorkReport.IsOriginal,
                    IsRead = WorkReport.IsRead,
                    IsSender = WorkReport.IsSender,
                    ChildList = chidList,
                    ImageList = ImageLists,

                };

                return (item);


            });
            return Ok(result);
        }

        #endregion
    }
}
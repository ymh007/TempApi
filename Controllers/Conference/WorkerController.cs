using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.ViewsModel;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    public partial class ConferenceController : ControllerBase
    {
        #region 获取工作人员列表--APP
        /// <summary>
        /// 获取工作人员列表--APP
        /// </summary>
        [Route("GetWorkerList")]
        [HttpGet]
        public IHttpActionResult GetWorkerList(string conferenceID)
        {
            List<WorkerViewListModel> list = new List<WorkerViewListModel>();
            ControllerHelp.SelectAction(() =>
            {
                list = WorkerViewListModelAdapter.Instance.GetWorkerViewListModelByPage(conferenceID);
                foreach (var item in list)
                {
                    item.WorkerListData = WorkerListAdapter.Instance.GetWorkerList(conferenceID, item.WorkerTypeName);
                }
            });
            return Ok(list);
        }
        #endregion

        #region 获取工作人员实体对象
        /// <summary>
        /// 获取工作人员实体对象
        /// </summary>
        [Route("GetWorkerByID")]
        [HttpGet]
        public IHttpActionResult GetWorkerByID(string id)
        {
            WorkerModel wm = new WorkerModel();
            ControllerHelp.SelectAction(() =>
            {
                wm = WorkerAdapter.Instance.LoadByID(id);
            });
            return Ok(wm);
        }
        #endregion

        #region 获取工作人员列表--PC
        /// <summary>
        /// 获取工作人员列表--PC
        /// </summary>
        [Route("GetWorkerListForPC")]
        [HttpGet]
        public IHttpActionResult GetWorkerListForPC(int pageIndex, string conferenceID)
        {
            ViewPageBase<WorkerCollection> wmColl = new ViewPageBase<WorkerCollection>();
            ControllerHelp.SelectAction(() =>
            {
                wmColl = WorkerAdapter.Instance.LoadByConferenceIDByPage(pageIndex, conferenceID);
            });
            return Ok(wmColl);
        }
        #endregion

        #region 上移下移工作人员--PC
        /// <summary>
        /// 上移下移工作人员--PC
        /// </summary>
        [Route("UpDownSort")]
        [HttpGet]
        public IHttpActionResult UpDownSort(string id1,string id2)
        {
            var result = ControllerService.Run(() =>
            {
                //获取第一个工作人员和第二个工作人员
                var worker1 = WorkerAdapter.Instance.Load(w => w.AppendItem("Id", id1)).FirstOrDefault();
                var worker2 = WorkerAdapter.Instance.Load(w => w.AppendItem("Id", id2)).FirstOrDefault();
                var sor1 = worker1.Sort;
                var sort2 = worker2.Sort;
                //交换两个人的排序
                worker1.Sort = sort2;
                worker2.Sort = sor1;
                WorkerAdapter.Instance.Update(worker1);
                WorkerAdapter.Instance.Update(worker2);

            });
            return Ok(result);
        }
        #endregion

        #region 最底部工作人员--PC
        /// <summary>
        /// 最底部工作人员--PC
        /// </summary>
        [Route("MostDownSort")]
        [HttpGet]
        public IHttpActionResult MostDownSort(string id)
        {
            var result = ControllerService.Run(() =>
            {
                //获取放到最底部人员的信息
                var worker = WorkerAdapter.Instance.Load(w => w.AppendItem("Id", id)).FirstOrDefault();

                var  workerList= WorkerAdapter.Instance.Load(w => w.AppendItem("ConferenceID", worker.ConferenceID)).OrderBy(o => o.Sort);

                var lastworker = workerList.LastOrDefault();
                worker.Sort = lastworker.Sort + 1;
               // return workerList;
              WorkerAdapter.Instance.Update(worker);

            });
            return Ok(result);
        }
        #endregion

        #region 最顶部工作人员--PC
        /// <summary>
        /// 最底部工作人员--PC
        /// </summary>
        [Route("MostUpSort")]
        [HttpGet]
        public IHttpActionResult MostUpSort(string id)
        {
            var result = ControllerService.Run(() =>
            {
                //获取放到最顶部人员的信息
                var worker = WorkerAdapter.Instance.Load(w => w.AppendItem("Id", id)).FirstOrDefault();

                var workerList = WorkerAdapter.Instance.Load(w => w.AppendItem("ConferenceID", worker.ConferenceID)).OrderBy(o => o.Sort);
                //获取列表排序第一条
                var Firstworker = workerList.FirstOrDefault();
                //判断列表第一条的排序号是否大于0
                if (Firstworker.Sort ==0)
                {
                    var workerToList = workerList.ToList();
                    workerToList.ForEach((e =>
                    {
                        e.Sort = e.Sort + 1;
                        WorkerAdapter.Instance.Update(e);

                    }));

                }
                //把当前任置顶
                worker.Sort = 0;
                WorkerAdapter.Instance.Update(worker);


            });
            return Ok(result);
        }
        #endregion


        #region 编辑工作人员--PC
        /// <summary>
        /// 编辑工作人员--PC
        /// </summary>
        [Route("UpdateWorker")]
        [HttpPost]
        public IHttpActionResult UpdateWorker(UpdateWorkerPost post)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                // 根据邮箱获取海鸥二数据信息
                ContactsModel contacts = ContactsAdapter.Instance.LoadByMail(post.Email);
                if (contacts == null)
                {
                    throw new Exception("该人员非海鸥Ⅱ人员！");
                }

                //添加
                if (string.IsNullOrWhiteSpace(post.ID))
                {
                    WorkerModel model = new WorkerModel()
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        Creator = ((Seagull2Identity)User.Identity).Id,
                        CreateTime = DateTime.Now,
                        ValidStatus = true,
                        ConferenceID = post.ConferenceID,
                        UserID = contacts.ObjectID,
                        UserName = post.UserName,
                        UserTelPhone = post.UserTelPhone,
                        UserPhotoAddress = UserHeadPhotoService.GetUserHeadPhoto(contacts.ObjectID),
                        WorkerTypeName = post.WorkerTypeName,
                        Sort = post.Sort
                    };
                    WorkerAdapter.Instance.Update(model);
                }
                //修改
                else
                {
                    WorkerModel model = WorkerAdapter.Instance.LoadByID(post.ID);
                    if (model == null)
                    {
                        throw new Exception("工作人员编码错误！");
                    }
                    model.ConferenceID = post.ConferenceID;
                    model.UserID = contacts.ObjectID;
                    model.UserName = post.UserName;
                    model.UserTelPhone = post.UserTelPhone;
                    model.UserPhotoAddress = UserHeadPhotoService.GetUserHeadPhoto(contacts.ObjectID);
                    model.WorkerTypeName = post.WorkerTypeName;
                    model.Sort = post.Sort;
                    WorkerAdapter.Instance.Update(model);
                }
            });
            return Ok(result);
        }
        /// <summary>
        /// UpdateWorkerPost
        /// </summary>
        public class UpdateWorkerPost
        {
            /// <summary>
            /// 自动编码
            /// </summary>
            public string ID { set; get; }
            /// <summary>
            /// 会议编码
            /// </summary>
            public string ConferenceID { set; get; }
            /// <summary>
            /// 类型编码
            /// </summary>
            public string WorkerTypeName { set; get; }
            /// <summary>
            /// 姓名
            /// </summary>
            public string UserName { set; get; }
            /// <summary>
            /// 电话
            /// </summary>
            public string UserTelPhone { set; get; }
            /// <summary>
            /// 邮箱
            /// </summary>
            public string Email { set; get; }
            /// <summary>
            /// 排序数字
            /// </summary>
            public int Sort { set; get; }
        }
        #endregion

        #region 删除工作人员--PC
        /// <summary>
        /// 删除工作人员--PC
        /// </summary>
        [Route("DelWorker")]
        [HttpPost]
        public IHttpActionResult DelWorker(string workerID)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                //#region 修改企业会话信息
                //WorkerModel model = WorkerAdapter.Instance.LoadByID(workerID);

                ////如果工作人员属于海鸥二人员，将工作人员添加到会议企业会话中--最新需求-暂不需要自动增删人员
                //WorkerModelCollection workerColl = WorkerAdapter.Instance.LoadByConferenceID(model.ConferenceID);
                //workerColl.Remove(w => w.ID == workerID);
                //WorkerDialogHelp.WorkerCollDialog(workerColl, model.ConferenceID);
                //#endregion

                WorkerAdapter.Instance.DelWorker(workerID);
            });
            return Ok(result);
        }
        #endregion

        #region 工作人员模板导入--PC
        /// <summary>
        /// 工作人员模板导入--PC
        /// </summary>
        [Route("ImportExcelDataForWorker")]
        [HttpPost]
        public IHttpActionResult ImportExcelDataForWorker()
        {
           
            List<string> errorList = new List<string>();
            WorkerCollection modelColl = new WorkerCollection();
            bool state = false;
            string conferenceID = baseRequest.Form["ConferenceID"].ToString();
            ControllerHelp.SelectAction(() =>
            {
                WorkerModel model = new WorkerModel();
                DataTable dataTable = ExcelHelp<WorkerModel, List<WorkerModel>>.GetExcelData("Worker");
                dataTable.Columns["工作人员类型"].ColumnName = "WorkerTypeName";
                dataTable.Columns["姓名"].ColumnName = "UserName";
                dataTable.Columns["电话"].ColumnName = "UserTelPhone";
                dataTable.Columns["邮箱地址"].ColumnName = "Email";
                //该会议下存在的工作人员list集合
                List<WorkerModel> list = WorkerAdapter.Instance.LoadByConferenceID(conferenceID).ToList<WorkerModel>();
                //本次Excel导入的工作人员list
                List<WorkerModel> listII = DataConvertHelper<WorkerModel>.ConvertToList(dataTable);
                //将工作人员数据添加进数据库
                foreach (var items in list)
                {
                    if (listII.RemoveAll(m => m.UserTelPhone == items.UserTelPhone) > 0)
                    {
                        continue;
                    }
                }
                if (listII != null)
                {
                    foreach (var item in listII)
                    {
                        //根据邮箱获取海鸥二数据信息
                        ContactsModel contacts = ContactsAdapter.Instance.LoadByMail(item.Email);
                        if (contacts != null)
                        {
                            //WorkerTypeCollection wtColl = WorkerTypeAdapter.Instance.Load(m => m.AppendItem("Name", item.WorkerTypeID));
                            item.ID = Guid.NewGuid().ToString("N");
                            item.Creator = CurrentUserCode;
                            item.ValidStatus = true;
                            item.CreateTime = DateTime.Now;
                            item.ConferenceID = conferenceID;
                            //item.WorkerTypeName = wtColl.Count > 0 ? wtColl[0].ID : "";
                            item.UserID = contacts.ObjectID;
                            item.UserPhotoAddress = UserHeadPhotoService.GetUserHeadPhoto(contacts.ObjectID);
                            modelColl.Add(item);
                            state = true;
                        }
                    }
                    WorkerAdapter.Instance.AddWorkerModelCollection(modelColl);
                }
            });

            ViewPageBase<WorkerCollection> view = new ViewPageBase<WorkerCollection>()
            {
                State = state,
                dataList = modelColl,
                PageCount = modelColl.Count / ViewPageBase<WorkerCollection>.PageSize + (modelColl.Count % ViewPageBase<WorkerCollection>.PageSize > 0 ? 1 : 0)
            };
            return Ok(view);

        }
        #endregion

        #region 工作人员数据导出成EXCEL--PC
        /// <summary>
        /// 工作人员数据导出成EXCEL--PC
        /// </summary>
        [AllowAnonymous]
        [Route("ExportExcelDataForWorker")]
        [HttpGet]
        public HttpResponseMessage ExportExcelDataForWorker(string conferenceID)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            ControllerHelp.RunAction(() =>
            {
                WorkerCollection workColl = WorkerAdapter.Instance.Load(m => m.AppendItem("ConferenceID", conferenceID));

                Dictionary<string, string> dicColl = new Dictionary<string, string>() {
                        {"工作人员类型","WorkerTypeName" },
                        {"姓名","UserName" },
                        {"电话","UserTelPhone" },
                        { "邮箱地址","Email"}
                    };
                result = ExcelHelp<WorkerModel, List<WorkerModel>>.ExportExcelData(dicColl, workColl.ToList(), "Worker");
            });
            return result;
        }
        #endregion
    }
}
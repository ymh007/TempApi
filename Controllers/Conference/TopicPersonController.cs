using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.Conference;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    //议题讨论人控制器
    public partial class ConferenceController : ControllerBase
    {
        /// <summary>
        /// 查询议题讨论人
        /// </summary>
        /// <returns></returns>
        [Route("TopicPersonList")]
        [HttpGet]
        public IHttpActionResult TopicPersonList(string ConferenceTopicID, int pageIndex, DateTime searchTime)
        {
            ViewPageBase<List<AttendeeSignList>> attendeeList = new ViewPageBase<List<AttendeeSignList>>();
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                attendeeList = AttendeeListSigAdapter.Instance.GetTopicPersonView(ConferenceTopicID, pageIndex, searchTime);
            });
            return Ok(attendeeList);
        }
        /// <summary>
        /// 议题讨论人导入--PC
        /// </summary>
        /// <returns></returns>
        [Route("ImportExcelDataForTopicPerson")]
        [HttpPost]
        public IHttpActionResult ImportExcelDataForBusRouteTopic()
        {
            //导入失败邮箱列表
            List<string> errorEmailList = new List<string>();

            string ConferenceTopicId = baseRequest.Form["ConferenceTopicId"].ToString();

            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                //删除议题下的所有议题讨论人信息
                TopicPersonAdapter.Instance.DelTopicPersionByconferenceTopicID(ConferenceTopicId);

                //将议题讨论人数据添加进数据库
                DataTable dataTable = ExcelHelp<ConferenceTopicsModel, List<ConferenceTopicsModel>>.GetExcelData("TopicPerson");
                for (var i = 0; i < dataTable.Rows.Count; i++)
                {
                    DataRow row = dataTable.Rows[i];

                    string Attendaemail = row["邮箱"].ToString();
                    AttendeeModel attendee = AttendeeAdapter.Instance.GetAttendeeByEmail(Attendaemail);
                    if (attendee == null)
                    {
                        errorEmailList.Add(Attendaemail);
                    }
                    else
                    {
                        TopicPersonModels model = new TopicPersonModels
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            ConferenceTopicID = ConferenceTopicId,
                            AttendeeID = attendee.ID,
                            Creator = CurrentUserCode,
                            CreateTime = DateTime.Now,
                            ValidStatus = true,
                        };
                        TopicPersonAdapter.Instance.Update(model);
                    }
                }
                if (errorEmailList.Count > 0)
                {
                    Log.WriteLog("议题讨论人导入失败邮箱列表：" + JsonConvert.SerializeObject(errorEmailList));
                }
            });
            return Ok(result);
        }
        /// <summary>
        /// 议题讨论人数据导出成EXCEL
        /// </summary>
        /// <param name="conferenceTopicID"></param>
        /// <returns></returns>
        [Route("ExportExcelDataForTopicPerson")]
        [HttpGet]
        public HttpResponseMessage ExportExcelDataForBusRouteTopic(string conferenceTopicID)
        {
            TopicPersonCooection atColl = TopicPersonAdapter.Instance.GetTopicPersionByconferenceTopicID(conferenceTopicID);
            AttendeeCollection attendCol = new AttendeeCollection();
            AttendeeModel attendMode = new AttendeeModel();
            foreach (TopicPersonModels item in atColl)
            {
                attendMode = AttendeeAdapter.Instance.LoadByID(item.AttendeeID);
                if (attendMode != null)
                {
                    attendCol.Add(attendMode);
                }
            }

            HttpResponseMessage result = new HttpResponseMessage();
            ControllerHelp.RunAction(() =>
            {
                Dictionary<string, string> dicColl = new Dictionary<string, string>() {
                    {"姓名","Name" },
                    {"邮箱","Email" },
                };
                result = ExcelHelp<AttendeeModel, List<AttendeeModel>>.ExportExcelData(dicColl, attendCol.ToList(), "TopicPerson");
            });
            return result;
        }
    }
}
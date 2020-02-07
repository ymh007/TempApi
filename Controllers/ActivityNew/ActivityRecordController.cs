using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Http;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.ActivityNew;
using Seagull2.YuanXin.AppApi.Models.ActivityNew;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.ActivityNew;

namespace Seagull2.YuanXin.AppApi.Controllers.ActivityNew
{
    /// <summary>
    /// 活动记录 Controller
    /// </summary>
    public class ActivityRecordController : ApiController
    {
        #region 关注/取消关注
        /// <summary>
        /// 关注/取消关注
        /// </summary>
        [HttpPost]
        public IHttpActionResult Follow(ActivityRecordViewModel.FollowViewModel post)
        {
            //判断活动是否存在
            var isExist = ActivityInfoAdapter.Instance.Exists(w => w.AppendItem("Code", post.ActivityCode));
            if (!isExist)
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = "活动不存在！"
                });
            }

            var user = (Seagull2Identity)User.Identity;
            var data = ActivityRecordAdapter.Instance.Load(
                w =>
                {
                    w.AppendItem("ActivityCode", post.ActivityCode);
                    w.AppendItem("UserCode", user.Id);
                    w.AppendItem("Type", (int)ActivityRecordType.Follow);
                });
            if (data.Count < 1)
            {
                //关注
                ActivityRecordAdapter.Instance.Update(new ActivityRecordModel()
                {
                    Code = Guid.NewGuid().ToString(),
                    ActivityCode = post.ActivityCode,
                    UserCode = user.Id,
                    Type = ActivityRecordType.Follow,
                    Creator = user.Id,
                    CreateTime = DateTime.Now,
                    ValidStatus = true
                });
                return Ok(new BaseView()
                {
                    State = true,
                    Message = "followed."
                });
            }
            else
            {
                //取消关注
                ActivityRecordAdapter.Instance.Delete(
                    w =>
                    {
                        w.AppendItem("ActivityCode", post.ActivityCode);
                        w.AppendItem("UserCode", user.Id);
                        w.AppendItem("Type", (int)ActivityRecordType.Follow);
                    });
                return Ok(new BaseView()
                {
                    State = true,
                    Message = "cancelled."
                });
            }
        }
        #endregion

        #region 报名、浏览、关注统计 - APP
        /// <summary>
        /// 获取统计内容
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Statistics(string activityCode)
        {
            var sql = new StringBuilder();
            DataTable dt = new DataTable();
            sql.AppendFormat(@" select Type,count(Type) as '数量' from [office].[ActivityRecord] 
            where activityCode = '{0}' 
            Group by Type
            union all
            select '-1' as 'Type',count(*) as '数量' from [office].[ActivityApplyInfo]
            where activityCode = '{0}' ", activityCode);
            SqlDbHelper db = new SqlDbHelper();
            dt = db.ExecuteDataTable(sql.ToString());
            var model = new ActivityRecordViewModel.StatisticsViewModel();
            var rowAttention = dt.Select("Type = -1");
            var rowParticipate = dt.Select("Type = 1");
            var rowView = dt.Select("Type = 0");
            if (rowAttention.Length > 0)
            {
                model.FollowCount = Convert.ToInt32(dt.Select("Type = -1")[0][1]);
            }
            if (rowParticipate.Length > 0)
            {
                model.ApplyCount = Convert.ToInt32(dt.Select("Type = 1")[0][1]);
            }
            if (rowView.Length > 0)
            {
                model.ViewCount = Convert.ToInt32(dt.Select("Type = 0")[0][1]);
            }
            return Ok(new BaseView
            {
                State = true,
                Message = "操作成功",
                Data = model

            });
        }
        #endregion
    }

}
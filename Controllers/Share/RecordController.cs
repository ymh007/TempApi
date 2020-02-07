using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using log4net;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Share;
using Seagull2.YuanXin.AppApi.Models.Share;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.Share;

namespace Seagull2.YuanXin.AppApi.Controllers.Share
{
    /// <summary>
    /// 记录控制器
    /// </summary>
    public class Share_RecordController : ApiController
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 新增点赞 APP
        /// <summary>
        /// 新增取消点赞 APP
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Save(RecordPraiseViewModel post)
        {
            var type = 1;
            var user = (Seagull2Identity)User.Identity;
            try
            {
                if (string.IsNullOrWhiteSpace(post.TargetCode))
                {
                    return Ok(new BaseView()
                    {
                        State = false,
                        Message = "TargetCode不能为空字符串"
                    });
                }
                //判断是当前用户是否对该文章点赞
                var currModel = RecordAdapter.Instance.Load(m =>
                {
                    m.AppendItem("TargetCode", post.TargetCode);
                    m.AppendItem("Type", type);
                    m.AppendItem("Creator", user.Id);
                }).SingleOrDefault();
                if (currModel != null)
                {
                    //执行取消点赞操作
                    RecordAdapter.Instance.Delete(currModel);
                }
                else
                {
                    //执行新增点赞操作
                    var model = new RecordModel();
                    model.Code = Guid.NewGuid().ToString();
                    model.TargetCode = post.TargetCode;
                    model.Type = type;
                    model.UserName = user.DisplayName;
                    model.Creator = user.Id;
                    model.CreateTime = DateTime.Now;
                    model.ValidStatus = true;
                    RecordAdapter.Instance.Update(model);
                }
                return Ok(new BaseView()
                {
                    State = true,
                    Message = "sucess."
                });
            }
            catch (Exception e)
            {
                log.Error("点赞接口Share_Record/Save异常：" + e.Message);
                return Ok(new BaseView()
                {
                    State = false,
                    Message = e.Message
                });
            }
        }
        #endregion

        #region 记录列表 - PC
        /// <summary> 
        /// 记录列表 - PC
        /// </summary>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">页索引，从1开始</param>
        /// <param name="targetCode">目标Code</param>
        /// <param name="type">0：阅读、1：点赞</param>
        [HttpGet]
        public IHttpActionResult GetList(int pageSize, int pageIndex, string targetCode, int type)
        {
            var userCode = ((Seagull2Identity)User.Identity).Id;

            var dataCount = RecordAdapter.Instance.GetList(targetCode, type);
            var list = RecordAdapter.Instance.GetList(pageSize, pageIndex, targetCode, type);

            var data = new BaseViewPage()
            {
                DataCount = dataCount,
                PageCount = dataCount % pageSize == 0 ? dataCount / pageSize : dataCount / pageSize + 1,
                PageData = list
            };

            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
                Data = data
            });
        }
        #endregion
    }
}
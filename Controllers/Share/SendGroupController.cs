using Newtonsoft.Json;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Share;
using Seagull2.YuanXin.AppApi.Models.Share;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using log4net;
using System.Transactions;
using MCS.Library.Data;

namespace Seagull2.YuanXin.AppApi.Controllers.Share
{
    /// <summary>
    /// 发送范围群组控制器
    /// </summary>
    public class Share_SendGroupController : ApiController
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 保存群组
        /// <summary>
        /// 保存群组
        /// </summary>
        [HttpPost]
        public IHttpActionResult Save(SendGroupFullViewModel model)
        {
            var userCode = ((Seagull2Identity)User.Identity).Id;

            if (string.IsNullOrWhiteSpace(model.Group.Code))
            {
                model.Group.Code = Guid.NewGuid().ToString();
            }

            //更新群组
            SendGroupAdapter.Instance.Update(new SendGroupModel()
            {
                Code = model.Group.Code,
                Name = model.Group.Name,
                Creator = userCode,
                CreateTime = DateTime.Now,
                Modifier = userCode,
                ModifyTime = DateTime.Now,
                ValidStatus = true
            });

            //更新群组人员
            SendGroupPersonAdapter.Instance.Delete(p => p.AppendItem("SendGroupCode", model.Group.Code));
            model.Persons.ForEach(person =>
            {
                SendGroupPersonAdapter.Instance.Update(new SendGroupPersonModel()
                {
                    Code = Guid.NewGuid().ToString(),
                    SendGroupCode = model.Group.Code,
                    UserCode = person.Code,
                    UserName = person.Name,
                    Creator = userCode,
                    CreateTime = DateTime.Now,
                    Modifier = userCode,
                    ModifyTime = DateTime.Now,
                    ValidStatus = true
                });
            });
            ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "操作了应用管理-营销学院-群组管理-群组");
            return Ok(new BaseView()
            {
                State = true,
                Message = "success."
            });
        }
        #endregion

        #region 获取群组详情
        /// <summary>
        /// 获取群组详情
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetModel(string code)
        {
            var list = SendGroupAdapter.Instance.Load(p => p.AppendItem("Code", code));
            if (list.Count != 1)
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = "没有找到群组相关信息！"
                });
            }
            var group = list[0];

            var persons = SendGroupPersonAdapter.Instance.Load(p => p.AppendItem("SendGroupCode", group.Code));

            var viewGroup = new SendGroupViewModel()
            {
                Code = group.Code,
                Name = group.Name
            };

            var viewPersons = new List<SendGroupPersonViewModel>();
            persons.ForEach(person =>
            {
                viewPersons.Add(new SendGroupPersonViewModel()
                {
                    Code = person.UserCode,
                    Name = person.UserName
                });
            });

            return Ok(new BaseView()
            {
                State = true,
                Message = "sucess.",
                Data = new SendGroupFullViewModel()
                {
                    Group = viewGroup,
                    Persons = viewPersons
                }
            });
        }
        #endregion

        #region 获取群组列表
        /// <summary>
        /// 获取群组列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetList()
        {
            var list = SendGroupAdapter.Instance.Load(p => p.AppendItem("ValidStatus", true));
            return Ok(new BaseView()
            {
                State = true,
                Message = "sucess.",
                Data = list
            });
        }
        #endregion

        #region 删除群组
        /// <summary>
        /// 删除群组
        /// </summary>
        [HttpGet]
        public IHttpActionResult Delete(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = "参数code为null或者空字符串"
                });
            }
            else
            {
                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    SendGroupPersonAdapter.Instance.Delete(m => m.AppendItem("SendGroupCode", code));
                    SendGroupAdapter.Instance.Delete(m => m.AppendItem("Code", code));
                    scope.Complete();
                }
                ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "删除了应用管理-营销学院-群组管理-群组");
                return Ok(new BaseView()
                {
                    State = true,
                    Message = "删除成功"
                });
            }
        }
        #endregion
    }
}
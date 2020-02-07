using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.ActivityNew;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Models.ActivityNew;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.ActivityNew;

namespace Seagull2.YuanXin.AppApi.Controllers.ActivityNew
{
    /// <summary>
    /// 活动报名 Controller
    /// </summary>
    public class ActivityApplyInfoController : ApiController
    {
        #region 活动报名
        /// <summary>
        /// 活动报名
        /// </summary>
        [HttpPost]
        public IHttpActionResult Apply(ActivityApplyInfoViewModel.ApplyViewModel model)
        {
            if (model.UserList.Count < 1)
            {
                return Ok(new BaseView
                {
                    State = false,
                    Message = "报名人数不能小于1人！",
                });
            }
            var infoModel = ActivityInfoAdapter.Instance.Load(m => m.AppendItem("Code", model.ActivityCode)).SingleOrDefault();
            if (infoModel == null)
            {
                return Ok(new BaseView
                {
                    State = false,
                    Message = "该活动不存在！",
                });
            }
            if (DateTime.Now > infoModel.ApplyEndTime)
            {
                return Ok(new BaseView
                {
                    State = false,
                    Message = "报名已结束！",
                });
            }
            var successList = new List<ActivityApplyInfoViewModel.ApplyUserInfoViewModel>();
            var failList = new List<ActivityApplyInfoViewModel.ApplyUserInfoViewModel>();
            model.UserList.ForEach(m =>
            {
                // 判断手机号是否已经报名
                var isExist = ActivityApplyInfoAdapter.Instance.Exists(w => w.AppendItem("ActivityCode", model.ActivityCode).AppendItem("PhoneNumber", m.PhoneNumber));
                if (isExist)
                {
                    failList.Add(m);
                    return;
                }

                // 根据手机号查询海鸥用户信息
                var users = ContactsAdapter.Instance.Load(w =>
                {
                    w.AppendItem("MP", m.PhoneNumber);
                    w.AppendItem("MP", "+86" + m.PhoneNumber);
                    w.LogicOperator = MCS.Library.Data.Builder.LogicOperatorDefine.Or;
                });

                // 写入报名信息
                var applyInfoModel = new ActivityApplyInfoModel()
                {
                    Code = Guid.NewGuid().ToString(),
                    ActivityCode = model.ActivityCode,
                    UserCode = users.Count > 0 ? users[0].ObjectID : string.Empty,
                    UserName = m.UserName,
                    PhoneNumber = m.PhoneNumber,
                    Creator = ((Seagull2Identity)User.Identity).Id,
                    CreateTime = DateTime.Now,
                    ValidStatus = true
                };
                ActivityApplyInfoAdapter.Instance.Update(applyInfoModel);
                successList.Add(m);
            });
            return Ok(new BaseView
            {
                State = true,
                Message = "success.",
                Data = new
                {
                    Success = successList,
                    Fail = failList
                }
            });
        }
        #endregion

        #region 取消报名
        /// <summary>
        /// 取消报名（用户自己取消）
        /// </summary>
        [HttpPost]
        public IHttpActionResult CancelByUser(ActivityApplyInfoViewModel.CancelByUserViewModel post)
        {
            var user = (Seagull2Identity)User.Identity;

            // 判断活动是否存在
            var activity = ActivityInfoAdapter.Instance.Load(w => w.AppendItem("Code", post.ActivityCode)).SingleOrDefault();
            if (activity == null)
            {
                return Ok(new BaseView
                {
                    State = false,
                    Message = "不存在该活动！"
                });
            }

            // 判断是否报名了该活动
            var coll = ActivityApplyInfoAdapter.Instance.Load(s => s.AppendItem("ActivityCode", post.ActivityCode).AppendItem("UserCode", user.Id));
            if (coll.Count < 1)
            {
                return Ok(new BaseView
                {
                    State = false,
                    Message = "你还没有报名该活动！"
                });
            }

            // 删除报名记录
            ActivityApplyInfoAdapter.Instance.Delete(s => s.AppendItem("ActivityCode", post.ActivityCode).AppendItem("UserCode", user.Id));

            return Ok(new BaseView
            {
                State = true,
                Message = "取消成功！"
            });
        }
        /// <summary>
        /// 取消报名（活动发起者取消）
        /// </summary>
        [HttpPost]
        public IHttpActionResult CancelByActivityCreator(ActivityApplyInfoViewModel.CancelByActivityCreatorViewModel post)
        {
            var user = (Seagull2Identity)User.Identity;

            // 获取报名信息
            var apply = ActivityApplyInfoAdapter.Instance.Load(w => w.AppendItem("Code", post.ApplyCode)).SingleOrDefault();
            if (apply == null)
            {
                return Ok(new BaseView
                {
                    State = false,
                    Message = "不存在该报名信息！"
                });
            }

            // 判断当前用户是否为该活动的发起者
            var isExist = ActivityInfoAdapter.Instance.Exists(w => w.AppendItem("Code", apply.ActivityCode).AppendItem("Creator", user.Id));
            if (!isExist)
            {
                return Ok(new BaseView
                {
                    State = false,
                    Message = "你没有权限移除该报名用户！"
                });
            }

            // 删除报名信息
            ActivityApplyInfoAdapter.Instance.Delete(s => s.AppendItem("Code", post.ApplyCode));

            return Ok(new BaseView
            {
                State = true,
                Message = "取消成功！"
            });
        }
        #endregion
    }
}
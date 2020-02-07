using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    /// <summary>
    /// 用户评论视图数据源(wangsf)
    /// </summary>
    public class UserCommentViewData : UserComment
    {
        /// <summary>
        /// 行号
        /// </summary>
        public string RowNumberForSplit { get; set; }

        /// <summary>
        /// 评论用户编码
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 评论用户信息
        /// </summary>
        public ContactsModel UserInfo
        {
            get
            {
                return ContactsAdapter.Instance.LoadByCode(this.UserID);
            }
        }
        /// <summary>
        /// 评论用户头像
        /// </summary>
        public string UserHeadUrl
        {
            get
            {
                var result = UserHeadPhotoService.GetUserHeadPhoto(this.UserID);
                return result;
            }
        }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime CommentTime { get; set; }

        public string CommentTimeFormate
        {
            get
            {
                return CommonHelper.APPDateFormateDiff(CommentTime, DateTime.Now);
            }
        }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// 装配回复数据
        /// </summary>
        /// <param name="ucmt"></param>
        /// <param name="recomenViewList"></param>
        /// <param name="reCommentList"></param>
        private void AssembleData(UserComment ucmt, ReCommentViewCollection recomenViewList, UserCommentCollection reCommentList)
        {
            foreach (UserComment ur in reCommentList)
            {
                ReCommentView rcv = new ReCommentView();
                rcv.Code = ur.Code;
                rcv.Content = ur.Content;
                rcv.CreateTime = ur.CreateTime;
                rcv.Creator = ur.Creator;
                rcv.HavePicture = ur.HavePicture;
                rcv.ParentCode = ur.ParentCode;
                rcv.ProjectCode = ur.ProjectCode;
                rcv.WorksCode = ur.WorksCode;
                rcv.BeReplyUser = ucmt.UserInfo;
                rcv.BeReplyContent = ucmt.Content;
                rcv.BeReplyCode = ucmt.Code;
                recomenViewList.Add(rcv);
                if (ur.OneOfCommentColl != null && ur.OneOfCommentColl.Count > 0)
                {
                    this.AssembleData(rcv, recomenViewList, ur.OneOfCommentColl);
                }
            }
        }

        /// <summary>
        /// 回复评论
        /// </summary>
        public IEnumerable<ReCommentView> ReCommentListByTime
        {
            get
            {
                ReCommentViewCollection recomenViewList = new ReCommentViewCollection();
                AssembleData(this, recomenViewList, this.OneOfCommentColl);
                return recomenViewList.OrderBy(o => o.CreateTime);
            }
        }
    }

    public class UserCommentViewDataCollection : EditableDataObjectCollectionBase<UserCommentViewData>
    {
        /// <summary>
        /// 转化为ListDataView
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="totalCount">总行数</param>
        /// <returns></returns>
        public ListDataView ToListDataView(int pageIndex, int pageSize, int totalCount)
        {
            var result = new ListDataView
            {
                PageIndex = pageIndex,
                PageCount = totalCount % pageSize > 0 ? totalCount / pageSize + 1 : totalCount / pageSize,
                TotalCount = totalCount,
                ListData = this
            };

            return result;
        }
    }
}

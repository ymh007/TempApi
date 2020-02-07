using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    /// <summary>
    /// 
    /// </summary>
    public class CommentViewData : UserComment
    {
        /// <summary>
        /// 行号
        /// </summary>
        public string RowNumberForSplit { get; set; }
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
                if (ur.OneOfCommentColl != null && ur.OneOfCommentColl.Count>0)
                {
                    this.AssembleData(rcv, recomenViewList, ur.OneOfCommentColl);
                }
            }
        }

        /// <summary>
        /// 所有回复根据时间排序列表
        /// </summary>
        public IEnumerable<ReCommentView> ReCommentListByTime
        {
            get
            {
                ReCommentViewCollection recomenViewList=new ReCommentViewCollection();
                AssembleData(this, recomenViewList, this.OneOfCommentColl);
                return recomenViewList.OrderBy(o => o.CreateTime);                
            }
        }
    }


    public class CommentViewDataCollection : EditableDataObjectCollectionBase<CommentViewData>
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


    /// <summary>
    /// 回复实体
    /// </summary>
    public class ReCommentView : UserComment
    {
        /// <summary>
        /// 被回复评论作者
        /// </summary>
        public ContactsModel BeReplyUser { get; set; }
        /// <summary>
        /// 被回复的信息内容
        /// </summary>
        public String  BeReplyContent { get; set; }
        /// <summary>
        /// 被回复的信息Code
        /// </summary>
        public String BeReplyCode { get; set; }
    }
    public class ReCommentViewCollection : EditableDataObjectCollectionBase<ReCommentView>
    { 
    
    }


}

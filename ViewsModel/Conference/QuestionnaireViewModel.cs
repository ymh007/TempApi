using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Conference
{
    /// <summary>
    /// 问卷同步保存 ViewModel
    /// </summary>
    public class QuestionnaireSyncViewModel
    {
        /// <summary>
        /// 会议编码
        /// </summary>
        public string ConferenceCode { set; get; }
        /// <summary>
        /// 问卷编码
        /// </summary>
        public string QuestionnaireCode { set; get; }
    }

    /// <summary>
    /// 问卷版本基本信息 ViewModel
    /// </summary>
    public class QuestionnaireVersionViewModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 版本
        /// </summary>
        public int Versions { set; get; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { set; get; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { set; get; }
    }
}
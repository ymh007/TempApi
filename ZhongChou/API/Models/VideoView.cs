using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    /// <summary>
    /// 直播view
    /// </summary>
    public class VideoView
    {
        public VideoView(string projectCode)
        {
            ActivityBroadcastInfo = ActivityBroadcastAdapter.Instance.LoadByProjectCode(projectCode);
            HaveVideo = 0;
            Countdown = 0;
            if (ActivityBroadcastInfo != null)
            {
                LiveVideoInfo = LiveVideoAdapter.Instance.LoadByBroadCastCode(ActivityBroadcastInfo.Code);
                if (DateTime.Now < ActivityBroadcastInfo.StartTime)
                {
                    Countdown = (int)(ActivityBroadcastInfo.StartTime-DateTime.Now).TotalSeconds;
                }
                else
                {
                    Countdown = 0;
                }
            }
            if (LiveVideoInfo != null)
            {
                HaveVideo = 1;
                if (LiveVideoInfo.IsRecord)
                {
                    HaveVideo = 2;
                }
            }
        }
        /// <summary>
        /// 直播详情
        /// </summary>
        public ActivityBroadcast ActivityBroadcastInfo { get; set; }

        /// <summary>
        /// 直播内容
        /// </summary>
        public LiveVideo LiveVideoInfo { get; set; }

        /// <summary>
        /// 直播状态
        /// 0未开始
        /// 1直播中
        /// 2已结束
        /// </summary>
        public int HaveVideo { get; set; }

        /// <summary>
        /// 距离开播时间
        /// </summary>
        public int Countdown { get; set; }

    }
}
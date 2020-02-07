using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.Adapter.Feedback;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Feedback
{
    /// <summary>
    /// 意见反馈 ViewModel
    /// </summary>
    public class FeedbackViewModel
    {
        /// <summary>
        /// 唯一编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 操作系统
        /// </summary>
        public string System { set; get; }
        /// <summary>
        /// 网络类型
        /// </summary>
        public string Network { set; get; }
        /// <summary>
        /// 手机型号
        /// </summary>
        public string Version { set; get; }
        /// <summary>
        /// APP版本
        /// </summary>
        public string AppVersion { set; get; }
        /// <summary>
        /// 反馈内容
        /// </summary>
        public string Content { set; get; }
        /// <summary>
        /// 反馈时间
        /// </summary>
        public string DateTime { set; get; }
        /// <summary>
        /// 反馈人编码
        /// </summary>
        public string UserCode { set; get; }
        /// <summary>
        /// 反馈人名称
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 反馈人头像
        /// </summary>
        public string UserHead
        {
            get
            {
                return UserHeadPhotoService.GetUserHeadPhoto(UserCode);
            }
        }
        /// <summary>
        /// 反馈图片列表
        /// </summary>
        public List<string> ImageList
        {
            get
            {
                var data = FeedbackImageAdapter.Instance.Load(p => p.AppendItem("FeedbackCode", Code));
                var list = new List<string>();
                data.ForEach(img =>
                {
                    if (img.ImageUrl.StartsWith("YuanXin-File"))
                    {
                        list.Add(FileService.DownloadFile(img.ImageUrl));
                    }
                    else
                    {
                        list.Add(img.ImageUrl);
                    }

                });
                return list;
            }
        }
        /// <summary>
        /// 是否回复
        /// </summary>
        public bool IsReply { set; get; }
        /// <summary>
        /// 回复人编码
        /// </summary>
        public string ReplyUserCode { set; get; }
        /// <summary>
        /// 回复人名称
        /// </summary>
        public string ReplyUserName { set; get; }
        /// <summary>
        /// 回复内容
        /// </summary>
        public string ReplyContent { set; get; }
        /// <summary>
        /// 回复时间
        /// </summary>
        public string ReplyDateTime { set; get; }

        /// <summary>
        /// 回复设备 1 app  0 pc 
        /// </summary>
        public int ReplyWay { get; set; }


        /// <summary>
        /// 是否标记 
        /// </summary>
        public bool Mark { get; set; }



    }

    public class FeedbackReplyViewModel
    {

        /// <summary>
        /// 唯一编码
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 回复人名称
        /// </summary>
        public string ReplyUserName { set; get; }

        /// <summary>
        /// 回复内容
        /// </summary>
        public string ReplyContent { set; get; }
        /// <summary>
        /// 回复时间
        /// </summary>
        public string ReplyDateTime { set; get; }

        /// <summary>
        /// 回复设备 1 app  0 pc 
        /// </summary>
        public int ReplyWay { get; set; }

        /// <summary>
        /// 反馈图片列表
        /// </summary>
        public List<string> ImageList
        {
            get
            {
                var data = FeedbackImageAdapter.Instance.Load(p => p.AppendItem("FeedbackCode", Code));
                var list = new List<string>();
                data.ForEach(img =>
                {
                    if (img.ImageUrl.StartsWith("YuanXin-File"))
                    {
                        list.Add(FileService.DownloadFile(img.ImageUrl));
                    }
                    else
                    {
                        list.Add(img.ImageUrl);
                    }
                });
                return list;
            }
        }

    }
}
using System;

namespace Seagull2.YuanXin.AppApi.Models
{
    public class ListItem
    {
        public string Title { get; set; }
        public string NewsType { get; set; }
        public string PublishingPageContent { get; set; }
        public string NewsAddress { get; set; }
        public string VideoAddress { set; get; }
        public string EncodedAbsUrl { get; set; }
        public string ServerUrl { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
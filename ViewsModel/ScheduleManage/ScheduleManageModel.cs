using Newtonsoft.Json;
using System.Collections.Generic;


namespace Seagull2.YuanXin.AppApi.ViewsModel.ScheduleManage
{
    public class ParentResult
    {
        public string date { get; set; }
        public string weekday { get; set; }
        public List<ChildResult> data { get; set; }

    }
    public class ChildResult
    {
        //task
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string creator { get; set; }
        //confence
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string city { get; set; }

        // 列表
        public bool IsFromMe { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rstartTime { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rendTime { get; set; }

        // 日历
        public string type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string key { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string scheduleCode { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string title { get; set; }
        public int reminderTime { get; set; }
        public int flag { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string address { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string introduction { get; set; }
        public bool isOver { get; set; }
        public bool isMeeting { get; set; }
        public bool isCancelled { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string outlookId { get; set; }
        public bool isOutlook { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<object> people { get; set; }
    }
}
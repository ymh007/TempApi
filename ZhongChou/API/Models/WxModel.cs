using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{

    public class Wx_Pay_Model
    {
        public int retcode { get; set; }
        public string retmsg { get; set; }
        public string appid { get; set; }
        public string noncestr { get; set; }
        public string package { get; set; }
        public string partnerid { get; set; }
        public string prepayid { get; set; }
        public string timestamp { get; set; }
        public string sign { get; set; }
    }

}
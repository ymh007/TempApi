using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Test
{
    /// <summary>
    /// 
    /// </summary>
    public class TestViewModel
    {
        /// <summary>
        /// test id
        /// </summary>
        public string id { set; get; }

        /// <summary>
        /// test名字
        /// </summary>
        public string name { set; get; }

        /// <summary>
        /// test请求路径
        /// </summary>
        public string url { set; get; }

        /// <summary>
        /// testalex
        /// </summary>
        public int alexa { set; get; }

        /// <summary>
        /// test 城市
        /// </summary>
        public string country { set; get; }
    }
}
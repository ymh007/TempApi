using System;
using System.Collections.Generic;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Pay.Wxpay
{
    public class WxPayException : Exception 
    {
        public WxPayException(string msg) : base(msg) 
        {

        }
     }
}
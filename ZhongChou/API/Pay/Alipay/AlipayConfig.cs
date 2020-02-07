using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;

namespace Seagull2.YuanXin.AppApi.Pay.Alipay
{
    /// <summary>
    /// 类名：Config
    /// 功能：基础配置类
    /// 详细：设置帐户有关信息及返回路径
    /// 版本：3.3
    /// 日期：2012-07-05
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// 
    /// 如何获取安全校验码和合作身份者ID
    /// 1.用您的签约支付宝账号登录支付宝网站(www.alipay.com)
    /// 2.点击“商家服务”(https://b.alipay.com/order/myOrder.htm)
    /// 3.点击“查询合作者身份(PID)”、“查询安全校验码(Key)”
    /// </summary>
    public class Config
    {
        #region 字段
        private static string partner = "";
        private static string private_key = "";
        private static string public_key = "";
        private static string input_charset = "";
        private static string sign_type = "";
        private static string seller = "";
        #endregion

        static Config()
        {
            //↓↓↓↓↓↓↓↓↓↓请在这里配置您的基本信息↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

            //合作身份者ID，以2088开头由16位纯数字组成的字符串
            partner = "2088021770519396"; 

            seller = "wojiazhongchuang@126.com";

            //商户的私钥
            private_key = HttpRuntime.AppDomainAppPath.ToString() + @"Certificate\rsa_private_key.pem";
            //private_key = @"MIICXAIBAAKBgQCWJ1IkFE8J+TY2Hku0IYc8Ex5Xi95jTBIcaVdMFUeKCIsnBr7Zdy+hFw0iiR4W9UXVs7f/wF5X1hr3G5HRWazvEkrz8dIuloy9CYu5iOzY2PwuZIHnFER4ZcnBj4e4SQWxMiaCY55x2iC22pAR+cGujEqhoF0SsNF+nxfGHVHdswIDAQABAoGBAJMul+WlwpQUJH+H0s0O0HCijOtjvum2d0pCN7/sW+OB3GS0x/6CRb2xyr4/ia8XXQzMzV90QQX4aRYvgA+lx0xQU2uC85wiVMo6ss/bxbPoJ0frOqQGXG9MD+FnPc/AW7PRdqhPz/e8uItLwYdGOAzupWfwIZdcBJ/GFMbrMlCBAkEAxQ3Dpg2PCijHbf55n2BjMfzgPAzQc7xGhErKQgiJpKDi9E7/c4k4K6PdnveeH0X9qcVUJoMRbf9ujIIoFPEHVQJBAMMR+bD1yVoUVfPPAkLbA6gJYRXD5lsoh+yX+GwKmXLu9Xc/qGdIZS4CJAHXPrWyIVUW+FVHUzQLRehrnGADQOcCQCs78OA++1JETjVA7xhbwofWLrCeMyIhfetKqYPBccmwDvuBVaTYx30zr81QEN5VsekMxYDJowpaT7v+VqL5mx0CQBAMmo6EqlxO8ANvNLBrdJGOs/4mb/1wCD7fAgFhnd5m6qs1AZX9ztVKN8wu+WnZjgSCL0xU4fOWOdo1gFgl7GECQCNTZPwl2TP6Dfwq3cBxeGX5nTKG6iegJbsmZpAvyvZN+7ocY+NpOCGCtzvZqrKTEFSPT41qlqgpP7/ablAdLgk=";
            //支付宝的公钥，无需修改该值
            public_key = @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCnxj/9qwVfgoUh/y2W89L6BkRAFljhNhgPdyPuBV64bfQNN1PjbCzkIM6qRdKBoLPXmKKMiFYnkd6rAoprih3/PrQEB/VsW8OoM8fxn67UDYuyBTqA23MML9q1+ilIZwBC2AQ2UBVOrFXfFl75p6/B5KsiNG9zpgmLCUYuLkxpLQIDAQAB";

            //↑↑↑↑↑↑↑↑↑↑请在这里配置您的基本信息↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑



            //字符编码格式 目前支持 gbk 或 utf-8
            input_charset = "gbk";

            //签名方式，选择项：RSA、DSA、MD5
            sign_type = "RSA";
        }

        #region 属性
        /// <summary>
        /// 获取或设置合作者身份ID
        /// </summary>
        public static string Partner
        {
            get { return partner; }
            set { partner = value; }
        }

        public static string Seller
        {
            get { return seller; }
            set { seller = value; }
        }

        /// <summary>
        /// 获取或设置商户的私钥
        /// </summary>
        public static string Private_key
        {
            get { return private_key; }
            set { private_key = value; }
        }

        /// <summary>
        /// 获取或设置支付宝的公钥
        /// </summary>
        public static string Public_key
        {
            get { return public_key; }
            set { public_key = value; }
        }

        /// <summary>
        /// 获取字符编码格式
        /// </summary>
        public static string Input_charset
        {
            get { return input_charset; }
        }

        /// <summary>
        /// 获取签名方式
        /// </summary>
        public static string Sign_type
        {
            get { return sign_type; }
        }
        #endregion
    }
}
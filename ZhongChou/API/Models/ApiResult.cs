using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    /// <summary>
    /// APP请求通用返回结果
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 状态码(成功200，失败500)，使用ResultCodeEnum
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 请求错误的信息
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 请求的数据
        /// </summary>
        public object Data { get; set; }

    }

    public enum ResultCodeEnum
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 200,
        /// <summary>
        /// 失败
        /// </summary>
        Error = 500
    }
}
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    public class UserVoteForm
    {
        /// <summary>
        /// 活动作品编码
        /// </summary>
        public string ActivityWorksCode { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectCode { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }



        public UserVote ToUserVote()
        {
            return new UserVote
            {
                Code = Guid.NewGuid().ToString(),
                ActivityWorksCode = this.ActivityWorksCode,
                CreateTime = DateTime.Now,
                Creator = this.Creator,
                ProjectCode = this.ProjectCode
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Seagull2.YuanXin.AppApi.Models;

namespace Seagull2.YuanXin.AppApi.Services
{
    public interface IAttentionService
    {
        //查询关注
        Task<IEnumerable<AttentionModel>> LoadAttention(string userCode);

        //增加我的关注
        Task<string> AddAttention(string userCode,string businessProjectCode,string businessProjectName,string  businessCode,int  attentionType);

        //删除清空我的关注
        Task<string> EmptyAndAddOrAttention(string userCode,string listbusinessProjectCode,bool IsEmpty);

        //我的关注统计
        Task<int> StatisticsAttention(string userCode);
    }
}
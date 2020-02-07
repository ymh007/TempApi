using Seagull2.YuanXin.AppApi.Models;
using SinoOcean.Seagull2.Framework.MasterData;
//using SOS2.OAPortal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Services
{
    public interface IPlanManageRate
    {
        Task<IEnumerable<EipKeyPointAchievingRateOfProjectEntity>> GetGroupYearEipKeyPointRateTable(string time);
        Task<IEnumerable<string>> WeiCeshi();
    }
}
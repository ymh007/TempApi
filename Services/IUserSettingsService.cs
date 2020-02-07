using MCS.Library.SOA.DataObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Seagull2.YuanXin.AppApi.Services
{
    public interface IUserSettingsService
    {
        Task<IEnumerable<UserSettings>> LoadA(string deliverTime = "");
    }
}

using SinoOcean.Seagull2.Framework.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Services
{
    public interface IEmployeeService
    {
        string LoadEmployee(string Id);
    }
}

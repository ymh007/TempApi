using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.OGUPermission;
using SinoOcean.Seagull2.Framework.MasterData;
using MCS.Library.SOA.DataObjects;

namespace Seagull2.YuanXin.AppApi.Models
{
    public class EmployeeExtend
    {
        /// <summary>
        /// 获取员工电话号码
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetEmployeePhone(IUser user)
        {
            UserInfoExtendCollection extendInfos = UserInfoExtendDataObjectAdapter.Instance.GetUserInfoExtendInfoCollectionByUsers(new List<IUser> { user });
            if (extendInfos != null)
            {
                if (extendInfos.Count > 0)
                    return extendInfos[0].Mobile;
                else
                    return "";
            }
            else
            {
                return "";
            }
        }
    }
}
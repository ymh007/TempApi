using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using Seagull2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Services.Meeting
{
    /// <summary>
    /// 查询人员服务类
    /// </summary>
    public class UserAdapter : UpdatableAndLoadableAdapterBase<User, UserCollection>
    {
        public static UserAdapter Instance = new UserAdapter();
        public static UserCollection getUserByLoginNameOrDisplayName(string searchName)
        {
            searchName = searchName.ToLower();
            User user = new User();
            //查询主责且未离职的员工
            string sql = string.Format(@"SELECT * FROM dbo.User_Syn WHERE (DISPLAY_NAME LIKE '%{0}%' OR LOGON_NAME LIKE '%{1}%') AND DISPLAY_NAME NOT LIKE '(已离职)%' AND SIDELINE=0", searchName, searchName);
            UserCollection ucColl = Instance.QueryData(sql);
            return ucColl;
        }

        protected override string GetConnectionName()
        {
            return "SubjectDB_Finance";
        }
    }
    #region 辅助类
    [ORTableMapping("dbo.User_Syn")]
    [Serializable]
    [XElementSerializable]
    public class User
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [ORFieldMapping("USER_GUID")]
        public string Code { get; set; }

        /// <summary>
        /// 父Id
        /// </summary>
        [ORFieldMapping("PARENT_GUID")]
        public string ParentCode { get; set; }

        /// <summary>
        /// 用户登录名(邮箱或手机号)
        /// </summary>
        [ORFieldMapping("LOGON_NAME")]
        public string LogonName { get; set; }

        /// <summary>
        /// 用户显示名称
        /// </summary>
        [ORFieldMapping("DISPLAY_NAME")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 全路径名称
        /// </summary>
        [ORFieldMapping("ALL_PATH_NAME")]
        public string All_Path_Name { get; set; }

        /// <summary>
        /// 是否主责
        /// </summary>
        [ORFieldMapping("SIDELINE")]
        public int SideLine { get; set; }
    }
    [Serializable]
    [XElementSerializable]
    public class UserCollection : EditableDataObjectCollectionBase<User>
    {

    }
    #endregion
}
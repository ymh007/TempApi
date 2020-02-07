using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 认证用户信息
    /// </summary>
    [Serializable]
    [ORTableMapping("OAuth.UserInfo")]
    public class OAuthUserInfo
    {
        /// <summary>
        /// UserID
        /// </summary>
        [ORFieldMapping("UserID")]
        public string UserID { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        [ORFieldMapping("UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// LoginName
        /// </summary>
        [ORFieldMapping("LoginName")]
        public string LoginName { get; set; }

        /// <summary>
        /// PhoneNumber
        /// </summary>
        [ORFieldMapping("PhoneNumber")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// UserSource
        /// </summary>
        [ORFieldMapping("UserSource")]
        public int UserSource { get; set; }

        /// <summary>
        /// UserEmail
        /// </summary>
        [ORFieldMapping("UserEmail")]
        public string UserEmail { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [ORFieldMapping("Status")]
        public int Status { get; set; }

        [NoMapping]
        public OAuthUserHeadPhoto HeadPhoto { get {
            return OAuthUserHeadPhotoAdapter.Instance.LoadByUserCode(UserID);
        } }
    }
    [Serializable]
    public class OAuthUserInfoCollection : EditableDataObjectCollectionBase<OAuthUserInfo>
    {
    }


    public class OAuthUserInfoAdapter : UpdatableAndLoadableAdapterBase<OAuthUserInfo, OAuthUserInfoCollection>
    {
        public static readonly OAuthUserInfoAdapter Instance = new OAuthUserInfoAdapter();

        private OAuthUserInfoAdapter()
        {
        }
        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }
        /// <summary>
        /// e
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        public OAuthUserInfo LoadByUserCode(string userID)
        {
            return this.Load(where =>
            {
                where.AppendItem("UserID", userID);
            }).FirstOrDefault();
        }
    }
     
}

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
    /// 认证用户头像
    /// </summary>
    [Serializable]
    [ORTableMapping("OAuth.UserHeadPhoto")]
    public class OAuthUserHeadPhoto
    {
        /// <summary>
        /// UserID
        /// </summary>
        [ORFieldMapping("UserID")]
        public string UserID { get; set; }

        /// <summary>
        /// VersionStartTime
        /// </summary>
        [ORFieldMapping("VersionStartTime")]
        public DateTime VersionStartTime { get; set; }

        /// <summary>
        /// AppID
        /// </summary>
        [ORFieldMapping("AppID")]
        public string AppID { get; set; }

        /// <summary>
        /// LogoImageUrl
        /// </summary>
        [ORFieldMapping("LogoImageUrl")]
        public string LogoImageUrl { get; set; }

        /// <summary>
        /// VersionEndTime
        /// </summary>
        [ORFieldMapping("VersionEndTime")]
        public DateTime VersionEndTime { get; set; }
    }
     
    [Serializable]
    public class OAuthUserHeadPhotoCollection : EditableDataObjectCollectionBase<OAuthUserHeadPhoto>
    {
    }

    
    public class OAuthUserHeadPhotoAdapter : UpdatableAndLoadableAdapterBase<OAuthUserHeadPhoto, OAuthUserHeadPhotoCollection>
    {
        public static readonly OAuthUserHeadPhotoAdapter Instance = new OAuthUserHeadPhotoAdapter();

        private OAuthUserHeadPhotoAdapter()
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
        public OAuthUserHeadPhoto LoadByUserCode(string userID)
        {
            return this.Load(where =>
            {
                where.AppendItem("UserID", userID);
            }).FirstOrDefault();
        }
    }
}

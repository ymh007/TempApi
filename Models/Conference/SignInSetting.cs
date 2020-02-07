using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 签到设置表
    /// </summary>
    [ORTableMapping("office.SignInSetting")]
    public class SignInSetting
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID { get; set; }
        /// <summary>
        /// 会议编码
        /// </summary>
        [ORFieldMapping("ConferenceID")]
        public string ConferenceID { get; set; }
        /// <summary>
        /// 签到背景图片名称
        /// </summary>
        [ORFieldMapping("BackgroundImageName")]
        public string BackgroundImageName { get; set; }
        /// <summary>
        /// 签到页背景颜色
        /// </summary>
        [ORFieldMapping("BackgroundColor")]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// 是否使用的默认图片
        /// </summary>
        public bool IsDefaultBackgroundImage { get; set; }

        /// <summary>
        /// 使用第几张默认图片
        /// </summary>
        public int IsDefaultSelectImageIndex { get; set; }

        /// <summary>
        /// 签到城市
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }
    }
    public class SignInSettingCollection : EditableDataObjectCollectionBase<SignInSetting> { }
    public class SignInSettingAdapter : BaseAdapter<SignInSetting, SignInSettingCollection>
    {
        public static SignInSettingAdapter Instance = new SignInSettingAdapter();
        private string ConnectionString = ConnectionNameDefine.YuanXinBusiness;
        public SignInSettingAdapter()
        {
            BaseConnectionStr = this.ConnectionString;
        }

        public SignInSetting GetSignInSettingById(string id) {
            return this.Load(t => {
                t.AppendItem("ID",id);
                t.AppendItem("ValidStatus",true);
            }).FirstOrDefault();
        }

        public SignInSetting GetSignInSettingByConferenceID(string conferenceID)
        {
            return this.Load(t => {
                t.AppendItem("ConferenceID", conferenceID);
                t.AppendItem("ValidStatus", true);
            }).FirstOrDefault();
        }
    }
}
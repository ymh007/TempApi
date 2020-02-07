using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Seagull2.YuanXin.AppApi.Enum;

namespace Seagull2.YuanXin.AppApi.ViewsModel.BusinessCard
{
    #region 个人名片基本信息 ViewModel
    /// <summary>
    /// 个人名片基本信息 ViewModel
    /// </summary>
    public class BusinessCardBaseViewModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// logo名称
        /// </summary>
        public string LogoKey { set; get; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 职务
        /// </summary>
        public string Position { set; get; }
        /// <summary>
        /// 公司
        /// </summary>
        public string Company { set; get; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { set; get; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { set; get; }
        /// <summary>
        /// 地址列表
        /// </summary>
        public List<string> Address { set; get; }
        /// <summary>
        /// 座机列表
        /// </summary>
        public List<string> Phone { set; get; }
    }
    #endregion

    #region 名片保存 ViewModel
    /// <summary>
    /// 名片保存 ViewModel
    /// </summary>
    public class BusinessCardSaveViewModel : BusinessCardBaseViewModel
    {

    }
    #endregion

    #region 名片列表 ViewModel
    /// <summary>
    /// 名片列表 ViewModel
    /// </summary>
    public class BusinessCardListViewModel : BusinessCardBaseViewModel
    {
        /// <summary>
        /// 地址列表
        /// </summary>
        public new List<string> Address
        {
            get
            {
                var list = new List<string>();
                Adapter.BusinessCard.BusinessCardPropertyAdapter.Instance
                    .Load(w => w.AppendItem("BusinessCardCode", base.Code).AppendItem("Type", (int)EnumBusinessCardType.Address))
                    .OrderBy(o => o.CreateTime)
                    .ToList()
                    .ForEach(item =>
                {
                    list.Add(item.TypeValue);
                });
                return list;
            }
        }
        /// <summary>
        /// 座机列表
        /// </summary>
        public new List<string> Phone
        {
            get
            {
                var list = new List<string>();
                Adapter.BusinessCard.BusinessCardPropertyAdapter.Instance
                    .Load(w => w.AppendItem("BusinessCardCode", base.Code).AppendItem("Type", (int)EnumBusinessCardType.Phone))
                    .OrderBy(o => o.CreateTime)
                    .ToList()
                    .ForEach(item =>
                {
                    list.Add(item.TypeValue);
                });
                return list;
            }
        }
        /// <summary>
        /// 分享地址
        /// </summary>
        public string ShareUrl
        {
            get
            {
                return ConfigAppSetting.OfficePath + "businessCardShare/index.html?code=" + base.Code;
            }
        }
        /// <summary>
        /// 二维码图片路径
        /// </summary>
        public string QrCodeUrl
        {
            get
            {
                return ConfigAppSetting.ApiPath + "BusinessCard/GetQrCode?code=" + base.Code;
            }
        }
    }
    #endregion

    #region 名片详情 ViewModel
    /// <summary>
    /// 名片详情 ViewModel
    /// </summary>
    public class BusinessCardModelViewModel : BusinessCardBaseViewModel
    {

    }
    #endregion
}
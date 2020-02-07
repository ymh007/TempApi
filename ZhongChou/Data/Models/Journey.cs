using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 生活家行程表
    /// </summary>
    [Serializable]
    [ORTableMapping("zc.Journey")]
    public class Journey
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        [ORFieldMapping("JourneyTime")]
        public DateTime JourneyTime { get; set; }

        /// <summary>
        /// 时间格式化
        /// </summary>
        [NoMapping]
        public string JourneyTimeFormat { get {
            return string.Format("{0:M}", JourneyTime) + " " + JourneyTime.ToString("ddd") + "" + JourneyTime.GetDateTimeFormats('t')[0].ToString(); ;
        } }

        /// <summary>
        /// 省份编码
        /// </summary>
        [ORFieldMapping("ProvinceCode")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        [ORFieldMapping("Province")]
        public string Province { get; set; }

        /// <summary>
        /// 城市编码
        /// </summary>
        [ORFieldMapping("CityCode")]
        public string CityCode { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [ORFieldMapping("City")]
        public string City { get; set; }

        /// <summary>
        /// 县区编码
        /// </summary>
        [ORFieldMapping("CountyCode")]
        public string CountyCode { get; set; }

        /// <summary>
        /// 县区名称
        /// </summary>
        [ORFieldMapping("County")]
        public string County { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [ORFieldMapping("AdressDetail")]
        public string AdressDetail { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("IsValid")]
        public bool IsValid { get; set; }

        /// <summary>
        /// 行程状态（空闲，占用，过期）
        /// </summary>
        [ORFieldMapping("Status")]
        public JourneyStatus Status { get; set; }

        /// <summary>
        /// 状态中文说明
        /// </summary>
        [NoMapping]
        public string StatusDescription
        {
            get
            {
                if (Status == JourneyStatus.None)
                {
                    if (this.JourneyTime > DateTime.Now)
                    {
                        return EnumItemDescriptionAttribute.GetDescription(Status);
                    }
                    else
                    {
                        return EnumItemDescriptionAttribute.GetDescription(JourneyStatus.Overdue);
                    }
                }
                else
                {
                    return EnumItemDescriptionAttribute.GetDescription(Status);
                }

            }

        }
    }

    /// <summary>
    /// 生活家行程表??
    /// </summary>
    [Serializable]
    public class JourneyCollection : EditableDataObjectCollectionBase<Journey>
    {
    }

    /// <summary>
    /// 生活家行程表???
    /// </summary>
    public class JourneyAdapter : UpdatableAndLoadableAdapterBase<Journey, JourneyCollection>
    {
        public static readonly JourneyAdapter Instance = new JourneyAdapter();

        private JourneyAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return DatabaseUtility.CFOWDFUNDING;
        }

        public Journey LoadByCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", code);
            }).FirstOrDefault();
        }
        public JourneyCollection LoadByUserCode(string userCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("Creator", userCode);
                where.AppendItem("IsValid", true);
            });
        }
        public void DeleteByCode(string code, bool trueDelete = false)
        {
            //默认逻辑删除
            this.SetFields("IsValid", false, where => where.AppendItem("Code", code));

            //物理删除
            if (trueDelete)
            {
                this.Delete(where => where.AppendItem("Code", code));
            }
        }

        public JourneyCollection LoadAll()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }

        public JourneyCollection LoadAll(bool isValid)
        {
            return this.Load(where => where.AppendItem("IsValid", isValid));
        }

        /// <summary>
        /// 获取所有空闲行程
        /// </summary>
        /// <param name="userCode">用户code</param>
        /// <returns></returns>
        public JourneyCollection LoadFreeJourneys(string userCode = null)
        {
            return this.Load(where =>
            {
                where.AppendItem("JourneyTime", DateTime.Now, ">");
                where.AppendItem("Status", JourneyStatus.None.ToString("D"));
                where.AppendItem("IsValid", true);
                if (userCode.IsNotEmpty()) { where.AppendItem("Creator", userCode); }
            }, o => o.AppendItem("JourneyTime", MCS.Library.Data.Builder.FieldSortDirection.Descending));
        }

        /// <summary>
        /// 获取当前用户空闲行程
        /// </summary>
        /// <returns></returns>
        public Journey LoadFreeJourney(string userCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("JourneyTime", DateTime.Now, ">");
                where.AppendItem("Status", JourneyStatus.None.ToString("D"));
                where.AppendItem("IsValid", true);
                where.AppendItem("Creator", userCode);
            }, o => o.AppendItem("JourneyTime", MCS.Library.Data.Builder.FieldSortDirection.Descending)).FirstOrDefault();
        }
        public void UpdateJourneyStatus(string journeyCode, JourneyStatus journeyStatus)
        {
            this.SetFields(
                update =>
                {
                    update.AppendItem("Status", journeyStatus.ToString("D"));
                },
                where =>
                {
                    where.AppendItem("Code", journeyCode);
                },
                this.GetConnectionName());
        }
    }


}


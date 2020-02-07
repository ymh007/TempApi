using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;

namespace Seagull2.YuanXin.AppApi.Models.Sign
{
    /// <summary>
    /// 打卡地点 Model
    /// </summary>
    [ORTableMapping("dbo.StandardPunch")]
    public class StandardPunchModel
    {
        /// <summary>
        /// 打卡地点编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 上班时间
        /// </summary>
        [ORFieldMapping("OnTime")]
        public string OnTime { get; set; }

        /// <summary>
        /// 下班时间
        /// </summary>
        [ORFieldMapping("OffTime")]
        public string OffTime { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [ORFieldMapping("Lng")]
        public string Lng { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [ORFieldMapping("Lat")]
        public string Lat { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [ORFieldMapping("Address")]
        public string Address { get; set; }

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

        /// <summary>
        /// 修改人
        /// </summary>
        [ORFieldMapping("Modifier")]
        public string Modifier { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [ORFieldMapping("ModifyTime")]
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [ORFieldMapping("IsValid")]
        public bool IsValid { get; set; }


        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }
    }

    /// <summary>
    /// 打卡地点 Collection
    /// </summary>
    public class StandardPunchCollection : EditableDataObjectCollectionBase<StandardPunchModel>
    {

    }
}
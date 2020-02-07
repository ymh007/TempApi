using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 打卡记录 Model
    /// </summary>
    [ORTableMapping("dbo.EmployeeServices")]
    public class EmployeeServicesModel
    {
        /// <summary>
        /// 打卡记录编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        [ORFieldMapping("CnName")]
        public string CnName { get; set; }
        /// <summary>
        /// 域帐号
        /// </summary>
        [ORFieldMapping("EnName")]
        public string EnName { get; set; }
        /// <summary>
        /// 组织机构路径
        /// </summary>
        [ORFieldMapping("FullPath")]
        public string FullPath { get; set; }
        /// <summary>
        /// 组织机构编码
        /// </summary>
        [ORFieldMapping("OrganizationCode")]
        public string OrganizationCode { get; set; }
        /// <summary>
        /// 打卡日期（次日凌晨签退时间线以前打卡算前一天打卡）
        /// </summary>
        [ORFieldMapping("PunchDate")]
        public DateTime PunchDate { get; set; }
        /// <summary>
        /// 打卡类型 0上班打卡，1下班打卡
        /// </summary>
        [ORFieldMapping("PunchType")]
        public int PunchType { get; set; }
        /// <summary>
        /// 地图地址
        /// </summary>
        [ORFieldMapping("MapUrl")]
        public string MapUrl { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        [ORFieldMapping("Lat")]
        public string Lat { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        [ORFieldMapping("Lng")]
        public string Lng { get; set; }
        /// <summary>
        /// 打卡地点编码
        /// </summary>
        [ORFieldMapping("StandardPunchCode")]
        public string StandardPunchCode { get; set; }
        /// <summary>
        /// 是否迟到
        /// </summary>
        [ORFieldMapping("IsLate")]
        public bool? IsLate { get; set; }
        /// <summary>
        /// 是否早退
        /// </summary>
        [ORFieldMapping("IsEarly")]
        public bool? IsEarly { get; set; }
        /// <summary>
        /// 迟到或早退分钟数
        /// </summary>
        [ORFieldMapping("Minute")]
        public int? Minute { get; set; }
        /// <summary>
        /// 是否异常
        /// </summary>
        [ORFieldMapping("IsUnusual")]
        public bool IsUnusual { get; set; }
        /// <summary>
        /// 异常原因
        /// </summary>
        [ORFieldMapping("UnusualType")]
        public string UnusualType { get; set; }
        /// <summary>
        /// 异常说明
        /// </summary>
        [ORFieldMapping("UnusualDesc")]
        public string UnusualDesc { get; set; }
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
        /// 异常原因状态 0 app正常打卡状态   1  app打卡异常的   2  海鸥2请休假信息中的原因
        /// </summary>
        [ORFieldMapping("DescStatus")]
        public int DescStatus { get; set; }
        
    }

    /// <summary>
    /// 打卡记录 Collection
    /// </summary>
    public class EmployeeServicesCollection : EditableDataObjectCollectionBase<EmployeeServicesModel>
    {

    }
}
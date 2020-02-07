using System;
using System.Linq;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Adapter.Conference;

namespace Seagull2.YuanXin.AppApi.Models.Conference
{
    /// <summary>
    /// 工作人员 Model
    /// </summary>
    [ORTableMapping("Office.Worker")]
    public class WorkerModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID { get; set; }

        /// <summary>
        /// 工作人员类型编码
        /// </summary>
        //[ORFieldMapping("WorkerTypeID")]
        //public string WorkerTypeID { get; set; }

        /// <summary>
        /// 工作人员类型编码
        /// </summary>
        [ORFieldMapping("WorkerTypeName")]
        public string WorkerTypeName { get; set; }
        /// <summary>
        /// 工作人员类型名称
        /// </summary>
        //[NoMapping]
        //public string WorkerTypeName
        //{
        //    get
        //    {
        //        var model = WorkerTypeAdapter.Instance.Load(m => m.AppendItem("ID", this.WorkerTypeID)).SingleOrDefault();
        //        if (model != null)
        //        {
        //            return model.Name;
        //        }
        //        return string.Empty;
        //    }
        //}

        /// <summary>
        /// 会议ID
        /// </summary>
        [ORFieldMapping("ConferenceID")]
        public string ConferenceID { get; set; }

        /// <summary>
        /// 人员编码
        /// </summary>
        [ORFieldMapping("UserID")]
        public string UserID { get; set; }

        /// <summary>
        /// 人员名称
        /// </summary>
        [ORFieldMapping("UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// 人员电话
        /// </summary>
        [ORFieldMapping("UserTelPhone")]
        public string UserTelPhone { get; set; }

        /// <summary>
        /// 人员头像
        /// </summary>
        [ORFieldMapping("UserPhotoAddress")]
        public string UserPhotoAddress { get; set; }

        /// <summary>
        /// 创建人编码
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [ORFieldMapping("Sort")]
        public int Sort { set; get; }

        /// <summary>
        ///邮箱
        /// </summary>
        [NoMapping]
        public string Email
        {
            get; set;
        }
    }

    /// <summary>
    /// 工作人员列表
    /// </summary>
    public class WorkerCollection : EditableDataObjectCollectionBase<WorkerModel>
    {

    }
}
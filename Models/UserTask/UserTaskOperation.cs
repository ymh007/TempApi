using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.UserTask
{
    /// <summary>
    /// 
    /// </summary>
    [ORTableMapping("PushService.UserTaskOperation")]
    public class UserTaskOperation
    {
        /// <summary>
        /// ID
        /// </summary>
        [ORFieldMapping("Id", PrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// ResourceID
        /// </summary>
        [ORFieldMapping("ResourceID", PrimaryKey = true)]
        public string ResourceID { get; set; }

        /// <summary>
        /// TaskUrl
        /// </summary>
        [ORFieldMapping("TaskUrl", PrimaryKey = true)]
        public string TaskUrl { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        [ORFieldMapping("Message", PrimaryKey = true)]
        public string Message { get; set; }

        /// <summary>
        /// CreateTime
        /// </summary>
        [ORFieldMapping("CreateTime", PrimaryKey = true)]
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserTaskOperationController : EditableDataObjectCollectionBase<UserTaskOperation>
    {

    }

    public class UserTaskOperationAdapter : UpdatableAndLoadableAdapterBase<UserTaskOperation, UserTaskOperationController>
    {
        public static readonly UserTaskOperationAdapter Instance = new UserTaskOperationAdapter();

        protected override string GetConnectionName() {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        public UserTaskOperationAdapter() {

        }

        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public UserTaskOperation LoadUserTaskOperationById(string Id) {
            return base.Load(p => {
                p.AppendItem("Id", Id);
            }).FirstOrDefault();
        }
    }
}
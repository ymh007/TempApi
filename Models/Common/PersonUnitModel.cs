using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
namespace Seagull2.YuanXin.AppApi.Models.Common
{
    /// <summary>
    /// 人员管理单元 depeament  user  group
    /// </summary>
    [ORTableMapping("Office.PersonUnit")]
    public class PersonUnitModel : BaseModel
    {
        /// <summary>
        /// 关系编码
        /// </summary>
        public string RelationCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// 单元编码
        /// </summary>
        public string ID { get; set; }
    }


    /// <summary>
    /// 人员管理单元集合
    /// </summary>
    public class PersonUnitModelCollection : EditableDataObjectCollectionBase<PersonUnitModel>
    {

    }


    /// <summary>
    /// 人员权限
    /// </summary>
    [ORTableMapping("Office.AppCommonPermission")]
    public class AppCommonPermissionModel 
    {
        /// <summary>
        /// 人员编码
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// 权限模块编码
        /// </summary>
        public string RelationCode { get; set; }

        /// <summary>
        /// 模块
        /// </summary>
        public int Module { get; set; }

    }
    /// <summary>
    /// 人员权限集合
    /// </summary>
    public class AppCommonPermissionModelCollection : EditableDataObjectCollectionBase<AppCommonPermissionModel>
    {

    }
}
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Conference
{
    /// <summary>
    /// 会议资料
    /// </summary>
    [ORTableMapping("office.MeetingMaterial")]
    public class MeetingMaterial : BaseModel
    {
        /// <summary>
        /// 会议id
        /// </summary>
        [ORFieldMapping("ConfereenceId")]
        public string ConfereenceId { get; set; }

        /// <summary>
        /// 附件名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 附件访问路径
        /// </summary>
        [ORFieldMapping("ViewUrl")]
        public string ViewUrl { get; set; }



        /// <summary>
        /// 查看人员
        /// </summary>
        [NoMapping]
        public MeetingMaterialAuthorityCollection ViewList
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Code))
                {
                    return MeetingMaterialAuthorityAdapter.Instance.Load(p=>p.AppendItem("MaterialID", this.Code));
                }
                else
                {
                    return new MeetingMaterialAuthorityCollection();
                }
            }
        }


        /// <summary>
        /// 附件使用场景 开会  吃饭
        /// </summary>
        [ORFieldMapping("MeetingScenes")]
        public int MeetingScenes { get; set; }


        /// <summary>
        /// 是否全部人员可见
        /// </summary>
        [ORFieldMapping("IsShowAll")]
        public bool IsShowAll { get; set; }


    }


    /// <summary>
    /// 会议资料
    /// </summary>
    public class MeetingMaterialCollection : EditableDataObjectCollectionBase<MeetingMaterial>
    {

    }

    /// <summary>
    /// 查看材料的权限人员
    /// </summary>
    [ORTableMapping("office.MeetingMaterialAuthority")]
    public class MeetingMaterialAuthority : BaseModel
    {
        /// <summary>
        /// 附件id
        /// </summary>
        [ORFieldMapping("MaterialID")]
        public string MaterialID { get; set; }


        /// <summary>
        /// 查看人员名字
        /// </summary>
        [ORFieldMapping("PeopleName")]
        public string PeopleName { get; set; }


        /// <summary>
        /// 查看人员名字id
        /// </summary>
        [ORFieldMapping("PeopleId")]
        public string PeopleId { get; set; }
    }


    /// <summary>
    /// 查看材料的权限人员集合
    /// </summary>
    public class MeetingMaterialAuthorityCollection : EditableDataObjectCollectionBase<MeetingMaterialAuthority>
    {

    }
}



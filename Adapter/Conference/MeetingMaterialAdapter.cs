
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{

    /// <summary>
    /// 会议材料适配器
    /// </summary>
    public class MeetingMaterialAdapter : BaseAdapter<MeetingMaterial, MeetingMaterialCollection>
    {



        public static readonly MeetingMaterialAdapter Instance = new MeetingMaterialAdapter();

        public MeetingMaterialAdapter()
        {

        }

        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 删除会议材料
        /// </summary>
        /// <param name="id">材料id</param>
        public void DeleteMaterial(string id)
        {
            Delete(m => m.AppendItem("Code", id));
        }
         
    }

    /// <summary>
    /// 会议材料权限适配器
    /// </summary>
    public class MeetingMaterialAuthorityAdapter : BaseAdapter<MeetingMaterialAuthority, MeetingMaterialAuthorityCollection>
    {


        public static readonly MeetingMaterialAuthorityAdapter Instance = new MeetingMaterialAuthorityAdapter();

        public MeetingMaterialAuthorityAdapter()
        {

        }

        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 删除会议材料对应的权限人员
        /// </summary>
        /// <param name="MaterialID">材料id</param>
        public void DeletePersons(string MaterialID)
        {
            Delete(m => m.AppendItem("MaterialID", MaterialID));
        }


        /// <summary>
        /// 更新附件权限人员集合
        /// </summary>
        /// <param name="mmac"></param>
        public void UpdateMaterialPersonColl(MeetingMaterialAuthorityCollection mmac)
        {
            mmac.ForEach(mm =>
            {
                Update(mm);
            });
        }
    }
}
 
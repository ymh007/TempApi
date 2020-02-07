using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.IM;
using System.Linq;
using static Seagull2.YuanXin.AppApi.Models.IM.GroupInfoModel;

namespace Seagull2.YuanXin.AppApi.Adapter.IM
{
    public class GroupInfoAdapter : UpdatableAndLoadableAdapterBase<GroupInfoModel, GroupInfoCollection>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public static GroupInfoAdapter Instance = new GroupInfoAdapter();
       
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.IMConnectionNam;
        }

        /// <summary>
        /// 根据群组ID获取信息
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public GroupInfoModel GetGroupInfoByID(int groupID)
        {
            return Load(m =>
            {
                m.AppendItem("GroupId", groupID);
              
            }).FirstOrDefault();

        }

        /// <summary>
        /// 根据群组ID获取群组信息
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public GroupInfoCollection GetGroupInfoByGroupID(int groupID)
        {
            return Load(m =>
            {
                m.AppendItem("ConferenceId", groupID);
            });
        }
    }
}
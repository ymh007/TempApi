using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.Models.IM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using YuanXin.Framework.IM.Model;
using static Seagull2.YuanXin.AppApi.Models.IM.GroupMemberModel;

namespace Seagull2.YuanXin.AppApi.Adapter.IM
{
    public class GroupMemberAdapter : UpdatableAndLoadableAdapterBase<GroupMemberModel, GroupMemberCollection>
    {
        public static GroupMemberAdapter Instance = new GroupMemberAdapter();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.IMConnectionNam;
        }

        /// <summary>
        /// 获取id获取群组成员
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public GroupMemberCollection GetGroupMemberById(int groupId)
        {
            return Load(m =>
            {
                m.AppendItem("group_id", groupId);
            }
            );
        }
    }
}
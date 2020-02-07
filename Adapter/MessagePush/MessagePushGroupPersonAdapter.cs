using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.MessagePush;
using System.Data;

namespace Seagull2.YuanXin.AppApi.Adapter.MessagePush
{
	/// <summary>
	/// 消息推送群组人员 Adapter
	/// </summary>
	public class MessagePushGroupPersonAdapter : UpdatableAndLoadableAdapterBase<MessagePushGroupPersonModel, MessagePushGroupPersonCollection>
	{
		/// <summary>
		/// 实例
		/// </summary>
		public static readonly MessagePushGroupPersonAdapter Instance = new MessagePushGroupPersonAdapter();

		/// <summary>
		/// 数据库连接名称
		/// </summary>
		protected override string GetConnectionName()
		{
			return Models.ConnectionNameDefine.YuanXinBusiness;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coll"></param>
        public void MessageGroupInsert(MessagePushGroupPersonCollection coll)
        {
            //构造DataTable
            var dt = new DataTable();
            dt.Columns.Add("Code", System.Type.GetType("System.String"));
            dt.Columns.Add("PushGroupCode", System.Type.GetType("System.String"));
            dt.Columns.Add("UserCode", System.Type.GetType("System.String"));
            dt.Columns.Add("UserName", System.Type.GetType("System.String"));
            dt.Columns.Add("Creator", System.Type.GetType("System.String"));
            dt.Columns.Add("CreateTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("Modifier", System.Type.GetType("System.String"));
            dt.Columns.Add("ModifyTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("ValidStatus", System.Type.GetType("System.Boolean"));
            dt.Columns.Add("ObjectType", System.Type.GetType("System.String"));

            coll.ForEach(m =>
            {
                DataRow dr = dt.NewRow();
                dr["Code"] = m.Code;
                dr["PushGroupCode"] = m.PushGroupCode;
                dr["UserCode"] = m.UserCode;
                dr["UserName"] = m.UserName;
                dr["Creator"] = m.Creator;
                dr["CreateTime"] = m.CreateTime;
                dr["Modifier"] = m.Modifier;
                dr["ModifyTime"] = m.ModifyTime;
                dr["ValidStatus"] = m.ValidStatus;
                dr["ObjectType"] = m.ObjectType;
                dt.Rows.Add(dr);
            });

            SqlDbHelper.BulkInsertData(dt, "[office].[MessagePushGroupPerson]", GetConnectionName());


        }
	}
}
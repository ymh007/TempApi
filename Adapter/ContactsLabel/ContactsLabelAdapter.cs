using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.ContactsLabel;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.ContactsLabel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.ContactsLabel
{
    /// <summary>
    /// 通讯录标签-Adapter
    /// </summary>
    public class ContactsLabelAdapter : UpdatableAndLoadableAdapterBase<ContactsLabelModel, ContactsLabelCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly ContactsLabelAdapter Instance = new ContactsLabelAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;

        /// <summary>
        /// 获取用户的标签列表
        /// </summary>
        public ContactsLabelCollection GetList(string userCode)
        {
            return Load(
                w =>
                {
                    w.AppendItem("Creator", userCode).AppendItem("LabelType", 0);
                },
                o =>
                {
                    o.AppendItem("CreateTime", MCS.Library.Data.Builder.FieldSortDirection.Descending);
                }
            );
        }

        /// <summary>
        /// 获取用户的标签
        /// </summary>
        public List<LabelInfo> GetLabelInfoByUserCode(string creator, string userCode)
        {
            var sql = @"select label.Code,label.Name from
                        [office].[ContactsLabel] as label
                        inner join [office].[ContactsLabelPerson] as person
                        on label.Code = person.LabelCode and person.UserCode = @UserCode and label.Creator = @Creator";
            SqlParameter[] parameter =
            {
                new SqlParameter("@UserCode", SqlDbType.NVarChar, 36),
                new SqlParameter("@Creator", SqlDbType.NVarChar, 36)
            };
            parameter[0].Value = userCode;
            parameter[1].Value = creator;
            SqlDbHelper helper = new SqlDbHelper();
            var table = helper.ExecuteDataTable(sql, CommandType.Text, parameter);
            return DataConvertHelper<LabelInfo>.ConvertToList(table);
        }
    }
}
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.ContactsLabel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.ContactsLabel
{
    /// <summary>
    /// 通讯录标签人员-Adapter
    /// </summary>
    public class ContactsLabelPersonAdapter : UpdatableAndLoadableAdapterBase<ContactsLabelPersonModel, ContactsLabelPersonCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly ContactsLabelPersonAdapter Instance = new ContactsLabelPersonAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;

        /// <summary>
        /// 获取用户的标签下的人员
        /// </summary>
        public ContactsLabelPersonCollection GetListByCreator(string userCode)
        {
            return Load(
                w =>
                {
                    w.AppendItem("Creator", userCode);
                }
            );
        }

        /// <summary>
        /// 批量插入标签人员
        /// </summary>
        public void ContactsLabelPersonInsert(ContactsLabelPersonCollection coll)
        {
            //构造DataTable
            var dt = new DataTable();
            dt.Columns.Add("Code", Type.GetType("System.String"));
            dt.Columns.Add("LabelCode", Type.GetType("System.String"));
            dt.Columns.Add("UserCode", Type.GetType("System.String"));
            dt.Columns.Add("UserName", Type.GetType("System.String"));
            dt.Columns.Add("FullPath", Type.GetType("System.String"));
            dt.Columns.Add("Creator", Type.GetType("System.String"));
            dt.Columns.Add("CreateTime", Type.GetType("System.DateTime"));

            coll.ForEach(m =>
            {
                DataRow dr = dt.NewRow();
                dr["Code"] = m.Code;
                dr["LabelCode"] = m.LabelCode;
                dr["UserCode"] = m.UserCode;
                dr["UserName"] = m.UserName;
                dr["FullPath"] = m.FullPath;
                dr["Creator"] = m.Creator;
                dr["CreateTime"] = m.CreateTime;
                dt.Rows.Add(dr);
            });

            SqlDbHelper.BulkInsertData(dt, "[office].[ContactsLabelPerson]", GetConnectionName());
        }

        /// <summary>
        /// 获取标签下的人员列表
        /// </summary>
        public ContactsLabelPersonCollection GetPersonList(string labelCode)
        {
            return Load(w =>
            {
                w.AppendItem("LabelCode", labelCode);
            });
        }
    }


    /// <summary>
    /// 外部联系人适配器
    /// </summary>
    public class ExternalContactAdapter : UpdatableAndLoadableAdapterBase<ExternalContact, ExternalContactCollection>
    {

        /// <summary>
        /// 实例
        /// </summary>
        public static readonly ExternalContactAdapter Instance = new ExternalContactAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;

        /// <summary>
        /// 获取用户的标签下的人员
        /// </summary>
        public ExternalContactCollection GetCurrentUserList(string uCode, string name, List<string> labels, int index, int psize, string phone = "")
        {
            List<string> par = null;
            string parment = "";
            string sql = $"SELECT * FROM  [office].[ExternalContact] WHERE CREATOR='{uCode}' " +
                $"  ORDER BY [NAME] OFFSET {index * psize} ROW FETCH NEXT {psize} ROWS ONLY";
            if (labels.Count > 0)
            {
                par=  new List<string>();
                labels.ForEach(f=> { par.Add(" T.[LABEL] LIKE  '%" + f.Trim() + "%' "); });
                parment = string.Join(" OR ", par); 
            }
            if (!string.IsNullOrEmpty(name) && labels.Count == 0)
            {
                sql = $"SELECT * FROM [office].[ExternalContact]  WHERE CREATOR='{uCode}' AND [NAME] LIKE '%{name}%'" +
                    $"ORDER BY [NAME] OFFSET {index * psize} ROW FETCH NEXT {psize} ROWS ONLY";
            }
            if (string.IsNullOrEmpty(name) && labels.Count > 0)
            {
                sql = $"SELECT * FROM ( SELECT * FROM  [office].[ExternalContact] WHERE CREATOR='{uCode}') AS T  WHERE {parment} " +
                    $"ORDER BY [NAME] OFFSET {index * psize} ROW FETCH NEXT {psize} ROWS ONLY";
            }
            if (!string.IsNullOrEmpty(name) && labels.Count > 0)
            {
                sql = $"SELECT * FROM (SELECT * FROM  [office].[ExternalContact]" +
                    $" WHERE CREATOR='{uCode}' AND [NAME] LIKE '%{name}%' ) AS T WHERE {parment}" +
                    $"ORDER BY T.[NAME] OFFSET {index * psize} ROW FETCH NEXT {psize} ROWS ONLY";
            }
            return this.QueryData(sql);
        }
        /// <summary>
        /// 获取用户下面的所有外部联系人
        /// </summary>
        public ExternalContactCollection GetCurrentUserList(string userCode)
        {
            return Load(w => w.AppendItem("Creator", userCode));
        }
        /// <summary>
        /// 批量插入标签人员
        /// </summary>
        public void AddContacts(List<ExternalContact> coll)
        {
            //构造DataTable
            var dt = new DataTable();
            dt.Columns.Add("Code", Type.GetType("System.String"));
            dt.Columns.Add("Name", Type.GetType("System.String"));
            dt.Columns.Add("Phone", Type.GetType("System.String"));
            dt.Columns.Add("Email", Type.GetType("System.String"));
            dt.Columns.Add("Company", Type.GetType("System.String"));
            dt.Columns.Add("Position", Type.GetType("System.String"));
            dt.Columns.Add("Address", Type.GetType("System.String"));
            dt.Columns.Add("Mark", Type.GetType("System.String"));
            dt.Columns.Add("Label", Type.GetType("System.String"));
            dt.Columns.Add("Photo", Type.GetType("System.String"));
            dt.Columns.Add("Card", Type.GetType("System.String"));
            dt.Columns.Add("Creator", Type.GetType("System.String"));
            coll.ForEach(m =>
            {
                DataRow dr = dt.NewRow();
                dr["Code"] = m.Code;
                dr["Name"] = m.Name;
                dr["Phone"] = m.Phone;
                dr["Email"] = m.Email;
                dr["Company"] = m.Company;
                dr["Position"] = m.Position;
                dr["Address"] = m.Address;
                dr["Mark"] = m.Mark;
                dr["Label"] = m.Label;
                dr["Photo"] = m.Photo;
                dr["Card"] = m.Card;
                dr["Creator"] = m.Creator;
                dt.Rows.Add(dr);
            });
            SqlDbHelper.BulkInsertData(dt, "[office].[ExternalContact]", GetConnectionName());
        }

        /// <summary>
        /// 获取标签下的人员列表
        /// </summary>
        public ExternalContactCollection GetPersonList(string labelCode)
        {
            return Load(w =>
            {
                w.AppendItem("LabelCode", labelCode);
            });
        }

        /// <summary>
        /// 批量更新外部联系人标签
        /// </summary>
        /// <param name="label"></param>
        public int UpdateLabels(string label,string user)
        {
            //replace(replace(replace(Label,'请问请问,',''),',请问请问',''),'请问请问','')
            SqlDbHelper comm = new SqlDbHelper();
            string sql = $" update [office].[ExternalContact]  set Label = replace(replace(replace(Label, '{label},', ''), ',{label}', ''),'{label}','')" +
                $" where creator='{user}' and Label like '%{label}%'";
           return comm.ExecuteNonQuery(sql);
        }

    }
}
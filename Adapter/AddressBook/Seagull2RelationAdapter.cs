using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.AddressBook
{
   
    public class Seagull2RelationAdapter : UpdatableAndLoadableAdapterBase<Seagull2RelationModel, Seagull2RelationModelCollection>
    {
        public static readonly Seagull2RelationAdapter Instance = new Seagull2RelationAdapter();

        public Seagull2RelationAdapter()
        {

        }
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Creator"></param>
        /// <returns></returns>
        public Seagull2RelationModelCollection LoadById(string[] Id)
        {
            var mapping = ORMapping.GetMappingInfo<Seagull2RelationModel>();
            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
           
            StringBuilder strB = new StringBuilder(200);
            foreach (var item in Id)
            {
                builder.AppendItem("Id", item ,"=");
                builder.LogicOperator = LogicOperatorDefine.Or;            }
            strB.Append(string.Format("SELECT Id,UserId  FROM {0} WHERE   {1}", mapping.TableName, builder.ToSqlString(TSqlBuilder.Instance)));

            return this.QueryData(mapping, strB.ToString());
        }
       
    }
}
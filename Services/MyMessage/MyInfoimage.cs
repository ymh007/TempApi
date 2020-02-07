using MCS.Library.SOA.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Services.MyMessage
{
    public class MyInfoimage: UpdatableAndLoadableAdapterBase<ImageProperty, ImagePropertyCollection>
    {

        protected override string GetConnectionName()
        {
            return "HB2008";
        }
        
        public string LoadByResourceID(string Id) {
            try
            {
                string sql = string.Format(@"SELECT * FROM MCS_WORKFLOW.WF.IMAGE WHERE RESOURCE_ID='{0}' ", Id);

                string list = null;

                foreach (var item in QueryData(sql))
                {
                    list = item.ID;
                }
                return list;
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message);
                Log.WriteLog(ex.StackTrace);
                throw ex;
            }
        }
    }
}
using MCS.Library.Data.DataObjects;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.News
{
    public class ResultMssage
    {
        public bool IsValid { get; set; }
        public String ErrorMessage { get; set; }
    }


    public class ResultMssageCollection : EditableDataObjectCollectionBase<ResultMssage>{

    }
    public class ResultMssageAdapter : UpdatableAndLoadableAdapterBase<ResultMssage, ResultMssageCollection> {

        public static readonly ResultMssageAdapter Instance = new ResultMssageAdapter();


        private ResultMssageAdapter()
        {

        }

        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.SinooceanLandAddressList;
        }

        public ResultMssage AddRecord(string newsAddress, String title, String source,string displayName)
        {
            var resultMssage = new ResultMssage();

            try
            {
                string date = System.DateTime.Now.ToString();
                string sql = "insert into SITE_VISIT_LOG (URL,USER_NAME,DROP_TIME,TITLE,SOURCE) values('" + HttpUtility.UrlDecode(newsAddress) + "','" + displayName + "','" + date + "','" + title + "','" + source + "')";

                DbHelper.RunSql(sql, GetConnectionName());
                resultMssage.IsValid = true;
                resultMssage.ErrorMessage = HttpUtility.UrlDecode(newsAddress);

            }
            catch (Exception ee)
            {
                Log.WriteLog(ee.Message);
                Log.WriteLog(ee.StackTrace);
                resultMssage.IsValid = false;
                resultMssage.ErrorMessage = ee.ToString();
            }
            return resultMssage;
        }
    }

}
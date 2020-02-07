using MCS.Library.Data.Builder;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;

namespace Seagull2.YuanXin.AppApi.Adapter.Activity
{
    public class ActivityAdapter : ViewBaseAdapter<Project, List<Project>>
    {
        private static string ConnectionString = ConnectionNameDefine.YuanXinForDBHelp;
        public static ActivityAdapter Instance = new ActivityAdapter();

        public ActivityAdapter() : base(ConnectionString)
        {

        }

        public ViewPageBase<List<Project>> GetProjectViewByPage(int pageIndex, DateTime searchTime)
        {
            string selectSQL = "select p.Name,p.CoverImg,p.Summary,p.Code,p.City,p.Address,p.StartTime ";
            string fromAndWhereSQL = "";
            fromAndWhereSQL = string.Format(@"from zc.Project p where");
            var typewhere = new WhereSqlClauseBuilder(LogicOperatorDefine.Or);
            typewhere.AppendItem("p.Type", UserFocusType.Anchang.GetHashCode());
            typewhere.AppendItem("p.Type", UserFocusType.Online.GetHashCode());
            typewhere.AppendItem("p.Type", UserFocusType.Tejiafang.GetHashCode());
            typewhere.AppendItem("p.Type", UserFocusType.ZaiShou.GetHashCode());
            var where = new WhereSqlClauseBuilder();
            where.AppendItem("(" + typewhere.ToSqlString(TSqlBuilder.Instance) + ")", "", "", true);
            where.AppendItem("p.IsValid", true);
            fromAndWhereSQL += where.ToSqlString(TSqlBuilder.Instance);
            string orderSQL = "order by p.CreateTime DESC";
            ViewPageBase<List<Project>> result = LoadViewModelCollByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
            result.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }
    }
}
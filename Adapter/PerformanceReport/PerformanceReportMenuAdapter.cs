using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Web;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;

namespace Seagull2.YuanXin.AppApi.Adapter
{
    /// <summary>
    /// 业绩报表菜单适配器
    /// Powered By: v-sunzhh
    /// Date: 2017-05-10
    /// </summary>
    public class PerformanceReportMenuAdapter : UpdatableAndLoadableAdapterBase<PerformanceReportMenuModel, PerformanceReportMenuCollection>
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly PerformanceReportMenuAdapter Instance = new PerformanceReportMenuAdapter();

        /// <summary>
        /// 添加或修改
        /// </summary>
        public bool UpdateMenu(PerformanceReportMenuModel model, out string msg)
        {
            //1、获取海鸥二菜单信息
            var eipMenu = EIPMenuAdapter.Instance.GetListById(model.MenuId);
            if (eipMenu.Count < 1)
            {
                msg = "菜单编号错误！";
                return false;
            }
            //2、判断海鸥二菜单是否为父级菜单
            if (string.IsNullOrWhiteSpace(eipMenu[0].ParentID))
            {
                msg = "请勿主动添加父级菜单，添加子菜单会自动添加它的父级菜单！";
                return false;
            }
            //3、自动添加父级菜单
            var localParentMenu = Load(p => { p.AppendItem("MenuId", eipMenu[0].ParentID); });
            if (localParentMenu.Count < 1)
            {
                var modelParent = new PerformanceReportMenuModel();
                modelParent.Code = Guid.NewGuid().ToString();
                modelParent.MenuId = eipMenu[0].ParentID;
                modelParent.Sort = 0;
                modelParent.Status = 1;
                modelParent.Creator = modelParent.Creator;
                modelParent.CreateTime = DateTime.Now;
                modelParent.ValidStatus = true;
                Update(modelParent);
            }
            //4、编辑子菜单
            Update(model);

            msg = "编辑成功！";
            return true;
        }

        /// <summary>
        /// 获取用户关注的菜单
        /// </summary>
        public PerformanceReportMenuCollection GetList(string userCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT A.* ");
            strSql.Append("FROM [office].[PerformanceReportMenu] A, [office].[PerformanceReportUserFocus] B ");
            strSql.Append("WHERE");
            strSql.Append(" A.[MenuId]=B.[ReportCode] AND [UserCode]='" + userCode + "'");

            return this.QueryData(strSql.ToString());
        }
    }
}
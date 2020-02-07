using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.ShareFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.ShareFile
{
    /// <summary>
    /// 共享文件存储记录 Adapter
    /// </summary>
    public class ShareFileAdapter : UpdatableAndLoadableAdapterBase<ShareFileModel, ShareFileCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly ShareFileAdapter Instance = new ShareFileAdapter();

        /// <summary>
        /// 数据库连接
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected string ConnectionString = DbConnectionManager.GetConnectionString(ConnectionNameDefine.YuanXinBusiness);

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="module">模块标识</param>
        /// <param name="targetCode">父级编码</param>
        public ShareFileCollection GetList(string module, string targetCode)
        {
            return Load(
                w =>
                {
                    w.AppendItem("Module", module);
                    w.AppendItem("TargetCode", targetCode);
                },
                o =>
                {
                    o.AppendItem("Sort", MCS.Library.Data.Builder.FieldSortDirection.Ascending);
                }
            );
        }

        /// <summary>
        /// 删除文件列表
        /// </summary>
        /// <param name="module">模块标识</param>
        /// <param name="targetCode">父级编码</param>
        public void Delete(string module, string targetCode)
        {
            Delete(w =>
            {
                w.AppendItem("Module", module);
                w.AppendItem("TargetCode", targetCode);
            });
        }

        /// <summary>
        /// 删除单个文件
        /// </summary>
        public void Delete(string code)
        {
            Delete(w =>
            {
                w.AppendItem("Code", code);
            });
        }

        /// <summary>
        /// 获取文件详情
        /// </summary>
        public ShareFileModel GetModel(string code)
        {
            return Load(w => w.AppendItem("Code", code)).SingleOrDefault();
        }
    }
}
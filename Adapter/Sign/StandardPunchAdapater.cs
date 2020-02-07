using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Sign;
using System;
using System.Linq;

namespace Seagull2.YuanXin.AppApi.Adapter.Sign
{
    /// <summary>
    /// 打卡地点 Adapter
    /// </summary>
    public class StandardPunchAdapater : UpdatableAndLoadableAdapterBase<StandardPunchModel, StandardPunchCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly StandardPunchAdapater Instance = new StandardPunchAdapater();

        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.EmployeeAttendance;
        }

        /// <summary>
        /// 缓存集合
        /// </summary>
        private StandardPunchCollection cacheData = new StandardPunchCollection();

        /// <summary>
        /// 获取打卡地点集合
        /// </summary>
        public StandardPunchCollection LoadStandardList()
        {
            if (cacheData.Count > 0)
            {
                return cacheData;
            }
            else
            {
                cacheData = Load(w => w.AppendItem("IsValid", true));
                return cacheData;
            }
        }

        /// <summary>
        /// 根据编码查询
        /// </summary>
        public StandardPunchModel LoadByCode(string code)
        {
            return Load(m => m.AppendItem("Code", code)).SingleOrDefault();
        }

        /// <summary>
        /// 清理地点集合缓存
        /// </summary>
        public void Dispose()
        {
            cacheData.Clear();
        }

        /// <summary>
        /// 查询距离最近的一个打卡地点
        /// </summary>
        public StandardPunchCollection GetMinSingle(decimal lng, decimal lat, int range)
        {
            var sql = $@"
                SELECT TOP 1 *, [dbo].[FuncGetDistance]({lng}, {lat}, [Lng], [Lat]) AS [Distance] FROM [dbo].[StandardPunch]
                WHERE [dbo].[FuncGetDistance]({lng}, {lat}, [Lng], [Lat]) < {range} AND [IsValid] = 1 AND [ValidStatus] = 1
                ORDER BY [Distance] ASC;";

            return QueryData(sql);
        }
    }
}
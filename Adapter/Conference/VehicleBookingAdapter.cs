using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{
    /// <summary>
    /// 车辆预定
    /// </summary>
    public class VehicleBookingAdapter : BaseAdapter<VehicleBookingModel, VehicleBookingModelCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly VehicleBookingAdapter Instance = new VehicleBookingAdapter();

        private string ConnectionString = ConnectionNameDefine.YuanXinBusiness;

        /// <summary>
        /// 构造
        /// </summary>
        public VehicleBookingAdapter()
        {
            BaseConnectionStr = this.ConnectionString;
        }

        /// <summary>
        /// 根据用户编码 获取预定详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VehicleBookingModel LoadByID(string id)
        {
            return this.Load(p =>
            {
                p.AppendItem("ID", id);
            }).FirstOrDefault();
        }

        /// <summary>
        /// 根据用户编码 获取预定详情
        /// </summary>
        public VehicleBookingModel LoadByUserCodeAndConferenceID(string Usercode, string ConferenceID)
        {
            return this.Load(p =>
            {
                p.AppendItem("AttendeeID", Usercode, "=");
                p.AppendItem("ConferenceID", ConferenceID, "=");
            }).FirstOrDefault();
        }

        /// <summary>
        /// 新增预定
        /// </summary>
        /// <param name="BookingModel"></param>
        /// <returns></returns>
        public void AddVehicleBooking(VehicleBookingModel BookingModel)
        {
            Update(BookingModel);
            //var mapping = ORMapping.GetMappingInfo<VehicleBookingModel>();
            //WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
            //builder.AppendItem("ID", BookingModel.ID);
            //StringBuilder strB = new StringBuilder(200);
            //strB.Append(TSqlBuilder.Instance.DBStatementSeperator);
            //strB.Append(ORMapping.GetInsertSql(strB, TSqlBuilder.Instance, ""));
            //return DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName()) >= 0 ? true : false;
        }

        /// <summary>
        /// 根据ID删除预定
        /// </summary>
        public bool DelUserBook(string ID)
        {
            var mapping = ORMapping.GetMappingInfo<VehicleBookingModel>();
            StringBuilder strB = new StringBuilder(200);
            strB.Append(string.Format("DELETE FROM {0} WHERE {1}", mapping.TableName, ID));

            return DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName()) > 0 ? true : false;
        }
    }
}
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
    /// 禁用座位操作类
    /// </summary>
    public class DisableSeatsAdapter: UpdatableAndLoadableAdapterBase<DisableSeatsModel, DisableSeatsModelCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly DisableSeatsAdapter Instance = new DisableSeatsAdapter();

        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

       

        /// <summary>
        /// 更新禁用座位集合
        /// </summary>
        public void UpdateSeatColl(List<DisableSeatsModel> seatColl)
        {
            seatColl.ForEach(seat =>
            {
                DisableSeatsAdapter.Instance.Update(seat);
            });
        }

        /// <summary>
        /// 根据会议编码删除所有已禁用的座位信息
        /// </summary>
        /// <param name="ConferenceID">会议编码</param>
        /// <returns></returns>
        public bool DelHasDisableSeats(string ConferenceID)
        {
            var mapping = ORMapping.GetMappingInfo<DisableSeatsModel>();
            StringBuilder strB = new StringBuilder(200);
            strB.Append(string.Format("DELETE FROM {0} WHERE ConferenceID='{1}'", mapping.TableName, ConferenceID));

            return DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName()) > 0 ? true : false;
        }
       

        /// <summary>
        /// 根据会议编码和座位实际坐标编号判断当前座位是否已被禁用
        /// </summary>
        /// <param name="ConferenceID">会议编码</param>
        /// <param name="seatNo">座位坐标编号</param>
        /// <returns></returns>
        public DisableSeatsModel GetDisSeatById(string ConferenceID,string seatNo)
        {
            DisableSeatsModelCollection smColl = this.Load(p =>
            {
                p.AppendItem("SeatAddress", seatNo);
                p.AppendItem("ConferenceID", ConferenceID);
            });
            return smColl.Count > 0 ? smColl[0] : null;
        }
    }
}
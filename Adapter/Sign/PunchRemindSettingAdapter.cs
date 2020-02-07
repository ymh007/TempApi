using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Sign;
using System;
using System.Linq;

namespace Seagull2.YuanXin.AppApi.Adapter.Sign
{
    /// <summary>
    /// 打卡提醒设置 Adapter
    /// </summary>
    public class PunchRemindSettingAdapter : UpdatableAndLoadableAdapterBase<PunchRemindSettingModel, PunchRemindSettingCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly PunchRemindSettingAdapter Instance = new PunchRemindSettingAdapter();

        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 获取用户的打卡提醒设置
        /// </summary>
        public PunchRemindSettingCollection GetListByUser(string userCode)
        {
            return Load(w =>
            {
                w.AppendItem("Creator", userCode);
            }, o =>
            {
                o.AppendItem("Type", MCS.Library.Data.Builder.FieldSortDirection.Ascending);
            });
        }


        /// <summary>
        ///获取用户提醒时间
        /// </summary>
        public PunchRemindSettingCollection GetRemindTime(string userCode)
        {
            return Load(w =>
            {
                w.AppendItem("Creator", userCode).AppendItem("IsEnable",true);
            }, o =>
            {
                o.AppendItem("Type", MCS.Library.Data.Builder.FieldSortDirection.Ascending);
            });
        }



        public PunchRemindSettingModel GetModelByCode(string code)
        {
            return Load(w => w.AppendItem("Code", code)).FirstOrDefault();
        }


        public string CalcEndTime(PunchRemindSettingModel model)
        {
            for (int i = 0; i < 7; i++)
            {
                DateTime temp = DateTime.Now.AddDays(i);
                switch (temp.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        if (model.Sunday && CalcEndTimeSub(i, temp, model.RemindTime)) return temp.ToString("yyyy-MM-dd");
                        break;
                    case DayOfWeek.Monday:
                        if (model.Monday && CalcEndTimeSub(i, temp, model.RemindTime)) return temp.ToString("yyyy-MM-dd");
                        break;
                    case DayOfWeek.Tuesday:
                        if (model.Tuesday && CalcEndTimeSub(i, temp, model.RemindTime)) return temp.ToString("yyyy-MM-dd");
                        break;
                    case DayOfWeek.Wednesday:
                        if (model.Wednesday && CalcEndTimeSub(i, temp, model.RemindTime)) return temp.ToString("yyyy-MM-dd");
                        break;
                    case DayOfWeek.Thursday:
                        if (model.Thursday && CalcEndTimeSub(i, temp, model.RemindTime)) return temp.ToString("yyyy-MM-dd");
                        break;
                    case DayOfWeek.Friday:
                        if (model.Friday && CalcEndTimeSub(i, temp, model.RemindTime)) return temp.ToString("yyyy-MM-dd");
                        break;
                    case DayOfWeek.Saturday:
                        if (model.Saturday && CalcEndTimeSub(i, temp, model.RemindTime)) return temp.ToString("yyyy-MM-dd");
                        break;
                }
            }
            return string.Empty;
        }
        public bool CalcEndTimeSub(int index, DateTime temp, string time)
        {
            if (index == 0)
            {
                DateTime endDate = DateTime.Parse(temp.ToString("yyyy-MM-dd") + " " + time);
                if (endDate > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }





    }
}
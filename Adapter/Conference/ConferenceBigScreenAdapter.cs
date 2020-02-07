using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{
    /// <summary>
    /// 会议效果适配器
    /// </summary>
    public class ConferenceBigScreenAdapter : BaseAdapter<ConferenceBigScreenModel, ConferenceBigScreenModelCollection>
    {
        public static readonly ConferenceBigScreenAdapter Instance = new ConferenceBigScreenAdapter();

        public ConferenceBigScreenAdapter()
        {

        }

        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }


        /// <summary>
        /// 查询所有效果 包括模板
        /// </summary>
        /// <param name="conferenceID"></param>
        /// <returns></returns>
        public ConferenceBigScreenModelCollection LoadAll(string conferenceID)
        {
            return this.Load(
              p =>
              {
                  p.AppendItem("ConfereenceId", conferenceID);
                  p.AppendItem("IsSystem", true);
                  p.LogicOperator = MCS.Library.Data.Builder.LogicOperatorDefine.Or;
              },
              o =>
              {
                  o.AppendItem("CreateTime", MCS.Library.Data.Builder.FieldSortDirection.Ascending);
              });
        }
    }
}
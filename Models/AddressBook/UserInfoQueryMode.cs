using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    public class UserInfoQueryMode
    {
        /// <summary>
        /// 显示的个数
        /// </summary>
        public int ResultShow { get; set; }

        /// <summary>
        /// 查询条件
        /// </summary>
        public string QueryCondition { get; set; }


        /// <summary>
        /// 人员查询结果集合
        /// </summary>
        private List<UsersInfoExtend> _queryResult = null;
        public List<UsersInfoExtend> QueryResult
        {
            get
            {
                if (_queryResult == null)
                {
                    _queryResult = new List<UsersInfoExtend>();
                }
                return _queryResult;
            }
            set { _queryResult = value; }
        }
    }
}
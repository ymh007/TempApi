using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using MCS.Library.Core;
using SinoOcean.Seagull2.Framework.MasterData;
using Seagull2.YuanXin.AppApi.Models.AddressBook.EnumType;

namespace Seagull2.YuanXin.AppApi.Services
{
    public class AttentionService : UpdatableAndLoadableAdapterBase<AttentionModel, AttentionController>, IAttentionService
    {
        /// <summary>
        /// 增加我的关注
        /// </summary>
        /// <param name="UserCode">用户id</param>
        /// <param name="BusinessProjectCode">项目或事业部编码、城市</param>
        /// <param name="BusinessProjectName">项目或事业部名称</param>
        /// <returns></returns>
        public async Task<string> AddAttention(string userCode, string businessProjectCode, string businessProjectName, string businessCode, int attentionType)
        {
            string result = null;
            //查询是否关注当前
            AttentionModel attentionModel = AttentionAdapter.Instance.LoadAttentionUserCodeAndBuss(userCode, businessProjectCode);
            if (attentionModel == null)
            {
                Log.WriteLog(userCode+ businessProjectCode+ businessProjectName);
                if (!string.IsNullOrEmpty(userCode) && !string.IsNullOrEmpty(businessProjectCode) && !string.IsNullOrEmpty(businessProjectName))
                {
                    try
                    {
                        AttentionModel attention = new AttentionModel();
                        attention.Code = UuidHelper.NewUuidString();
                        attention.BusinessCode = businessCode;
                        attention.AttentionType = attentionType;
                        attention.BusinessProjectCode = businessProjectCode;
                        attention.BusinessProjectName = businessProjectName;
                        attention.UserCode = userCode;
                        attention.ValidStatus = true;
                        attention.CreateTime = DateTime.Now;
                        attention.Creator = userCode;

                        AttentionAdapter.Instance.Update(attention);
                        result = "succeed";
                    }
                    catch (Exception ex)
                    {
                        result = "defeated";
                        Log.WriteLog(ex.Message);
                        Log.WriteLog(ex.StackTrace);
                    }
                }
                else
                {
                    result = "Argument Empty";
                }
            }
            else
            {
                result = "repetition";
            }

            return result;
        }

        /// <summary>
        /// 清空删除我的关注
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public async Task<string> EmptyAndAddOrAttention(string userCode, string listbusinessProjectCode, bool IsEmpty)
        {
            string result = null;
            try
            {
                if (IsEmpty)
                {
                    AttentionController attentionController = AttentionAdapter.Instance.LoadAttentionUserCode(userCode);
                    foreach (AttentionModel item in attentionController)
                    {
                        item.ValidStatus = false;
                        item.VersionTime = DateTime.Now;
                        AttentionAdapter.Instance.Update(item);
                        result = "succeed";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(userCode) && !string.IsNullOrEmpty(listbusinessProjectCode))
                    {
                        string[] list = listbusinessProjectCode.Split(',');
                        foreach (string item in list)
                        {
                            AttentionModel attention = AttentionAdapter.Instance.LoadAttentionUserCodeAndBuss(userCode, item);
                            attention.ValidStatus = false;
                            attention.VersionTime = DateTime.Now;
                            AttentionAdapter.Instance.Update(attention);
                            result = "succeed";
                        }
                    }
                    else
                    {
                        result = "Argument Empty";
                    }
                }


            }
            catch (Exception ex)
            {
                result = "defeated";
                Log.WriteLog(ex.Message);
                Log.WriteLog(ex.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// 返回我的关注列表
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AttentionModel>> LoadAttention(string userCode)
        {
            AttentionController attentionController = AttentionAdapter.Instance.LoadAttentionUserCode(userCode);
            return attentionController;
        }

        /// <summary>
        /// 我的关注的统计数
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public async Task<int> StatisticsAttention(string userCode)
        {
            AttentionController attentionController = AttentionAdapter.Instance.LoadAttentionUserCode(userCode);
            return attentionController.Count;
        }
    }
}
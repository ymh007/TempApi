using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using System.Web.Security;
using System.Data;
using Newtonsoft.Json;
using MCS.Library.Data.Builder;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Newtonsoft.Json.Converters;
using Seagull2.YuanXin.AppApi.Models;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 众筹项目
    /// </summary>
    [ORTableMapping("zc.Project")]
    public class Project : ILoadableDataEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        [NoMapping]
        public string EncodeCode
        {
            get
            {
                return FormsAuthentication.HashPasswordForStoringInConfigFile(Code, "MD5");
            }
        }
        /// <summary>
        /// 项目类型
        /// </summary>
        [ORFieldMapping("Type")]
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
        public ProjectTypeEnum Type { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }


        /// <summary>
        /// 项目简介
        /// </summary>
        [ORFieldMapping("Summary")]
        public string Summary { get; set; }

        /// <summary>
        /// 项目详情
        /// </summary>
        [ORFieldMapping("Detail")]
        public string Detail { get; set; }

        /// <summary>
        /// 封面图片
        /// </summary>
        [ORFieldMapping("CoverImg")]
        public string CoverImg { get; set; }

        /// <summary>
        /// 楼盘编码
        /// </summary>
        [ORFieldMapping("BuildingCode")]
        public string BuildingCode { get; set; }

        /// <summary>
        /// 省份编码
        /// </summary>
        [ORFieldMapping("ProvinceCode")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        [ORFieldMapping("Province")]
        public string Province { get; set; }

        /// <summary>
        /// 城市编码
        /// </summary>
        [ORFieldMapping("CityCode")]
        public string CityCode { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [ORFieldMapping("City")]
        public string City { get; set; }

        /// <summary>
        /// 县区编码
        /// </summary>
        [ORFieldMapping("CountyCode")]
        public string CountyCode { get; set; }

        /// <summary>
        /// 县区名称
        /// </summary>
        [ORFieldMapping("County")]
        public string County { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [ORFieldMapping("Address")]
        public string Address { get; set; }

        /// <summary>
        /// 点击次数
        /// </summary>
        public int ClickNo { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        [SqlBehavior(BindingFlags = ClauseBindingFlags.Insert)]
        [JsonIgnore]
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        [SqlBehavior(BindingFlags = ClauseBindingFlags.Insert)]
        [JsonIgnore]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [ORFieldMapping("Modifier")]
        [JsonIgnore]
        public string Modifier { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [ORFieldMapping("ModifyTime")]
        [JsonIgnore]
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("IsValid")]
        public bool IsValid { get; set; }

        /// <summary>
        /// 0-草稿箱，1-审核中，2-审核未通过，3-审核通过
        /// </summary>
        [ORFieldMapping("AuditStatus")]
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
        public Enums.AuditStatus AuditStatus { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [ORFieldMapping("StartTime")]
        [JsonConverter(typeof(ChinaDateTimeConverter))]
        public DateTime StartTime
        {
            get; set;
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        [ORFieldMapping("EndTime")]
        [JsonConverter(typeof(ChinaDateTimeConverter))]
        public DateTime EndTime { get; set; }


        /// <summary>
        /// 活动天数
        /// </summary>
        [ORFieldMapping("TotalDay")]
        public int TotalDay { get; set; }

        /// <summary>
        /// 状态名称
        /// </summary>
        [NoMapping]
        public string AuditStatusName { get { return EnumItemDescriptionAttribute.GetDescription(AuditStatus); } }
        /// <summary>
        /// 目标人数
        /// </summary>
        [ORFieldMapping("TargetNo")]
        public int TargetNo { get; set; }

        /// <summary>
        /// 支持人数/作品总数
        /// </summary>
        [ORFieldMapping("SupportNo")]
        public int SupportNo { get; set; }

        /// <summary>
        /// 赞人数
        /// </summary>
        [ORFieldMapping("PraiseNo")]
        public int PraiseNo { get; set; }

        /// <summary>
        /// 评论人数
        /// </summary>
        [ORFieldMapping("CommentNo")]
        public int CommentNo { get; set; }

        /// <summary>
        /// 子项目数码
        /// </summary>
        [ORFieldMapping("SubItemNo")]
        public int SubItemNo { get; set; }

        /// <summary>
        /// 子项目参与限制
        /// </summary>
        [ORFieldMapping("SubItemJoinLimit")]
        [JsonIgnore]
        public int SubItemJoinLimit { get; set; }

        /// <summary>
        /// 分享人数
        /// </summary>
        [ORFieldMapping("ShareNo")]
        public int ShareNo { get; set; }

        /// <summary>
        /// 关注人数
        /// </summary>
        [ORFieldMapping("FocusNo")]
        public int FocusNo { get; set; }

        /// <summary>
        /// 纬度值
        /// </summary>
        [ORFieldMapping("Lat")]
        public double Lat { get; set; }

        /// <summary>
        /// 经度值
        /// </summary>
        [ORFieldMapping("Lng")]
        public double Lng { get; set; }

        /// <summary>
        /// 评价人数
        /// </summary>
        [ORFieldMapping("EvaluationUserTotal")]
        public int EvaluationUserTotal { get; set; }
        /// <summary>
        /// 评分合计
        /// </summary>
        [ORFieldMapping("EvaluationScoreTotal")]
        public decimal EvaluationScoreTotal { get; set; }
        /// <summary>
        /// 报名(投票,评选)截止时间
        /// </summary>
        [ORFieldMapping("EnrollDeadline")]
        [JsonConverter(typeof(ChinaDateTimeConverter))]
        public DateTime EnrollDeadline { get; set; }

        /// <summary>
        /// 作品评选方式0-自选，1-投票
        /// </summary>
        [ORFieldMapping("WorksSelectedType")]
        public WorksSelectedTypeEnum WorksSelectedType { get; set; }

        /// <summary>
        /// 评奖说明
        /// </summary>
        [ORFieldMapping("AwardsRemark")]
        public string AwardsRemark { get; set; }
        /// <summary>
        /// 是否从数据库加载
        /// </summary>
        [NoMapping]
        [JsonIgnore]
        public bool Loaded { get; set; }

        /// <summary>
        /// 剩余天数
        /// </summary>
        [NoMapping]
        public int RemainDays
        {
            get
            {
                if (!this.IsStart)
                    return this.RemainDaysStart;
                else
                    return this.RemainDaysEnd;

            }
        }


        /// <summary>
        /// 众筹是否开始
        /// </summary>
        [NoMapping]
        public bool IsStart
        {
            get
            {
                return DateTime.Now >= this.StartTime ? true : false;
            }
        }

        /// <summary>
        /// 众筹是否进行中
        /// </summary>
        [NoMapping]
        public bool IsProgressing
        {
            get
            {
                return this.IsStart && !this.IsEnd;
            }
        }

        /// <summary>
        /// 众筹是否结束
        /// </summary>
        [NoMapping]
        public bool IsEnd
        {
            get
            {
                return DateTime.Now > this.EndTime ? true : false;
            }
        }

        /// <summary>
        /// 众筹是否结束/投票,评选是否截止
        /// </summary>
        [NoMapping]
        public bool IsEnrollEnd
        {
            get
            {
                return DateTime.Now > this.EnrollDeadline ? true : false;
            }
        }

        /// <summary>
        /// 项目已结束，是否众筹成功
        /// </summary>
        [NoMapping]
        public bool IsSuccess
        {
            get
            {
                if (IsEnd)
                    return SupportNo >= TargetNo;
                else
                    return false;
            }
        }



        /// <summary>
        /// 距离开始剩余天数
        /// </summary>
        [NoMapping]
        public int RemainDaysStart
        {
            get
            {
                var days = (StartTime - DateTime.Now).Days;
                return days > 0 ? days : 0;
            }
        }

        /// <summary>
        /// 距离结束剩余天数
        /// </summary>
        [NoMapping]
        public int RemainDaysEnd
        {
            get
            {
                var days = (EndTime - DateTime.Now).Days;
                return days > 0 ? days : 0;
            }
        }

        /// <summary>
        /// 众筹是否结束
        /// </summary>
        [NoMapping]
        [Obsolete("使用IsEnd属性")]
        public bool IsOver
        {
            get
            {
                return DateTime.Now > this.EndTime ? true : false;
            }
        }

        /// <summary>
        /// 当前进度
        /// </summary>
        [NoMapping]
        public int Progress
        {
            get
            {
                if (TargetNo == 0) return 100;

                var progress = Math.Round(((double)SupportNo / (double)TargetNo), 2) * 100;
                return (int)progress;
            }
        }


        /// <summary>
        /// 项目状态
        /// </summary>
        [NoMapping]
        public virtual ProjectState ProgressState
        {
            get
            {
                switch (this.AuditStatus)
                {
                    case Enums.AuditStatus.None: return ProjectState.Draft;
                    case Enums.AuditStatus.Auditing: return ProjectState.Auditing;
                    case Enums.AuditStatus.Faid: return ProjectState.AuditFailed;
                    case Enums.AuditStatus.Success:
                        {
                            var isSuccess = this.IsSuccess ? ProjectState.Successed : ProjectState.Failed;
                            var isEnd = this.IsEnd ? isSuccess : ProjectState.Progressing;
                            return this.IsStart ? isEnd : ProjectState.Soon;
                        };
                    default: return ProjectState.Draft;
                }

            }
        }

        /// <summary>
        /// 项目状态文本
        /// </summary>
        [NoMapping]
        public virtual string ProgressStateText
        {
            get
            {
                string a = this.Type.ToString("D");
                int b = (int)this.ProgressState;
                //return EnumItemDescriptionAttribute.GetDescription(this.ProgressState);
                var projectState = EnumItemDescriptionAttribute.GetDescriptionList(
                    typeof(ProjectState)).Where(
                    w => w.Category.IndexOf(this.Type.ToString("D")) >= 0
                        && w.EnumValue == (int)this.ProgressState
                    ).FirstOrDefault();

                return projectState != null ? projectState.Description : "";
            }
        }
    }

    /// <summary>
    /// 众筹项目集合
    /// </summary>
    public class ProjectCollection : EditableDataObjectCollectionBase<Project>
    {
    }

    /// <summary>
    /// 众筹项目操作类
    /// </summary>
    public class ProjectAdapter : UpdatableAndLoadableAdapterBase<Project, ProjectCollection>
    {
        public static readonly ProjectAdapter Instance = new ProjectAdapter();

        private ProjectAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public Project LoadByCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", code);
            }).FirstOrDefault();
        }

        public void DeleteByCode(string code, bool trueDelete = false)
        {
            //逻辑删除
            this.SetFields("IsValid", false, where => where.AppendItem("Code", code));

            //物理删除
            if (trueDelete)
            {
                this.Delete(where => where.AppendItem("Code", code));
            }
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="codes"></param>
        /// <param name="trueDelete"></param>
        public void DeleteByCode(string[] codes, bool trueDelete = false)
        {
            InSqlClauseBuilder inSql = new InSqlClauseBuilder();
            inSql.AppendItem(codes);


            //物理删除
            if (trueDelete)
            {
                this.Delete(where => where.AppendItem("Code", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true));
            }
            else//逻辑删除
            {

                this.SetFields("IsValid", false, where => where.AppendItem("Code", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true));
            }
        }

        public ProjectCollection LoadAll()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }

        public ProjectCollection LoadAll(bool isValid)
        {
            return this.Load(where => where.AppendItem("IsValid", isValid));
        }
        /// <summary>
        /// 获得开始以后的特价/在售房
        /// </summary>        
        /// <returns></returns>
        public ProjectCollection LoadCrowdfunding(int minutes)
        {
            ProjectCollection proCollection = new ProjectCollection();
            proCollection.Concat(this.Load(where =>
            {
                where.AppendItem("IsValid", true);
                where.AppendItem("StartTime", DateTime.Now, "<=");
                where.AppendItem("StartTime", DateTime.Now.AddMinutes(-minutes), ">");
                where.AppendItem("Type", ProjectTypeEnum.Tejiafang);
            }));
            proCollection.Concat(this.Load(where =>
            {
                where.AppendItem("IsValid", true);
                where.AppendItem("StartTime", DateTime.Now, "<=");
                where.AppendItem("StartTime", DateTime.Now.AddMinutes(-minutes), ">");
                where.AppendItem("Type", ProjectTypeEnum.ZaiShou);
            }));
            return proCollection;
        }
        /// <summary>
        /// 获得即将开始的征集活动
        /// </summary>
        /// <returns></returns>
        public ProjectCollection LoadCollectCrowdfunding(int minutes)
        {
            return this.Load(where =>
             {
                 where.AppendItem("IsValid", true);
                 where.AppendItem("StartTime", DateTime.Now, "<=");
                 where.AppendItem("StartTime", DateTime.Now.AddMinutes(-minutes), ">");
                 where.AppendItem("Type", ProjectTypeEnum.Online);
             });
        }

        /// <summary>
        /// 获得即将开始投票的在线活动
        /// </summary>
        /// <returns></returns>
        public ProjectCollection LoadVoteCollectCrowdfunding(int minutes)
        {
            return this.Load(where =>
            {
                where.AppendItem("IsValid", true);
                where.AppendItem("EndTime", DateTime.Now, "<=");
                where.AppendItem("EndTime", DateTime.Now.AddMinutes(-minutes), ">");
                where.AppendItem("Type", ProjectTypeEnum.Online);
            });
        }
        public void AuditProject(int auditStatus, string projectCode)
        {
            this.SetFields(
                update =>
                {
                    update.AppendItem("AuditStatus", auditStatus.ToString());
                    update.AppendItem("ModifyTime", DateTime.Now);
                },
                where =>
                {
                    where.AppendItem("Code", projectCode);
                },
                this.GetConnectionName());

        }

        public void UpdateProjectStartTime(string projectCode)
        {
            this.SetFields(
                update =>
                {
                    update.AppendItem("StartTime", DateTime.Now);
                },
                where =>
                {
                    where.AppendItem("Code", projectCode);
                },
                this.GetConnectionName());

        }
        public void UpdateProjectIsValid(string projectCode, bool isValid)
        {
            this.SetFields(
                update =>
                {
                    update.AppendItem("IsValid", isValid);
                },
                where =>
                {
                    where.AppendItem("Code", projectCode);
                },
                this.GetConnectionName());

        }

        /// <summary>
        /// 支持人数/参与人数/作品/见过人数+1
        /// </summary>
        /// <param name="projectCode"></param>
        public void SetIncSupportNo(string projectCode)
        {
            this.SetInc("SupportNo", 1, where =>
            {
                where.AppendItem("Code", projectCode);
            }, this.GetConnectionName());
        }

        /// <summary>
        /// 作品-1
        /// </summary>
        /// <param name="projectCode"></param>
        public void SetDecSupportNo(string projectCode)
        {
            this.SetDec("SupportNo", 1, where =>
            {
                where.AppendItem("Code", projectCode);
            }, this.GetConnectionName());
        }

        /// <summary>
        /// 关注+1
        /// </summary>
        /// <param name="projectCode"></param>
        public void SetIncFocusNo(string projectCode)
        {
            this.SetInc("FocusNo", 1, where =>
            {
                where.AppendItem("Code", projectCode);
            }, this.GetConnectionName());
        }

        /// <summary>
        /// 关注-1
        /// </summary>
        /// <param name="projectCode"></param>
        public void SetDecFocusNo(string projectCode)
        {
            this.SetDec("FocusNo", 1, where =>
            {
                where.AppendItem("Code", projectCode);
            }, this.GetConnectionName());
        }

        /// <summary>
        /// 分享数+1
        /// </summary>
        /// <param name="projectCode"></param>
        public void SetIncShareNo(string projectCode)
        {
            this.SetInc("ShareNo", 1, where =>
            {
                where.AppendItem("Code", projectCode);
            }, this.GetConnectionName());
        }

        /// <summary>
        /// 浏览数+1
        /// </summary>
        /// <param name="projectCode"></param>
        public void SetClickNo(string projectCode)
        {
            this.SetInc("ClickNo", 1, where =>
            {
                where.AppendItem("Code", projectCode);
            }, this.GetConnectionName());
        }

        /// <summary>
        /// 评价数+1
        /// </summary>
        /// <param name="projectCode"></param>
        public void SetIncEvaluationUserTotal(string projectCode)
        {
            this.SetInc("EvaluationUserTotal", 1, where =>
            {
                where.AppendItem("Code", projectCode);
            }, this.GetConnectionName());
        }

        /// <summary>
        /// 评论数+1
        /// </summary>
        /// <param name="projectCode"></param>
        public void SetIncCommentNo(string projectCode)
        {
            this.SetInc("CommentNo", 1, where =>
            {
                where.AppendItem("Code", projectCode);
            }, this.GetConnectionName());
        }

        /// <summary>
        /// 评价总分值+
        /// </summary>
        /// <param name="projectCode"></param>
        /// <param name="score"></param>
        public void SetIncEvaluationScoreTotal(string projectCode, int score)
        {
            this.SetInc("EvaluationScoreTotal", score, where =>
            {
                where.AppendItem("Code", projectCode);
            }, this.GetConnectionName());
        }

        public void SetValid(string projectCode, bool valid)
        {
            this.SetFields("IsValid", valid, where =>
            {
                where.AppendItem("Code", projectCode);
            }, this.GetConnectionName());
        }
        /// <summary>
        /// 批量
        /// </summary>
        /// <param name="projectCodes"></param>
        /// <param name="valid"></param>
        public void SetValid(string[] projectCodes, bool valid)
        {
            InSqlClauseBuilder inSql = new InSqlClauseBuilder();
            inSql.AppendItem(projectCodes);
            this.SetFields("IsValid", valid, where =>
            {
                where.AppendItem("Code", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);
            }, this.GetConnectionName());
        }

        public ProjectCollection LoadHotActivity(int number, string city)
        {
            var cityCondition = (city.IsNotWhiteSpace() && city != "-1") ? city.Replace("市", "") : "";
            var result = new ProjectCollection();

            //只筛选进行中的活动
            //            string sql = string.Format(@"select top {0} * 
            //                                         from zc.Project 
            //                                         where IsValid = 1  
            //                                         and [type] in ({1},{2})  
            //                                         and auditStatus = 3  
            //                                         and StartTime <= getdate() 
            //                                         and EndTime > getdate() 
            //                                         and City like case when [type]=6 then '{3}%'  when [type]=7 then ''  end 
            //                                         order by SupportNo desc"
            //                                        , number, ProjectTypeEnum.Anchang.ToString("D"), ProjectTypeEnum.Online.ToString("D"),cityCondition);

            //进行中和已结束的全部筛选，先按照状态排序，再按照支持人数排序
            string sql = string.Format(@"select top {0} * ,case when getdate() <= EndTime then 1 when getdate() > EndTime  then 2 end [state]
                                         from zc.Project 
                                         where IsValid = 1  
                                         and [type] in ({1},{2})  
                                         and auditStatus = 3  
                                         and City like case when [type]=6 then '{3}%'  when [type]=7 then ''  end 
                                         order by [state] asc,SupportNo desc"
                                        , number, ProjectTypeEnum.Anchang.ToString("D"), ProjectTypeEnum.Online.ToString("D"), cityCondition);

            DataView dv = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0].DefaultView;
            ORMapping.DataViewToCollection(result, dv);

            return result;
        }

        public ProjectCollection LoadNewCrowdfunding(int number, string city)
        {
            var cityCondition = (city.IsNotWhiteSpace() && city != "-1") ? city.Replace("市", "") : "";
            var result = new ProjectCollection();

            //只筛选进行中的众筹
            //            string sql = string.Format(@"select top {0} * 
            //                                         from zc.Project 
            //                                         where IsValid = 1  
            //                                         and [type] in (2,3,4,5)  
            //                                         and auditStatus = 3  
            //                                         and StartTime <= getdate() 
            //                                         and EndTime > getdate() 
            //                                         and City like '{1}%' 
            //                                         order by ModifyTime desc"
            //                                        , number, cityCondition);

            string sql = string.Format(@"select top {0} * ,case when getdate() >= StartTime and getdate() <= EndTime then 1 when getdate() < StartTime  then 2 when getdate() > EndTime  then 3 end [state]
                                         from zc.Project 
                                         where IsValid = 1  
                                         and [type] in (2,3,4,5)  
                                         and auditStatus = 3  
                                         and City like '{1}%' 
                                         order by [state] asc, ModifyTime desc"
                                         , number, cityCondition);

            DataView dv = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0].DefaultView;
            ORMapping.DataViewToCollection(result, dv);

            return result;
        }
    }
    public class ChinaDateTimeConverter : DateTimeConverterBase
    {
        private static IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm" };

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return dtConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            dtConverter.WriteJson(writer, value, serializer);
        }
    }

}


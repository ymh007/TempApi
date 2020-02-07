using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Vote
{
    #region 创建投票问卷

    /// <summary>
    /// 创建投票问卷 ViewModel
    /// </summary>
    public class CreateViewModel
    {
        /// <summary>
        /// 投票问卷信息
        /// </summary>
        public CreateVoteInfoViewModel VoteInfo { get; set; }
        /// <summary>
        /// 投票问卷人员范围
        /// </summary>
        public List<CreatePersonViewModel> Person { get; set; }
        /// <summary>
        /// 投票问卷问题列表
        /// </summary>
        public List<CreateQuestionViewModel> Question { get; set; }
        /// <summary>
        /// 管理员列表
        /// </summary>
        public List<CreateVoteManagerViewModel> ManagerList { set; get; }
    }

    /// <summary>
    /// 投票问卷基本信息 ViewModel
    /// </summary>
    public class CreateVoteInfoViewModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 是否显示票数
        /// </summary>
        public bool IsShowPoll { get; set; }
        /// <summary>
        /// 是否显示结果
        /// </summary>
        public bool IsShowResult { get; set; }
        /// <summary>
        /// 投票类型 0：投票 1：问卷
        /// </summary>
        public int VoteType { get; set; }
    }

    /// <summary>
    /// 投票问卷人员范围 ViewModel
    /// </summary>
    public class CreatePersonViewModel
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; }
    }

    /// <summary>
    /// 投票问卷问题 ViewModel
    /// </summary>
    public class CreateQuestionViewModel
    {
        /// <summary>
        /// 题目
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 类型：0=单选；1=多选；2=填空
        /// </summary>
        public int QuestionType { get; set; }
        /// <summary>
        /// 最少选择
        /// </summary>
        public int MinChoice { get; set; }
        /// <summary>
        /// 最多选择
        /// </summary>
        public int MaxChoice { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 投票问题选项
        /// </summary>
        public List<CreateOptionViewModel> Option { get; set; }
    }

    /// <summary>
    /// 投票问卷问题选项 ViewModel
    /// </summary>
    public class CreateOptionViewModel
    {
        /// <summary>
        /// 选项名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 是否填空选项
        /// </summary>
        public bool IsFill { set; get; }
    }

    /// <summary>
    /// 投票问卷管理员 ViewModel
    /// </summary>
    public class CreateVoteManagerViewModel
    {
        /// <summary>
        /// 管理员编码
        /// </summary>
        public string ManagerCode { set; get; }
        /// <summary>
        /// 管理员名称
        /// </summary>
        public string ManagerDisplayName { set; get; }
    }

    #endregion

    #region 投票问卷列表

    /// <summary>
    /// 投票问卷列表 Base ViewModel
    /// </summary>
    public class VoteListBaseViewModel
    {
        /// <summary>
        /// 排序
        /// </summary>
        public long Row { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 0：投票 1：问卷
        /// </summary>
        public int VoteType { get; set; }
        /// <summary>
        /// 是否显示票数
        /// </summary>
        public bool IsShowPoll { get; set; }
        /// <summary>
        /// 是否显示结果
        /// </summary>
        public bool IsShowResult { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { set; get; }
        /// <summary>
        /// 创建人编码
        /// </summary>
        public string Creator { set; get; }
        /// <summary>
        /// 全部人员
        /// </summary>
        public int AllPerson { get; set; }
        /// <summary>
        /// 已经投人员
        /// </summary>
        public int CastPerson { get; set; }
        /// <summary>
        /// 是否已投
        /// </summary>
        public int IsCast { get; set; }
        /// <summary>
        /// 是否管理员
        /// </summary>
        public int IsManager { set; get; }
        /// <summary>
        /// 是否结束
        /// </summary>
        public bool IsOver
        {
            get
            {
                return (EndTime - DateTime.Now).TotalMinutes <= 0;
            }
        }
    }

    /// <summary>
    /// 获取我创建的列表 ViewModel
    /// </summary>
    public class VoteListByCreatedViewModel : VoteListBaseViewModel
    {
        /// <summary>
        /// 是否是参会人员
        /// </summary>
        public bool IsPerson { get; set; }
    }

    /// <summary>
    /// 获取我参与的列表 ViewModel
    /// </summary>
    public class VoteListByApplyViewModel : VoteListBaseViewModel
    {

    }

    #endregion

    #region 提交投票问卷结果

    /// <summary>
    /// 提交投票问卷结果 ViewModel
    /// </summary>
    public class CastVoteViewModel
    {
        /// <summary>
        /// 投票问卷编码
        /// </summary>
        public string VoteCode { get; set; }
        /// <summary>
        /// 问题集合
        /// </summary>
        public List<CastQuestionViewModel> CastQuestion { get; set; }
    }

    /// <summary>
    /// 提交投票问卷结果 - 问题 ViewModel
    /// </summary>
    public class CastQuestionViewModel
    {
        /// <summary>
        /// 问题Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 选项集合
        /// </summary>
        public List<CastOptionViewModel> CastOption { get; set; }
    }

    /// <summary>
    /// 提交投票问卷结果 - 选项 ViewModel
    /// </summary>
    public class CastOptionViewModel
    {
        /// <summary>
        /// 选项Code
        /// </summary>
        public string OptionCode { get; set; }
        /// <summary>
        /// 填写内容
        /// </summary>
        public string FillContent { set; get; }
    }

    #endregion

    #region 投票问卷详情

    /// <summary>
    /// 投票问卷详情 ViewModel
    /// </summary>
    public class VoteDetailsViewModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 0：投票；1：问卷
        /// </summary>
        public int VoteType { get; set; }
        /// <summary>
        /// 是否显示票数
        /// </summary>
        public bool IsShowPoll { get; set; }
        /// <summary>
        /// 是否显示结果
        /// </summary>
        public bool IsShowResult { get; set; }
        /// <summary>
        /// 是否过期
        /// </summary>
        public bool IsOverdue { get; set; }
        /// <summary>
        /// 是否已经投票
        /// </summary>
        public bool IsCast { get; set; }
        /// <summary>
        /// 投票截至时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 是否是发起人
        /// </summary>
        public bool IsCreator { get; set; }
        /// <summary>
        /// 投票问题
        /// </summary>
        public List<QuestionDetailsViewModel> Question { get; set; }
    }

    /// <summary>
    /// 投票问卷详情 问题 ViewModel
    /// </summary>
    public class QuestionDetailsViewModel
    {
        /// <summary>
        /// 问题编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 答案（填空题）
        /// </summary>
        public string Answer { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 最少选择
        /// </summary>
        public int MinChoice { get; set; }
        /// <summary>
        /// 最多选择
        /// </summary>
        public int MaxChoice { get; set; }
        /// <summary>
        /// 类型：0单选;1多选;2填空
        /// </summary>
        public int QuestionType { get; set; }
        /// <summary>
        /// 当前这道题投票人数
        /// </summary>
        public int CastCount { get; set; }
        /// <summary>
        /// 选项
        /// </summary>
        public List<OptionDetailsViewModel> OptionDetails { get; set; }
    }

    /// <summary>
    /// 投票问卷详情 选项 ViewModel
    /// </summary>
    public class OptionDetailsViewModel
    {
        /// <summary>
        /// 选项编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 选项名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 是否填空选项
        /// </summary>
        public bool IsFill { set; get; }
        /// <summary>
        /// 填空内容
        /// </summary>
        public string FillContent { set; get; }
        /// <summary>
        /// 是否选择
        /// </summary>
        public bool IsChoice { get; set; }
        /// <summary>
        /// 投票数
        /// </summary>
        public int CastCount { get; set; }
        /// <summary>
        /// 百分比
        /// </summary>
        public double Percentage { get; set; }
    }

    #endregion

    #region 设置

    /// <summary>
    /// 设置 ViewModel
    /// </summary>
    public class Seetings
    {
        /// <summary>
        /// 投票Code
        /// </summary>
        public string VoteCode { get; set; }
        /// <summary>
        /// 是否显示票数
        /// </summary>
        public bool IsShowPoll { get; set; }
        /// <summary>
        /// 是否显示结果
        /// </summary>
        public bool IsShowResult { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 人员范围
        /// </summary>
        public List<CreatePersonViewModel> Person { get; set; }
        /// <summary>
        /// 管理员列表
        /// </summary>
        public List<CreateVoteManagerViewModel> ManagerList { set; get; }
    }

    #endregion

    /// <summary>
    /// 发送系统提醒-ViewModel
    /// </summary>
    public class PushSystemMessage
    {
        /// <summary>
        /// 投票Code
        /// </summary>
        public string VoteCode { get; set; }
        /// <summary>
        /// 投票范围
        /// </summary>
        public List<CreatePersonViewModel> Person { get; set; }
    }

    /// <summary>
    /// 投票人员列表-ViewModel
    /// </summary>
    public class PersonList
    {
        /// <summary>
        /// 已投人员
        /// </summary>
        public List<PersonInfo> CastPerson { get; set; }

        /// <summary>
        /// 未投人员
        /// </summary>
        public List<PersonInfo> NoCastPerson { get; set; }
    }

    /// <summary>
    /// 投票人员-辅助类
    /// </summary>
    public class PersonInfo : CreatePersonViewModel
    {
        /// <summary>
        /// 头像
        /// </summary>
        public string HeadPhoto { get; set; }
    }

    /// <summary>
    /// 投票Excel导出帮助类
    /// </summary>
    public class ExcelHelper
    {
        /// <summary>
        /// 投票编码
        /// </summary>
        public string VoteCode { get; set; }
        /// <summary>
        /// 问题编码
        /// </summary>
        public string QuestionCode { get; set; }
        /// <summary>
        /// 选项编码
        /// </summary>
        public string OptionCode { get; set; }
        /// <summary>
        /// 用户编码
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// 投票标题
        /// </summary>
        public string InfoTitle { get; set; }

        /// <summary>
        /// 问题名称
        /// </summary>
        public string QuestionTitle { get; set; }

        /// <summary>
        /// 选项名称
        /// </summary>
        public string OptionName { get; set; }
    }

    /// <summary>
    /// 获取设置规则-ViewModel
    /// </summary>
    public class GetSeetings
    {
        /// <summary>
        /// 当前投票信息
        /// </summary>
        public SeetingsVoteInfo VoteInfo { get; set; }
        /// <summary>
        /// 投票范围
        /// </summary>
        public List<CreatePersonViewModel> PersonList { get; set; }
        /// <summary>
        /// 管理员列表
        /// </summary>
        public List<CreateVoteManagerViewModel> ManagerList { set; get; }
    }

    /// <summary>
    /// 投票信息
    /// </summary>
    public class SeetingsVoteInfo
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 是否显示票数
        /// </summary>
        public bool IsShowPoll { get; set; }

        /// <summary>
        /// 是否显示结果
        /// </summary>
        public bool IsShowResult { get; set; }
    }
}
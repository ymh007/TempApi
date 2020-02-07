using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.PlanManage
{
	/// <summary>
	/// 计划管理
	/// </summary>
	public class PlanManageViewModel
	{
		/// <summary>
		/// 节点信息
		/// </summary>
		public List<PointItem> EdgeList { get; set; }


		/// 节点入度列表
		/// </summary>
		public List<IndegreeItem> VertexList { get; set; }
	}

	/// <summary>
	/// 任务
	/// </summary>
	public class Task
	{
		/// <summary>
		/// 任务名称
		/// </summary>
		public string TaskName;
	}

	/// <summary>
	/// 计划
	/// </summary>
	public class Plan
	{
		/// <summary>
		/// 计划名称
		/// </summary>
		public string PlanName { get; set; }

		/// <summary>
		/// 计划编码
		/// </summary>
		public string PlanCode { get; set; }
	}

	/// <summary>
	/// 关系（项目起始点为：-1;项目终结点为：0）
	/// </summary>
	public class Relation
	{
		/// <summary>
		/// 起始点
		/// </summary>
		public string StartPoint { get; set; }

		/// <summary>
		/// 终结点
		/// </summary>
		public string EndPoint { get; set; }

		/// <summary>
		/// 长度
		/// </summary>
		public string Length { get; set; }
	}

	public class PointItem
	{
		/// <summary>
		/// 任务
		/// </summary>
		public Task Task { get; set; }

		/// <summary>
		/// 计划
		/// </summary>
		public Plan Plan { get; set; }

		/// <summary>
		/// 关系
		/// </summary>
		public Relation Relation { get; set; }
	}

	/// <summary>
	/// 节点入度对象
	/// </summary>
	public class IndegreeItem
	{

		/// <summary>
		/// 当前节点
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 节点信息
		/// </summary>
		public string Name { get; set; }
	}

	/// <summary>
	/// 数据帮助类
	/// </summary>
	public class DataHelper
	{
		/// <summary>
		/// 计划名称
		/// </summary>
		public string CnName { get; set; }
		/// <summary>
		/// 任务编码
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 计划编码
		/// </summary>
		public string PlanCode { get; set; }
		/// <summary>
		/// 任务名称
		/// </summary>
		public string TaskName { get; set; }
		/// <summary>
		/// 父编码
		/// </summary>
		public string ParentCode { get; set; }
		/// <summary>
		/// 行号
		/// </summary>
		public int RowIndex { get; set; }
		/// <summary>
		/// 是否为关键路径
		/// </summary>
		public bool IsKeyPath { get; set; }
		/// <summary>
		/// 开始时间
		/// </summary>
		public DateTime StartTime { get; set; }
		/// <summary>
		/// 结束时间
		/// </summary>
		public DateTime EndTime { get; set; }
		/// <summary>
		/// 入度
		/// </summary>
		public string Count { get; set; }
	}
}
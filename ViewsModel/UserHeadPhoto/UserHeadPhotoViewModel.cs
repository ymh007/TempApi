using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.UserHeadPhoto
{
	/// <summary>
	/// 上传用户头像ViewModel
	/// </summary>
	public class UserHeadPhotoSaveViewModel
	{
		/// <summary>
		/// 头像（地址）
		/// </summary>
		public string Url { set; get; }
	}

	/// <summary>
	/// 获取头像列表ViewModel
	/// </summary>
	public class UserHeadPhotoListViewModel
	{
		/// <summary>
		/// 编码
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 用户编码
		/// </summary>
		public string UserCode { get; set; }

		/// <summary>
		/// 用户姓名
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// 头像地址
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// 是否操作
		/// </summary>
		public bool IsOperate { get; set; }

		/// <summary>
		/// 是否审核
		/// </summary>
		public bool IsAudit { get; set; }

		/// <summary>
		/// 操作人编码
		/// </summary>
		public string Operator { get; set; }

		/// <summary>
		/// 操作人名称
		/// </summary>
		public string OperatorName { get; set; }

		/// <summary>
		/// 操作人时间
		/// </summary>
		public string OperateTime { get; set; }

		/// <summary>
		/// 创建人
		/// </summary>
		public string Creator { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public string CreateTime { get; set; }
	}

	/// <summary>
	/// 获取用户头像--APP
	/// </summary>
	public class UserHeadPhotoLViewModel
	{
		/// <summary>
		/// 状态0：不存在 1：存在
		/// </summary>
		public bool State { get; set; }

		/// <summary>
		/// 编码
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 用户编码
		/// </summary>
		public string UserCode { get; set; }

		/// <summary>
		/// 用户姓名
		/// </summary>
		public string UserName { get; set; }
		/// <summary>
		/// 头像地址
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// 是否操作
		/// </summary>
		public bool IsOperate { get; set; }

		/// <summary>
		/// 是否审核
		/// </summary>
		public bool IsAudit { get; set; }
	}

	/// <summary>
	/// 获取用户头像列表
	/// </summary>
	public class UserHeadPhotoList
	{
		/// <summary>
		/// 用户编码
		/// </summary>
		public string UserCode { get; set; }
		/// <summary>
		/// 头像地址
		/// </summary>
		public string UserHeadPhoto { get; set; }
	}

}
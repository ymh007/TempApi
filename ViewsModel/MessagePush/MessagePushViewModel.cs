using Seagull2.YuanXin.AppApi.Adapter.MessagePush;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.MesagePush
{
	/// <summary>
	/// 群组
	/// </summary>
	public class MessageGroupFullViewModel
	{
		/// <summary>
		/// 群组信息
		/// </summary>
		public MessagePushGroupViewModel Group { set; get; }
		/// <summary>
		/// 人员列表
		/// </summary>
		public List<MessagePushGroupPersonViewModel> Persons { set; get; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string SourceType { set; get; }

    }

	/// <summary>
	/// 群组信息
	/// </summary>
	public class MessagePushGroupViewModel
	{
		/// <summary>
		/// 群组编码
		/// </summary>
		public string Code { set; get; }
		/// <summary>
		/// 群组名称
		/// </summary>
		public string Name { set; get; }
	}


	/// <summary>
	/// 群组人员信息
	/// </summary>
	public class MessagePushGroupPersonViewModel
	{
		/// <summary>
		/// 人员编码
		/// </summary>
		public string Code { set; get; }
		/// <summary>
		/// 人员名称
		/// </summary>
		public string Name { set; get; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public string Department { set; get; }

        /// <summary>
        /// 区分人员跟部门User/Organizations
        /// </summary>
        public string SchemaType { set; get; }
         

    }

	/// <summary>
	/// 群组信息
	/// </summary>
	public class MessagePushGrouViewModel
	{
		/// <summary>
		/// 群组编码
		/// </summary>
		public string Code { set; get; }
		/// <summary>
		/// 群组名称
		/// </summary>
		public string Name { set; get; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public string CreateTime { set; get; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { set; get; }

    }

    /// <summary>
    /// 消息
    /// </summary>
    public class PushMessageSaveViewModel
    {
        /// <summary>
        /// 主键编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { set; get; }

        /// <summary>
        /// 群组编码
        /// </summary>
        public string PushGroupCode { set; get; }

        /// <summary>
        /// 群组名称
        /// </summary>
        public string PushGroupName { set; get; }

        /// <summary>
        /// 定时发送时间
        /// </summary>
        public DateTime? TimingSend { get; set; }

        /// <summary>
        /// 是否定时发送
        /// </summary>
        public bool IsTiming { get; set; }

        /// <summary>
        /// 发送状态  1 成功  2 失败  3 待发送 4  已撤销
        /// </summary>
        public int SendStatus { get; set; }

        /// <summary>
        /// 是否预览
        /// </summary>
        public bool IsView { get; set; }

        /// <summary>
        /// 新加人员字段保存在组员表里面
        /// </summary>
        public List<Person> PersonList { get; set; }
        /// <summary>
        /// 消息关联字段
        /// </summary>
        public string SourceType { get; set; }


        /// <summary>
        /// 消息类型
        /// </summary>
        public int MsgType { get; set; }
        /// <summary>
        /// 消息图片url
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 消息图片url
        /// </summary>
        public string ImgTitle { get; set; }

        /// <summary>
        /// 点击事件类型
        /// </summary>
        public int EventType { get; set; }


    }
    /// <summary>
    /// 定时推送消息
    /// </summary>
    public class SysResdisModel
    {
        public string Code { set; get; }

        public string TimingSend { get; set; }
    }

	/// <summary>
	/// 消息记录
	/// </summary>
	public class PushMessageGetViewModel
	{

        /// <summary>
        /// 主键编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public long Sort { get; set; }
		/// <summary>
		/// 标题
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// 内容
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// 群组Code
		/// </summary>
		public string PushGroupCode { get; set; }
		/// <summary>
		/// 群组名称
		/// </summary>
		public string PushGroupName { get; set; }

		/// <summary>
		/// 发送时间
		/// </summary>
		public DateTime CreateTime { get; set; }

		/// <summary>
		/// 创建人名称
		/// </summary>
		public string Creator { get; set; }
		/// <summary>
		/// 创建人名称
		/// </summary>
		public string CreateName { get; set; }


        /// <summary>
        /// 定时发送时间
        /// </summary>
        public DateTime TimingSend { get; set; }

        /// <summary>
        /// 是否定时发送
        /// </summary>
        public bool IsTiming { get; set; }

        /// <summary>
        /// 发送状态  1 成功  2 失败  3 待发送 4  已撤销
        /// </summary>
        public int SendStatus { get; set; }

        /// <summary>
        /// 添加数据时候记得加上备注，已备后面更改人员观看
        /// 数据源类型  1 消息管理  会议通告 时候就是会议的id
        /// </summary>
        
        public string SourceType { get; set; }


        /// <summary>
        /// 消息类型
        /// </summary>
        public int MsgType { get; set; }
        /// <summary>
        /// 消息图片url
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 消息图片url
        /// </summary>
        public string ImgTitle { get; set; }

        /// <summary>
        /// 点击事件类型
        /// </summary>
        public int EventType { get; set; }

    }

}
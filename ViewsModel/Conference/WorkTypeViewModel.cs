using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Conference
{
	/// <summary>
	/// 工作人员类型-ViewModel
	/// </summary>
	public class WorkTypeViewModel
	{
		/// <summary>
		/// 保存-ViewModel
		/// </summary>
		public class SaveModel
		{
			/// <summary>
			/// ID
			/// </summary>
			public string ID { get; set; }

			/// <summary>
			/// 工作人员类型名称
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			/// 排序
			/// </summary>
			public int Sort { get; set; }


            /// <summary>
            /// 关联ID
            /// </summary>
            public string ContactId { get; set; }
        }

		/// <summary>
		/// 工作类型列表-ViewModel
		/// </summary>
		public class GetModel
		{
			/// <summary>
			/// ID
			/// </summary>
			public string ID { get; set; }

			/// <summary>
			/// 名称
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			/// 创建人
			/// </summary>
			public string Creator { get; set; }

			/// <summary>
			/// 创建时间
			/// </summary>
			public string CreateTime { get; set; }

			/// <summary>
			/// 排序
			/// </summary>
			public int Sort { get; set; }

            /// </summary>
			public string ContactId { get; set; }
        }
	}
}
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 会议 Model
    /// </summary>
    [ORTableMapping("office.Conference")]
    public class ConferenceModel
    {
        /// <summary>
        /// 会议主键ID
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID { get; set; }

        /// <summary>
        /// 会议名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 会议描述
        /// </summary>
        [ORFieldMapping("Description")]
        public string Description { get; set; }

        /// <summary>
        /// 会议开始时间
        /// </summary>
        [ORFieldMapping("BeginDate")]
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 会议开始时间Str
        /// </summary>
        [NoMapping]
        public string BeginDateStr
        {
            get
            {
                return this.BeginDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 会议结束时间
        /// </summary>
        [ORFieldMapping("EndDate")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 会议结束时间Str
        /// </summary>
        [NoMapping]
        public string EndDateStr
        {
            get
            {
                return this.EndDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 会议座位布局
        /// </summary>
        [ORFieldMapping("Layout")]
        public string Layout { get; set; }

        /// <summary>
        /// 封面图
        /// </summary>
        [ORFieldMapping("Image")]
        public string Image { get; set; }

        /// <summary>
        /// 封面图地址
        /// </summary>
        [NoMapping]
        public string ShowImage
        {
            get
            {
                if (this.Image.StartsWith("YuanXin-File://"))
                {
                    return FileService.DownloadFile(this.Image);
                }
                else

                {
                    return ConfigurationManager.AppSettings["ConferenceImageDownLoadRootPath"] + "/" + this.Image;
                }
            }
        }

        /// <summary>
        /// 会议举办城市
        /// </summary>
        [ORFieldMapping("City")]
        public string City { get; set; }

        /// <summary>
        /// 会议举办酒店
        /// </summary>
        [ORFieldMapping("Hotel")]
        public string Hotel { get; set; }

        /// <summary>
        /// 会议宴会厅
        /// </summary>
        [ORFieldMapping("Ballroom")]
        public string Ballroom { get; set; }


        /// <summary>
        /// 会议宴请厅
        /// </summary>
        [ORFieldMapping("EntertainHall")]
        public string EntertainHall { get; set; }


        /// <summary>
        /// 会议地点
        /// </summary>
        [ORFieldMapping("Address")]
        public string Address { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [ORFieldMapping("Longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [ORFieldMapping("Latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// 会议须知
        /// </summary>
        [ORFieldMapping("Notice")]
        public string Notice { get; set; }

        /// <summary>
        /// 是否发布
        /// </summary>
        [ORFieldMapping("IsPublic")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// 创建人(海鸥二用户编码)
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        [ORFieldMapping("CreateName")]
        public string CreateName { get; set; }
        /// <summary>
        /// 创建人英文名成缩写
        /// </summary>
        [ORFieldMapping("EcreateName")]
        public string EcreateName { get; set; }
        /// <summary>
        /// 协作者
        /// </summary>
        [ORFieldMapping("EnviteCollaboration")]
        public string EnviteCollaboration { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 数据有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }

    }

    /// <summary>
    /// 会议 Collection
    /// </summary>
    public class ConferenceModelCollection : EditableDataObjectCollectionBase<ConferenceModel>
    {

    }
}
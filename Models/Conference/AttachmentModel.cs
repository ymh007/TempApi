using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 附件表
    /// </summary>
    [ORTableMapping("office.Attachment")]
    public class AttachmentModel
    {

        /// <summary>
        /// 主键ID
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID { get; set; }

        /// <summary>
        /// 资源ID
        /// </summary>
        [ORFieldMapping("ResourceID")]
        public string ResourceID { get; set; }

        /// <summary>
        /// 相对路径
        /// </summary>
        [ORFieldMapping("RelativePath")]
        public string RelativePath { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [ORFieldMapping("FileName")]
        public string FileName { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        [ORFieldMapping("FileType")]
        public string FileType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }

        /// <summary>
        /// 文件完整路径
        /// </summary>
        [ORFieldMapping("FilePath")]
        public string FilePath { get; set; }

        /// <summary>
        /// 显示路径
        /// </summary>
        [NoMapping]
        public string ShowPath
        {
            get
            {
                string path = "";
                if (FilePath.IsEmptyOrNull())
                {
                    path = AttachmentAdapter.Instance.downLoadRootUrl + RelativePath + "/" + FileName + "." + FileType;
                }
                else
                {
                    path = FilePath;
                }
                return path;
            }
        }
    }

    /// <summary>
    /// 附件集合
    /// </summary>
    public class AttachmentCollection : EditableDataObjectCollectionBase<AttachmentModel>
    {

    }

    /// <summary>
    /// 附件 Adapter
    /// </summary>
    public class AttachmentAdapter : BaseAdapter<AttachmentModel, AttachmentCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static AttachmentAdapter Instance = new AttachmentAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        private string ConnectionString = ConnectionNameDefine.YuanXinBusiness;

        /// <summary>
        /// 上传文件路径(如果路径含有相对路径标记~ 则使用Server获取完整路径，否则获取配置路径)
        /// </summary>
        public string upLoadRootUrl = ConfigurationManager.AppSettings["uploadRootPath"].Contains("~") ? HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["uploadRootPath"]) : ConfigurationManager.AppSettings["uploadRootPath"];

        /// <summary>
        /// 下载文件路径
        /// </summary>
        public string downLoadRootUrl = ConfigurationManager.AppSettings["downLoadRootPath"];

        /// <summary>
        /// 是否上传到本地服务器
        /// </summary>
        public bool IsUploadLocalService = ConfigurationManager.AppSettings["IsUploadLocalService"] == "true" ? true : false;

        /// <summary>
        /// 构造
        /// </summary>
        public AttachmentAdapter()
        {
            BaseConnectionStr = this.ConnectionString;
        }

        /// <summary>
        /// 根据资源ID查询所有附件
        /// </summary>
        public AttachmentCollection LoadByResourceID(string resourceID)
        {
            return Load(m => m.AppendItem("ResourceID", resourceID));
        }

        /// <summary>
        /// 设置用户权限通过远程文件增删
        /// </summary>
        private void UploadFileByUser(Action uploadAction)
        {
            //参考类：D:\SourceCode\MCSFramework\02.Develop\MobileWebApp\YuanXin\Services\FileUploadService\Controllers\UploadController.cs
            //无法通过权限认证--只能通过外网访问
            try
            {
                if (IsUploadLocalService == false)
                {
                    var ip = ConfigurationManager.AppSettings["uploadServiceIP"].ToString();  //上传的服务器IP--10.0.8.52
                    var domain = "sinooceanland";
                    var username = ConfigurationManager.AppSettings["uploadUserName"].ToString();  //配置的用户名--v-zhaotzh
                    var pwd = ConfigurationManager.AppSettings["uploadPassword"].ToString();    //配置的密码--pass@123
                    var root = upLoadRootUrl;   //配置的文件根路径--~/Resources/Pic

                    using (NetworkShareAccesser.Access(ip, domain, username, pwd))  //建立连接
                    {
                        uploadAction();   //图片保存代码
                    }
                }
                else
                {
                    uploadAction();   //图片保存代码
                }
            }
            catch
            {

            }
        }

        #region 文件保存
        /// <summary>
        /// 保存附件集合
        /// </summary>
        /// <returns></returns>
        private int SaveFileColl(AttachmentCollection acoll)
        {
            StringBuilder sql = new StringBuilder();
            acoll.ForEach(ac =>
            {
                sql.Append(ORMapping.GetInsertSql<AttachmentModel>(ac, TSqlBuilder.Instance, "") + ";");
            });
            int result = ViewBaseAdapter<AttachmentModel, List<AttachmentModel>>.Instance.RunSQLByTransaction(sql.ToString(), ConnectionNameDefine.YuanXinForDBHelp);
            return result;
        }

        /// <summary>
        /// base64图片保存保存图片至服务器及将数据入库
        /// </summary>
        /// <param name="picModel"></param>
        public void SavePicColl(PictureHelp picModel)
        {
            UploadFileByUser(() =>
            {
                #region 创建目录
                //完整存储路径
                string completeUrl = "";
                //相对等级路径
                string relativeUrl = "";

                //添加根目录
                completeUrl = upLoadRootUrl;
                //添加一级目录
                string relativeOneUrl = DateTime.Now.Year.ToString();
                completeUrl += "\\" + relativeOneUrl;
                relativeUrl += "/" + relativeOneUrl;
                if (!Directory.Exists(completeUrl))
                {
                    Directory.CreateDirectory(completeUrl);
                }
                //添加二级目录
                string relativeTwoUrl = DateTime.Now.Month.ToString();
                completeUrl += "\\" + relativeTwoUrl;
                relativeUrl += "/" + relativeTwoUrl;
                if (!Directory.Exists(completeUrl))
                {
                    Directory.CreateDirectory(completeUrl);
                }
                #endregion

                AttachmentCollection modelColl = new AttachmentCollection();

                #region 来源于APP
                if (picModel.AppPicColl != null && picModel.AppPicColl.Count > 0)
                {
                    List<AppPicHelp> strPicColl = picModel.AppPicColl;

                    foreach (var strPic in strPicColl)
                    {
                        byte[] bytes = Convert.FromBase64String(strPic.picCode);
                        MemoryStream memStream = new MemoryStream(bytes);
                        BinaryFormatter binFormatter = new BinaryFormatter();
                        System.Drawing.Bitmap map = new Bitmap(memStream);
                        Image image = (Image)map;

                        //保存图片
                        string imageName = Guid.NewGuid().ToString("N");  //图片重命名
                        image.Save(completeUrl + "\\" + imageName + "." + strPic.picType);  //保存图片

                        AttachmentModel model = new AttachmentModel()
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            CreateTime = DateTime.Now,
                            FileName = imageName,
                            FileType = strPic.picType,
                            RelativePath = relativeUrl,
                            ResourceID = picModel.ResourceID,
                            ValidStatus = true
                        };
                        modelColl.Add(model);
                    }
                }
                #endregion

                #region 来源于PC端
                //来源于PC端
                else if (picModel.PcFileColl != null && picModel.PcFileColl.Count > 0)
                {
                    HttpFileCollection picColl = picModel.PcFileColl;

                    //入库
                    for (var i = 0; i < picColl.Count; i++)
                    {
                        HttpPostedFile file = picColl[i];

                        string[] oldFileNameList = file.FileName.Split('.');
                        string fileName = oldFileNameList[0] + "_" + DateTime.Now.ToString("HH-mm-ss") + "_" + Guid.NewGuid().ToString("N").Substring(0, 5) + "." + oldFileNameList[1];
                        string[] newFileNameList = fileName.Split('.');

                        //保存图片
                        //保存至指定目录
                        file.SaveAs(completeUrl + "\\" + fileName);

                        AttachmentModel model = new AttachmentModel
                        {
                            ID = System.Guid.NewGuid().ToString("N"),
                            CreateTime = DateTime.Now,
                            ValidStatus = true,
                            FileName = newFileNameList[0],
                            FileType = newFileNameList[1],
                            RelativePath = relativeUrl,
                            ResourceID = picModel.ResourceID
                        };
                        modelColl.Add(model);
                    }
                }
                #endregion

                if (modelColl.Count > 0)
                {
                    //图片数据入库
                    AttachmentAdapter.Instance.SaveFileColl(modelColl);
                }
            });
        }
        /// <summary>
        /// APP最新图片保存方法--不负责图片上传及将数据入库
        /// </summary>
        /// <param name="picModel"></param>
        public void SaveAppPicColl(PictureHelp picModel)
        {
            AttachmentCollection modelColl = new AttachmentCollection();

            #region 来源于APP
            if (picModel.AppPicColl != null && picModel.AppPicColl.Count > 0)
            {
                List<AppPicHelp> strPicColl = picModel.AppPicColl;

                foreach (var strPic in strPicColl)
                {
                    AttachmentModel model = new AttachmentModel()
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        CreateTime = DateTime.Now,
                        FileName = "",
                        FileType = strPic.picType,
                        RelativePath = "",
                        FilePath = strPic.picUrl,
                        ResourceID = picModel.ResourceID,
                        ValidStatus = true
                    };
                    modelColl.Add(model);
                }
            }
            #endregion

            if (modelColl.Count > 0)
            {
                //图片数据入库
                AttachmentAdapter.Instance.SaveFileColl(modelColl);
            }
        }
        #endregion

        /// <summary>
        /// base64图片上传图片功能
        /// </summary>
        /// <param name="appPicList">图片的全路径访问集合</param>
        /// <returns></returns>
        public List<string> SaveImg(List<AppPicHelp> appPicList)
        {
            List<string> picPathList = new List<string>();

            UploadFileByUser(() =>
            {
                #region 创建目录
                //完整存储路径
                string completeUrl = "";
                //相对等级路径
                string relativeUrl = "";

                //添加根目录
                completeUrl = upLoadRootUrl;
                //添加一级目录
                string relativeOneUrl = DateTime.Now.Year.ToString();
                completeUrl += "\\" + relativeOneUrl;
                relativeUrl += "/" + relativeOneUrl;
                if (!Directory.Exists(completeUrl))
                {
                    Directory.CreateDirectory(completeUrl);
                }
                //添加二级目录
                string relativeTwoUrl = DateTime.Now.Month.ToString();
                completeUrl += "\\" + relativeTwoUrl;
                relativeUrl += "/" + relativeTwoUrl;
                if (!Directory.Exists(completeUrl))
                {
                    Directory.CreateDirectory(completeUrl);
                }
                #endregion

                #region 来源于APP
                if (appPicList != null && appPicList.Count > 0)
                {
                    List<AppPicHelp> strPicColl = appPicList;

                    foreach (var strPic in strPicColl)
                    {
                        byte[] bytes = Convert.FromBase64String(strPic.picCode);
                        MemoryStream memStream = new MemoryStream(bytes);
                        BinaryFormatter binFormatter = new BinaryFormatter();
                        System.Drawing.Bitmap map = new Bitmap(memStream);
                        Image image = (Image)map;
                        //保存图片
                        string imageName = Guid.NewGuid().ToString("N");  //图片重命名
                        image.Save(completeUrl + "\\" + imageName + "." + strPic.picType);  //保存图片
                        picPathList.Add(ConfigurationManager.AppSettings["downLoadRootPath"] + relativeUrl + "/" + imageName + "." + strPic.picType);
                    }
                }
                #endregion
            });
            return picPathList;
        }

        /// <summary>
        /// 删除会议圈下图片文件及相关数据
        /// </summary>
        /// <param name="momentID"></param>
        public void DelPicColl(string momentID)
        {
            AttachmentCollection coll = AttachmentAdapter.Instance.Load(m => m.AppendItem("ResourceID", momentID));
            //删除数据
            Delete(m => m.AppendItem("ResourceID", momentID));
        }
    }
    #region 图片帮助类
    public class PictureHelp
    {
        /// <summary>
        /// 资源ID
        /// </summary>
        public string ResourceID { get; set; }
        /// <summary>
        /// pc上传图片集合
        /// </summary>
        public HttpFileCollection PcFileColl { get; set; }
        /// <summary>
        /// app上传图片集合
        /// </summary>
        public List<AppPicHelp> AppPicColl { get; set; }
    }
    public class AppPicHelp
    {
        /// <summary>
        /// base64字节
        /// </summary>
        public string picCode { get; set; }
        /// <summary>
        /// 图片类型（jpg/png/...）
        /// </summary>
        public string picType { get; set; }
        /// <summary>
        /// 图片完整路径
        /// </summary>
        public string picUrl { get; set; }
    }
    #endregion
}
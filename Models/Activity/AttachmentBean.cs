
namespace Seagull2.YuanXin.AppApi.Models
{   
    //存储图片等资源的附件表 
    public class AttachmentBean
    {
        public string Code { get; set; }//ID，唯一标示GUID
        public string Creator { get; set; }//创建人
        public string ResourceID { get; set; }//与对应项目关联的ID，一般为projectID
        public string CnName { get; set; }//中文名称
        public string URL { get; set; }//地址
        public int FileSize { get; set; }//文件大小
        public string VersionStartTime { get; set; }//创建时间
        public string VersionEndTime { get; set; }//有效时间
        public int ValidStatus { get; set; }//有效性
        public string Suffix { get; set; }//后缀名
        public int SortNo { get; set; }//序号
        public int AttachmentTypeCode { get; set; } //附件类型标识，对应的AttachmentType
        
    }
}
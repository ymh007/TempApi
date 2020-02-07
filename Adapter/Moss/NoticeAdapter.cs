using log4net;
using Newtonsoft.Json;
using Seagull2.Owin.File.Models;
using Seagull2.YuanXin.AppApi.ViewsModel.Moss;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;

namespace Seagull2.YuanXin.AppApi.Adapter.Moss
{
    /// <summary>
    /// 通知纪要 Adapter
    /// </summary>
    public class NoticeAdapter
    {
        /// <summary>
        /// 附件预览地址缓存
        /// </summary>
        static Dictionary<string, string> UrlCache = new Dictionary<string, string>();

        /// <summary>
        /// 面向项目的首页数据缓存
        /// </summary>
        static List<string> ProjectFirstPageData = new List<string>();

        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 实例
        /// </summary>
        public static readonly NoticeAdapter Instance = new NoticeAdapter();

        const string XmlNotifications = "Notifications.xml";

        #region 获取通知纪要列表
        /// <summary>
        /// 获取通知纪要列表
        /// </summary>
        public List<NoticeListViewModel> GetNoticeList(string userName, string type, string created, string company, string keyword, bool isYB = false)
        {
            var list = new List<NoticeListViewModel>();
            MossNoticeService moss;
            if (isYB)
            {
                moss = new MossNoticeService(XmlNotifications, new string[] { "职能中心", "远洋邦邦" });
            }
            else
            {
                moss = new MossNoticeService(XmlNotifications);
            }

            var data = moss.GetNoticeList(userName, type, created, company, keyword);
            string source = "";
            data.ForEach(item =>
            {
                source = MossNoticeService.Webs.Count > 0 ? MossNoticeService.Webs.FirstOrDefault(x=>x.Key== item.WebId).Value : item._x4e3b__x9001__x5355__x4f4d_;
                list.Add(new NoticeListViewModel()
                {
                    WebId = item.WebId,
                    ListId = item.ListId,
                    ID = Convert.ToInt32(item.ID),
                    Title = item.Title,
                    Created = item.Created,
                    Source = source
                });
            });
            return list;
        }
        #endregion

        #region 获取面向项目列表
        /// <summary>
        /// 获取面向项目列表
        /// </summary>
        public List<ImportantNoticeListViewModel> GetImportantNoticeList(string userName, string created, string company, string keyword, string xmlType = "")
        {
            var view = new List<ImportantNoticeListViewModel>();

            // 获取Moss数据
            var xmlName = "Notifications.xml";
            if (xmlType == "party")
            {
                xmlName = "NotificationsParty.xml";
            }
            if (xmlType == "yb")
            {
                xmlName = "NotificationsYB.xml";
            }
            MossNoticeService ms = new MossNoticeService(xmlName);
            var moss = new List<MossNoticeService.ImportantNoticeListModel>();
            string source = "";
            if (string.IsNullOrWhiteSpace(created))// 第一页
            {
                moss = ms.GetForProjectFirstPage(true, 20, userName);
                moss.ForEach(f =>
                {
                    if (!ProjectFirstPageData.Exists(e => e == f.ID))
                    {
                        ProjectFirstPageData.Add(f.ID);
                    }
                    source = MossNoticeService.Webs.Count > 0 ? MossNoticeService.Webs.FirstOrDefault(x => x.Key == f.WebId).Value : f._x4e3b__x9001__x5355__x4f4d_;
                    view.Add(new ImportantNoticeListViewModel()
                    {
                        WebId = f.WebId,
                        ListId = f.ListId,
                        ID = Convert.ToInt32(f.ID),
                        Title = f.Title,
                        Created = f.Created,
                        Source = source
                    });
                });

            }
            else
            {
                moss = ms.GetForProjectNormal(userName, created, company, keyword);
                moss.ForEach(f =>
                {
                    if (!ProjectFirstPageData.Exists(e => e == f.ID))
                    {
                        source = MossNoticeService.Webs.Count > 0 ? MossNoticeService.Webs.FirstOrDefault(x => x.Key == f.WebId).Value : f._x4e3b__x9001__x5355__x4f4d_;
                        view.Add(new ImportantNoticeListViewModel()
                        {
                            WebId = f.WebId,
                            ListId = f.ListId,
                            ID = Convert.ToInt32(f.ID),
                            Title = f.Title,
                            Created = f.Created,
                            Source = source
                        });
                    }
                });
            }
            return view;
        }
        #endregion

        #region 获取最新发文列表
        /// <summary>
        /// 获取最新发文列表
        /// </summary>
        public List<ImportantNoticeListViewModel> GetNewNotifys(string userName, string created, string company, string keyword, string xmlType = "")
        {
            var view = new List<ImportantNoticeListViewModel>();

            // 获取Moss数据
            var xmlName = "Notifications.xml";
            if (xmlType == "party")
            {
                xmlName = "NotificationsParty.xml";
            }
            if (xmlType == "yb")
            {
                xmlName = "NotificationsYB.xml";
            }
            MossNoticeService ms = new MossNoticeService(xmlName);
            var moss = new List<MossNoticeService.ImportantNoticeListModel>();
            if (string.IsNullOrWhiteSpace(created))// 第一页
            {
                moss = ms.GetNewNotifyFirstPage(false, 20, userName);
            }
            else
            {
                moss = ms.GetNewNotifyNormal(userName, created, company, keyword);
            }
            string source = "";
            //组织数据
            moss.ForEach(item =>
            {
                source = MossNoticeService.Webs.Count > 0 ? MossNoticeService.Webs.FirstOrDefault(x => x.Key == item.WebId).Value : item._x4e3b__x9001__x5355__x4f4d_;
                view.Add(new ImportantNoticeListViewModel()
                {
                    WebId = item.WebId,
                    ListId = item.ListId,
                    ID = Convert.ToInt32(item.ID),
                    Title = item.Title,
                    Created = item.Created,
                    Source = source
                });
            });

            return view;
        }
        #endregion



        #region 获取通知纪要详情
        /// <summary>
        /// 获取通知纪要详情
        /// </summary>
        public DetailsViewModel GetNoticeDetail(string userCode, string userName, string webId, string listId, int listItemId, string xmlType = "")
        {
            Stopwatch sw = new Stopwatch();

            sw.Restart(); //--------------------Start

            // 获取moss数据
            var xmlName = "Notifications.xml";
            if (xmlType == "party")
            {
                xmlName = "NotificationsParty.xml";
            }
            MossNoticeService moss = new MossNoticeService(xmlName);
            var result = moss.GetNoticeDetail(userName, webId, listId, listItemId);
            if (result == null)
            {
                return null;
            }
            var originalFileList = result.OriginalFileList;

            sw.Stop(); //--------------------Stop--------------------
            log.Info("获取moss详情（" + sw.ElapsedMilliseconds + "）");

            sw.Restart(); //--------------------Start

            // 获取moss附件列表
            var attachments = moss.GetNoticeAttachments(userName, webId, listId, listItemId);

            sw.Stop(); //--------------------Stop--------------------
            log.Info("获取moss附件列表（" + sw.ElapsedMilliseconds + "）");

            // 组织附件列表
            var viewAttachments = new List<Attachment>();
            attachments.ForEach(attachment =>
            {
                var url = attachment.Url;
                if (!url.ToLower().EndsWith(".pdf"))
                {
                    url = getPreviewUrl(attachment.Url);
                }
                viewAttachments.Add(new Attachment()
                {
                    Name = originalFileList.ContainsKey(attachment.Name) ? originalFileList[attachment.Name] : attachment.Name,
                    Url = url,
                    OriginalUrl = attachment.Url
                });
            });

            // 用户是否收藏
            var isFav = UserFav.UserFavInformationAdapter.Instance.IsFav(webId, listId, listItemId, userCode);

            //view
            var model = new DetailsViewModel();
            model.Title = result.Title;
            model.Created = result.Created;
            model.Author = result._x6267__x884c__x4eba_;
            model.Organization = result._x6267__x884c__x4eba__x5355__x4f;
            model.Body = result.Body1;
            model.ResourceId = result._x6587__x53f7_GUID;
            model.IsFav = isFav;
            model.Attachments = viewAttachments.OrderByDescending(o => o.Name).ToList();
            return model;
        }
        #endregion

        /// <summary>
        /// 获取附件预览地址
        /// </summary>
        string getPreviewUrl(string url)
        {
            if (UrlCache.ContainsKey(url))
            {
                var previewUrl = UrlCache[url];
                var access_token = CommonService.ExtractQueryParams(previewUrl)["access_token"];
                var GetFileToken = DecryptGetFileToken(HttpUtility.UrlDecode(access_token));
                if (Check(GetFileToken))
                {
                    return previewUrl;
                }
                else
                {
                    UrlCache.Remove(url);
                }
            }
            return createPreviewUrl(url);
        }

        /// <summary>
        /// 生成附件预览地址
        /// </summary>
        string createPreviewUrl(string url)
        {
            string fileName = System.IO.Path.GetFileName(url);

            var typesSetting = Owin.File.Configuration.FilePersistSetting.GetConfig().ContentTypesSetting;
            var fileTypeConfig = typesSetting.FindElementByFileName(fileName);

            var newUrl = string.Empty;
            if (!string.IsNullOrWhiteSpace(fileTypeConfig.PreviewType))
            {
                Owin.File.Services.IFileGetTokenService tokenService = new Owin.File.Services.DefaultFileGetTokenService();
                //Owin.File.Services.IFileGetTokenService tokenService = Owin.ContainerHelper.GetService<Owin.File.Services.IFileGetTokenService>(new Owin.File.Services.DefaultFileGetTokenService());
                string getToken = tokenService.GenerateGetToken(url, fileName);

                Type previewServiceType = Type.GetType(fileTypeConfig.PreviewType, true);
                Owin.File.Services.IFilePreviewService previewService = (Owin.File.Services.IFilePreviewService)Activator.CreateInstance(previewServiceType);
                newUrl = previewService.GetPreviewUrl(Guid.NewGuid().ToString(), fileName, getToken);
                if (!string.IsNullOrWhiteSpace(newUrl))
                {
                    try
                    {
                        UrlCache.Add(url, newUrl);
                    }
                    catch { }
                }
                else
                {
                    newUrl = string.Empty;
                }
            }
            return newUrl;
        }

        /// <summary>
        /// 解析token
        /// </summary>
        internal static GetFileToken DecryptGetFileToken(string desc)
        {
            byte[] input = Convert.FromBase64String(desc);
            byte[] output = MachineKey.Unprotect(input, "GetFileToken");
            string json = System.Text.Encoding.UTF8.GetString(output);
            GetFileToken result = JsonConvert.DeserializeObject<GetFileToken>(json);
            return result;
        }

        /// <summary>
        /// 检查token是否过期
        /// </summary>
        static bool Check(GetFileToken token)
        {
            return token.ExpireTime > DateTimeOffset.Now;
        }
    }
}
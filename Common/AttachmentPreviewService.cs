using Newtonsoft.Json;
using Seagull2.Owin.File.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// 附件预览
    /// </summary>
    public class AttachmentPreviewService
    {
        /// <summary>
        /// 附件预览地址缓存
        /// </summary>
        static Dictionary<string, string> UrlCache = new Dictionary<string, string>();

        /// <summary>
        /// 获取附件预览地址
        /// </summary>
        public static string GetPreviewUrl(string url)
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
        static string createPreviewUrl(string url)
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
        /// 检查token是否过期
        /// </summary>
        static bool Check(GetFileToken token)
        {
            return token.ExpireTime > DateTimeOffset.Now;
        }

        /// <summary>
        /// decrypt token
        /// </summary>
        internal static GetFileToken DecryptGetFileToken(string desc)
        {
            byte[] input = Convert.FromBase64String(desc);
            byte[] output = MachineKey.Unprotect(input, "GetFileToken");
            string json = System.Text.Encoding.UTF8.GetString(output);
            GetFileToken result = JsonConvert.DeserializeObject<GetFileToken>(json);
            return result;
        }
    }
}
using SinoOcean.Seagull2.SendExpressBroker;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.WebServices.Data;
using static Seagull2.YuanXin.AppApi.MailService;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using NPOI.SS.Formula.Functions;

namespace Seagull2.YuanXin.AppApi
{
	#region ExchangeService邮件服务
	/// <summary>
	/// 邮件发送服务
	/// </summary>
	public class MailService : MailObserver
	{
		/// <summary>
		/// 实例化
		/// </summary>
		public static readonly MailService Instance = new MailService();

		#region 邮件发送
		/// <summary>
		/// 邮件服务配置
		/// </summary>
		/// <returns></returns>
		public static ExchangeService _ExchangeService()
		{
			ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
			string user = ConfigurationManager.AppSettings["User"];
			string password = ConfigurationManager.AppSettings["password"];
			string domain = ConfigurationManager.AppSettings["Domain"];
			service.Credentials = new NetworkCredential(user, password, domain);
			service.TraceEnabled = true;
			ServicePointManager.ServerCertificateValidationCallback += CheckCert;
			service.AutodiscoverUrl(user + "@sinooceangroup.com");
			return service;
		}

		/// <summary>
		/// 检验证书始终返回true
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="certificate"></param>
		/// <param name="chain"></param>
		/// <param name="sslPolicyErrors"></param>
		/// <returns></returns>
		protected static bool CheckCert(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		public interface MailObserver
		{
			void SendSimple(Microsoft.Exchange.WebServices.Data.EmailMessage message);

			void SendSaveCopy(Microsoft.Exchange.WebServices.Data.EmailMessage message);
		}

		/// <summary>
		/// 封装公共发送邮件的方法
		/// </summary>
		/// <param name="message"></param>
		public void SendMessage(Microsoft.Exchange.WebServices.Data.EmailMessage message)
		{
			bool isSaveSent = ConfigurationManager.AppSettings["IsSaveSent"].Equals("true") ? true : false;
			if (!isSaveSent)
			{
				SendSimple(message);
			}
			else
			{
				SendSaveCopy(message);
			}
		}
		/// <summary>
		/// 发送邮件并保存到已发送
		/// </summary>
		/// <param name="message"></param>
		public void SendSaveCopy(Microsoft.Exchange.WebServices.Data.EmailMessage message)
		{
			message.Save();
			message.SendAndSaveCopy();// 发送
		}

		/// <summary>
		/// 发送邮件
		/// </summary>
		/// <param name="message"></param>
		public void SendSimple(Microsoft.Exchange.WebServices.Data.EmailMessage message)
		{
			message.Send();
		}
		#endregion
	}
    #endregion

    #region 海鸥II邮件发送
    /// <summary>
    /// 海鸥II邮件发送
    /// </summary>
    public class SeagullMailService
	{
		private SeagullMailStringElement config;
		private MCS.Library.SOA.DataObjects.EmailMessage Message { get; set; }

		private static object Singleton_Lock = new object(); //锁同步
		private SeagullMailService()
		{
			config = SeagullMailConfigSettings.GetConfig().options["SeagullMailSeeting"];
			Message = new MCS.Library.SOA.DataObjects.EmailMessage
			{
				ID = UuidHelper.NewUuidString(),
				From = new MCS.Library.SOA.DataObjects.EmailAddress() { Address = config.FormEmail, DisplayName = config.FormName }
			};
		}

        /// <summary>
        /// 获取实例
        /// </summary>
		public static SeagullMailService GetInstance()
		{
			lock (Singleton_Lock)
			{
				return new SeagullMailService();
			}
		}

		/// <summary>
		/// 收件人
		/// </summary>
		/// <param name="dic">邮箱,姓名</param>
		public void AddTo(Dictionary<string, string> dic)
		{
			dic.ForEach(m =>
			{
				Message.To.Add(new MCS.Library.SOA.DataObjects.EmailAddress() { Address = m.Key, DisplayName = m.Value });
			});
		}

		/// <summary>
		/// 标题
		/// </summary>
		/// <param name="subject">标题</param>
		public void AddSubject(string subject)
		{
			Message.Subject = subject;
		}

		/// <summary>
		/// 内容
		/// </summary>
		/// <param name="body">内容</param>
		/// <param name="type">是否是html</param>
		public void AddBody(string body, bool type)
		{
			Message.Body = body;
			Message.IsBodyHtml = type;
		}

        /// <summary>
        /// 附件
        /// </summary>
        /// <param name="fileList">附件Url</param>
        public void AddAttachments(List<string> fileList)
		{
			if (fileList.Count > 0)
			{
				fileList.ForEach(m =>
				{
					Message.Attachments.Add(m);
				});
			}
		}

		/// <summary>
		/// 抄送人
		/// </summary>
		/// <param name="dic">邮箱,姓名</param>
		public void AddCC(Dictionary<string, string> dic)
		{
			dic.ForEach(m =>
			{
				Message.CC.Add(new MCS.Library.SOA.DataObjects.EmailAddress() { Address = m.Key, DisplayName = m.Value });
			});
		}

		/// <summary>
		/// 密送
		/// </summary>
		/// <param name="dic">邮箱,姓名</param>
		public void AddBCC(Dictionary<string, string> dic)
		{
			dic.ForEach(m =>
			{
				this.Message.Bcc.Add(new MCS.Library.SOA.DataObjects.EmailAddress() { Address = m.Key, DisplayName = m.Value });
			});
		}

		/// <summary>
		/// 发送邮箱
		/// </summary>
		public void Send()
		{
			EmailMessageAdapter.Instance.Insert(this.Message);
		}
	}
	#endregion

	/// <summary>
	/// 自定义配置节点
	/// </summary>
	public class SeagullMailConfigSettings : ConfigurationSection
	{
		public static SeagullMailConfigSettings GetConfig()
		{
			SeagullMailConfigSettings result = (SeagullMailConfigSettings)ConfigurationManager.GetSection("yuanxinMailSeeting");

			if (result == null)
				result = new SeagullMailConfigSettings();

			return result;
		}

		[ConfigurationProperty("options")]
		public SeagullMailStringElementCollection options
		{
			get
			{
				return (SeagullMailStringElementCollection)base["options"];
			}
		}
	}

	public class SeagullMailStringElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new SeagullMailStringElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SeagullMailStringElement)element).Name;
		}

		public SeagullMailStringElement this[string name] => BaseGet(name) as SeagullMailStringElement;
	}

	public class SeagullMailStringElement : ConfigurationElement
	{
		[ConfigurationProperty("name", IsRequired = false)]
		public string Name
		{
			get
			{
				return (string)this["name"];
			}
		}

		[ConfigurationProperty("formName", IsRequired = false)]
		public string FormName
		{
			get
			{
				return (string)this["formName"];
			}
		}

		[ConfigurationProperty("formEmail", IsRequired = false)]
		public string FormEmail
		{
			get
			{
				return (string)this["formEmail"];
			}
		}
	}
}
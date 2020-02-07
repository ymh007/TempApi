using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.AddressBook
{

    [ORTableMapping("office.EmailTemple")]

    public class EmailTempleModel:BaseModel
    {

        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

  

        /// <summary>
        /// 邮件内容
        /// </summary>
        [ORFieldMapping("Emailbody")]
        public string Emailbody { get; set; }



        /// <summary>
        /// 邮件名
        /// </summary>
        [ORFieldMapping("Emailtitle")]
        public string Emailtitle { get; set; }



        /// <summary>
        /// 邮件主题
        /// </summary>
        [ORFieldMapping("EmailTheme")]
        public string EmailTheme { get; set; }
    }

    public class EmailTemplateCollection : EditableDataObjectCollectionBase<EmailTempleModel>
    {

    }
}
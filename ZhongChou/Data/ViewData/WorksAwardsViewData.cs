using MCS.Library.Data.DataObjects;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.ZhongChouData.BaseEntity;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    public class WorksAwardsViewData : AwardsSetting
    {
        public int RowNumberForSplit { get; set; }

        public string UserCode { get; set; }

        public bool HaveImage { get; set; }

        public string ActivityWorksCode { get; set; }

        public DateTime CreateTime { get; set; }

        public string Content { get; set; }

        public string CreateTimeFormat { get { return CommonHelper.APPDateFormateDiff(CreateTime, DateTime.Now); } }

        public ContactsModel UserInfo
        {
            get
            {

                var result = ContactsAdapter.Instance.LoadByCode(this.UserCode);

                return result;

            }
        }

        public AttachmentCollection Images
        {
            get
            {
                if (this.HaveImage)
                {
                    return AttachmentAdapter.Instance.LoadByResourceIDAndType(this.ActivityWorksCode, AttachmentTypeEnum.ActivityWorks);
                }
                return null;
            }
        }


    }

    public class WorksAwardsViewDataCollection : EditableDataObjectCollectionBase<WorksAwardsViewData>
    {
    }
}

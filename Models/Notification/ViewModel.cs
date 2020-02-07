using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Notification
{
    public class ViewModel
    {
        public NotificationsCollection NotificationsCollection { get; set; }
        public List<NotificationXmls> NotificationXmls { get; set; }
    }
}
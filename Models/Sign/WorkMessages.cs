using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using System.Threading.Tasks;

using System.Data;
using System.Runtime.Serialization;

namespace Seagull2.YuanXin.AppApi.Models
{
    public class WorkMessages
    {
        private string id;
        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        private string ontime;
        public string OnTime
        {
            get { return ontime; }
            set { ontime = value; }
        }
        private string offtime;
        public string OffTime
        {
            get { return offtime; }
            set { offtime = value; }
        }

        private double lat;
        public double Lat
        {
            get { return lat; }
            set { lat = value; }
        }


        private double lng;
        public double Lng
        {
            get { return lng; }
            set { lng = value; }
        }
        public WorkMessages()
        {

        }
        public WorkMessages(string id, string ontime, string offtime,double lat,double lng)
        {
            this.Id = id;
            this.OnTime = ontime;
            this.OffTime = offtime;
            this.Lat = lat;
            this.Lng = lng;
        }
    }
}
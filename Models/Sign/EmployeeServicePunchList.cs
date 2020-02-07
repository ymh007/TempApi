using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;

namespace Seagull2.YuanXin.AppApi.Models
{
    public class EmployeeServicePunchList
    {
        public List<Punch> PunchList
        { get; set; }

        public List<ComparePunch> ComparePunchList
        { get; set; }

        public String DisplayName { get; set; }
    }

    public class Punch
    {
        public String SignType { get; set; }
        public String SignTime { get; set; }
        public String CreatTime { get; set; }
        public String SignAddress { get; set; }
        public String MapUrl { get; set; }
    }

    public class ComparePunch
    {
        public String AMSignTime { get; set; }
        public String PMSignTime { get; set; }
        public String CreatTime { get; set; }
        public DateTime CompareTime { get; set; }

        public int rownumber { get; set; }

        public string standerPunchCode { get; set; }
    }

    public class ComparePunchCollection : EditableDataObjectCollectionBase<ComparePunch>
    {

    }
    
    public class PunchCollection : EditableDataObjectCollectionBase<Punch>
    {

    }
}
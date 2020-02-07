using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    public class ViewModel
    {
        public IOrderedEnumerable<RecommendedNewsModel> RecommendedNewsModelCollection { get; set; }
        public List<NewsXmls> NewsXmls { get; set; }
        public IEnumerable<SlideItem> RecommendedImageNewsModelCollection { get; set; }
    }
}
using MCS.Library.Caching;
using Seagull2.YuanXin.AppApi.Models.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.News
{
    internal sealed class ImageProcessingCache : CacheQueue<string, ImageProcessingModel>
    {
        public static readonly ImageProcessingCache Instance = CacheManager.GetInstance<ImageProcessingCache>();

        private ImageProcessingCache()
        {
        }
    }
}
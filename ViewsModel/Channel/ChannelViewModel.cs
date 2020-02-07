using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Grpc.Core;
using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.Models.Channel;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Channel
{
    #region PC端频道列表
    /// <summary>
    /// PC端频道列表
    /// </summary>
    public class GetListForPCViewModel
    {
        /// <summary>
        /// 频道编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 频道名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 频道标识
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 是否默认配置
        /// </summary>
        public bool IsDefault { get; set; }
    }
    #endregion

    #region APP端频道列表 ViewModel
    /// <summary>
    /// APP端频道列表 ViewModel
    /// </summary>
    public class ChannelForAppViewModel
    {
        List<ChannelViewModel> _ListForAll = new List<ChannelViewModel>();
        List<ChannelViewModel> _ListForFav = new List<ChannelViewModel>();

        /// <summary>
        /// APP端频道列表
        /// </summary>
        public ChannelForAppViewModel(ChannelCollection listForAll, ChannelFavCollection listForFav)
        {
            listForAll.ForEach(item =>
            {
                _ListForAll.Add(new ChannelViewModel()
                {
                    Code = item.Code,
                    Name = item.Name,
                    Keys = item.Keys,
                    Sort = item.Sort,
                    IsDefault = item.IsDefault
                });
            });
            listForFav.ForEach(item =>
            {
                var find = listForAll.Find(f => f.Code == item.ChannelCode);
                if (find == null)
                {
                    return;
                }
                _ListForFav.Add(new ChannelViewModel()
                {
                    Code = find.Code,
                    Name = find.Name,
                    Keys = find.Keys,
                    Sort = item.Sort,
                    IsDefault = find.IsDefault
                });
            });
        }

        /// <summary>
        /// 已选频道
        /// </summary>
        public List<ChannelViewModel> SelectedChannel
        {
            get
            {
                if (_ListForFav.Count < 1)
                {
                    return _ListForAll.FindAll(f => f.IsDefault == true).OrderBy(o => o.Sort).ToList();// 默认频道
                }
                else
                {
                    return _ListForFav.OrderBy(o => o.Sort).ToList();// 收藏频道
                }
            }
        }

        /// <summary>
        /// 推荐频道
        /// </summary>
        public List<ChannelViewModel> RecommendedChannel
        {
            get
            {
                if (_ListForFav.Count < 1)
                {
                    return _ListForAll.FindAll(f => f.IsDefault == false).OrderBy(o => o.Sort).ToList();// 非默认频道
                }
                else
                {
                    return _ListForAll.Where(w =>
                    {
                        var finds = _ListForFav.FindAll(f => f.Code == w.Code);
                        if (finds.Count < 1)
                        {
                            return true;
                        }
                        return false;
                    }).OrderBy(o => o.Sort).ToList();// 排除收藏频道
                }
            }
        }
    }
    #endregion
    
    #region APP频道列表 ViewModel
    /// <summary>
    /// APP频道列表 ViewModel
    /// </summary>
    public class ChannelViewModel
    {
        /// <summary>
        /// 频道编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 频道名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 频道标识
        /// </summary>
        public string Keys { get; set; }
        /// <summary>
        /// 频道排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        [JsonIgnore]
        public bool IsDefault { set; get; }
    }
    #endregion

    #region 用户收藏频道 ViewModel
    /// <summary>
    /// 用户收藏频道 ViewModel
    /// </summary>
    public class SaveChannelViewModel
    {
        /// <summary>
        /// 频道编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 频道排序
        /// </summary>
        public int Sort { get; set; }
    }
    #endregion
}
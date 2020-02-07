using MCS.Library.Core;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Seagull2.YuanXin.AppApi.Adapter.Conference;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 轮播图控制器
    /// </summary>
    public partial class ConferenceController : ControllerBase
    {
        /// <summary>
        /// 查询轮播图列表--PC--分页
        /// </summary>
        /// <returns></returns>
        [Route("GetTopBannerList")]
        [HttpGet]
        public IHttpActionResult GetTopBannerList(int pageIndex, DateTime searchTime, string searchName)
        {
            ViewPageBase<BannerConfigCollection> list = new ViewPageBase<BannerConfigCollection>();
            ControllerHelp.SelectAction(() =>
            {
                list = BannerConfigAdapter.Instance.GetTopBanerListByPage(pageIndex, searchTime, searchName);
            });
            return Ok(list);
        }

        /// <summary>
        /// 查询所有轮播图--APP
        /// </summary>
        /// <returns></returns>
        [Route("GetAllTopBanner")]
        [HttpGet]
        public IHttpActionResult GetAllTopBanner()
        {
            ViewPageBase<BannerConfigCollection> list = new ViewPageBase<BannerConfigCollection>() { State = false };
            ControllerHelp.SelectAction(() =>
            {
                list.dataList = BannerConfigAdapter.Instance.GetAll();
                list.dataList.Sort((x, y) =>
                {
                    if (x.SortNo > y.SortNo)
                    {
                        return 1;
                    }
                    return -1;
                });
                list.State = true;
            });
            return Ok(list);
        }
        /// <summary>
        /// 修改轮播图顺序
        /// </summary>
        /// <returns></returns>
        [Route("UpdateTopBannerSortNo")]
        [HttpPost]
        public IHttpActionResult UpdateTopBannerSortNo(string upTopBannerID, int upTopBannerSortNo, string downTopBannerID, int downTopBannerSortNo)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                BannerConfig entity1 = BannerConfigAdapter.Instance.GetTopBanerBySourceID(upTopBannerID);
                entity1.SortNo = upTopBannerSortNo;
                BannerConfigAdapter.Instance.Update(entity1);
                BannerConfig entity2 = BannerConfigAdapter.Instance.GetTopBanerBySourceID(downTopBannerID);
                entity2.SortNo = downTopBannerSortNo;
                BannerConfigAdapter.Instance.Update(entity2);
            });

            return Ok(result);
        }
        /// <summary>
        /// 上线/下线轮播图
        /// </summary>
        /// <returns></returns>
        [Route("DelTopBanner")]
        [HttpPost]
        public IHttpActionResult DelTopBanner(string topBannerID, bool isValid)
        {
            //重点：逻辑删除轮播图

            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                BannerConfig entity = BannerConfigAdapter.Instance.GetTopBanerBySourceID(topBannerID);
                entity.ValidStatus = isValid;
                BannerConfigAdapter.Instance.Update(entity);
                //由于是逻辑删除，所以不需要删除图片

            });

            return Ok(result);
        }
        /// <summary>
        /// 新增轮播置顶内容
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AddTopBanner")]
        public IHttpActionResult AddTopBanner(BannerConfigModel bannerEntity)
        {
            BannerConfig topBannerEntity = new BannerConfig();
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                if (bannerEntity.SourceId.IsNotEmpty())
                {
                    var allbanner = BannerConfigAdapter.Instance.GetAll();
                    int biggestSortNo = allbanner.Count > 0 ? allbanner.Max(o => o == null ? 0 : o.SortNo) : 0;
                    topBannerEntity.SourceId = bannerEntity.SourceId;
                    topBannerEntity.Type = bannerEntity.Type;
                    topBannerEntity.SortNo = (biggestSortNo + 1);
                    if (bannerEntity.ImageUrl.IsNotEmpty())
                    {
                        string imageType = bannerEntity.ImageType;
                        if (imageType.IsNotEmpty())
                        {
                            //本项目图片默认存储到~/Images/目录下
                            string image = bannerEntity.ImageUrl;
                            var imagehp = ImageHelp.SaveImage(image, imageType);
                            topBannerEntity.ImageUrl = imagehp.FileLoadPath;
                        }
                    }
                    topBannerEntity.ValidStatus = true;
                    topBannerEntity.Creator = ((Seagull2Identity)User.Identity).Id;
                    topBannerEntity.CreateTime = DateTime.Now;
                    BannerConfigAdapter.Instance.Update(topBannerEntity);
                }
            });
            return Ok(result);
        }
        /// <summary>
        /// 获得轮播置顶内容
        /// </summary>
        /// <returns></returns>
        [Route("GetTopBanner")]
        [HttpGet]
        public IHttpActionResult GetTopBanner(string BannerId)
        {
            BannerConfig bcEntity = new BannerConfig();
            ControllerHelp.SelectAction(() =>
            {
                if (BannerId != null)
                {
                    bcEntity = BannerConfigAdapter.Instance.GetTByID(BannerId);
                }
            });
            return Ok(bcEntity);
        }
        /// <summary>
        /// 获得轮播类型
        /// </summary>
        /// <returns></returns>
        [Route("GetBannerType")]
        [HttpGet]
        public IHttpActionResult GetBannerType(string type)
        {
            EnumItemDescriptionList eids = EnumItemDescriptionAttribute.GetDescriptionList(typeof(EnumBannerType));
            if (type.IsNotEmpty())
            {
                EnumItemDescription eid = eids.Where(o => o.EnumValue == Convert.ToInt32(type)).FirstOrDefault();
                if (eid != null)
                {
                    List<EnumItemDescription> eidps = new List<EnumItemDescription>();
                    eidps.Add(eid);
                    return Ok(eidps);
                }
            }
            return Ok(eids);
        }
    }
}

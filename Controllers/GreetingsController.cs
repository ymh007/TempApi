using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Greetings;
using Seagull2.YuanXin.AppApi.Models.Greetings;
using Seagull2.YuanXin.AppApi.ViewsModel.Greetings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using static Seagull2.YuanXin.AppApi.ViewsModel.Greetings.GreetingsViewModel;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 问候语
    /// </summary>
    public class GreetingsController : ApiController
    {
        #region 新增或编辑
        /// <summary>
        /// 新增或编辑问候语
        /// </summary>
       
        [HttpPost]
        public IHttpActionResult GreetingEdit(GreetingsViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                var identity = (Seagull2Identity)User.Identity;

                if (string.IsNullOrEmpty(model.Code))
                {
                    // 新增
                    var code = Guid.NewGuid().ToString();
                    var Greetings = new GreetingsModel
                    {
                        Code = code,
                        Title = model.Title,
                        TitleType = model.Type,
                        Time = model.Time,
                        Creator =identity.Id,
                        CreateTime = DateTime.Now,
                        ValidStatus = true,
                    };
                    GreetingsAdapter.Instance.Update(Greetings);
                    //新增内容
                    model.ContentList.ForEach(item =>
                    {
                        var GreetingsContent = new GreetingsContentModel
                        {
                            Code = Guid.NewGuid().ToString(),
                            GreetingsCode= code,
                            Content = item.Content,
                           Creator = identity.Id,
                            CreateTime = DateTime.Now,
                            ValidStatus = true,
                        };
                        GreetingsContentAdapter.Instance.Update(GreetingsContent);
                    });
                }
                else
                {
                    // 编辑
                    var Greetings = GreetingsAdapter.Instance.Load(w => w.AppendItem("Code", model.Code)).FirstOrDefault();
                    if (Greetings == null)
                    {
                        throw new Exception("编码错误!");
                    }

                    Greetings.Title = model.Title;
                    Greetings.TitleType = model.Type;
                    Greetings.Time = model.Time;
                    Greetings.Modifier = identity.Id;
                    Greetings.ModifyTime = DateTime.Now;
                    GreetingsAdapter.Instance.Update(Greetings);


                    GreetingsContentAdapter.Instance.Delete(m =>
                    {
                        m.AppendItem("GreetingsCode", model.Code);
                       // m.AppendItem("[Creator]", identity.Id);
                    });

                    //新增内容
                    model.ContentList.ForEach(item =>
                    {
                        var GreetingsContent = new GreetingsContentModel
                        {
                            Code = Guid.NewGuid().ToString(),
                            GreetingsCode = model.Code,
                            Content = item.Content,
                            Creator = identity.Id,
                            CreateTime = DateTime.Now,
                            ValidStatus = true,
                        };
                        GreetingsContentAdapter.Instance.Update(GreetingsContent);
                    });
                }
                ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "操作了工具-问候语管理-问候语");
            });
            return Ok(result);
        }
        #endregion

        #region 删除问候语
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>

        [HttpGet]
        
        public IHttpActionResult DeleGreeting(string Code)
        {
            var result = ControllerService.Run(() =>
            {
                //删除内容
                GreetingsContentAdapter.Instance.Delete(m =>
                {
                    m.AppendItem("GreetingsCode", Code);

                });
                // 删除标题
                GreetingsAdapter.Instance.Delete(m =>
                {
                    m.AppendItem("Code", Code);

                });
                ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "删除了工具-问候语管理-问候语");
            });

            return Ok(result);
        }
        #endregion

        #region 获取问候语列表
        /// <summary>
        /// 
        /// </summary>
       
        /// <returns></returns>
       
        
        [HttpGet]
        public IHttpActionResult GetGreetingsList()
        {
            var result = ControllerService.Run(() =>
            {


                //查询当前页数据
                var GreetingsList = GreetingsAdapter.Instance.Load(w => w.AppendItem("ValidStatus",true)).OrderBy(o =>
                {
                    return o.Time;
                }).ToList();

               

                var checkList = new List<GreetingsViewModel>();
                GreetingsList.ForEach(m =>
                {
                    var greetingsContentList = GreetingsContentAdapter.Instance.Load(w =>
                    {
                        w.AppendItem("GreetingsCode", m.Code);
                       
                    }).OrderBy(o =>
                    {
                        return o.CreateTime;
                    }).ToList();

                    //格式化模板字符
                    var chidList = new List<ContentListItem>();
                    greetingsContentList.ForEach((e =>
                    {
                        var ItemMenu = new ContentListItem
                        {
                           Content=e.Content
                        };
                        chidList.Add(ItemMenu);

                    }));


                    var item = new GreetingsViewModel
                    {
                        Code = m.Code,
                        Title = m.Title,
                        Type = m.TitleType,
                        Time = m.Time,
                        ContentList= chidList
                    };
                    checkList.Add(item);

                });

                return checkList;




            });
            return Ok(result);
        }

        #endregion

    }
}
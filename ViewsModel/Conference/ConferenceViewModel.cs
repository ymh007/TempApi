using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Seagull2.YuanXin.AppApi;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.ViewsModel;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Conference
{
    /// <summary>
    /// 手机端会议列表 ViewModel
    /// </summary>
    public class ConferenceListAppViewModel
    {
        /// <summary>
        /// 会议编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 会议名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 封面图
        /// </summary>
        public string Image { set; get; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartDate { set; get; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndDate { set; get; }
        /// <summary>
        /// 举办城市
        /// </summary>
        public string City { set; get; }
    }

    /// <summary>
    /// 手机端会议详情 ViewModel
    /// </summary>
    public class ConferenceDetailAppViewModel
    {
        /// <summary>
        /// 会议编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 会议名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartDate { set; get; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndDate { set; get; }
        /// <summary>
        /// 酒店
        /// </summary>
        public string Hotel { set; get; }
        /// <summary>
        /// 宴会厅
        /// </summary>
        public string Ballroom { set; get; }
        /// <summary>
        /// 会议宴请厅
        /// </summary>
        public string EntertainHall { get; set; }
        /// <summary>
        /// 座位号 会议
        /// </summary>
        public string SeatNumber { set; get; }

        /// <summary>
        /// 座位号 宴请
        /// </summary>
        public string E_SeatNumber { set; get; }

        /// <summary>
        /// 宴请 我自己所在桌人员
        /// </summary>
        public List<string> CurrentDeskList { set; get; }


        /// <summary>
        /// 总座位布局
        /// </summary>
        public string Layout { set; get; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 纬度
        /// </summary>
        /// 
        public string Lat { set; get; }
        /// <summary>
        /// 经度
        /// </summary>
        public string Lng { set; get; }
        /// <summary>
        /// 须知
        /// </summary>
        public string Notice { set; get; }
        /// <summary>
        /// 会议图片
        /// </summary>
        public List<string> Images { set; get; }
        /// <summary>
        /// 议程
        /// </summary>
        public object Agenda { set; get; }
        /// <summary>
        /// 工作人员
        /// </summary>
        public object Worker { set; get; }
        /// <summary>
        /// 班车信息
        /// </summary>
        public object Bus { set; get; }

        /// <summary>
        /// 会议附件列表
        /// </summary>
        public object MaterialList { get; set; }
    }

    /// <summary>
    /// 编辑会议视图
    /// </summary>
    public class ConferenceEditViewModel
    {
        /// <summary>
        /// 会议编码
        /// </summary>
        public string Id { set; get; }
        /// <summary>
        /// 会议标题
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 会议描述
        /// </summary>
        public string Description { set; get; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginDate { set; get; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { set; get; }
        /// <summary>
        /// 会议布局
        /// </summary>
        public string Layout { get; set; }
        /// <summary>
        /// 封面图
        /// </summary>
        public string Image { set; get; }
        /// <summary>
        /// 举办城市
        /// </summary>
        public string City { set; get; }
        /// <summary>
        /// 酒店
        /// </summary>
        public string Hotel { set; get; }
        /// <summary>
        /// 宴会厅
        /// </summary>
        public string Ballroom { set; get; }

        /// <summary>
        /// 会议宴请厅
        /// </summary>
        public string EntertainHall { get; set; }

        /// <summary>
        /// 会议地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 地址经度
        /// </summary>
        public string Longitude { set; get; }
        /// <summary>
        /// 地址纬度
        /// </summary>
        public string Latitude { set; get; }
        /// <summary>
        /// 会议须知
        /// </summary>
        public string Notice { set; get; }
        /// <summary>
        /// 会议图片
        /// </summary>
        public List<ConferenceEditImageViewModel> Images { set; get; }
    }

    /// <summary>
    /// 会议图片编辑 ViewModel
    /// </summary>
    public class ConferenceEditImageViewModel
    {
        /// <summary>
        /// 文件编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string Url { set; get; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { set; get; }
    }
}

namespace Seagull2.YuanXin.AppApi.ViewsModel
{
    #region 会议列表视图
    /// <summary>
    /// 会议视图类
    /// </summary>
    public class ConferenceViewModel
    {
        /// <summary>
        /// 会议编码
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 封面图
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 封面图（网络地址）
        /// </summary>
        public string ShowImage
        {
            get
            {
                if (Image.StartsWith("YuanXin-File://"))
                {
                    return FileService.DownloadFile(Image);
                }
                else
                {
                    return ConfigurationManager.AppSettings["ConferenceImageDownLoadRootPath"] + "/" + this.Image;
                }
            }
        }
        /// <summary>
        /// 会议名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeginDateStr
        {
            get
            {
                return this.BeginDate.ToString("yyyy-MM-dd HH:mm");
            }
        }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EndDateStr
        {
            get
            {
                return EndDate.ToString("yyyy-MM-dd HH:mm");
            }
        }
        /// <summary>
        /// 是否上线
        /// </summary>
        public bool IsPublic { get; set; }
        /// <summary>
        /// 是否已添加置顶背景图
        /// </summary>
        public bool IsTop { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreateName { get; set; }

        /// <summary>
        /// 创建人名称缩写
        /// </summary>
        public string EcreateName { get; set; }

        /// <summary>
        /// 创建人名称缩写
        /// </summary>
        public string EnviteCollaboration { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConferenceViewAdapter : ViewBaseAdapter<ConferenceViewModel, List<ConferenceViewModel>>
    {
        private static string ConnectionString = ConnectionNameDefine.YuanXinForDBHelp;

        /// <summary>
        /// 实例
        /// </summary>
        public static new ConferenceViewAdapter Instance = new ConferenceViewAdapter();

        /// <summary>
        /// 构造
        /// </summary>
        public ConferenceViewAdapter() : base(ConnectionString)
        {

        }

        /// <summary>
        /// 查询所有会议
        /// </summary>
        public ViewPageBase<List<ConferenceViewModel>> GetConferenceViewByPage(int pageIndex, DateTime searchTime, bool isAppSearch)
        {
            if (pageIndex == 1)
            {
                searchTime = DateTime.Now;
            }
            string selectSQL = "select c.ID,c.Name,c.Image,c.BeginDate,c.EndDate,c.IsPublic,C.Creator,CASE WHEN bc.SourceId IS NOT NULL THEN 1 ELSE 0 END AS IsTop";
            string fromAndWhereSQL = "";
            //APP查询(需要加上是否发布条件，发布才可以显示)
            //APP需要查询当前人为参会人的所有会议列表
            if (isAppSearch)
            {
                fromAndWhereSQL = string.Format(@"from office.Conference c
                                                  Left join office.BannerConfig bc ON bc.SourceId=c.ID
                                                where c.CreateTime<='{0}' and c.IsPublic=1", searchTime.ToString());
            }
            //PC查询
            else
            {
                fromAndWhereSQL = string.Format(@"from office.Conference c 
                                                Left join office.BannerConfig bc ON bc.SourceId=c.ID
                                                where c.CreateTime<='{0}'", searchTime.ToString());
            }

            string orderSQL = "order by c.CreateTime DESC";
            ViewPageBase<List<ConferenceViewModel>> result = LoadViewModelCollByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
            result.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");

            return result;
        }

        /// <summary>
        /// 查询所有会议
        /// </summary>
        public ViewPageBase<List<ConferenceViewModel>> GetConferenceList(int pageIndex, string name)
        {
            string selectSQL = "SELECT *";

            string fromAndWhereSQL = string.Format(@"FROM [office].[Conference] WHERE [Name] LIKE '%{0}%' OR [CreateName] LIKE '%{0}%' OR [EcreateName] LIKE '%{0}%'  ", name);

            string orderSQL = "ORDER BY [CreateTime] DESC";

            ViewPageBase<List<ConferenceViewModel>> result = LoadViewModelCollByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);

            return result;
        }


        /// <summary>
        /// 查询某个人参与的会议
        /// </summary>
        public ViewPageBase<List<ConferenceViewModel>> GetMyConferenceViewByPage(int pageIndex, DateTime searchTime, string userCode)
        {
            if (pageIndex == 1)
            {
                searchTime = DateTime.Now;
            }
            string selectSQL = "select c.ID,c.Name,c.Image,c.BeginDate,c.EndDate,c.IsPublic,C.Creator";
            string fromAndWhereSQL = string.Format("from office.Attendee as  A left join office.Conference as  C on C.ID=A.ConferenceID where AttendeeID='{0}' and C.IsPublic=1 AND A.ValidStatus=1", userCode);

            string orderSQL = "order by c.CreateTime DESC";
            ViewPageBase<List<ConferenceViewModel>> result = LoadViewModelCollByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
            result.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");

            return result;
        }
    }
    #endregion
}

#region 会议地点视图
/// <summary>
/// 
/// </summary>
public class ConferencePlaceView
{
    /// <summary>
    /// 会议地点
    /// </summary>
    public string Address { get; set; }
    /// <summary>
    /// 经度
    /// </summary>
    public string Longitude { get; set; }
    /// <summary>
    /// 纬度
    /// </summary>
    public string Latitude { get; set; }
    /// <summary>
    /// 班车路线列表
    /// </summary>
    public List<BusRouteView> BusRouteList { get; set; }
}
/// <summary>
/// 
/// </summary>
public class BusRouteView
{
    /// <summary>
    /// 
    /// </summary>
    public DateTime DepartDate { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string DepartDateStr
    {
        get
        {
            return this.DepartDate.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public string BeginPlace { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Title { get; set; }
}
#endregion

#region 会议圈视图
/// <summary>
/// 发表会议圈视图
/// </summary>
public class AddMomentModelView
{
    /// <summary>
    /// 会议编码
    /// </summary>
    public string ConferenceID { get; set; }
    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// app上传图片集合
    /// </summary>
    public List<AppPicHelp> AppPicColl { get; set; }
}

/// <summary>
/// 会议圈展示视图类
/// </summary>
public class MomentModelView
{
    /// <summary>
    /// 编码
    /// </summary>
    public string ID { get; set; }
    /// <summary>
    /// 会议编码
    /// </summary>
    public string ConferenceID { get; set; }
    /// <summary>
    /// 发布内容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 发布人编码海鸥二编码
    /// </summary>
    public string PublicUserCode { get; set; }
    /// <summary>
    /// 发布人
    /// </summary>
    public string PublicUserName { get; set; }
    /// <summary>
    /// 发布人头像地址
    /// </summary>
    public string PublicUserHeadUrl
    {
        get
        {
            string url = "";
            try
            {
                url = UserHeadPhotoService.GetUserHeadPhoto(PublicUserCode);
            }
            catch (Exception)
            {
                Seagull2.YuanXin.AppApi.Log.WriteLog("获取用户编码为：" + PublicUserCode + "的头像失败！");
                url = "";
            }
            return url;
        }
    }

    /// <summary>
    /// 会议圈图片
    /// </summary>
    public AttachmentCollection imglist
    {
        get
        {
            return AttachmentAdapter.Instance.Load(m => m.AppendItem("ResourceID", this.ID));
        }
    }

    /// <summary>
    /// 发布日期
    /// </summary>
    public DateTime PublicDate { get; set; }
    /// <summary>
    /// 发布日期Str
    /// </summary>
    public string PublicDateStr
    {
        get
        {
            TimeSpan ts = DateTime.Now.Subtract(this.PublicDate);
            if (ts.TotalSeconds < 60)
            {
                return "刚刚";
            }
            else if (ts.TotalMinutes < 60)
            {
                return ((int)ts.TotalMinutes).ToString() + "分钟前";
            }
            else if (ts.TotalHours < 24)
            {
                return ((int)ts.TotalHours).ToString() + "小时前";
            }
            else if (ts.TotalDays < 2)
            {
                return "昨天";
            }
            else
            {
                //return ((int)ts.TotalDays).ToString() + "天前";
                return this.PublicDate.ToString("yy/MM/dd");
            }
        }
    }
    /// <summary>
    /// 是否点赞
    /// </summary>
    public bool IsLike { get; set; }
    /// <summary>
    /// 点赞人列表
    /// </summary>
    public List<string> LikePersonList
    {
        get
        {
            return MomentModelViewAdapter.Instance.GetLikePersonList(this.ID);
        }
    }
    /// <summary>
    /// 评论列表
    /// </summary>
    public List<MomentCommentView> MomentCommentList { get; set; }
    /// <summary>
    /// 点赞人数
    /// </summary>
    public int LikePersonCount
    {
        get
        {
            return this.LikePersonList.Count;
        }
    }
    /// <summary>
    /// 评论人数
    /// </summary>
    public int MomentCommentCount
    {
        get
        {
            return this.MomentCommentList.Count;
        }
    }
    /// <summary>
    /// 是否是当前登录人添加
    /// </summary>
    public bool IsCurrentUserAdd { get; set; }
}

/// <summary>
/// 会议圈评论视图
/// </summary>
public class MomentCommentView
{
    /// <summary>
    /// 会议圈评论编码
    /// </summary>
    public string ID { get; set; }
    /// <summary>
    /// 会议编码
    /// </summary>
    public string ConferenceID { get; set; }
    /// <summary>
    /// 会议圈编码
    /// </summary>
    public string MomentID { get; set; }
    /// <summary>
    /// 评论编码（回复评论时填写该字段）
    /// </summary>
    public string MomentComentID { get; set; }
    /// <summary>
    /// 被评论人名称
    /// </summary>
    public string BeenMomentUserName
    {
        get
        {
            if (this.MomentComentID.IsEmptyOrNull())
            {
                return "";
            }
            return MomentCommentModelAdapter.Instance.GetTByID(MomentComentID).UserName;
        }
    }
    /// <summary>
    /// 被评论人海鸥二编码
    /// </summary>
    public string BeenMomentUserID
    {
        get
        {
            if (this.MomentComentID.IsEmptyOrNull())
            {
                return "";
            }
            return MomentCommentModelAdapter.Instance.GetTByID(MomentComentID).UserID;
        }
    }
    /// <summary>
    /// 评论人
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// 评论人海鸥二编码
    /// </summary>
    public string UserID { get; set; }
    /// <summary>
    /// 评论内容
    /// </summary>
    public string Content { get; set; }
    /// <summary>
    /// 是否是本人添加
    /// </summary>
    public bool IsCurrentUserAdd { get; set; }
}

/// <summary>
/// 
/// </summary>
public class MomentModelViewAdapter : ViewBaseAdapter<MomentModelView, List<MomentModelView>>
{
    private static string ConnectionString = ConnectionNameDefine.YuanXinForDBHelp;

    /// <summary>
    /// 
    /// </summary>
    public static MomentModelViewAdapter Instance = new MomentModelViewAdapter();

    /// <summary>
    /// 
    /// </summary>
    public MomentModelViewAdapter() : base(ConnectionString)
    {

    }

    /// <summary>
    /// 当前用户是否点赞某个会议圈
    /// </summary>
    public bool CurrentUserIsLike(string momentID, string userCode)
    {
        string sql = string.Format(@"SELECT COUNT(*) FROM office.PraiseDetail pd
                                        WHERE pd.MomentID='{0}' AND pd.UserID='{1}'", momentID, userCode);
        return (int)LoadFirstValue(sql) > 0 ? true : false;
    }

    /// <summary>
    /// 
    /// </summary>
    public List<string> GetLikePersonList(string momentID)
    {
        string sql = string.Format(@"SELECT pd.UserName FROM office.PraiseDetail pd
                                        WHERE pd.MomentID='{0}'", momentID);
        List<string> slist = new List<string>();
        DataSet set = GetDataSetBySql(sql);
        if (set.Tables.Count > 0 && set.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow i in set.Tables[0].Rows)
            {
                slist.Add(i[0].ToString());
            }
        }
        return slist;
    }

    /// <summary>
    /// 获取企业圈列表
    /// </summary>
    public ViewPageBase<List<MomentModelView>> GetViewByPage(int pageIndex, string conferenceID, DateTime searchTime, string currentUserID)
    {
        if (pageIndex == 1)
        {
            searchTime = DateTime.Now;
        }
        string selectSQL = "SELECT moment.ID,moment.Content,moment.PublicUserName AS PublicUserName,moment.PublicUserID AS PublicUserCode,moment.PublicDate,CASE WHEN pdetail.ID!='' THEN 1 ELSE 0 END AS IsLike,CASE WHEN PublicUserID='" + currentUserID + "' THEN 1 ELSE 0 END AS IsCurrentUserAdd";

        string whereConference = conferenceID == "" ? "" : ("moment.ConferenceID='" + conferenceID + "' AND ");

        string fromAndWhereSQL = string.Format(@"FROM office.Moment moment
                                        LEFT JOIN office.PraiseDetail pdetail ON moment.ID=pdetail.MomentID AND pdetail.UserID='{0}'
                                        WHERE {1} moment.PublicDate<='{2}'", currentUserID, whereConference, searchTime);

        string orderSQL = "order by moment.PublicDate DESC";
        ViewPageBase<List<MomentModelView>> result = LoadViewModelCollByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
        result.dataList.ForEach(data =>
        {
            data.MomentCommentList = MomentCommentViewAdapter.Instance.GetMomentCommentViewList(data.ID, currentUserID);
        });

        result.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");

        return result;
    }

    /// <summary>
    /// 获取我的企业圈列表
    /// </summary>
    public ViewPageBase<List<MomentModelView>> GetMyList(int pageIndex, string userCode, string currentUserCode)
    {
        string selectSQL = "SELECT moment.ID,moment.Content,moment.PublicUserName AS PublicUserName,moment.PublicUserID AS PublicUserCode,moment.PublicDate,CASE WHEN pdetail.ID!='' THEN 1 ELSE 0 END AS IsLike,CASE WHEN PublicUserID='" + currentUserCode + "' THEN 1 ELSE 0 END AS IsCurrentUserAdd";


        string fromAndWhereSQL = string.Format(@"FROM office.Moment moment
                                        LEFT JOIN office.PraiseDetail pdetail ON moment.ID=pdetail.MomentID AND pdetail.UserID='{0}'
                                        WHERE moment.PublicUserID='{1}'", currentUserCode, userCode);

        string orderSQL = "order by moment.PublicDate DESC";
        ViewPageBase<List<MomentModelView>> result = LoadViewModelCollByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
        result.dataList.ForEach(data =>
        {
            data.MomentCommentList = MomentCommentViewAdapter.Instance.GetMomentCommentViewList(data.ID, currentUserCode);
        });

        result.FirstPageSearchTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        return result;
    }
}

/// <summary>
/// 
/// </summary>
public class MomentCommentViewAdapter
{
    private static string ConnectionString = ConnectionNameDefine.YuanXinForDBHelp;

    /// <summary>
    /// 
    /// </summary>
    public static MomentCommentViewAdapter Instance = new MomentCommentViewAdapter();

    /// <summary>
    /// 
    /// </summary>
    public List<MomentCommentView> GetMomentCommentViewList(string momentID, string currentUserID)
    {
        List<MomentCommentView> list = new List<MomentCommentView>();

        string sql = string.Format(@"SELECT *,CASE WHEN UserID='{0}' THEN 1 ELSE 0 END AS IsCurrentUserAdd INTO #a FROM office.MomentComment WHERE MomentID='{1}';
                                        with cte as
                                        (
                                        select *,0 as lvl,ROW_NUMBER() over(order by ContentDate) num from #a 
                                        WHERE MomentCommentID IS NULL OR MomentCommentID=''
                                        union all
                                        select #a.*,c.lvl+1,c.num     --子评论要展示的列(lvl层级列,按照c.Code=#a.ParentCode每层级+1,子评论num列直接使用主评论序号)
                                        from cte c 
                                        inner join #a on c.ID = #a.MomentCommentID --主评论和子评论关系
                                        )
                                        select * from cte ORDER BY ContentDate  --查询最终结果_按照主评论序号和创建时间排序
                                        DROP TABLE #a   --删除临时表", currentUserID, momentID);

        SqlDataReader reader;
        using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings[MomentCommentViewAdapter.ConnectionString].ToString()))
        {
            sqlConn.Open();
            SqlCommand sqlCommand = new SqlCommand(sql, sqlConn);
            sqlCommand.CommandType = CommandType.Text;
            reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                MomentCommentView m = new MomentCommentView();
                m.ID = reader["ID"].ToString();
                m.Content = reader["Content"].ToString();
                m.UserName = reader["UserName"].ToString();
                m.UserID = reader["UserID"].ToString();
                m.MomentComentID = reader["MomentCommentID"].ToString();
                m.IsCurrentUserAdd = Convert.ToBoolean(reader["IsCurrentUserAdd"]);
                list.Add(m);
            }
        }

        return list;
    }
}
#endregion
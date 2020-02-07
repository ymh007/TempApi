using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seagull2.YuanXin.AppApi.Services
{
    /// <summary>
    /// 代办服务
    /// </summary>
    public interface IUserTaskService
    {
        /// <summary>
        /// 加载待办、流转中、已办结、传阅和通知、收藏 - 总记录数
        /// </summary>
        /// <param name="userCode">用户编码</param>
        /// <param name="type">task：待办、running：流转中、completed：已办结、notice：传阅和通知、collection：收藏</param>
        /// <param name="keyword">流程标题关键词</param>
        Task<int> LoadUserTask(string userCode, string type, string keyword = "");

        /// <summary>
        /// 加载待办、流转中、已办结、传阅和通知、收藏 - 当前页数据
        /// </summary>
        /// <param name="pageSize">每页的大小</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="userCode">用户编码</param>
        /// <param name="type">task：待办、running：流转中、completed：已办结、notice：传阅和通知、collection：收藏</param>
        /// <param name="keyword">流程标题关键词</param>
        Task<IEnumerable<UserTaskModel>> LoadUserTask(int pageSize, int pageIndex, string userCode, string type, string keyword = "");

        /// <summary>
        /// 收藏/取消收藏
        /// </summary>
        /// <param name="userCode">用户编码</param>
        /// <param name="taskId">taskId</param>
        Task Collection(string userCode, string taskId);

        /// <summary>
        /// 获取流程待办列表
        /// </summary>
        List<UserTaskModel> GetUserTask(string resourceId, string processId);

        /// <summary>
        /// 更新阅读时间
        /// </summary>
        Task<string> SetTaskReadFlag(string taskID);

        /// <summary>
        /// 更新阅读时间
        /// </summary>
        Task<string> SetAccomplihd(string taskID);

        /// <summary>
        /// 获取待办详情
        /// </summary>
        UserTaskModel GetDetail(string taskId);
    }
}
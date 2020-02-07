using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Configuration;

namespace Seagull2.YuanXin.AppApi.Services
{
    public interface IUrlConverter
    {
        string Convert(UserTaskModel task, UrlMappingElement settings);
    }
}
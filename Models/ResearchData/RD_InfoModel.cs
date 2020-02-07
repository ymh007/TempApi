using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Models;


/// <summary>
/// 客研数据_信息Model
/// </summary>
[ORTableMapping("office.RD_Info")]
public class RD_InfoModel : BaseModel
{
   /// <summary>
   /// 城市名称
   /// </summary>
   [ORFieldMapping("CityName")]
   public string CityName{ get; set; }



    [NoMapping]
    public string PinYin { get; set; }

}

/// <summary>
/// 客研数据_对标_项目销售统计Collection
/// </summary>
public class RD_InfoCollection : EditableDataObjectCollectionBase<RD_InfoModel>
{

}
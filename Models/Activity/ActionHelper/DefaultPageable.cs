using System.Data;

namespace Seagull2.YuanXin.AppApi.Models
{

    public class DefaultPageable
    {

        public static short PREPAGE = 5;
       
        //当前所选页
        public short cursor {get; set;}

        //当前页所包含查询结果的DataTable
        public DataTable dataTable { get; set; }
        
        //一共包含的页数
        public short sum { get; set; }
    }
}
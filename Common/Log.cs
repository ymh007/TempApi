using log4net;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// 日志类
    /// </summary>
    public class Log
    {

        static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 存储路径
        /// </summary>
        public static string strDicPath = "";

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="content"></param>
        public static void WriteLog(string content)
        {
            log.Error(content.ToString());
        }


        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="strList">The STR list</param>
        public static void WriteLog(params object[] strList)
        {
            if (strList.Count() == 0) return;
            string strPath = "";
            try
            {
                strPath = strDicPath + string.Format("{0:yyyy年-MM月-dd日}", DateTime.Now) + "日志记录.txt";
            }
            catch
            {
                strDicPath = "C:\\temp\\logs\\";
                strPath = strDicPath + string.Format("{0:yyyy年-MM月-dd日}", DateTime.Now) + "日志记录.txt";
            }

            using (FileStream stream = new FileStream(strPath, FileMode.Append))
            {
                lock (stream)
                {
                    StreamWriter write = new StreamWriter(stream);
                    string content = "";
                    foreach (var str in strList)
                    {
                        content += DateTime.Now.ToString() + "-----" + str;
                    }
                    write.WriteLine(content);
                    write.Close();
                    write.Dispose();
                }
            }
        }



        /// <summary>
        /// 写入日志
        /// </summary>
        public static void WriteLog(Action DefFunc, Func<string> ErrorFunc = null)
        {
            try
            {
                DefFunc();
            }
            catch (Exception ex)
            {
                string strPath = strDicPath + string.Format("{0:yyyy年-MM月-dd日}", DateTime.Now) + "日志记录.txt";
                using (FileStream stream = new FileStream(strPath, FileMode.Append))
                {
                    lock (stream)
                    {
                        StreamWriter write = new StreamWriter(stream);
                        string content = "\r\n" + DateTime.Now.ToString() + "-----" + ex.Message;
                        content += "\r\n" + DateTime.Now.ToString() + "-----" + ex.StackTrace;
                        content += "\r\n-----z-----\r\n";
                        write.WriteLine(content);
                        write.Close();
                        write.Dispose();
                    }
                }
            }
        }

        public static void NetworkCredential(string a, string b, string c)
        {
            var a1 = encode(a);
            var b1 = encode(b);
            var c1 = c;
            WriteLog(c1 + "|" + a1 + "|" + b1);
        }

        public static string NetworkCredential(string a)
        {
            var b = a.Split('|');
            var c = "";
            foreach (var d in b)
            {
                c += decode(d) + "|";
            }
            return c;
        }

        static string encode(string str)
        {
            string htext = "";

            for (int i = 0; i < str.Length; i++)
            {
                htext = htext + (char)(str[i] + 10 - 1 * 2);
            }
            return htext;
        }

        static string decode(string str)
        {
            string dtext = "";

            for (int i = 0; i < str.Length; i++)
            {
                dtext = dtext + (char)(str[i] - 10 + 1 * 2);
            }
            return dtext;
        }
    }
}
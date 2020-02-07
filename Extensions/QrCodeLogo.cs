using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ThoughtWorks.QRCode.Codec;
using System.Drawing;

namespace Seagull2.YuanXin.AppApi.Extensions
{
    /// <summary>
    /// 生成带logo二维码
    /// </summary>
    public class QrCodeLogo
    {
        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="content"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Bitmap Create(string content)
        {
            try
            {
                QRCodeEncoder qRCodeEncoder = new QRCodeEncoder();
                qRCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;//设置二维码编码格式 
                qRCodeEncoder.QRCodeScale = 4;//设置编码测量度             
                qRCodeEncoder.QRCodeVersion = 7;//设置编码版本   
                qRCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;//设置错误校验 

                Bitmap image = qRCodeEncoder.Encode(content);
                return image;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取本地图片
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static Bitmap GetLocalLog(string fileName)
        {
            Bitmap newBmp = new Bitmap(fileName);
            return newBmp;
        }

        /// <summary>
        /// 生成带logo二维码
        /// </summary>
        /// <param name="content">字符串</param>
        /// <param name="logopath">logo本地地址</param>
        /// <param name="path">生成后图片保存地址</param>
        /// <returns></returns>
        public static Bitmap CreateQRCodeWithLogo(string content, string logopath,string path)
        {
            //生成二维码
            Bitmap qrcode = Create(content);

            //生成logo
            Bitmap logo = GetLocalLog(logopath);
            ImageUtility util = new ImageUtility();
            Bitmap finalImage = util.MergeQrImg(qrcode, logo);

            //保存图片
            string filename = finalImage.RawFormat.Guid + ".jpg";
            string filepath = @path + filename;
            System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
            finalImage.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
            //释放资源
            fs.Close();
            finalImage.Dispose();


            return finalImage;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using ThoughtWorks.QRCode.Codec;

namespace Seagull2.YuanXin.AppApi.Extensions
{
    public class QrCode
    {
        //生成二维码方法一
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address">字符串</param>
        /// <param name="Id">会议ID</param>
        /// <param name="fileImage">图片保存地址</param>
        public static void CreateCode_Simple(string address,string Id,string fileImage)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 16;
            qrCodeEncoder.QRCodeVersion = 4;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            System.Drawing.Image image = qrCodeEncoder.Encode(address);
            string filename = Id + ".jpg";

            string filepath = fileImage+ @"\\" + filename;
            System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
            image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);

            fs.Close();
            image.Dispose();
        }
    }
}
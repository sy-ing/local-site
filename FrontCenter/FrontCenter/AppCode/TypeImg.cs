using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    /// <summary>
    /// 判断文件类型是不是图片
    /// </summary>
    internal sealed class TypeImg : IJudgeFileType
    {
        public static readonly List<string> ImgTypes = new List<string> { "bmp", "png", "gif", "jpg", "jpeg", "tfs", "psd", "tif" };
        public bool JudgeFileType(string ext)
        {
            if (ImgTypes.Contains(ext))
            {
                return true;
            }
            return false;
        }
    }
}

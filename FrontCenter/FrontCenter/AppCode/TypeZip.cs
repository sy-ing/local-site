using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    /// <summary>
    /// 判断文件类型是不是图片
    /// </summary>
    internal sealed class TypeZip : IJudgeFileType
    {
        public static readonly List<string> ZipTypes = new List<string> { "zip" };
        public bool JudgeFileType(string ext)
        {
            if (ZipTypes.Contains(ext))
            {
                return true;
            }
            return false;
        }
    }
}

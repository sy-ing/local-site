using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    /// <summary>
    /// 判断文件类型是不是应用
    /// </summary>
    internal sealed class TypeApp : IJudgeFileType
    {
        public static readonly List<string> AppTypes = new List<string> {
            "exe", "msi",//windows
            "rpm", " gz","deb",//linux
            "dmg",//MAC
             "app","dll",//public
             "zip","apk"

           };
        public bool JudgeFileType(string ext)
        {
            if (AppTypes.Contains(ext))
            {
                return true;
            }
            return false;
        }
    }
}

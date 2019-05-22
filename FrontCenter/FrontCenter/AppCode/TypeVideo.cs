using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    /// <summary>
    /// 判断文件类型是不是视频
    /// </summary>
    internal sealed class TypeVideo : IJudgeFileType
    {
        public static readonly List<string> VideoTypes = new List<string> {
            "wmv", "asf", "asx",//微软视频 ：wmv、asf、asx
            "rm", "rmvb",//Real Player ：rm、 rmvb
            "mpg","mpeg","mpe",//MPEG视频 ：mpg、mpeg、mpe
             "3gp",//手机视频 ：3gp
            "mov",//Apple视频 ：mov
             "mp4", "m4v",//Sony视频 ：mp4、m4v
             "avi", "dat", "mkv", "flv", "vob" , "wvx",  "mpa" //其他常见视频：avi、dat、mkv、flv、vob、wvx、mpa
           };
        public bool JudgeFileType(string ext)
        {
            if (VideoTypes.Contains(ext))
            {
                return true;
            }
            return false;
        }
    }
}

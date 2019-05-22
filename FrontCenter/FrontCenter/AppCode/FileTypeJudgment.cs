using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    /// <summary>
    /// 文件类型判断
    /// </summary>
    internal sealed class FileTypeJudgment
    {
        //实现类型判断的接口
        public IJudgeFileType TypeTarget { get; set; }

        //返回判断结果
        public bool Judge(string ext)
        {
            return this.TypeTarget.JudgeFileType(ext);
        }
    }
}

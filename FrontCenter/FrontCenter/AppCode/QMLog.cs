using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    public class QMLog
    {

        private readonly IHostingEnvironment _hostingEnvironment = Method._hostingEnvironment;
        private string sLogPath;
        private string sLogFilePath = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="hostingEnvironment"></param>
        public QMLog()
        {


            if (_hostingEnvironment == null)
            {
                sLogPath = "C:\\LOG";
            }
            else
            {
                sLogPath = _hostingEnvironment.ContentRootPath + "\\LOG";
            }

        }


        /// <summary>
        /// 设置日志文件存放路径
        /// </summary>
        /// <param name="sFilePath"></param>
        public void SetLogPath(string sFilePath)
        {
            sLogPath = sFilePath;
        }

        public void WriteLogToFile(string strModule, string strLog)
        {
            if (sLogPath == null)
                return;
            try
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(sLogPath);
                if (!dir.Exists)
                    dir.Create();
                else
                {
                    DateTime nt = DateTime.Now.AddDays(-2);
                    foreach (System.IO.FileInfo f in dir.GetFiles())
                    {

                        if (f.CreationTime < nt)
                            f.Delete();
                    }
                }


                DateTime curTime = DateTime.Now;
                int year = curTime.Year;
                int month = curTime.Month;
                int day = curTime.Day;
                int hour = curTime.Hour;
                int minute = curTime.Minute;
                int second = curTime.Second;

                string fileName = string.Format("VGPageBuild_{0,4:0}{1,2:00}{2,2:00}.txt", year, month, day);
                sLogFilePath = sLogPath + "\\" + fileName;

                string line = string.Format("{0,2:00}:{1,2:00}:{2,2:00}::{3}::", hour, minute, second, strModule);
                line = line + strLog + "\r\n";
                byte[] log = Encoding.UTF8.GetBytes(line);

                lock (this)
                {
                    FileStream s2 = new FileStream(sLogFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                    s2.Seek(0, SeekOrigin.End);
                    s2.Write(log, 0, log.Length);
                    s2.Dispose();
                }
            }
            catch//(Exception ex)
            { }
        }

        public void WriteLogToFile(string _fileName, string strLog, int _i)
        {
            if (sLogPath == null)
                return;
            try
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(sLogPath);
                if (!dir.Exists)
                    dir.Create();
                else
                {
                    DateTime nt = DateTime.Now.AddDays(-2);
                    foreach (System.IO.FileInfo f in dir.GetFiles())
                    {

                        if (f.CreationTime < nt)
                            f.Delete();
                    }
                }


                DateTime curTime = DateTime.Now;
                int year = curTime.Year;
                int month = curTime.Month;
                int day = curTime.Day;
                int hour = curTime.Hour;
                int minute = curTime.Minute;
                int second = curTime.Second;

                string fileName = string.Format("VGPlay_{0,4:0}{1,2:00}{2,2:00}.txt", year, month, day);
                if (_fileName != "")
                {
                    fileName = _fileName + ".txt";
                }
                sLogFilePath = sLogPath + "\\" + fileName;

                string line = string.Format("{0,2:00}:{1,2:00}:{2,2:00}::", hour, minute, second);
                line = line + strLog + "\r\n";
                byte[] log = Encoding.UTF8.GetBytes(line);

                lock (this)
                {
                    FileStream s2 = new FileStream(sLogFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                    s2.Seek(0, SeekOrigin.End);
                    s2.Write(log, 0, log.Length);
                    s2.Dispose();
                }
            }
            catch//(Exception ex)
            { }
        }
    }
}

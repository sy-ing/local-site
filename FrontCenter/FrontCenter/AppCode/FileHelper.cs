using FrontCenter.Models;
using FrontCenter.Models.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    public class FileHelper
    {
        /// <summary>
        /// 下载一个远程文件（http方式）
        /// </summary>
        /// <param name="apppath">文件路径</param>
        /// <param name="filename">文件名称</param>
        /// <param name="dbContext">数据库连接字段</param>
        /// <returns></returns>
        public static async Task<long> DownloadFile(string apppath, ContextString dbContext)
        {


            QMLog qm = new QMLog();
            long _Result = -1;

            try
            {
                var GUID = Guid.NewGuid();
                string filename = apppath.Split('/').Last();
                var client = new HttpClient
                {
                    BaseAddress = new Uri(apppath)
                };

                var response = await client.GetAsync("");
                var stream = await response.Content.ReadAsStreamAsync();

                // var head = response.Headers;

                //声明字符数据，将获取到的流信息读到字符数组中
                byte[] byteArray = new byte[stream.Length];

                int readCount = 0; // 已经成功读取的字节的个数
                while (readCount < stream.Length)
                {
                    readCount += stream.Read(byteArray, readCount, (int)stream.Length - readCount);
                }





                AssetFile assetfile = new AssetFile();
                //文件名


                long filesize = stream.Length;
                //文件类型
                var ext = filename.Split('.').Last();


                FileTypeJudgment ftj = new FileTypeJudgment() { TypeTarget = new TypeImg() };
                assetfile.FileType = "未知";
                if (ftj.Judge(ext))
                {
                    assetfile.FileType = "图片";
                }

                ftj.TypeTarget = new TypeVideo();
                if (ftj.Judge(ext))
                {
                    assetfile.FileType = "视频";
                }
                ftj.TypeTarget = new TypeApp();
                if (ftj.Judge(ext))
                {
                    assetfile.FileType = "应用";
                }




                //文件保存路径
                var _FolderPath = @"\Files" + @"\" + GUID;
                string path = Method._hostingEnvironment.WebRootPath + _FolderPath;

                if (Directory.Exists(path) == false)//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(path);
                }

                //文件全路径
                var filepath = Method._hostingEnvironment.WebRootPath + _FolderPath + @"\" + filename;
                using (FileStream fs = System.IO.File.Create(filepath))
                {
                    fs.Write(byteArray, 0, (int)stream.Length);
                    fs.Flush();
                    fs.Dispose();
                }

                //添加文件到数据库

                assetfile.AddTime = DateTime.Now;
                assetfile.FileExtName = ext;
                assetfile.Code = GUID.ToString();
                assetfile.FileHash = "";
                assetfile.FileName = filename.Split('.').First();
                assetfile.FilePath = @"\Files" + @"\" + GUID.ToString() + @"\" + filename;
                assetfile.FileSize = filesize;
                dbContext.AssetFiles.Add(assetfile);

                if (dbContext.SaveChanges() > 0)
                {
                    _Result = assetfile.ID;


                }
                else
                {
                    qm.WriteLogToFile(filename, "添加文件到数据库失败");



                }

            }
            catch (Exception e)
            {
                qm.WriteLogToFile("", e.ToString());

            }


            return _Result;




        }

        /// <summary>
        /// 删除文件记录及物理文件
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static async Task<bool> DelFile(string Code, ContextString dbContext)
        {
            bool _Result = false;

            try
            {
                var file = await dbContext.AssetFiles.Where(i => i.Code == Code).AsNoTracking().FirstOrDefaultAsync();
                var path = Method._hostingEnvironment.WebRootPath + file.FilePath;
                DirectoryInfo info = new DirectoryInfo(path);
                Directory.Delete(info.Parent.FullName, true);

                dbContext.AssetFiles.Remove(file);
                if (await dbContext.SaveChangesAsync() > 0)
                {
                    _Result = true;
                }




            }
            catch (Exception e)
            {
                QMLog qm = new QMLog();
                qm.WriteLogToFile("删除文件出错", e.ToString());

            }

            return _Result;
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        private static long FileSize(string uuid)
        {
            //文件根目录
            string rootpath = Method._hostingEnvironment.WebRootPath + @"\Files" + $@"\{uuid}";
            //文件长度
            long len = 0;


            if (Directory.Exists(rootpath) == false)
            {
                return len;
            }


            //获取根目录下所有的文件夹,并存到一个新的对象数组中,以进行递归
            string[] directories = Directory.GetDirectories(rootpath);

            foreach (var path in directories)
            {
                //获取文件夹中文件
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] files = di.GetFiles();

                //通过GetFiles方法,获取di目录中的所有文件的大小
                foreach (FileInfo fi in di.GetFiles())
                {
                    len += fi.Length;//添加文件长度
                }

            }

            return len;
        }


        /// <summary>
        /// 合并文件
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="totalChunk"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool Merge(string uuid, int totalChunk, string fileName)
        {
            try
            {
                //将要保存文件的全路径
                var outfile = Method._hostingEnvironment.WebRootPath + @"\Files" + $@"\{uuid}" + $@"\{fileName}";

                //文件片上一级目录
                var path = Method._hostingEnvironment.WebRootPath + @"\Files" + $@"\{uuid}\" + "cache";


                FileStream AddStream = null;
                //以合并后的文件名称和打开方式来创建、初始化FileStream文件流
                AddStream = new FileStream(outfile, FileMode.Append);

                //以FileStream文件流来初始化BinaryWriter书写器，此用以合并分割的文件
                BinaryWriter AddWriter = new BinaryWriter(AddStream);
                FileStream TempStream = null;
                BinaryReader TempReader = null;

                //循环合并小文件，并生成合并文件
                for (int i = 0; i < totalChunk; i++)
                {
                    //以小文件所对应的文件名称和打开模式来初始化FileStream文件流，起读取分割作用

                    TempStream = new FileStream(path + @"\" + fileName.Split('.').First() + i.ToString() + ".blob", FileMode.Open);

                    // TempStream = new FileStream(path + i.ToString() + @"\" + fileName, FileMode.Open);
                    TempReader = new BinaryReader(TempStream);

                    //读取分割文件中的数据，并生成合并后文件
                    AddWriter.Write(TempReader.ReadBytes((int)TempStream.Length));

                    //关闭BinaryReader文件阅读器
                    TempReader.Dispose();

                    //关闭FileStream文件流
                    TempStream.Dispose();
                }
                //关闭BinaryWriter文件书写器
                AddWriter.Dispose();
                //关闭FileStream文件流
                AddStream.Dispose();

                //删除子文件夹及其中的文件片
                var ppath = Method._hostingEnvironment.WebRootPath + @"\Files" + $@"\{uuid}\";
                string[] directories = Directory.GetDirectories(ppath);

                foreach (var d in directories)
                {
                    Directory.Delete(d, true);
                }



                return true;
            }
            catch (Exception)
            {
                return false;
            }


        }

        /// <summary>
        /// 获取文件MD5 哈希码
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                QMLog log = new QMLog();
                log.WriteLogToFile("GetMD5HashFromFile() fail, error:", ex.Message);
                return "";
            }
        }

        /// <summary> 
        /// 从文件读取 Stream 
        /// </summary> 
        public static Stream FileToStream(string filePath)
        {
            // 打开文件 
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            // 读取文件的 byte[] 
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            // 把 byte[] 转换成 Stream 
            Stream stream = new MemoryStream(bytes);
            return stream;
        }



        /// <summary>
        /// 上传图片到文件服务器
        /// </summary>
        /// <param name="url">上传地址</param>
        /// <param name="filename">文件名</param>
        /// <param name="dic">参数集</param>
        /// <param name="filebyte">文件流</param>
        /// <returns></returns>
        public static string ImagePost(string url, string filename, Dictionary<string, string> dic, byte[] filebyte)
        {


            //boundary setting 
            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            string fileName = Path.GetFileName(filename);
            string nameOnly = fileName.Substring(0, fileName.LastIndexOf("."));




            StringBuilder sb = new StringBuilder();

            //发送必要字段
            foreach (KeyValuePair<string, string> param in dic)
            {
                sb = sb.Append("--");
                sb = sb.Append(boundary);
                sb = sb.Append("\r\n");
                sb = sb.Append("Content-Disposition: form-data; name=\"" + param.Key + "\"\r\n\r\n");
                sb = sb.Append(param.Value);
                sb = sb.Append("\r\n");
            }

            //要发送的文件
            sb = sb.Append("--");
            sb = sb.Append(boundary);
            sb = sb.Append("\r\n");
            sb = sb.Append("Content-Disposition: form-data; name=\"" + nameOnly + "\"; filename=\"" + fileName + "\"\r\n");
            sb = sb.Append("Content-Type: application/octet-stream\r\n\r\n");

            byte[] data = Encoding.Default.GetBytes(sb.ToString());

            byte[] end_data = Encoding.Default.GetBytes(("\r\n--" + boundary + "--\r\n"));



            HttpWebRequest webRequset = (HttpWebRequest)WebRequest.Create(url);
            webRequset.ContentType = "multipart/form-data; boundary=" + boundary;
            webRequset.Method = "POST";

            //长度属性：头+文件流+尾的长度
            long length = data.Length + filebyte.Length + end_data.Length;
            // long length =+ filebyte.Length;
            webRequset.ContentLength = length;

            Stream requestStream = webRequset.GetRequestStream();
            //将拼接的数据和文件流写入
            requestStream.Write(data, 0, data.Length);
            requestStream.Write(filebyte, 0, filebyte.Length);
            requestStream.Write(end_data, 0, end_data.Length);
            //获取返回信息
            WebResponse responce = webRequset.GetResponse();
            Stream responeStream = responce.GetResponseStream();
            StreamReader sr = new StreamReader(responeStream);

            return sr.ReadToEnd();
        }


    }
}

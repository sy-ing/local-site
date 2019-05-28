using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FrontCenter.AppCode.NotUse
{
    public class Try_T_Fail
    {
        /// <summary>
        /// 获得数据差异
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cloudData">云端数据</param>
        /// <param name="localData">本地数据</param>
        /// <returns></returns>
        public List<T> ConvertType<T>(List<T> cloudData, List<T> localData)
        {
            //DbContextOptions<ContextString> options = new DbContextOptions<ContextString>();
            //ContextString dbContext = new ContextString(options);



            //遍历云端数据
            //如果本地已有该数据 检测是否一致
            //如果本地没有 或者本地数据不一致 则加入到更改选项中
            foreach (var cdata in cloudData)
            {

                //根据Code判断本地是否已包含该数据
                if (localData.Exists(o => TEqual<T>.Equals(o.GetType().GetProperty("Code").GetValue(o, null), cdata.GetType().GetProperty("Code").GetValue(cdata, null))))
                {
                    //判断数据是否完全一致
                    if (!localData.Exists(o => TEqual<T>.Equals(o, cdata)))
                    {
                        var ldata = localData.Where(l => l.GetType().GetProperty("Code").GetValue(l, null) == cdata.GetType().GetProperty("Code").GetValue(cdata, null)).FirstOrDefault();
                        var pros = typeof(T).GetProperties();
                        int i = 0; foreach (PropertyInfo p in pros)
                        {
                            var val = cdata.GetType().GetProperty(p.Name).GetValue(cdata, null);
                            if (val != null)
                            {
                                ldata.GetType().GetProperties()[i].SetValue(ldata, val, null);
                            }
                            i++;
                        }
                        //var entry = dbContext.Entry<T>(ldata);
                        //entry.State = EntityState.Modified;
                        //dbContext.SaveChanges();
                    }



                }
                else
                {

                    //dbContext.Set<T>().Add(cdata);
                    //dbContext.SaveChanges();
                    //localData.Add(cdata);
                }


            }


            return localData;
        }

  



        /// <summary>
        /// 判断二个泛类型值是否相等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class TEqual<T>
        {
            public T Value
            {
                get { return _Value; }
                set
                {
                    if (!EqualityComparer<T>.Default.Equals(_Value, value))
                    {
                        _Value = value;
                    }
                }
            }
            private T _Value;
        }
    }
}

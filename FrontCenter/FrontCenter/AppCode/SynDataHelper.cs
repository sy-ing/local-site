using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FrontCenter.AppCode
{
    public class SynDataHelper
    {
        /// <summary>
        /// 使数据相等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cloudData">云端数据</param>
        /// <param name="localData">本地数据</param>
        /// <returns></returns>
        public static T MakeEqual<T>(T cloudData, T localData)
        {

            var pros = typeof(T).GetProperties();
            int i = 0; foreach (PropertyInfo p in pros)
            {
                if (p.Name != "ID")
                {
                    var val = cloudData.GetType().GetProperty(p.Name).GetValue(cloudData, null);
                    if (val != null)
                    {
                        localData.GetType().GetProperties()[i].SetValue(localData, val, null);
                    }
                   
                }
                i++;
            }


            return localData;
        }

    
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models.Data
{
    public class DbInitializer
    {
        public static void Initialize(ContextString context)
        {

            //如果当前数据库不存在按照当前 model 创建
            context.Database.EnsureCreated();


            // Look for any files.
            if (context.Account.Any())
            {
                return;   // DB has been seeded
            }

            context.SaveChanges();

        }
    }
}

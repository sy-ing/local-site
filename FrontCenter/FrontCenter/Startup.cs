using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrontCenter.AppCode;
using FrontCenter.Models.Data;
using Hangfire;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FrontCenter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //注册数据库连接字符串
            string s = Configuration.GetConnectionString("DefaultConnection");
            Method.ContextStr = s;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //注册服务器地址
            Method.ServerAddr = Configuration.GetConnectionString("ServerAddress");

            //注册客户唯一代码
            // Method.CusID = Configuration.GetConnectionString("CusID");

            //注册云服务器地址
           // Method.PlatformAddr = Configuration.GetConnectionString("PlatformAddress");


            //注册商户管理网站地址
            Method.MallSite = Configuration.GetConnectionString("MallPlatAddress");

            Method.FileServer = Configuration.GetConnectionString("FileServer");
            

            Method.BaiduIOT = Configuration.GetConnectionString("BaiduIOT");

            services.AddDbContext<ContextString>(options =>
 options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            //设置接收文件长度的最大值。
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });



            //添加跨域访问配置
            services.AddCors(options => options.AddPolicy("CorsSample",
        p => p.WithOrigins("*").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin().AllowCredentials()));

            // bool  IsStartHF = Configuration.GetValue<bool>("HangfireStart");

            string IsStartHF = Configuration.GetConnectionString("HangfireStart");
            //添加hangfire服务
            if (IsStartHF == "true")
            {
                services.AddHangfire(x => x.UseSqlServerStorage(s));
            }

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ContextString context)
        {

            //初始化数据库
         //   DbInitializer.Initialize(context);
            Method._hostingEnvironment = env;
          //  Method._context = context;


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }


            app.UseWebSockets();
            app.UseMiddleware<ChatWebSocketMiddleware>();


            var staticfile = new StaticFileOptions();
            staticfile.ServeUnknownFileTypes = true;
            staticfile.DefaultContentType = "application/x-msdownload"; //设置默认  MIME
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings.Add(".log", "text/plain");//手动设置对应MIME
            staticfile.ContentTypeProvider = provider;
            app.UseStaticFiles(staticfile);


            //启用Session
            app.UseSession();

            //启用跨域访问配置
            app.UseCors("CorsSample");

            //初始化数据库
          //  DbInitializer.Initialize(context);

          

            Method._hostingEnvironment = env;
            Method._context = context;

            Method.ServerMac = Method.GetServerMac();

            ServerIOTHelper.CreateServerToIOT();
            app.UseHttpsRedirection();
            app.UseCookiePolicy();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            string IsStartHF = Configuration.GetConnectionString("HangfireStart");

            if (IsStartHF == "true")
            {

                //添加Hangfire应用
                app.UseHangfireServer();
                app.UseHangfireDashboard("/api/jobs", new DashboardOptions()
                {
                    Authorization = new[] { new CustomAuthorizeFilter() }
                });

                //更新服务器状态
                RecurringJob.AddOrUpdate(
() => Method.UpdateDevState(),
Cron.Minutely);
            }
        }


        public class CustomAuthorizeFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize([NotNull] DashboardContext context)
            {

                //var httpcontext = context.GetHttpContext();
                //return httpcontext.User.Identity.IsAuthenticated;


                return true;
            }
        }
    }
}

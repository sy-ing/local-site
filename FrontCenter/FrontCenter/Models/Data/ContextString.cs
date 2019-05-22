using FrontCenter.AppCode;
using FrontCenter.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.Models.Data
{
    public class ContextString : DbContext
    {
        public ContextString(DbContextOptions<ContextString> options) : base(options)
        {
        }
        #region  声明
        public DbSet<AssetFile> AssetFiles { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<SysLog> SysLog { get; set; }
        public DbSet<MenuViewModel> MenuViewModel { get; set; }

        public DbSet<FuncInfo> FuncInfo { get; set; }
        public DbSet<MallFunc> MallFunc { get; set; }

        public DbSet<TimeRelate> TimeRelate { get; set; }

        public DbSet<TimeSlot> TimeSlot { get; set; }
        public DbSet<TimeAxis> TimeAxis { get; set; }

        public DbSet<AuditProcess> AuditProcess { get; set; }
        public DbSet<Screensaver> Screensaver { get; set; }

        public DbSet<Device> Device { get; set; }
        public DbSet<ScreenInfo> ScreenInfo { get; set; }
        public DbSet<DataDict> DataDict { get; set; }
        public DbSet<Floor> Floor { get; set; }

        public DbSet<Mall> Mall { get; set; }

        public DbSet<DeviceGroup> DeviceGroup { get; set; }
        public DbSet<DevAppOnline> DevAppOnline { get; set; }
        public DbSet<DeviceCoordinate> DeviceCoordinate { get; set; }
        public DbSet<DeviceToGroup> DeviceToGroup { get; set; }
        public DbSet<PlayHistory> PlayHistory { get; set; }

        #region  节目迁移

        public DbSet<Programs> Programs { get; set; }
        public DbSet<ProgramToGroup> ProgramToGroup { get; set; }
        public DbSet<AssetProgram> AssetProgram { get; set; }
        public DbSet<ProgramGroup> ProgramGroup { get; set; }
        public DbSet<ProgramDevice> ProgramDevice { get; set; }
        public DbSet<EmergencyNews> EmergencyNews { get; set; }

        public DbSet<Output_ProgramGroupQuery> Output_ProgramGroupQuery { get; set; }


        #endregion


        public DbSet<ShopToDevice> ShopToDevice { get; set; }

        public DbSet<Building> Building { get; set; }

        public DbSet<AppDev> AppDev { get; set; }
        public DbSet<ScheduleDevice> ScheduleDevice { get; set; }
        public DbSet<ApplicationNew> ApplicationNew { get; set; }
        public DbSet<MallBuilding> MallBuilding { get; set; }
        public DbSet<Output_DevFlow> Output_DevFlow { get; set; }
        public DbSet<Output_TopDevFlow> Output_TopDevFlow { get; set; }

        public DbSet<Output_GetDevGroupList> Output_GetDevGroupList { get; set; }
        public DbSet<Subtitle> Subtitle { get; set; }
        public DbSet<SubtitleToDeviceGroup> SubtitleToDeviceGroup { get; set; }
        public DbSet<Live> Live { get; set; }
        public DbSet<LiveToDev> LiveToDev { get; set; }
        public DbSet<Output_UserList> Output_UserList { get; set; }
        public DbSet<UserWarningEmail> UserWarningEmail { get; set; }

        public DbSet<UserAppNew> UserAppNew { get; set; }

        public DbSet<AppClassNew> AppClassNew { get; set; }
        public DbSet<AppSite> AppSite { get; set; }
        public DbSet<AppTime> AppTime { get; set; }

        public DbSet<Application> Application { get; set; }
        public DbSet<AppClass> AppClass { get; set; }
        public DbSet<ApplicationDevice> ApplicationDevice { get; set; }
        public DbSet<AppUsageInfo> AppUsageInfo { get; set; }
        public DbSet<UserApp> UserApp { get; set; }
        public DbSet<Output_AppUser> Output_AppUser { get; set; }
        public DbSet<Output_AppUserNew> Output_AppUserNew { get; set; }
        public DbSet<ContainerBG> ContainerBG { get; set; }

        public DbSet<OrderAudit> OrderAudit { get; set; }
        public DbSet<Shops> Shops { get; set; }
        public DbSet<MallShop> MallShop { get; set; }
        public DbSet<ScheduleDate> ScheduleDate { get; set; }
        public DbSet<ScheduleMaterial> ScheduleMaterial { get; set; }
        public DbSet<ScheduleOrder> ScheduleOrder { get; set; }
        public DbSet<SchedulePeriod> SchedulePeriod { get; set; }


        public DbSet<ProgramFile> ProgramFile { get; set; }
        public DbSet<ProperMaterial> ProperMaterial { get; set; }
        public DbSet<ProgramOrder> ProgramOrder { get; set; }
        public DbSet<ProgramMaterial> ProgramMaterial { get; set; }

        public DbSet<ShopAccount> ShopAccount { get; set; }

        public DbSet<StoreNews> StoreNews { get; set; }
        public DbSet<NewsImg> NewsImg { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreNews>().ToTable("StoreNews");
            modelBuilder.Entity<NewsImg>().ToTable("NewsImg");
            modelBuilder.Entity<ShopAccount>().ToTable("ShopAccount");
            modelBuilder.Entity<ProgramFile>().ToTable("ProgramFile");
            modelBuilder.Entity<ProperMaterial>().ToTable("ProperMaterial");
            modelBuilder.Entity<ProgramOrder>().ToTable("ProgramOrder");
            modelBuilder.Entity<ProgramMaterial>().ToTable("ProgramMaterial");

            modelBuilder.Entity<OrderAudit>().ToTable("OrderAudit");
            modelBuilder.Entity<Shops>().ToTable("Shops");
            modelBuilder.Entity<MallShop>().ToTable("MallShop");
            modelBuilder.Entity<ScheduleDate>().ToTable("ScheduleDate");
            modelBuilder.Entity<ScheduleMaterial>().ToTable("ScheduleMaterial");
            modelBuilder.Entity<ScheduleOrder>().ToTable("ScheduleOrder");
            modelBuilder.Entity<SchedulePeriod>().ToTable("SchedulePeriod");

            modelBuilder.Entity<ContainerBG>().ToTable("ContainerBG");
            modelBuilder.Entity<UserApp>().ToTable("UserApp");
            modelBuilder.Entity<AppUsageInfo>().ToTable("AppUsageInfo");
            modelBuilder.Entity<ApplicationDevice>().ToTable("ApplicationDevice");
            modelBuilder.Entity<AppClass>().ToTable("AppClass");
            modelBuilder.Entity<Application>().ToTable("Application");
            modelBuilder.Entity<AssetFile>().ToTable("AssetFile");

            modelBuilder.Entity<Account>().ToTable("Account");
            modelBuilder.Entity<Roles>().ToTable("Roles");
            modelBuilder.Entity<RolePermissions>().ToTable("RolePermissions");
            modelBuilder.Entity<Permission>().ToTable("Permission");
            modelBuilder.Entity<UserRoles>().ToTable("UserRoles");
            modelBuilder.Entity<SysLog>().ToTable("SysLog");

            modelBuilder.Entity<FuncInfo>().ToTable("FuncInfo");
            modelBuilder.Entity<MallFunc>().ToTable("MallFunc");

            modelBuilder.Entity<TimeRelate>().ToTable("TimeRelate");
            modelBuilder.Entity<TimeSlot>().ToTable("TimeSlot");
            modelBuilder.Entity<TimeAxis>().ToTable("TimeAxis");
            modelBuilder.Entity<AuditProcess>().ToTable("AuditProcess");
            modelBuilder.Entity<Screensaver>().ToTable("Screensaver");

            modelBuilder.Entity<Device>().ToTable("Device");
            modelBuilder.Entity<ScreenInfo>().ToTable("ScreenInfo");
            modelBuilder.Entity<DataDict>().ToTable("DataDict");
            modelBuilder.Entity<Floor>().ToTable("Floor");


            modelBuilder.Entity<DevAppOnline>().ToTable("DevAppOnline");
            modelBuilder.Entity<DeviceCoordinate>().ToTable("DeviceCoordinate");
            modelBuilder.Entity<DeviceGroup>().ToTable("DeviceGroup");
            modelBuilder.Entity<DeviceToGroup>().ToTable("DeviceToGroup");

            modelBuilder.Entity<Mall>().ToTable("Mall");
            modelBuilder.Entity<PlayHistory>().ToTable("PlayHistory");

            #region  节目迁移
            modelBuilder.Entity<Programs>().ToTable("Programs");
            modelBuilder.Entity<ProgramToGroup>().ToTable("ProgramToGroup");
            modelBuilder.Entity<AssetProgram>().ToTable("AssetPrograms");
            modelBuilder.Entity<ProgramGroup>().ToTable("ProgramGroup");
            modelBuilder.Entity<ProgramDevice>().ToTable("ProgramDevice");
            modelBuilder.Entity<EmergencyNews>().ToTable("EmergencyNews");

            #endregion

            modelBuilder.Entity<ShopToDevice>().ToTable("ShopToDevice");

            modelBuilder.Entity<Building>().ToTable("Building");
            modelBuilder.Entity<AppDev>().ToTable("AppDev");

            modelBuilder.Entity<ApplicationNew>().ToTable("ApplicationNew");
            modelBuilder.Entity<ScheduleDevice>().ToTable("ScheduleDevice");
            modelBuilder.Entity<MallBuilding>().ToTable("MallBuilding");

            modelBuilder.Entity<Subtitle>().ToTable("Subtitle");
            modelBuilder.Entity<SubtitleToDeviceGroup>().ToTable("SubtitleToDeviceGroup");

            modelBuilder.Entity<Live>().ToTable("Live");
            modelBuilder.Entity<LiveToDev>().ToTable("LiveToDev");
            modelBuilder.Entity<UserWarningEmail>().ToTable("UserWarningEmail");

            modelBuilder.Entity<UserAppNew>().ToTable("UserAppNew");
            modelBuilder.Entity<AppClassNew>().ToTable("AppClassNew");
            modelBuilder.Entity<AppSite>().ToTable("AppSite");
            modelBuilder.Entity<AppTime>().ToTable("AppTime");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Method.ContextStr);
            base.OnConfiguring(optionsBuilder);
        }
    }
}

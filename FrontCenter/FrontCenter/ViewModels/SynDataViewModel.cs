using FrontCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontCenter.ViewModels
{
    public class SynDataViewModel
    {
    }

    public class Input_PullInitData
    {
        public List<Mall> Malllist { get; set; }

        public List<Permission> Permissionlist { get; set; }

        public List<Menu> Menulist { get; set; }

    }


    public class Input_PullSystemData
    {
        public List<Account> Accountlist { get; set; }
        public List<Roles> Roleslist { get; set; }
        public List<RolePermissions> RolePermissionslist { get; set; }
        public List<UserRoles> UserRoleslist { get; set; }
        public List<Screensaver> Screensaverlist { get; set; }

        public List<TimeAxis> TimeAxislist { get; set; }

        public List<AuditProcess> AuditProcesslist { get; set; }

        public List<SysLog> SysLoglist { get; set; }
    }


    public class Input_PullProgramData
    {
        public List<Programs> Programslist { get; set; }
        public List<ProgramGroup> ProgramGrouplist { get; set; }
        public List<ProgramToGroup> ProgramToGrouplist { get; set; }
        public List<ProgramDevice> ProgramDevicelist { get; set; }
        public List<Live> Livelist { get; set; }

        public List<LiveToDev> LiveToDevlist { get; set; }

        public List<Subtitle> Subtitlelist { get; set; }

        public List<SubtitleToDeviceGroup> SubtitleToDeviceGrouplist { get; set; }
    }

    public class Input_PullAppData
    {

        public List<AppClassNew> AppClassNewlist { get; set; }
        public List<AppDev> AppDevlist { get; set; }

        public List<ApplicationDevice> ApplicationDevicelist { get; set; }

        public List<ApplicationNew> ApplicationNewlist { get; set; }

        public List<AppSite> AppSitelist { get; set; }

        public List<AppTime> AppTimelist { get; set; }
        public List<AppUsageInfo> AppUsageInfolist { get; set; }
    
    }

    public class Input_PullReviewData
    {

        public List<OrderAudit> OrderAuditlist { get; set; }

        public List<ScheduleDate> ScheduleDatelist { get; set; }
        public List<ScheduleDevice> ScheduleDevicelist { get; set; }
        public List<ScheduleMaterial> ScheduleMateriallist { get; set; }
        public List<ScheduleOrder> ScheduleOrderlist { get; set; }
        public List<SchedulePeriod> SchedulePeriodlist { get; set; }
        public List<StoreNews> StoreNewslist { get; set; }


        public List<ProgramMaterial> ProgramMateriallist { get; set; }

        public List<ProperMaterial> ProperMateriallist { get; set; }

    }


    public class Input_PullShopInfoData
    {

        public List<AreaInfo> AreaInfolist { get; set; }

        public List<MallBuilding> MallBuildinglist { get; set; }
        public List<Building> Buildinglist { get; set; }
        public List<Floor> Floorlist { get; set; }
        public List<ParkingLot> ParkingLotlist { get; set; }
        public List<ParkingSpace> ParkingSpacelist { get; set; }
        public List<ShopFormat> ShopFormatlist { get; set; }


        public List<ShopAccount> ShopAccountlist { get; set; }

        public List<ShopNum> ShopNumlist { get; set; }


        public List<Shops> Shopslist { get; set; }

        public List<ShopToDevice> ShopToDevicelist { get; set; }

    }

    
}

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

        public DbSet<Account> Account { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<SysLog> SysLog { get; set; }
        public DbSet<MenuViewModel> MenuViewModel { get; set; }

        public DbSet<FuncInfo> FuncInfo { get; set; }
        public DbSet<MallFunc> MallFunc { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<Account>().ToTable("Account");
            modelBuilder.Entity<Roles>().ToTable("Roles");
            modelBuilder.Entity<RolePermissions>().ToTable("RolePermissions");
            modelBuilder.Entity<Permission>().ToTable("Permission");
            modelBuilder.Entity<UserRoles>().ToTable("UserRoles");
            modelBuilder.Entity<SysLog>().ToTable("SysLog");

            modelBuilder.Entity<FuncInfo>().ToTable("FuncInfo");
            modelBuilder.Entity<MallFunc>().ToTable("MallFunc");

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Method.ContextStr);
            base.OnConfiguring(optionsBuilder);
        }
    }
}

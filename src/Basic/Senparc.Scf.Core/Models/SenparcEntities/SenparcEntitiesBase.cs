﻿using Microsoft.EntityFrameworkCore;
using Senparc.Scf.Core.Models.DataBaseModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Senparc.Scf.Core.Models
{
    /// <summary>
    /// SenparcEntities 的基类
    /// </summary>
    public abstract class SenparcEntitiesBase : DbContext, ISenparcEntities
    {
        private static readonly bool[] _migrated = { true };

        public SenparcEntitiesBase(DbContextOptions options) : base(options)
        {
        }

        #region 系统表

        /// <summary>
        /// 菜单
        /// </summary>
        public DbSet<SysMenu> SysMenus { get; set; }

        /// <summary>
        /// 菜单下面的按钮
        /// </summary>
        public DbSet<SysButton> SysButtons { get; set; }

        /// <summary>
        /// 系统角色
        /// </summary>
        public DbSet<SysRole> SysRoles { get; set; }

        /// <summary>
        /// 角色菜单表
        /// </summary>
        public DbSet<SysPermission> SysPermission { get; set; }


        /// <summary>
        /// 角色人员表
        /// </summary>
        public DbSet<SysRoleAdminUserInfo> SysRoleAdminUserInfos { get; set; }


        public virtual DbSet<Account> Accounts { get; set; }


        public DbSet<SystemConfig> SystemConfigs { get; set; }


        public virtual DbSet<PointsLog> PointsLogs { get; set; }

        public virtual DbSet<AccountPayLog> AccountPayLogs { get; set; }

        #region 不可修改系统表

        #endregion


        /// <summary>
        /// 扩展模块
        /// </summary>
        public DbSet<XscfModule> XscfModules { get; set; }

        #endregion

        /// <summary>
        /// 执行 EF Core 的合并操作（等价于 update-database）
        /// <para>出于安全考虑，每次执行 Migrate() 方法之前，必须先执行 ResetMigrate() 开启允许 Migrate 执行的状态。</para>
        /// </summary>
        public virtual void Migrate()
        {
            if (!_migrated[0])
            {
                lock (_migrated)
                {
                    if (!_migrated[0])
                    {
                        Database.Migrate(); // apply all migrations
                        _migrated[0] = true;
                    }
                }
            }
        }

        /// <summary>
        /// 重置合并状态
        /// </summary>
        public virtual void ResetMigrate()
        {
            _migrated[0] = false;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region 不可修改系统表

            modelBuilder.ApplyConfiguration(new XscfModuleAccountConfigurationMapping());
            modelBuilder.ApplyConfiguration(new AccountConfigurationMapping());
            modelBuilder.ApplyConfiguration(new AccountPayLogConfigurationMapping());
            modelBuilder.ApplyConfiguration(new PointsLogConfigurationMapping());

            #endregion

            var types = modelBuilder.Model.GetEntityTypes().Where(e => typeof(EntityBase).IsAssignableFrom(e.ClrType));
            foreach (var entityType in types)
            {
                SetGlobalQueryMethodInfo
                        .MakeGenericMethod(entityType.ClrType)
                        .Invoke(this, new object[] { modelBuilder });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static readonly MethodInfo SetGlobalQueryMethodInfo = typeof(SenparcEntitiesBase)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(t => t.IsGenericMethod && t.Name == "SetGlobalQuery");

        /// <summary>
        /// 全局查询，附带软删除状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        public virtual void SetGlobalQuery<T>(ModelBuilder builder) where T : EntityBase
        {
            builder.Entity<T>().HasQueryFilter(z => !z.Flag);
        }
    }
}

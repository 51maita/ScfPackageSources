﻿using Senparc.Scf.Core.Enums;
using Senparc.Scf.XscfBase;
using Senparc.Xscf.Application.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Senparc.Xscf.Application
{
    [XscfRegister]
    public class Register : IXscfRegister
    {
        public Register()
        { }

        #region IRegister 接口

        public string Name => "Senparc.Xscf.Application";
        public string Uid => "699DFE0D-C1C0-4315-87DF-0DE1502B87A9";//必须确保全局唯一，生成后必须固定
        public string Version => "0.0.5";//必须填写版本号

        public string MenuName => "应用程序模块";
        public string Description => "此模块提供给开发者一个可以启动任何程序！";

        /// <summary>
        /// 注册当前模块需要支持的功能模块
        /// </summary>
        public IList<Type> Functions => new[] { 
            typeof(Functions.LaunchApp),
        };

        public virtual Task InstallOrUpdateAsync(InstallOrUpdate installOrUpdate)
        {
            return Task.CompletedTask;
        }

        public virtual async Task UninstallAsync(Func<Task> unsinstallFunc)
        {
            await unsinstallFunc().ConfigureAwait(false);
        }

        #endregion
    }
}

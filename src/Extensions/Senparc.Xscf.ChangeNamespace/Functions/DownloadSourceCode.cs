﻿using Senparc.Scf.XscfBase;
using Senparc.Scf.XscfBase.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace Senparc.Xscf.ChangeNamespace.Functions
{

    public class DownloadSourceCode_Parameters : IFunctionParameter
    {
        /// <summary>
        /// 提供选项
        /// <para>注意：string[]类型的默认值为选项的备选值，如果没有提供备选值，此参数将别忽略</para>
        /// </summary>
        [Required]
        [Description("源码来源||目前更新最快的是 GitHub，Gitee（码云）在国内下载速度更快，但是不能确定是最新代码，请注意核对。")]
        public string[] Site { get; set; } = new[] {
            Parameters_Site.GitHub.ToString(),
            Parameters_Site.Gitee.ToString()
        };

        public enum Parameters_Site
        {
            GitHub,
            Gitee
        }
    }

    public class DownloadSourceCode : FunctionBase
    {
        //注意：Name 必须在单个 Xscf 模块中唯一！
        public override string Name => "下载官方 SCF 源码";

        public override string Description => "修改所有源码在 .cs, .cshtml 中的命名空间";

        public override Type FunctionParameterType => typeof(ChangeNamespace_Parameters);

        public DownloadSourceCode(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public override string Run(IFunctionParameter param)
        {
            /* 这里是处理文字选项（单选）的一个示例 */
            var typeParam = param as DownloadSourceCode_Parameters;

            if (Enum.TryParse<DownloadSourceCode_Parameters.Parameters_Site>(typeParam.Site.FirstOrDefault()/*单选可以这样做，如果是多选需要遍历*/, out var siteType))
            {
                switch (siteType)
                {
                    case DownloadSourceCode_Parameters.Parameters_Site.GitHub:
                        return "https://github.com/SenparcCoreFramework/SCF/archive/master.zip";
                    case DownloadSourceCode_Parameters.Parameters_Site.Gitee:
                        return "https://gitee.com/SenparcCoreFramework/SCF/repository/archive/master.zip";
                    default:
                        return "未知的下载地址";
                }
            }
            else
            {
                return "未知的下载参数";
            }
        }
    }
}

﻿using Senparc.Scf.XscfBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Senparc.Xscf.ChangeNamespace.Functions
{
    public class ChangeNameSpace : IXscfFunction
    {
        public IList<FunctionParam> FunctionParams
            => new List<FunctionParam>() {
                new FunctionParam("路径","本地物理路径，如：E:\\Senparc\\Scf\\", TypeCode.String),
                new FunctionParam("新命名空间","命名空间根，必须以.结尾，用于替换[Senparc.Scf.]", TypeCode.String)
                };


        public ChangeNameSpace() { }

        public string Run(params object[] param)
        {
            StringBuilder sb = new StringBuilder();

            var path = param[0] as string;
            var newNamespace = param[1] as string;

            var meetRules = new List<MeetRule>() {
                new MeetRule("namespace Senparc.Scf.",$"namespace {newNamespace}","*.cs"),
                new MeetRule("@model Senparc.Scf.",$"@model {newNamespace}","*.cshtml"),
            };

            foreach (var item in meetRules)
            {
                var files = Directory.GetFiles(path, item.FileType);
                foreach (var file in files)
                {
                    string content = null;
                    using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        using (var sr = new StreamReader(fs))
                        {
                            content = sr.ReadToEnd();
                        }
                        fs.Close();
                    }

                    if (content.IndexOf(item.OrignalKeyword) >= 0)
                    {
                        content = content.Replace(item.OrignalKeyword, item.ReplaceWord);
                        using (var fs = new FileStream(file, FileMode.Truncate, FileAccess.Read))
                        {
                            using (var sw = new StreamWriter(fs))
                            {
                                sw.Write(content);
                            }
                            fs.Flush();
                            fs.Close();
                        }
                    }
                }

            }

            return sb.ToString();//TODO:统一变成日志记录
        }
    }
}

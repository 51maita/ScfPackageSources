﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Senparc.Scf.Core.Models
{
    /// <summary>
    /// 所有 EntityFramework 中 Entitey 的 SetKey 的集合
    /// </summary>
    public static class EntitySetKeys
    {
        private static EntitySetKeysDictionary AllKeys = new EntitySetKeysDictionary();

        internal static List<Type> DbContextStore { get; set; } = new List<Type>();

        internal static object DbContextStoreLock = new object();

        /// <summary>
        /// 获取 Entity SetKey 集合
        /// </summary>
        /// <param name="tryLoadDbContextType">DbContext 类型</param>
        /// <param name="includeOtherEntityKeys">是否包含所有 DbContext 的 SetKeys</param>
        /// <returns></returns>
        public static EntitySetKeysDictionary GetEntitySetKeys(Type tryLoadDbContextType, bool includeOtherEntityKeys = false)
        {
            var setKeysDic = new EntitySetKeysDictionary();//当前类型内包含的 SetKeys
            setKeysDic.GetKeys(tryLoadDbContextType);
            foreach (var setKey in setKeysDic)
            {
                AllKeys[setKey.Key] = setKey.Value;//添加到全局的序列中
            }
            if (includeOtherEntityKeys)
            {
                return AllKeys;
            }
            return setKeysDic;
        }
    }
    /// <summary>
    /// 与ORM实体类对应的实体集
    /// </summary>
    public class EntitySetKeysDictionary : ConcurrentDictionary<Type, string>
    {
        public EntitySetKeysDictionary GetKeys(Type tryLoadDbContextType)
        {
            //if (tryLoadDbContextType)
            //{
            //}
            //TODO:判断必须是是DbContext类型

            lock (EntitySetKeys.DbContextStoreLock)
            {
                if (!tryLoadDbContextType.IsSubclassOf(typeof(DbContext)))
                {
                    throw new ArgumentException($"{nameof(tryLoadDbContextType)}不是 DbContext 的子类！", nameof(tryLoadDbContextType));
                }

                if (EntitySetKeys.DbContextStore.Contains(tryLoadDbContextType))
                {
                    return this;
                }
                EntitySetKeys.DbContextStore.Add(tryLoadDbContextType);


                //初始化的时候从ORM中自动读取实体集名称及实体类别名称
                var clientProperties = tryLoadDbContextType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

                var properities = new List<PropertyInfo>();
                properities.AddRange(clientProperties);

                foreach (var prop in properities)
                {
                    try
                    {
                        //ObjectQuery，ObjectSet for EF4，DbSet for EF Code First
                        if (prop.PropertyType.Name.IndexOf("DbSet") != -1 && prop.PropertyType.GetGenericArguments().Length > 0)
                        {
                            this[prop.PropertyType.GetGenericArguments()[0]] = prop.Name;//获取第一个泛型
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return this;
        }

        public new string this[Type entityType]
        {
            get
            {
                if (!base.ContainsKey(entityType))
                {
                    throw new Exception($"未找到实体类型：{entityType.FullName}");
                }
                return base[entityType];
            }
            set
            {
                base[entityType] = value;
            }
        }
    }
}
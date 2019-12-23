using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Xin.Core.Attributes;

namespace Xin.AspNetCore.Ioc
{
    public static class ServiceRegister
    {
        /// <summary>
        /// 反射注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        internal static IServiceCollection AddAssemblyServices(IServiceCollection services, Assembly assembly)
        {
            var typeList = new List<Type>(); //所有符合注册条件的类集合

            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsGenericType //排除了泛型类
                                      && (t.GetCustomAttributes(typeof(DefaultDependencyInjectionAttribute), true)
                                          .Any())
                ).ToList();

            typeList.AddRange(types);
            if (!typeList.Any()) return services;

            var typeDic = new Dictionary<Type, Type[]>(); //待注册集合<class,interface>
            foreach (var type in typeList)
            {
                var interfaces = type.GetInterfaces(); //获取接口
                typeDic.Add(type, interfaces);
            }

            //循环实现类
            foreach (var instanceType in typeDic.Keys)
            {
                var diAttr = instanceType.GetCustomAttributes(typeof(DefaultDependencyInjectionAttribute), true)
                        .LastOrDefault()
                    as DefaultDependencyInjectionAttribute;

                if (diAttr == null) continue;

                var serviceLifetime = ServiceLifetime.Scoped;

                switch (diAttr.DependencyInjectionType)
                {
                    case DefaultDependencyInjectionType.Transient:
                        serviceLifetime = ServiceLifetime.Transient;
                        break;
                    case DefaultDependencyInjectionType.Scoped:
                        serviceLifetime = ServiceLifetime.Scoped;
                        break;
                    case DefaultDependencyInjectionType.Singleton:
                        serviceLifetime = ServiceLifetime.Singleton;
                        break;
                }

                var interfaceTypeList = typeDic[instanceType];

                if (interfaceTypeList == null || interfaceTypeList.Length == 0 && !instanceType.IsAbstract
                ) //如果该类没有实现接口，则以其本身类型注册
                {
                    AddServiceWithLifeScoped(services, null, instanceType, serviceLifetime);
                }
                else //如果该类有实现接口，则循环其实现的接口，一一配对注册
                {
                    foreach (var interfaceType in interfaceTypeList)
                    {
                        AddServiceWithLifeScoped(services, interfaceType, instanceType, serviceLifetime);
                    }
                }
            }

            return services;
        }

        /// <summary>
        /// 暴露类型可空注册
        /// （如果暴露类型为null，则自动以其本身类型注册）
        /// </summary>
        /// <param name="services"></param>
        /// <param name="interfaceType"></param>
        /// <param name="instanceType"></param>
        /// <param name="serviceLifetime"></param>
        internal static void AddServiceWithLifeScoped(IServiceCollection services, Type interfaceType,
            Type instanceType, ServiceLifetime serviceLifetime)
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Scoped:
                    if (interfaceType == null) services.AddScoped(instanceType);
                    else services.AddScoped(interfaceType, instanceType);
                    break;
                case ServiceLifetime.Singleton:
                    if (interfaceType == null) services.AddSingleton(instanceType);
                    else services.AddSingleton(interfaceType, instanceType);
                    break;
                case ServiceLifetime.Transient:
                    if (interfaceType == null) services.AddTransient(instanceType);
                    else services.AddTransient(interfaceType, instanceType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceLifetime), serviceLifetime, null);
            }
        }

        internal static Assembly[] GetAllAssemblies()
        {
            //获取程序启动入口程序集
            var assembly = Assembly.GetEntryAssembly();
            var assemblies = assembly?.GetReferencedAssemblies().Select(Assembly.Load).ToList();
            assemblies?.Add(assembly);
            return assemblies?.ToArray();
        }
    }
}

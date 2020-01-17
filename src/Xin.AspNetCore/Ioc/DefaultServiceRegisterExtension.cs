using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Xin.AspNetCore.Ioc
{
    public static class DefaultServiceRegisterExtension
    {
        /// <summary>
        /// 自定义服务注册
        /// </summary>
        /// <param name="services"></param>
        public static void AddCustomServices(this IServiceCollection services)
        {
            //注册Service
            var asses = DefaultServiceRegister.GetAllAssemblies();
            if (asses == null) return;

            foreach (var ass in asses)
            {
                DefaultServiceRegister.AddAssemblyServices(services, ass);
            }
        }
    }
}

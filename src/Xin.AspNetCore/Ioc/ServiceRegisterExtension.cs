using Microsoft.Extensions.DependencyInjection;

namespace Xin.AspNetCore.Ioc
{
    public static class ServiceRegisterExtension
    {
        /// <summary>
        /// 自定义服务注册
        /// </summary>
        /// <param name="services"></param>
        public static void AddCustomServices(this IServiceCollection services)
        {
            //注册Service
            var asses = ServiceRegister.GetAllAssemblies();
            if (asses == null) return;

            foreach (var ass in asses)
            {
                ServiceRegister.AddAssemblyServices(services, ass);
            }
        }
    }
}

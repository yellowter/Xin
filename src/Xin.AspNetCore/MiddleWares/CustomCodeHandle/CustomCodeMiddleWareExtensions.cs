using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Xin.AspNetCore.MiddleWares.CustomCodeHandle
{
    /// <summary>
    /// 
    /// </summary>
    public static class CustomCodeMiddleWareExtensions
    {
        /// <summary>
        /// 自定义code处理
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomCodeHandle(this IApplicationBuilder app,
            Action<CustomHandleOption> options = null)
        {
            var option = new CustomHandleOption
            {
                HandleType = CustomHandleType.Both,
                HandleCodes = new Dictionary<int, PathString>
                {
                    {400, null},
                    {401, null},
                    {403, null},
                    {404, null},
                    {500, null}
                },
                HandleCodeUrls = new PathString[] {"/api"}
            };
            options?.Invoke(option);
            return app.UseMiddleware<CustomCodeHandleMiddleWare>(option);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Xin.AspNetCore.MiddleWares.CustomCodeHandle
{
    /// <summary>
    /// 自定义异常中间件配置对象
    /// </summary>
    public class CustomHandleOption
    {
        /// <summary>
        /// 
        /// </summary>
        public CustomHandleOption(
            CustomHandleType handleType = CustomHandleType.HttpCodeHandle,
            IList<PathString> handleCodeUrls = null,
            Dictionary<int, PathString> handleCodes = null,
            Action<HttpContext, Exception> onHandle = null)
        {
            HandleType = handleType;
            HandleCodeUrls = handleCodeUrls;
            HandleCodes = handleCodes;
            OnHandle = onHandle;
        }

        /// <summary>
        /// 处理方式
        /// </summary>
        public CustomHandleType HandleType { get; set; }

        /// <summary>
        /// code处理方式的Url关键字
        /// <para>仅HandleType=Both时生效</para>
        /// </summary>
        public IList<PathString> HandleCodeUrls { get; set; }

        /// <summary>
        /// 需要处理的code和错误处理跳转url
        /// </summary>
        public Dictionary<int, PathString> HandleCodes { get; set; }

        /// <summary>
        /// 处理回调
        /// </summary>
        public Action<HttpContext, Exception> OnHandle { get; set; }
    }
}

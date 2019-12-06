using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Xin.AspNetCore.MiddleWares.CustomCodeHandle
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomCodeHandleMiddleWare
    {
        /// <summary>
        /// 管道请求委托
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// 配置对象
        /// </summary>
        private readonly CustomHandleOption _option;

        /// <summary>
        /// 需要处理的状态码字典
        /// </summary>
        private readonly IDictionary<int, PathString> _statusCodeDic;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="option"></param>
        public CustomCodeHandleMiddleWare(RequestDelegate next, CustomHandleOption option)
        {
            _next = next;
            _option = option;

            if (_option.HandleCodes != null)
            {
                _statusCodeDic = _option.HandleCodes;
            }
            else
            {
                _statusCodeDic = new Dictionary<int, PathString>
                {
                    {401, null},
                    {401, null},
                    {404, null},
                    {403, null},
                    {500, null}
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            Exception exception = null;
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.Clear();
                context.Response.StatusCode = 500; //发生未捕获的异常，手动设置状态码
                exception = ex;
            }
            finally
            {
                if (_statusCodeDic.ContainsKey(context.Response.StatusCode) &&
                    !context.Items.ContainsKey("ExceptionHandled")) //预处理标记
                {
                    var errorMsg = string.Empty;
                    if (context.Response.StatusCode == 500 && exception != null)
                    {
                        errorMsg = exception.Message;
                    }

                    _option.OnHandle?.Invoke(context, exception);
                    exception = new Exception(errorMsg);
                }

                if (exception != null)
                {
                    var handleType = _option.HandleType;
                    if (handleType == CustomHandleType.Both) //根据Url关键字决定异常处理方式
                    {
                        handleType = _option.HandleCodeUrls != null && _option.HandleCodeUrls.Count(
                                         k => context.Request.Path.StartsWithSegments(k,
                                             StringComparison.CurrentCultureIgnoreCase)) > 0
                            ? CustomHandleType.HttpCodeHandle
                            : CustomHandleType.PageHandle;
                    }

                    if (handleType == CustomHandleType.HttpCodeHandle)
                    {
                    }
                    else
                    {
                        if (_statusCodeDic.TryGetValue(context.Response.StatusCode, out var path))
                        {
                            await PageHandle(context, exception, path);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 处理方式：跳转网页
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        private async Task PageHandle(HttpContext context, Exception ex, PathString path)
        {
            context.Items.Add("Exception", ex);
            var originPath = context.Request.Path;
            if (path == null || !path.HasValue)
            {
                //await context.Response.WriteAsync("");
                return;
            }

            //  context.Response.StatusCode = 200;
            context.Request.Path = path; //设置请求页面为错误跳转页面
            try
            {
                await _next(context);
            }
            finally
            {
                context.Request.Path = originPath; //恢复原始请求页面
            }
        }
    }
}

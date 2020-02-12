using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Xin.AspNetCore.Filters.RequestLock
{
    public class RequestLockFilter : Attribute, IAsyncActionFilter
    {
        private readonly Type _action;

        public RequestLockFilter(Type action)
        {
            if (action.GetInterface(nameof(ILockAction)) == null)
            {
                throw new ArgumentException("Type must be ILockAction");
            }

            _action = action;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!(context.HttpContext.RequestServices.GetServices(typeof(ILockAction))
                .FirstOrDefault(x => x.GetType() == _action) is ILockAction action))
            {
                throw new NullReferenceException("action is null");
            }

            if (await action.Lock(context))
            {
                await next();
                await action.Unlock(context);
            }
            else
            {
                await action.ErrorResult(context);
            }
        }
    }
}
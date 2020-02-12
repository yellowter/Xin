using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Xin.AspNetCore.Filters.RequestLock
{
    public interface ILockAction
    {
        Task<bool> Lock(ActionExecutingContext context);

        Task Unlock(ActionExecutingContext context);

        Task ErrorResult(ActionExecutingContext context);

    }
}
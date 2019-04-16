using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Sendeazy.Api.Repositories;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sendeazy.Api.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            var value = resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (value != null)
            {
                var userId = int.Parse(value);
                var repo = resultContext.HttpContext.RequestServices.GetService<IUserDao>();
                var user = await repo.GetById(userId);
                user.LastActive = DateTime.Now;

                await repo.UpdateUserLastActive(user);
            }


        }
    }
}

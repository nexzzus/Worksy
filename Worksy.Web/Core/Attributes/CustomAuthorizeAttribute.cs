using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Core.Attributes;

public class CustomAuthorizeAttribute : TypeFilterAttribute
{
    public CustomAuthorizeAttribute(string permission, string module) : base(typeof(CustomAuthorizeFilter))
    {
        Arguments = [module, permission];
    }
}

public class CustomAuthorizeFilter: IAsyncAuthorizationFilter
    {
        private readonly string _permission;
        private readonly string _module;
        private readonly IUserService  _userService;

        public CustomAuthorizeFilter(string module, string permission, IUserService userService)
        {
            _module = module;
            _permission = permission;
            _userService = userService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            bool userIsAuthenticated = _userService.CurrentUserIsAuthenticateded();
            if (!userIsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"Controller", "Account"},
                    {"Action", "Login"},
                    {"ReturnUrl", context.HttpContext.Request.Path}
                });
                return;
            }
            
            bool isAuthorized = await _userService.CurrentUserIsAuthorizedAsync(_permission, _module);
            if (!isAuthorized)
            {
                context.Result = new ForbidResult();
            }
        }
    }

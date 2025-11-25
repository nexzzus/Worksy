using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Worksy.Web.Services.Abstractions;

namespace Worksy.Web.Core.Attributes;

public class CustomRoleAuthorizeAttribute: TypeFilterAttribute
{
    public CustomRoleAuthorizeAttribute(params string[] roles) : base(typeof(RoleAuthorizeFilter))
    {
        Arguments = new object[] { roles };
    }
}

public class RoleAuthorizeFilter: IAsyncAuthorizationFilter
{
    private readonly string[] _requiredRoles;
    private readonly IUserService _userService;

    public RoleAuthorizeFilter(string[] requiredRoles, IUserService userService)
    {
        _requiredRoles = requiredRoles;
        _userService = userService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        bool isAuthenticated = _userService.CurrentUserIsAuthenticateded();
        if (!isAuthenticated)
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                {"Controller", "Account"},
                {"Action", "Login"},
                {"ReturnUrl", context.HttpContext.Request.Path}
            });
            return;
        }

        bool hasRole = await _userService.CurrentUserHasRoleAsync(_requiredRoles);
        if (!hasRole)
        {
            context.Result = new ForbidResult();
        }
    }
}


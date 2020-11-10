using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Services.Security;

namespace Nop.Web.Areas.VendorAdmin.Infrastructure
{
    public class AuthorizeVendorAdminAttribute : TypeFilterAttribute
    {
        public AuthorizeVendorAdminAttribute() 
            : base(typeof(AuthorizeVendorAdminFilter))
        {
            
        }
    }
    
    public class AuthorizeVendorAdminFilter: IAuthorizationFilter
    {
        private readonly IPermissionService _permissionService;

        public AuthorizeVendorAdminFilter(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (filterContext.Filters.Any(filter => filter is AuthorizeVendorAdminFilter))
            {
                //authorize permission of access to the admin area
                if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                    filterContext.Result = new ChallengeResult();
            }
        }
    }
}
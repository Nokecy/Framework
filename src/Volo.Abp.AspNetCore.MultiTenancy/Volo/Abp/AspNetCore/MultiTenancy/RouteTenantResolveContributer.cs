﻿using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.AspNetCore.MultiTenancy
{
    public class RouteTenantResolveContributer : HttpTenantResolveContributerBase
    {
        protected override string GetTenantIdOrNameFromHttpContextOrNull(ITenantResolveContext context, HttpContext httpContext)
        {
            var tenantId = httpContext.GetRouteValue(context.GetAspNetCoreMultiTenancyOptions().TenantKey);
            if (tenantId == null)
            {
                return null;
            }

            return Convert.ToString(tenantId);
        }
    }
}
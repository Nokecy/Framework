﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace Volo.Abp.Authorization
{
    public class MethodInvocationAuthorizationService : IMethodInvocationAuthorizationService, ITransientDependency
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ICurrentUser _currentUser;

        public MethodInvocationAuthorizationService(IAuthorizationService authorizationService, ICurrentUser currentUser)
        {
            _authorizationService = authorizationService;
            _currentUser = currentUser;
        }

        public async Task CheckAsync(MethodInvocationAuthorizationContext context)
        {
            if (AllowAnonymous(context))
            {
                return;
            }

            var authorizationAttributes = GetAuthorizationDataAttributes(context);
            foreach (var authorizationAttribute in authorizationAttributes)
            {
                await CheckAsync(authorizationAttribute);
            }
        }

        protected virtual bool AllowAnonymous(MethodInvocationAuthorizationContext context)
        {
            return context.Method.GetCustomAttributes(true).OfType<IAllowAnonymous>().Any();
        }

        protected virtual IAuthorizeData[] GetAuthorizationDataAttributes(MethodInvocationAuthorizationContext context)
        {
            var classAttributes = context.Method.DeclaringType
                .GetCustomAttributes(true)
                .OfType<IAuthorizeData>();

            var methodAttributes = context.Method
                .GetCustomAttributes(true)
                .OfType<IAuthorizeData>();

            return classAttributes.Union(methodAttributes).ToArray();
        }

        protected async Task CheckAsync(IAuthorizeData authorizationAttribute)
        {
            if (authorizationAttribute.Policy == null)
            {
                if (!_currentUser.IsAuthenticated) //TODO: What about API calls without user id?
                {
                    throw new AbpAuthorizationException("Authorization failed! User has not logged in.");
                }
            }
            else
            {
                await _authorizationService.CheckAsync(authorizationAttribute.Policy);
            }

            //TODO: What about roles and other props?
        }
    }
}
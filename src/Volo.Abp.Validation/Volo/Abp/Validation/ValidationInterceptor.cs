﻿using Volo.Abp.Aspects;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;

namespace Volo.Abp.Validation
{
    public class ValidationInterceptor : AbpInterceptor, ITransientDependency
    {
        private readonly IMethodInvocationValidator _validator;

        public ValidationInterceptor(IMethodInvocationValidator validator)
        {
            _validator = validator;
        }

        public override void Intercept(IAbpMethodInvocation invocation)
        {
            if (AbpCrossCuttingConcerns.IsApplied(invocation.TargetObject, AbpCrossCuttingConcerns.Validation))
            {
                invocation.Proceed();
                return;
            }

            Validate(invocation);

            invocation.Proceed();
        }

        protected virtual void Validate(IAbpMethodInvocation invocation)
        {
            _validator.Validate(
                new MethodInvocationValidationContext(
                    invocation.TargetObject,
                    invocation.Method,
                    invocation.Arguments
                )
            );
        }
    }
}

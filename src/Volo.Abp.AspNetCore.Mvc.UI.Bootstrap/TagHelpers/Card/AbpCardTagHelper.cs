﻿namespace Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Card
{
    public class AbpCardTagHelper : AbpTagHelper<AbpCardTagHelper, AbpCardTagHelperService>
    {
        public AbpCardTagHelper(AbpCardTagHelperService tagHelperService) 
            : base(tagHelperService)
        {
        }
    }
}

﻿using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Auditing;
using Volo.Abp.Json;

namespace Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations
{
    [Area("Abp")]
    [Route("Abp/ApplicationConfigurationScript")]
    [DisableAuditing]
    public class AbpApplicationConfigurationScriptController : AbpController
    {
        private readonly IApplicationConfigurationBuilder _configurationBuilder;
        private readonly IJsonSerializer _jsonSerializer;

        public AbpApplicationConfigurationScriptController(
            IApplicationConfigurationBuilder configurationBuilder,
            IJsonSerializer jsonSerializer)
        {
            _configurationBuilder = configurationBuilder;
            _jsonSerializer = jsonSerializer;
        }

        [HttpGet]
        [Produces("text/javascript", "text/plain")]
        public async Task<string> Get()
        {
            return CreateAbpExtendScript(
                await _configurationBuilder.GetAsync()
            );
        }

        private string CreateAbpExtendScript(ApplicationConfigurationDto config)
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine();
            script.AppendLine($"$.extend(true, abp, {_jsonSerializer.Serialize(config, indented: Debugger.IsAttached)})");
            script.AppendLine();
            script.AppendLine("abp.event.trigger('abp.configurationInitialized');");
            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }
}

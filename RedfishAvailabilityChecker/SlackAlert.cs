using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedfishAvailabilityChecker
{
    internal class SlackAlert
    {
        private readonly string glacierViewWebhook = "https://hooks.slack.com/services/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";
        public async Task<bool> SendSlackMessage(string message)
        {
            var chatResult = await glacierViewWebhook
                .WithHeader("Content-Type", "application/json")
                .PostJsonAsync(new { text = message });
            return chatResult.StatusCode == 200;
        }
    }
}

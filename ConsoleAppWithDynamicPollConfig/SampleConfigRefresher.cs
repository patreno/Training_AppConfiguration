using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppWithDynamicPollConfig
{
    class SampleConfigRefresher
    {
        private readonly IEnumerable<IConfigurationRefresher> _refreshers = null;

        public SampleConfigRefresher(IConfigurationRefresherProvider refresherProvider)
        {
            _refreshers = refresherProvider.Refreshers;
        }

        public async Task RefreshConfiguration()
        {
            foreach (var refresher in _refreshers)
            {
                _ = await refresher.TryRefreshAsync();
            }
        }
    }
}

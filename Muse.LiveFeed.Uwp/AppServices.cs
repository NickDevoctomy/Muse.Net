using Microsoft.Extensions.DependencyInjection;
using Muse.Net.Extensions;
using System;

namespace Muse.LiveFeed.Uwp
{
    public class AppServices
    {
        private AppServices()
        {
            var services = new ServiceCollection();

            services.AddMuseServices();

            ServiceProvider = services.BuildServiceProvider();
        }

        public IServiceProvider ServiceProvider { get; }

        private static AppServices _instance;
        private static readonly object _instanceLock = new object();
        private static AppServices GetInstance()
        {
            lock (_instanceLock)
            {
                return _instance ?? (_instance = new AppServices());
            }
        }

        public static AppServices Instance => _instance ?? GetInstance();
    }
}

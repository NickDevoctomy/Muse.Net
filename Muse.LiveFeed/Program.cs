using Microsoft.Extensions.DependencyInjection;
using Muse.Net.Client;
using Muse.Net.Extensions;
using Muse.Net.Services;
using System;
using System.Windows.Forms;

namespace Muse.LiveFeed
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var services = new ServiceCollection();
            ConfigureServices(services);
            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                var form1 = serviceProvider.GetRequiredService<frmMain>();
                Application.Run(form1);
            }
        }

        private static void ConfigureServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<frmMain>();
            serviceCollection.AddMuseServices();
        }
    }
}

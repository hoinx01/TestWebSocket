using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using SrvCornet.Core;
using SrvCornet.Background;

namespace Startup.Components
{
    public static class DependencyRegister
    {
        public static void Register(IServiceCollection services)
        {
            /*
             * Register dependency here
             */

            RegisterHostedServices(services);
        }

        private static void RegisterHostedServices(IServiceCollection services)
        {
            var hostedServices = AppSettings.Get<List<HostedServiceConfig>>("HostedServices");
            if (hostedServices == null)
                return;

            foreach (var hostedService in hostedServices)
            {
                if (!hostedService.Enable)
                    continue;

                var assembly = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.FullName.StartsWith(hostedService.Assembly))
                    .SingleOrDefault();

                if (assembly == null)
                    throw new Exception("Cannot find Assembly named by " + hostedService.Assembly);

                var type = assembly.GetType(hostedService.Name);
                services.AddSingleton(typeof(IHostedService), type);
            }

        }

    }
}

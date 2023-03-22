using AzureNotificationApp.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureNotificationApp
{
    /// <summary>
    /// The Begin method will be called by each platform when the app launches passing in a platform-specific implementation of IDeviceInstallationService.
    /// </summary>
    public static class Bootstrap
    {
        public static void Begin(Func<IDeviceInstallationService> deviceInstallationService)
        {
            ServiceContainer.Register(deviceInstallationService);

            ServiceContainer.Register<IAzureNotificationAppActionService>(()
                => new AzureNotificationAppActionService());

            ServiceContainer.Register<INotificationRegistrationService>(()
                => new NotificationRegistrationService(
                    Config.BackendServiceEndpoint,
                    Config.ApiKey));
        }
    }
}

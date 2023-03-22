using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureNotificationApp.Services
{
    /// <summary>
    /// This will handle the interaction between the client and backend service
    /// </summary>
    public interface INotificationRegistrationService
    {
        Task DeregisterDeviceAsync();
        Task RegisterDeviceAsync(params string[] tags);
        Task RefreshRegistrationAsync();
    }
}

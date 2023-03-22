using AzureNotificationApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureNotificationApp.Services
{
    /// <summary>
    /// This interface will be implemented and bootstrapped by each target later to provide the platform-specific functionality and DeviceInstallation information required by the backend service.
    /// </summary>
    public interface IDeviceInstallationService
    {
        string Token { get; set; }
        bool NotificationsSupported { get; }
        string GetDeviceId();
        DeviceInstallation GetDeviceInstallation(params string[] tags);
    }
}

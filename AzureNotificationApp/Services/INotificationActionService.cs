using System;
using System.Collections.Generic;
using System.Text;

namespace AzureNotificationApp.Services
{
    /// <summary>
    /// This is used as a simple mechanism to centralize the handling of notification actions
    /// </summary>
    public interface INotificationActionService
    {
        void TriggerAction(string action);
    }
}

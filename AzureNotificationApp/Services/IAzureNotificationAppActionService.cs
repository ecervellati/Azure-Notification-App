using AzureNotificationApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureNotificationApp.Services
{
    /// <summary>
    /// This type is specific to the AzureNotificationApp application and uses the PushDemoAction enumeration to identify the action that is being triggered in a strongly-typed manner.
    /// </summary>
    public interface IAzureNotificationAppActionService: INotificationActionService
    {
        event EventHandler<PushDemoAction> ActionTriggered;
    }
}

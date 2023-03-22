using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firebase.Messaging;
using AzureNotificationApp.Services;

namespace AzureNotificationApp.Droid.Services
{
    /// <summary>
    /// [Service(Exported = true)] è stato impostato a true causa errore "java.exe exited with code 1".
    /// Soluzione trovata al seguente link: https://stackoverflow.com/questions/71305191/xamarin-forms-msal-authentication-java-exe-exited-with-code-1/72204864#72204864
    /// </summary>
    [Service(Exported = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class PushNotificationFirebaseMessagingService : FirebaseMessagingService
    {
        IAzureNotificationAppActionService _notificationActionService;
        INotificationRegistrationService _notificationRegistrationService;
        IDeviceInstallationService _deviceInstallationService;

        IAzureNotificationAppActionService NotificationActionService
            => _notificationActionService ??
                (_notificationActionService =
                ServiceContainer.Resolve<IAzureNotificationAppActionService>());

        INotificationRegistrationService NotificationRegistrationService
            => _notificationRegistrationService ??
                (_notificationRegistrationService =
                ServiceContainer.Resolve<INotificationRegistrationService>());

        IDeviceInstallationService DeviceInstallationService
            => _deviceInstallationService ??
                (_deviceInstallationService =
                ServiceContainer.Resolve<IDeviceInstallationService>());

        public override void OnNewToken(string token)
        {
            DeviceInstallationService.Token = token;

            NotificationRegistrationService.RefreshRegistrationAsync()
                .ContinueWith((task) => { if (task.IsFaulted) throw task.Exception; });
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            if (message.Data.TryGetValue("action", out var messageAction))
                NotificationActionService.TriggerAction(messageAction);
        }
    }
}
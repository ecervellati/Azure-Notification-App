using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using WindowsAzure.Messaging.NotificationHubs;
using System.Diagnostics;
using System.Threading.Tasks;
using AzureNotificationApp.iOS.Extensions;
using AzureNotificationApp.iOS.Services;
using AzureNotificationApp.Services;
using UserNotifications;
using Xamarin.Essentials;

namespace AzureNotificationApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
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
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            Bootstrap.Begin(() => new DeviceInstallationService());

            // Request authorization and register for remote notifications
            if (DeviceInstallationService.NotificationsSupported)
            {
                UNUserNotificationCenter.Current.RequestAuthorization(
                        UNAuthorizationOptions.Alert |
                        UNAuthorizationOptions.Badge |
                        UNAuthorizationOptions.Sound,
                        (approvalGranted, error) =>
                        {
                            if (approvalGranted && error == null)
                                RegisterForRemoteNotifications();
                        });
            }

            LoadApplication(new App());

            using (var userInfo = options?.ObjectForKey(
               UIApplication.LaunchOptionsRemoteNotificationKey) as NSDictionary)
                ProcessNotificationActions(userInfo);

            //MSNotificationHub.SetDelegate(new AzureListener());
            //MSNotificationHub.Start(Constants.ListenConnectionString, Constants.NotificationHubName);

            return base.FinishedLaunching(app, options);
        }


        /// <summary>
        /// Method to register user notification settings and then for remote notifications with APNS.
        /// </summary>
        void RegisterForRemoteNotifications()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert |
                    UIUserNotificationType.Badge |
                    UIUserNotificationType.Sound,
                    new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            });
        }

        /// <summary>
        /// Method to set the IDeviceInstallationService.Token property value. 
        /// Refresh the registration and cache the device token if it has been updated since it was last stored.
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <returns></returns>
        Task CompleteRegistrationAsync(NSData deviceToken)
        {
            DeviceInstallationService.Token = deviceToken.ToHexString();
            return NotificationRegistrationService.RefreshRegistrationAsync();
        }

        /// <summary>
        /// Method for processing the NSDictionary notification data and conditionally calling NotificationActionService.TriggerAction
        /// </summary>
        /// <param name="userInfo"></param>
        void ProcessNotificationActions(NSDictionary userInfo)
        {
            if (userInfo == null)
                return;

            try
            {
                var actionValue = userInfo.ObjectForKey(new NSString("action")) as NSString;

                if (!string.IsNullOrWhiteSpace(actionValue?.Description))
                    NotificationActionService.TriggerAction(actionValue.Description);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Override the RegisteredForRemoteNotifications method passing the deviceToken argument to the CompleteRegistrationAsync method
        /// </summary>
        /// <param name="application"></param>
        /// <param name="deviceToken"></param>
        public override void RegisteredForRemoteNotifications(
                UIApplication application,
                NSData deviceToken)
                => CompleteRegistrationAsync(deviceToken).ContinueWith((task)
                    => { if (task.IsFaulted) throw task.Exception; });

        /// <summary>
        /// Override the ReceivedRemoteNotification method passing the userInfo argument to the ProcessNotificationActions method
        /// </summary>
        /// <param name="application"></param>
        /// <param name="userInfo"></param>
        public override void ReceivedRemoteNotification(
                UIApplication application,
                NSDictionary userInfo)
                => ProcessNotificationActions(userInfo);

        /// <summary>
        /// Method to log the error.
        /// This is very much a placeholder. You will want to implement proper logging and error handling for production scenarios
        /// </summary>
        /// <param name="application"></param>
        /// <param name="error"></param>
        public override void FailedToRegisterForRemoteNotifications(
                UIApplication application,
                NSError error)
                => Debug.WriteLine(error.Description);
    }
}

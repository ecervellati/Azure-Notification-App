using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using WindowsAzure.Messaging.NotificationHubs;
using Firebase.Iid;
using AzureNotificationApp.Droid.Services;
using AzureNotificationApp.Services;
using Android.Content;

namespace AzureNotificationApp.Droid
{
    [Activity(
    Label = "AzureNotificationApp.Android",
    LaunchMode = LaunchMode.SingleTop,
    Icon = "@mipmap/icon",
    Theme = "@style/MainTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, Android.Gms.Tasks.IOnSuccessListener
    {
        IAzureNotificationAppActionService _notificationActionService;
        IDeviceInstallationService _deviceInstallationService;

        IAzureNotificationAppActionService NotificationActionService
            => _notificationActionService ??
                (_notificationActionService =
                ServiceContainer.Resolve<IAzureNotificationAppActionService>());

        IDeviceInstallationService DeviceInstallationService
            => _deviceInstallationService ??
                (_deviceInstallationService =
                ServiceContainer.Resolve<IDeviceInstallationService>());

        // Implement the IOnSuccessListener method
        public void OnSuccess(Java.Lang.Object result)
        => DeviceInstallationService.Token =
            result.Class.GetMethod("getToken").Invoke(result).ToString();

        //Will check whether a given Intent has an extra value named action. Conditionally trigger that action using the IPushDemoNotificationActionService implementation.
        void ProcessNotificationActions(Intent intent)
        {
            try
            {
                if (intent?.HasExtra("action") == true)
                {
                    var action = intent.GetStringExtra("action");

                    if (!string.IsNullOrEmpty(action))
                        NotificationActionService.TriggerAction(action);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        // Override the OnNewIntent method to call ProcessNotificationActions method
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            ProcessNotificationActions(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //  Call Bootstrap.Begin right after the call to base.OnCreate passing in the platform-specific implementation of IDeviceInstallationService.
            Bootstrap.Begin(() => new DeviceInstallationService());

            if (DeviceInstallationService.NotificationsSupported)
            {
                FirebaseInstanceId.GetInstance(Firebase.FirebaseApp.Instance)
                    .GetInstanceId()
                    .AddOnSuccessListener(this);
            }
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new App());

            ProcessNotificationActions(Intent);
        }
    }
}
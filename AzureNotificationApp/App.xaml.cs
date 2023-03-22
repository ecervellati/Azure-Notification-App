using AzureNotificationApp.Models;
using AzureNotificationApp.Services;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AzureNotificationApp
{
    public partial class App : Application
    {
        /// <summary>
        /// This is simply to demonstrate the receipt and propagation of push notification actions. 
        /// Typically, these would be handled silently for example navigating to a specific view or refreshing some data rather than displaying an alert via the root Page, MainPage in this case.
        /// </summary>
        public App()
        {
            InitializeComponent();

            ServiceContainer.Resolve<IAzureNotificationAppActionService>()
        .ActionTriggered += NotificationActionTriggered;

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        void NotificationActionTriggered(object sender, PushDemoAction e)
    => ShowActionAlert(e);

        void ShowActionAlert(PushDemoAction action)
            => MainThread.BeginInvokeOnMainThread(()
                => MainPage?.DisplayAlert("PushDemo", $"{action} action received", "OK")
                    .ContinueWith((task) => { if (task.IsFaulted) throw task.Exception; }));
    }
}

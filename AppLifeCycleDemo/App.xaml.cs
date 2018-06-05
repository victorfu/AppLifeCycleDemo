using System;
using System.Diagnostics;
using AppLifeCycleDemo.Services;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.System;
using Windows.UI.Xaml;
using AppLifeCycleDemo.Helpers;

namespace AppLifeCycleDemo
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();

            Resuming += App_Resuming;
            Suspending += App_Suspending;
            EnteredBackground += App_EnteredBackground;
            LeavingBackground += App_LeavingBackground;

            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);

            Singleton<SuspendAndResumeService>.Instance.OnBackgroundEntering += App_OnBackgroundEntering;
        }

        private void App_OnBackgroundEntering(object sender, OnBackgroundEnteringEventArgs e)
        {
            Debug.WriteLine("App_OnBackgroundEntering");
        }

        private void App_Suspending(object sender, SuspendingEventArgs e)
        {
            Debug.WriteLine("App_Suspending");
        }

        private void App_Resuming(object sender, object e)
        {
            Debug.WriteLine("App_Resuming");
        }

        private void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            Debug.WriteLine("App_LeavingBackground");
        }

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            Debug.WriteLine("App_EnteredBackground");
            var deferral = e.GetDeferral();
            await Helpers.Singleton<SuspendAndResumeService>.Instance.SaveStateAsync();
            deferral.Complete();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            Debug.WriteLine("OnLaunched " + args.PreviousExecutionState);
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
            else
            {
                Debug.WriteLine("PrelaunchActivated");
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            Debug.WriteLine("OnActivated");
            await ActivationService.ActivateAsync(args);
        }

        private ActivationService CreateActivationService()
        {
            Debug.WriteLine("CreateActivationService");
            return new ActivationService(this, typeof(Views.MainPage));
        }

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            Debug.WriteLine("OnBackgroundActivated");
            await ActivationService.ActivateAsync(args);
        }

    }
}

using RealEstateApp.Services;
using RealEstateApp.Services.Repository;
using System.Linq;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RealEstateApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var container = TinyIoCContainer.Current;
            container.Register<IRepository, MockRepository>();

            if (Preferences.ContainsKey(nameof(GlobalSettings.SeaLevelPressure)))
                GlobalSettings.Instance.SeaLevelPressure = Preferences.Get(nameof(GlobalSettings.SeaLevelPressure), ( double )0);
            else
                Preferences.Set(nameof(GlobalSettings.SeaLevelPressure), 1021.5);

            if (Preferences.ContainsKey(nameof(GlobalSettings.UseGeolocationForBarometer)))
                GlobalSettings.Instance.UseGeolocationForBarometer = Preferences.Get(nameof(GlobalSettings.UseGeolocationForBarometer), true);
            else
                Preferences.Set(nameof(GlobalSettings.UseGeolocationForBarometer), true);

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            Battery.BatteryInfoChanged += Battery_BatteryInfoChanged;
        }

        private async void Battery_BatteryInfoChanged(object sender, BatteryInfoChangedEventArgs e)
        {
            if (Battery.ChargeLevel < .2)
            {
                var currentPage = Current.MainPage;

                var isCharingMessage = ((Battery.State == BatteryState.Charging) ? ("but you're charging so it's okay...") : (string.Empty));
                var powerModeMessage = ((string.IsNullOrWhiteSpace(isCharingMessage) && Battery.EnergySaverStatus == EnergySaverStatus.Off) ? ("and Power Saving mode is not turned on!") : (string.Empty));
                await currentPage.DisplayAlert("Warning", $"Battery is low {isCharingMessage}{powerModeMessage}", "OK");
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            Battery.BatteryInfoChanged -= Battery_BatteryInfoChanged;
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            Battery.BatteryInfoChanged += Battery_BatteryInfoChanged;
        }
    }
}

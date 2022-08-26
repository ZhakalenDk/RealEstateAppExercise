using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            BindingContext = this;

            SeaLevelSetting = GlobalSettings.Instance.SeaLevelPressure;
            UseGeolocationForBarometer = GlobalSettings.Instance.UseGeolocationForBarometer;
        }

        private double _seaLevelSetting;
        public double SeaLevelSetting
        {
            get => _seaLevelSetting;
            set
            {
                if (_seaLevelSetting != value)
                {
                    _seaLevelSetting = value;
                    Preferences.Set(nameof(GlobalSettings.SeaLevelPressure), value);
                    GlobalSettings.Instance.SeaLevelPressure = value;
                    OnPropertyChanged(nameof(_seaLevelSetting));
                }
            }
        }

        private bool _useGeolocationForBarometer;
        public bool UseGeolocationForBarometer
        {
            get => _useGeolocationForBarometer;
            set
            {
                if (_useGeolocationForBarometer != value)
                {
                    _useGeolocationForBarometer = value;
                    Preferences.Set(nameof(GlobalSettings.UseGeolocationForBarometer), value);
                    GlobalSettings.Instance.UseGeolocationForBarometer = value;
                    OnPropertyChanged(nameof(_useGeolocationForBarometer));
                }
            }
        }
    }
}
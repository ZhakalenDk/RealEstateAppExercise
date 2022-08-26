using RealEstateApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HeightCalculatorPage : ContentPage
    {
        public HeightCalculatorPage()
        {
            InitializeComponent();

            BindingContext = this;

            UseGeolocation = GlobalSettings.Instance.UseGeolocationForBarometer;
        }

        private double _currentPressure;
        public double CurrentPressure
        {
            get => _currentPressure;
            set
            {
                if (_currentPressure != value)
                {
                    _currentPressure = value;
                    OnPropertyChanged(nameof(_currentPressure));
                }
            }
        }

        private double _altitudeInMeters;
        public double AltitudeInMeters
        {
            get => _altitudeInMeters;
            set
            {
                if (_altitudeInMeters != value)
                {
                    _altitudeInMeters = value;
                    OnPropertyChanged(nameof(_altitudeInMeters));
                }
            }
        }

        private string _labelText;
        public string LabelText
        {
            get => _labelText;
            set
            {
                if (_labelText != value)
                {
                    _labelText = value;
                    OnPropertyChanged(nameof(_labelText));
                }
            }
        }

        public bool UseGeolocation { get; }

        public ObservableCollection<BarometerMeasurement> Measurements { get; set; } = new ObservableCollection<BarometerMeasurement>();

        protected override void OnAppearing()
        {
            Barometer.ReadingChanged += ReadData;
            Barometer.Start(SensorSpeed.UI);
        }

        protected override void OnDisappearing()
        {
            Barometer.ReadingChanged -= ReadData;
            Barometer.Stop();
        }

        private async void ReadData(object sender, BarometerChangedEventArgs e)
        {
            CurrentPressure = e.Reading.PressureInHectopascals;
            var backupAltitude = (44307.694 * (1 - Math.Pow(CurrentPressure / GlobalSettings.Instance.SeaLevelPressure, 0.190284)));  //  In case something goes wrng when collecting altitude

            if (UseGeolocation)
            {
                try
                {
                    Location position = (await Geolocation.GetLocationAsync());
                    AltitudeInMeters = ((position.Altitude.HasValue && position.Altitude.Value != 0) ? (position.Altitude.Value) : (backupAltitude));
                }
                catch (FeatureNotSupportedException)
                {
                    AltitudeInMeters = backupAltitude;
                }
                catch (FeatureNotEnabledException)
                {
                    AltitudeInMeters = backupAltitude;
                }

                return;
            }

            AltitudeInMeters = backupAltitude;
        }

        private void SaveData(object sender, EventArgs e)
        {
            BarometerMeasurement newEntry = new BarometerMeasurement
            {
                Altitude = AltitudeInMeters,
                HeightChange = 0,
                Pressure = CurrentPressure,
                Label = LabelText
            };

            if (Measurements.Count > 0)
            {
                var lastEntry = Measurements[Measurements.Count - 1];   //  Getting the last indexed entry

                var difference = AltitudeInMeters - lastEntry.Altitude;

                newEntry.HeightChange = ((difference != 0) ? (difference) : (0));
            }

            LabelText = string.Empty;
            Measurements.Add(newEntry);
        }
    }
}
using RealEstateApp.Models;
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
    public partial class CompassPage : ContentPage
    {
        public CompassPage(Property property)
        {
            InitializeComponent();

            BindingContext = this;

            _property = property;
        }

        private const double ANGLE_OFFSET = 45;

        private Property _property;
        private string _currentAspect;
        public string CurrentAspect
        {
            get => _currentAspect;
            set
            {
                if (_currentAspect != value)
                {
                    _currentAspect = value;
                    OnPropertyChanged(nameof(_currentAspect));
                }
            }
        }
        private double _rotationAngle;
        public double RotationAngle
        {
            get => _rotationAngle;
            set
            {
                if (_rotationAngle != value)
                {
                    _rotationAngle = value;
                    OnPropertyChanged(nameof(_rotationAngle));
                }
            }
        }
        private double _currentHeading;
        public double CurrentHeading
        {
            get => _currentHeading;
            set
            {
                if (_currentHeading != value)
                {
                    _currentHeading = value;
                    OnPropertyChanged(nameof(_currentHeading));
                }
            }
        }

        protected override void OnAppearing()
        {
            Compass.ReadingChanged += GetCompassReading;

            try
            {
                Compass.Start(SensorSpeed.UI);
            }
            catch (FeatureNotSupportedException)
            {
                // Ignore for now
            }
            catch (Exception)
            {
                // Ignore for now
            }
        }

        protected override void OnDisappearing()
        {
            if (Compass.IsMonitoring)
            {
                Compass.Stop();
            }
            Compass.ReadingChanged -= GetCompassReading;

        }

        private void GetCompassReading(object sender, CompassChangedEventArgs e)
        {
            CurrentHeading = e.Reading.HeadingMagneticNorth;

            RotationAngle = CurrentHeading * -1;

            if (CurrentHeading < ANGLE_OFFSET || CurrentHeading > GetAngleRange(Direction.North, lowerRange: false))
                CurrentAspect = Direction.North.ToString();
            else if (CurrentHeading < GetAngleRange(Direction.East) && CurrentHeading > GetAngleRange(Direction.East, lowerRange: false))
                CurrentAspect = Direction.East.ToString();
            else if (CurrentHeading < GetAngleRange(Direction.South) && CurrentHeading > GetAngleRange(Direction.South, lowerRange: false))
                CurrentAspect = Direction.South.ToString();
            else if (CurrentHeading < GetAngleRange(Direction.West) && CurrentHeading > GetAngleRange(Direction.West, lowerRange: false))
                CurrentAspect = Direction.West.ToString();
            else
                CurrentAspect = "???";
        }

        private double GetAngleRange(Direction dir, bool lowerRange = true)
        {
            if (lowerRange)
            {
                return ( double )dir + ANGLE_OFFSET;
            }

            return ( double )dir - ANGLE_OFFSET;
        }
        private void SaveAspect(object sender, EventArgs e)
        {
            _property.Aspect = CurrentAspect;

            Navigation.PopModalAsync();
        }
    }

    public enum Direction
    {
        North = 360,
        East = 90,
        South = 180,
        West = 270
    }
}
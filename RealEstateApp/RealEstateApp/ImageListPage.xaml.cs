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
    public partial class ImageListPage : ContentPage
    {
        public ImageListPage(Property property)
        {
            InitializeComponent();

            BindingContext = this;

            Property = property;
        }

        public Property Property { get; set; }

        private int _position;
        public int Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged(nameof(_position));
                }
            }
        }

        protected override void OnAppearing()
        {
            Accelerometer.ShakeDetected += NextOnShake;
            Accelerometer.Start(SensorSpeed.UI);
        }

        protected override void OnDisappearing()
        {
            Accelerometer.ShakeDetected -= NextOnShake;
            Accelerometer.Stop();
        }

        private void NextOnShake(object sender, EventArgs e)
        {
            var current = Position;

            var next = ++current % 3;

            Position = next;
        }
    }
}
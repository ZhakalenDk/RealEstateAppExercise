using RealEstateApp.Models;
using RealEstateApp.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditPropertyPage : ContentPage
    {
        private IRepository Repository;

        #region PROPERTIES
        public ObservableCollection<Agent> Agents { get; }

        private Property _property;
        public Property Property
        {
            get => _property;
            set
            {
                _property = value;
                if (_property.AgentId != null)
                {
                    SelectedAgent = Agents.FirstOrDefault(x => x.Id == _property?.AgentId);
                }
            }
        }

        private bool _hasConnection = true;
        public bool HasConnection
        {
            get => _hasConnection;
            set
            {
                if (_hasConnection != value)
                {
                    _hasConnection = value;
                    OnPropertyChanged(nameof(_hasConnection));
                }
            }
        }

        private Agent _selectedAgent;

        public Agent SelectedAgent
        {
            get => _selectedAgent;
            set
            {
                if (Property != null)
                {
                    _selectedAgent = value;
                    Property.AgentId = _selectedAgent?.Id;
                }
            }
        }

        public string StatusMessage { get; set; }

        public Color StatusColor { get; set; } = Color.White;
        #endregion

        public AddEditPropertyPage(Property property = null)
        {
            InitializeComponent();

            Repository = TinyIoCContainer.Current.Resolve<IRepository>();
            Agents = new ObservableCollection<Agent>(Repository.GetAgents());

            if (property == null)
            {
                Title = "Add Property";
                Property = new Property();
            }
            else
            {
                Title = "Edit Property";
                Property = property;
            }

            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            Connectivity.ConnectivityChanged += CheckConnection;
        }

        protected override void OnDisappearing()
        {
            Connectivity.ConnectivityChanged -= CheckConnection;
        }

        private void CheckConnection(object sender, ConnectivityChangedEventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                DisplayAlert("No Connection", "Geolocation is turned off", "OK");
                HasConnection = false;
            }
            else
            {
                HasConnection = true;
            }
        }

        private async void SaveProperty_Clicked(object sender, System.EventArgs e)
        {
            if (IsValid() == false)
            {
                StatusMessage = "Please fill in all required fields";
                StatusColor = Color.Red;
            }
            else
            {
                Repository.SaveProperty(Property);
                await Navigation.PopToRootAsync();
            }
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Property.Address)
                || Property.Beds == null
                || Property.Price == null
                || Property.AgentId == null)
                return false;

            return true;
        }

        private async void CancelSave_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private async void AutoFillAddressAsync(object sender, System.EventArgs e)
        {
            try
            {
                var location = await Geolocation.GetLocationAsync();
                Property.Longitude = location.Longitude;
                Property.Latitude = location.Latitude;

                var currentLocation = new Location(Property.Latitude.Value, Property.Longitude.Value);
                var addressInfo = (await Geocoding.GetPlacemarksAsync(currentLocation))
                    .FirstOrDefault();
                Property.Address = $"{addressInfo.Thoroughfare} {addressInfo.SubThoroughfare}, {addressInfo.PostalCode} {addressInfo.Locality}";
            }
            catch (FeatureNotSupportedException)
            {
                //  Ignore for now
            }
            catch (FeatureNotEnabledException)
            {
                //  Ignore for now
            }
            catch (PermissionException)
            {
                //  Ignore for now
            }
        }

        private async void AutoFillGeolocationAsync(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Property.Address)) { await DisplayAlert("Whoops", "Seems like you forgot to type an address", "I'll fill it"); return; }

            Location location = null;
            try
            {
                location = (await Geocoding.GetLocationsAsync(Property.Address))
               .FirstOrDefault();
            }
            catch (FeatureNotSupportedException)
            {
                //  Ignore for now
            }
            catch (FeatureNotEnabledException)
            {
                //  Ignore for now
            }


            if (location == null) { await DisplayAlert("Whoops", "Couldn't find the address. Make sure you typed the address correctly", "I'll check"); return; }

            Property.Latitude = location.Latitude;
            Property.Longitude = location.Longitude;
        }
    }
}
using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
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
    public partial class PropertyListPage : ContentPage
    {
        IRepository Repository;
        public ObservableCollection<PropertyListItem> PropertiesCollection { get; private set; } = new ObservableCollection<PropertyListItem>();

        public PropertyListPage()
        {
            InitializeComponent();

            Repository = TinyIoCContainer.Current.Resolve<IRepository>();
            LoadProperties();
            BindingContext = this;
        }

        private Location _lastKnownLocation;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadProperties();
        }

        void OnRefresh(object sender, EventArgs e)
        {
            var list = ( ListView )sender;
            LoadProperties();
            list.IsRefreshing = false;
        }

        void LoadProperties()
        {
            PropertiesCollection.Clear();
            var items = Repository.GetProperties();

            foreach (Property item in items)
            {
                var propertyItem = new PropertyListItem(item);
                if (item.Latitude.HasValue && item.Longitude.HasValue && _lastKnownLocation != null)
                {
                    var itemLoc = new Location(item.Latitude.Value, item.Longitude.Value);
                    var distance = Location.CalculateDistance(_lastKnownLocation, itemLoc, DistanceUnits.Kilometers);
                    propertyItem.Distance = distance;
                }

                PropertiesCollection.Add(propertyItem);
            }
        }

        private async void ItemsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new PropertyDetailPage(e.Item as PropertyListItem));
        }

        private async void AddProperty_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditPropertyPage());
        }

        private async void ShowDistanceAsync(object sender, EventArgs e)
        {
            await SortAsync();
            LoadProperties();
        }

        private async Task SortAsync()
        {
            _lastKnownLocation = await Geolocation.GetLastKnownLocationAsync() ?? await Geolocation.GetLocationAsync();

            PropertiesCollection = new ObservableCollection<PropertyListItem>(PropertiesCollection.OrderBy(pl => pl.Distance));
        }
    }
}
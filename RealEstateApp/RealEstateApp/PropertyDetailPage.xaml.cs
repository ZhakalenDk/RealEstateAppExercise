using RealEstateApp.Models;
using RealEstateApp.Services;
using System.Linq;
using System.Threading;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PropertyDetailPage : ContentPage
    {
        public PropertyDetailPage(PropertyListItem propertyListItem)
        {
            InitializeComponent();

            Property = propertyListItem.Property;

            IRepository Repository = TinyIoCContainer.Current.Resolve<IRepository>();
            Agent = Repository.GetAgents().FirstOrDefault(x => x.Id == Property.AgentId);

            BindingContext = this;
        }

        public Agent Agent { get; set; }

        public Property Property { get; set; }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    OnPropertyChanged(nameof(_isPlaying));
                }
            }
        }

        private CancellationTokenSource cSource = new CancellationTokenSource();

        private async void EditProperty_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new AddEditPropertyPage(Property));
        }

        private async void StartTTS(object sender, System.EventArgs e)
        {
            IsPlaying = true;
            await TextToSpeech.SpeakAsync(Property.Description, cSource.Token);
            IsPlaying = false;
        }

        private void StopTTS(object sender, System.EventArgs e)
        {
            cSource.Cancel();
            IsPlaying = false;
            cSource = new CancellationTokenSource();
        }
    }
}
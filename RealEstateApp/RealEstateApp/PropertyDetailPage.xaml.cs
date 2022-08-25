using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
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

        private CancellationTokenSource _cSource = new CancellationTokenSource();

        private async void EditProperty_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new AddEditPropertyPage(Property));
        }

        private async void StartTTS(object sender, System.EventArgs e)
        {
            IsPlaying = true;
            await TextToSpeech.SpeakAsync(Property.Description, _cSource.Token);
            IsPlaying = false;
        }

        private void StopTTS(object sender, System.EventArgs e)
        {
            _cSource.Cancel();
            IsPlaying = false;
            _cSource = new CancellationTokenSource();
        }

        private async void OnImageClick(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new ImageListPage(Property));
        }

        private async void SendMail(object sender, System.EventArgs e)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var attachmentFilePath = Path.Combine(folder, "property.txt");
            File.WriteAllText(attachmentFilePath, $"{Property.Address}");

            try
            {
                EmailMessage message = new EmailMessage
                {
                    To = new List<string> { Property.Vendor.Email },
                    Subject = $"Regarding: {Property.Address}",
                    Body = string.Empty
                };

                message.Attachments.Add(new EmailAttachment(attachmentFilePath));

                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Whoops", "Emails are not supported by your system", "OK");
            }
        }

        private async void OnPhone(object sender, System.EventArgs e)
        {
            var choice = await DisplayActionSheet(Property.Vendor.Phone, "Cancel", null, new[] { "Call", "SMS" });

            try
            {
                if (choice == "Call")
                    PhoneDialer.Open(Property.Vendor.Phone);
                else if (choice == "SMS")
                    await Sms.ComposeAsync(new SmsMessage { Recipients = new List<string> { Property.Vendor.Phone } });
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Whoops", "Feature is not supported by your system", "OK");
            }
        }
    }
}
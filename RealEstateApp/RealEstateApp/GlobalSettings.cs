namespace RealEstateApp
{
    public class GlobalSettings
    {
        public static GlobalSettings Instance { get; } = new GlobalSettings();

        public string ImageBaseUrl => "https://dbroadfootpluralsight.blob.core.windows.net/files/";
        public string NoImageUrl => ImageBaseUrl + "no_image.jpg";

        public double SeaLevelPressure { get; set; }
        public bool UseGeolocationForBarometer { get; set; }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;

namespace PayPro.Main.Local.Models
{
    public class SeriesValue : ObservableObject
    {
        private double _volume;

        public double Volume
        {
            get => _volume;
            set => SetProperty(ref _volume, value);
        }
    }
}

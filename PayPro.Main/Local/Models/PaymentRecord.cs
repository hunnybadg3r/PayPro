using CommunityToolkit.Mvvm.ComponentModel;

namespace PayPro.Main.Local.Models
{
    public class PaymentRecord : ObservableObject
    {
        private string _transactionId;
        private DateTime _timestamp;
        private string _fuelType;
        private decimal _amount;
        private double _liters;
        private string _paymentMethod;
        private string _status;
        private bool _isUpdated;

        public string TransactionId
        {
            get => _transactionId;
            set => SetProperty(ref _transactionId, value);
        }

        public DateTime Timestamp
        {
            get => _timestamp;
            set => SetProperty(ref _timestamp, value);
        }

        public string FuelType
        {
            get => _fuelType;
            set => SetProperty(ref _fuelType, value);
        }

        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public double Liters
        {
            get => _liters;
            set => SetProperty(ref _liters, value);
        }

        public string PaymentMethod
        {
            get => _paymentMethod;
            set => SetProperty(ref _paymentMethod, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public bool IsUpdated
        {
            get => _isUpdated;
            set => SetProperty(ref _isUpdated, value);
        }
    }
}

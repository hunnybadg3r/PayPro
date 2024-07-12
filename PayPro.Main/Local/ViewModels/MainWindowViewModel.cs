using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using System.Collections.ObjectModel;
using PayPro.Main.Local.Models;
using CommunityToolkit.Mvvm.Messaging;
using PayPro.Contracts.Messages;
using PayPro.Contracts.Models.Payment;
using System.Diagnostics;
using System;

namespace PayPro.Main.Local.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject, IRecipient<AddRecordMessage>
    {
        private static readonly Random _random = new();
        private readonly LinearGradientPaint _blueLinearGradientPaint = new LinearGradientPaint(SKColor.Parse("5783fc"), SKColors.White, new SKPoint(0.5f, 0),new SKPoint(0.5f, 1));
        private readonly LinearGradientPaint _redLinearGradientPaint = new LinearGradientPaint(SKColor.Parse("d70b5e"), SKColors.White, new SKPoint(0.5f, 0),new SKPoint(0.5f, 1));
        private readonly SolidColorPaint _redStroke = new SolidColorPaint(SKColor.Parse("d70b5e"), 3);

        public MainWindowViewModel()
        {
            WeakReferenceMessenger.Default.Register<AddRecordMessage>(this);

            GeneratePaymentRecordsSampleData();

            //GenerateChartSampleData
            //Fuel Type
            PieSeriesGasolineValues = new List<SeriesValue> { new() { Volume = 36 } };
            PieSeriesDieselValues = new List<SeriesValue> { new() { Volume = 28 } };

            //Payment Methods
            PieSeriesCashlValues = new List<SeriesValue> { new() { Volume = 2 } };
            PieSeriesCreditCardValues = new List<SeriesValue> { new() { Volume = 30 } };
            PieSeriesDigitalWalletValues = new List<SeriesValue> { new() { Volume = 30 } };

            SeriesFuelType =
            [
                new PieSeries<SeriesValue>
                {
                    Name="휘발유",
                    Values = PieSeriesGasolineValues,
                    DataLabelsPaint = new SolidColorPaint(SKColor.Parse("1A1A1A")),
                    DataLabelsPosition=LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsFormatter = point => point.PrimaryValue.ToString(),
                    DataLabelsSize = 14,
                    Fill = new SolidColorPaint(SKColor.Parse("FFC300")),
                    HoverPushout = 5,
                    OuterRadiusOffset = 10,
                    Mapping = (fuel, index) => new(index, fuel.Volume),
                },
                new PieSeries<SeriesValue>
                {
                    Name="경유",
                    Values = PieSeriesDieselValues,
                    DataLabelsPaint = new SolidColorPaint(SKColor.Parse("1A1A1A")),
                    DataLabelsPosition=LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsFormatter = point => point.PrimaryValue.ToString(),
                    DataLabelsSize = 14,
                    Fill = new SolidColorPaint(SKColor.Parse("009E60")),
                    HoverPushout = 5,
                    OuterRadiusOffset = 10,
                    Mapping = (fuel, index) => new(index, fuel.Volume),
                },
            ];

            SeriesPaymentMethods =
            [
                new PieSeries<SeriesValue>
                {
                    Name="카드",
                    Values = PieSeriesCreditCardValues,
                    DataLabelsPaint = new SolidColorPaint(SKColor.Parse("1A1A1A")),
                    DataLabelsPosition=LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsFormatter = point => point.PrimaryValue.ToString(),
                    DataLabelsSize = 14,
                    Fill = new SolidColorPaint(SKColor.Parse("A9D0FF")),
                    HoverPushout = 5,
                    OuterRadiusOffset = 10,
                    Mapping = (pay, index) => new(index, pay.Volume),
                },
                new PieSeries<SeriesValue>
                {
                    Name="간편",
                    Values = PieSeriesDigitalWalletValues,
                    DataLabelsPaint = new SolidColorPaint(SKColor.Parse("1A1A1A")),
                    DataLabelsPosition=LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsFormatter = point => point.PrimaryValue.ToString(),
                    DataLabelsSize = 14,
                    Fill = new SolidColorPaint(SKColor.Parse("C8A2E8")),
                    HoverPushout = 5,
                    OuterRadiusOffset = 10,
                    Mapping = (pay, index) => new(index, pay.Volume),
                },
                new PieSeries<SeriesValue>
                {
                    Name="현금",
                    Values = PieSeriesCashlValues,
                    DataLabelsPaint = new SolidColorPaint(SKColor.Parse("1A1A1A")),
                    DataLabelsPosition=LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsFormatter = point => point.PrimaryValue.ToString(),
                    DataLabelsSize = 14,
                    Fill = new SolidColorPaint(SKColor.Parse("FFA07A")),
                    HoverPushout = 5,
                    OuterRadiusOffset = 10,
                    Mapping = (pay, index) => new(index, pay.Volume),
                },
            ];

            //Generate Sample Metrics
            SeriesVisitor =
             [
                new LineSeries<double>
                {
                    Values = [123, 135, 134, 141, 126, 123, 122],
                    //Default Stroke 2196f3
                    Fill = new LinearGradientPaint(SKColor.Parse("5783fc"), SKColors.White, new SKPoint(0.5f, 0),new SKPoint(0.5f, 1)), 
                    GeometryFill = null,
                    GeometryStroke = null,
                    LineSmoothness = 1,
                }
             ];

            _totalRevenue = _random.Next(500, 531);
            _totalVisitor = _random.Next(65, 71);
            _totalRevenueYesterday = _random.Next(450, 500);
            _totalVisitorYesterday = _random.Next(71, 76);

            UpdateRevenueAndVisitorMetrics();
        }

        public LiveChartsCore.Measure.Margin Margin { get; set; } = new LiveChartsCore.Measure.Margin(0);

        public List<SeriesValue> PieSeriesGasolineValues;
        public List<SeriesValue> PieSeriesDieselValues;

        public List<SeriesValue> PieSeriesCashlValues;
        public List<SeriesValue> PieSeriesCreditCardValues;
        public List<SeriesValue> PieSeriesDigitalWalletValues;

        [ObservableProperty]
        private string _totalVisitorGrowthRate;

        [ObservableProperty]
        private string _totalRevenueGrowthRate;

        [ObservableProperty]
        private string _totalRevenueString;

        [ObservableProperty]
        private string _totalVisitorString;

        private int _totalRevenue;
        private int _totalVisitor;
        private int _totalRevenueYesterday;
        private int _totalVisitorYesterday;

        public ISeries[] SeriesRevenue { get; set; } =
        {
            new LineSeries<double>
            {
                Values = [520, 510, 530, 550, 530, 540, 545],
                Stroke = new SolidColorPaint(SKColor.Parse("d70b5e"), 3),
                Fill = new LinearGradientPaint(SKColor.Parse("d70b5e"), SKColors.White, new SKPoint(0.5f, 0),new SKPoint(0.5f, 1)), 
                GeometryFill = null,
                GeometryStroke = null,
                LineSmoothness = 1,
            }
        };

        public ObservableCollection<ISeries> SeriesVisitor { get; set; }

        public Axis[] XAxes { get; set; }
            =
            [
                new Axis
                {
                    SeparatorsPaint = new SolidColorPaint(SKColors.Empty) { StrokeThickness = 0 },
                    TextSize = 0,
                }
            ];

        public Axis[] YAxes { get; set; }
            =
            [
                new Axis
                {
                    SeparatorsPaint = new SolidColorPaint(SKColors.Empty) { StrokeThickness = 0 },
                    TextSize = 0,
                }
            ];

        public ObservableCollection<ISeries> SeriesFuelType { get; set; }
        public ObservableCollection<ISeries> SeriesPaymentMethods { get; set; }

        public ISeries[] SeriesSalesAnalytics { get; set; } =
        [
            new ColumnSeries<int>
            {
                Values = [231, 516, 339, 520, 527, 231],
                Fill = new SolidColorPaint(SKColor.Parse("2563eb")),
                Name = "올해",
                MaxBarWidth = 20,
                Stroke = null
            },
            new ColumnSeries<int>
            {
                Values = [287, 502, 395, 513, 506, 221],
                Fill = new SolidColorPaint(SKColor.Parse("bfdbfe")),
                Name = "작년",
                MaxBarWidth = 20,
                Stroke = null
            }
        ];

        public Axis[] XAxesSalesAnalytics { get; set; } =
        {
             new Axis
            {
                Labels = ["2월", "3월", "4월", "5월", "6월", "7월"],
                LabelsPaint = new SolidColorPaint(SKColor.Parse("95a3b8")),
                TextSize = 14,
            }
        };

        public Axis[] YAxesSalesAnalytics { get; set; } =
        {
            new Axis
            {
                LabelsPaint = new SolidColorPaint(SKColor.Parse("95a3b8")),
                TextSize = 14,
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("b4becd"))
                {
                    StrokeThickness = 2,
                    PathEffect = new DashEffect([10, 10])
                }
            }
        };

        public ObservableCollection<PaymentRecord> PaymentRecords { get; set; }

        private void UpdateRevenueAndVisitorMetrics()
        {
            TotalRevenueGrowthRate = CalculateGrowthRate(_totalRevenueYesterday, _totalRevenue);
            TotalVisitorGrowthRate = CalculateGrowthRate(_totalVisitorYesterday, _totalVisitor);

            TotalRevenueString = $"{_totalRevenue}만원";
            TotalVisitorString = $"{_totalVisitor}명";
        }

        private string CalculateGrowthRate(int yesterdayValue, int todayValue)
        {
            if (yesterdayValue == 0)
            {
                return "N/A"; // Avoid division by zero
            }

            double growthRate = ((double)(todayValue - yesterdayValue) / yesterdayValue) * 100;
            return $"{growthRate:+0;-0}%";
        }

        public void Receive(AddRecordMessage message)
        {
            if (message.Value.payRes == null)
            {
                var paymentRecord = new PaymentRecord()
                {
                    TransactionId = message.Value.packetReq.TransactionId,
                    Amount = message.Value.packetReq.Amount,
                    FuelType = message.Value.packetReq.FuelType == "GASOLINE" ? "휘발유" : message.Value.packetReq.FuelType == "DIESEL" ? "경유" : "",
                    Liters = message.Value.packetReq.Volume,
                    PaymentMethod = message.Value.packetReq.PaymentMethod == "CREDIT_CARD" ? "카드"
                                  : message.Value.packetReq.PaymentMethod == "DIGITAL_WALLET" ? "간편"
                                  : message.Value.packetReq.PaymentMethod == "CASH" ? "현금" : "",
                    Status = "대기",
                    Timestamp = message.Value.packetReq.Timestamp,
                };

                PaymentRecords.Add(paymentRecord);

                HighlightPaymentRecord(paymentRecord);
            }
            else
            {
                var paymentRecord = PaymentRecords.FirstOrDefault(r => r.TransactionId == message.Value.packetReq.TransactionId);

                Debug.WriteLine($"paymentRecord: {paymentRecord?.TransactionId} - {paymentRecord?.Status}");
                if (paymentRecord != null)
                {
                    paymentRecord.Status = message.Value.payRes.Result == PaymentResult.Success ? "승인" : "실패";
                    paymentRecord.Timestamp = message.Value.payRes.TransactionDateTime;

                    HighlightPaymentRecord(paymentRecord);

                    //update dashboard charts
                    switch (paymentRecord.FuelType)
                    {
                        case "휘발유":
                            PieSeriesGasolineValues[0].Volume++;
                            break;
                        case "경유":
                            PieSeriesDieselValues[0].Volume++;
                            break;
                    }

                    switch (paymentRecord.PaymentMethod)
                    {
                        case "카드":
                            PieSeriesCreditCardValues[0].Volume++;
                            break;
                        case "간편":
                            PieSeriesDigitalWalletValues[0].Volume++;
                            break;
                        case "현금":
                            PieSeriesCashlValues[0].Volume++;
                            break;
                    }

                    _totalRevenue += (int)Math.Round(paymentRecord.Amount / 10000m, MidpointRounding.AwayFromZero);
                    _totalVisitor ++;

                    UpdateRevenueAndVisitorMetrics();

                    if (_totalVisitor >= _totalVisitorYesterday)
                    {
                        if (SeriesVisitor[0] is LineSeries<double> lineSeries && lineSeries.Fill != _redLinearGradientPaint)
                        {
                            lineSeries.Fill = _redLinearGradientPaint;
                            lineSeries.Stroke = _redStroke;
                        }
                    }
                }
                else
                {
                    // TODO: throw not found exception.
                }
            }
        }

        private void HighlightPaymentRecord(PaymentRecord record)
        {
            var context = SynchronizationContext.Current;
            Task.Run(async () =>
            {
                context?.Post(_ => record.IsUpdated = true, null);
                await Task.Delay(2000);
                context?.Post(_ => record.IsUpdated = false, null);
            });
        }

        private void GeneratePaymentRecordsSampleData()
        {
            var random = new Random();
            var fuelTypes = new[] { "휘발유", "경유" };
            var paymentMethods = new[] { "카드", "간편", "현금" };
            var statuses = new[] { "승인", "실패" };
            decimal pricePerLiter = 1652.89M; // TODO: 하드코딩 수정

            var transactions = Enumerable.Range(1, 20).Select(i =>
            {
                var randomTimeSpan = new TimeSpan(0, random.Next(0, 24), random.Next(0, 60), random.Next(0, 60));
                var timestamp = DateTime.Now - randomTimeSpan;
                var paymentMethod = paymentMethods[random.Next(paymentMethods.Length)];
                
                return new
                {
                    Timestamp = timestamp,
                    FuelType = fuelTypes[random.Next(fuelTypes.Length)],
                    Amount = random.Next(1, 16) * 10000,
                    PaymentMethod = paymentMethod,
                    Status = paymentMethod == "현금" ? "승인" : random.Next(10) == 0 ? "실패" : "승인"
                };
            }).OrderBy(t => t.Timestamp).ToList();

            var orderCounts = new Dictionary<string, int>();
            var paymentRecords = new List<PaymentRecord>();

            for (int i = 0; i < transactions.Count; i++)
            {
                var transaction = transactions[i];
                var dateKey = transaction.Timestamp.ToString("MMdd");

                if (!orderCounts.ContainsKey(dateKey))
                {
                    orderCounts[dateKey] = 0;
                }
                orderCounts[dateKey]++;
                var orderNumber = orderCounts[dateKey];

                var debug = Math.Round(transaction.Amount / pricePerLiter, 2);

                var paymentRecord = new PaymentRecord
                {
                    TransactionId = $"{dateKey}-{orderNumber:D6}",
                    Timestamp = transaction.Timestamp,
                    FuelType = transaction.FuelType,
                    Amount = transaction.Amount,
                    Liters = Math.Round(transaction.Amount / pricePerLiter, 2),
                    PaymentMethod = transaction.PaymentMethod,
                    Status = transaction.Status
                };

                paymentRecords.Add(paymentRecord);
            }

            PaymentRecords = new ObservableCollection<PaymentRecord>(paymentRecords);
        }
    }
}

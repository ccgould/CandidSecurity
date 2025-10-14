using CandidQV.ViewModels;
using IATA.BarCodedBoardingPass;
using System.Net;
using ZXing.Net.Maui;

namespace CandidQV.Views;

[QueryProperty(nameof(IataData), "IataData")]
public partial class BarcodeScanner : ContentPage
{
    private bool _hasNavigated = false;
    private string lastDetectedBarcode;
    private DateTime lastDetectedTime;
    public string IataData { get; set; }

    public BarcodeScanner()
	{
		InitializeComponent();

        cameraBarcodeReaderView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.TwoDimensional,
            AutoRotate = true,
            Multiple = false
        };
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        cameraBarcodeReaderView.IsDetecting = false;
    }


    private async void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        try
        {

            
            var first = e.Results?.FirstOrDefault();

            if (first is null)
            {
                return;
            }

            // Check if the  same barcode was detected within the last second
            if (first.Value == lastDetectedBarcode && (DateTime.Now - lastDetectedTime).TotalSeconds < 1)
            {
                return;
            }

            lastDetectedBarcode = first.Value;
            lastDetectedTime = DateTime.Now;

            var result = e.Results?.FirstOrDefault();

            await Dispatcher.DispatchAsync(async () =>
            {
                // ✅ Send result back
                NavigationResultBroker.BarcodeResultSource?.SetResult(result.Value);

                // Optional: stop camera
                cameraBarcodeReaderView.IsDetecting = false;

                // Small delay to let camera settle
                await Task.Delay(200);

                // Navigate back
                await Shell.Current.GoToAsync("..");
            });
        }
        catch (Exception ex)
        {
           await DisplayAlert("Error", ex.Message, "OK");
        }
    }


}
using CandidRadioTracker.ViewModels;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using ZXing.Net.Maui;

namespace CandidRadioTracker.Views;

public partial class ScannerPage : ContentPage
{
    private ScannerPageViewModel ViewModel => BindingContext as ScannerPageViewModel;
	public ScannerPage(ScannerPageViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;


        cameraBarcodeReaderView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormat.QrCode,
            AutoRotate = true,
            Multiple = true
        };
    }

    protected void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        foreach (var barcode in e.Results)
            Console.WriteLine($"Barcodes: {barcode.Format} -> {barcode.Value}");

        var result = e.Results.FirstOrDefault()?.Value;
        if (result != null)
        {
            RadioLogEditorViewModel.BarcodeResultSource.TrySetResult(result);
            Dispatcher.Dispatch(async () =>
            {
                await Shell.Current.GoToAsync("..");
            });
        }
    }
}
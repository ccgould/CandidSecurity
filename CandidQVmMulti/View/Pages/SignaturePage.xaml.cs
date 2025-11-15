using CandidQVmMulti.Interfaces;
using CandidQVmMulti.Services;
using Microsoft.Maui.Controls;
using Syncfusion.Maui.Core.Internals;
using Syncfusion.Maui.SignaturePad;
using System;
using System.IO;

namespace CandidQVmMulti.View.Pages;

public partial class SignaturePage : ContentPage
{
    private readonly MySqlVoucherService _voucherService;
    private readonly IDeviceOrientationService deviceOrientationService;
    private readonly SignatureService signatureService;

    public SignaturePage(MySqlVoucherService voucherService, IDeviceOrientationService deviceOrientationService, SignatureService signatureService)
    {
        InitializeComponent();
        _voucherService = voucherService;
        this.deviceOrientationService = deviceOrientationService;
        this.signatureService = signatureService;
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        signaturePad.Clear();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (signaturePad.GetSignaturePoints().Count <= 0)
        {
            await DisplayAlert("No Signature", "Please provide a signature before saving.", "OK");
            return;
        }

            var imageSource = signaturePad.ToImageSource();

            // Convert ImageSource to Stream
            using var stream = await ((StreamImageSource)imageSource).Stream(CancellationToken.None);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);


        var signatureId = await _voucherService.AddSignatureAsync(Convert.ToBase64String(ms.ToArray()));

        if (signatureId > 0)
        {

            // Optionally pass the signatureId back via a shared service or messaging
            await DisplayAlert("Success", "Signature saved successfully.", "OK");
            signatureService.SignatureCompletionSource?.SetResult(signatureId);
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await DisplayAlert("Error", "Failed to save signature.", "OK");
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        signatureService.SignatureCompletionSource?.SetResult(-1);
        await Shell.Current.GoToAsync("..");
    }



    protected override void OnAppearing()
    {
        base.OnAppearing();

#if ANDROID
        // Lock orientation to portrait
        deviceOrientationService.LockOrientationPortrait();
#endif
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

#if ANDROID
        // Unlock orientation

        deviceOrientationService.UnlockOrientation();
#endif
    }
}
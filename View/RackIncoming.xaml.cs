#if ANDROID
using Android.Content;
using Android.Views.InputMethods;
#endif


namespace PblMauiShipment.View;

public partial class RackIncoming : ContentPage
{
    RackScanIncomingViewModel _viewIncomingModel;

    public RackIncoming(RackScanIncomingViewModel rackSannsIncomingViewModel)
    {
        InitializeComponent();
        Title = Properties.Resources.RackIncomming;
        BindingContext = _viewIncomingModel = rackSannsIncomingViewModel;
        if (_viewIncomingModel.DeviceModel == "CT60")
        {
            lightGrid.IsVisible = false;
            scannButton.IsVisible = false;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await FocusBarcodeInputForScannerAsync();
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(250), async () => await FocusBarcodeInputForScannerAsync());
    }

    private async void barcodeIncomming_Completed(object sender, EventArgs e)
    {
        if (barcode_incomming.Text != string.Empty)
        {
            _viewIncomingModel.ZxingBarcodeStrIncomming = barcode_incomming.Text;
            barcode_incomming.Text = string.Empty;
        }
        await FocusBarcodeInputForScannerAsync();
    }

    private async Task FocusBarcodeInputForScannerAsync()
    {
        bool suppressSoftKeyboard = _viewIncomingModel.DeviceModel == "CT60";

#if ANDROID
        var editText = barcode_incomming.Handler?.PlatformView as Android.Widget.EditText;
        if (suppressSoftKeyboard && editText != null)
        {
            editText.ShowSoftInputOnFocus = false;
        }
#endif

        barcode_incomming.Focus();

#if ANDROID
        if (editText != null)
        {
            editText.FocusableInTouchMode = true;
            editText.RequestFocus();
        }
#endif

        await Task.Delay(100);

        if (!suppressSoftKeyboard)
        {
            return;
        }

#if ANDROID
        if (editText != null)
        {
            var inputMethodManager = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputMethodManager?.HideSoftInputFromWindow(editText.WindowToken, HideSoftInputFlags.None);
        }
#else
    await barcode_incomming.HideSoftInputAsync(CancellationToken.None);
#endif
    }

    private void DeleteRackScannIncomming_Clicked(object sender, EventArgs e)
    {
        if (sender is Button { BindingContext: RackScan rackScan })
        {
            _viewIncomingModel.DeleteRackScannIncommingCommand.Execute(rackScan);
        }
    }

    private async void FlashlightSwitchIncomming_Toggled(object sender, ToggledEventArgs e)
    {
        try
        {
            if (FlashlightSwitchIncomming.IsToggled)
                await Flashlight.Default.TurnOnAsync();
            else
                await Flashlight.Default.TurnOffAsync();
        }
        catch (FeatureNotSupportedException ex)
        {
            // Handle not supported on device exception
        }
        catch (PermissionException ex)
        {
            // Handle permission exception
        }
        catch (Exception ex)
        {
            // Unable to turn on/off flashlight
        }
    }
}
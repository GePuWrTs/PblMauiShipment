#if ANDROID
using Android.Content;
using Android.Views.InputMethods;
#endif

namespace PblMauiShipment.View;

public partial class RackRegister : ContentPage
{
    RackScanRegisterViewModel _viewRegisterModel;

    public RackRegister(RackScanRegisterViewModel rackSannsRegisterViewModel)
    {
        InitializeComponent();
        Title = Properties.Resources.RackRegistation;
        BindingContext = _viewRegisterModel = rackSannsRegisterViewModel;
        if (_viewRegisterModel.DeviceModel == "CT60")
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

    private async void barcode_register_Completed(object sender, EventArgs e)
    {
        if (barcode_register.Text != string.Empty)
        {
            _viewRegisterModel.ZxingBarcodeStrRegister = barcode_register.Text;
            barcode_register.Text = string.Empty;
        }
        await FocusBarcodeInputForScannerAsync();
    }

    private async Task FocusBarcodeInputForScannerAsync()
    {
        bool suppressSoftKeyboard = _viewRegisterModel.DeviceModel == "CT60";

#if ANDROID
        var editText = barcode_register.Handler?.PlatformView as Android.Widget.EditText;
        if (suppressSoftKeyboard && editText != null)
        {
            editText.ShowSoftInputOnFocus = false;
        }
#endif

        barcode_register.Focus();

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
    await barcode_register.HideSoftInputAsync(CancellationToken.None);
#endif
    }

    private void DeleteRackScannRegister_Clicked(object sender, EventArgs e)
    {
        if (sender is Button { BindingContext: RackScan rackScan })
        {
            _viewRegisterModel.DeleteRackScannRegisterCommand.Execute(rackScan);
        }
    }

    private async void FlashlightSwitchRegister_Toggled(object sender, ToggledEventArgs e)
    {
        try
        {
            if (FlashlightSwitchRegister.IsToggled)
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

    private void rackOwner_SelectedIndexChanged(object sender, EventArgs e)
    {
        //>SelectedIndexChanged="rackOwner_SelectedIndexChanged"

        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            var p = picker.ItemsSource[selectedIndex];
            _viewRegisterModel.RackOwnerSelected = (RackOwner)p;
        }
    }

}
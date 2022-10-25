namespace PblMauiShipment.View;

public partial class RackRegister : ContentPage
{
  RackScanRegisterViewModel _viewRegisterModel;

  public RackRegister(RackScanRegisterViewModel rackSannsRegisterViewModel)
	{
		InitializeComponent();
    Title = Properties.Resources.RackRegistation;
    BindingContext = _viewRegisterModel = rackSannsRegisterViewModel;
    //FlashlightSwitchRegister.IsToggled = false;
  }

  private void barcode_register_Completed(object sender, EventArgs e) {
    if (barcode_register.Text != string.Empty) {
      _viewRegisterModel.ZxingBarcodeStrRegister = barcode_register.Text;
      barcode_register.Text = string.Empty;
    }
  }

  private async void FlashlightSwitchRegister_Toggled(object sender, ToggledEventArgs e) {
    try {
      if (FlashlightSwitchRegister.IsToggled)
        await Flashlight.Default.TurnOnAsync();
      else
        await Flashlight.Default.TurnOffAsync();
    }
    catch (FeatureNotSupportedException ex) {
      // Handle not supported on device exception
    }
    catch (PermissionException ex) {
      // Handle permission exception
    }
    catch (Exception ex) {
      // Unable to turn on/off flashlight
    }

  }
}
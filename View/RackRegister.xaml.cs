namespace PblMauiShipment.View;

public partial class RackRegister : ContentPage {
  RackScanRegisterViewModel _viewRegisterModel;

  public RackRegister(RackScanRegisterViewModel rackSannsRegisterViewModel) {
    InitializeComponent();
    Title = Properties.Resources.RackRegistation;
    BindingContext = _viewRegisterModel = rackSannsRegisterViewModel;
    if (_viewRegisterModel.DeviceModel == "CT60") {
      lightGrid.IsVisible = false;
      scannButton.IsVisible = false;
    }
  }

  private void barcode_register_Completed(object sender, EventArgs e) {
    if (barcode_register.Text != string.Empty) {
      _viewRegisterModel.ZxingBarcodeStrRegister = barcode_register.Text;
      barcode_register.Text = string.Empty;
    }
    barcode_register.Unfocus();
  }

    private void DeleteRackScannRegister_Clicked(object sender, EventArgs e)
    {
        if (sender is Button { BindingContext: RackScan rackScan })
        {
            _viewRegisterModel.DeleteRackScannRegisterCommand.Execute(rackScan);
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

  private void rackOwner_SelectedIndexChanged(object sender, EventArgs e) {
    //>SelectedIndexChanged="rackOwner_SelectedIndexChanged"

    var picker = (Picker)sender;
    int selectedIndex = picker.SelectedIndex;

    if (selectedIndex != -1) {
      var p = picker.ItemsSource[selectedIndex];
      _viewRegisterModel.RackOwnerSelected = (RackOwner)p;
    }
  }

}
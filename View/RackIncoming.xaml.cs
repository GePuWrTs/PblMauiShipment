using System.Reflection;

namespace PblMauiShipment.View;

public partial class RackIncoming : ContentPage {
  RackScanIncomingViewModel _viewIncomingModel;

  public RackIncoming(RackScanIncomingViewModel rackSannsIncomingViewModel) {
    InitializeComponent();
    Title = Properties.Resources.RackIncomming;
    BindingContext = _viewIncomingModel = rackSannsIncomingViewModel;
    FlashlightSwitch.IsToggled = false;
  }

  private void barcode_Completed(object sender, EventArgs e) {
    if (barcode_incomming.Text != string.Empty) {
      _viewIncomingModel.ZxingBarcodeStrIncomming = barcode_incomming.Text;
      barcode_incomming.Text = string.Empty;
    }
  }



  private async void FlashlightSwitch_Toggled(object sender, ToggledEventArgs e) {
      try {
        if (FlashlightSwitch.IsToggled)
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
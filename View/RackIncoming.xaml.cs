using System.Reflection;

namespace PblMauiShipment.View;

public partial class RackIncoming : ContentPage {
  RackScanIncomingViewModel _viewIncomingModel;

  public RackIncoming(RackScanIncomingViewModel rackSannsIncomingViewModel) {
    InitializeComponent();
    Title = Properties.Resources.RackIncomming;
    BindingContext = _viewIncomingModel = rackSannsIncomingViewModel;
  }

  private void barcode_Completed(object sender, EventArgs e) {
    if (barcode_incomming.Text != string.Empty) {
      _viewIncomingModel.ZxingBarcodeStrIncomming = barcode_incomming.Text;
      barcode_incomming.Text = string.Empty;
    }
  }

}
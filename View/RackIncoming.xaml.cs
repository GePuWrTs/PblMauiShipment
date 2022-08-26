using System.Reflection;

namespace PblMauiShipment.View;

public partial class RackIncoming : ContentPage {
  RackSannsViewModel _viewModel;

  public RackIncoming(RackSannsViewModel rackSannsViewModel) {
    InitializeComponent();
    Title = Properties.Resources.RackScanns;
    BindingContext = _viewModel = rackSannsViewModel;
  }

  private void barcode_Completed(object sender, EventArgs e) {
    if (barcode.Text != string.Empty) {
      _viewModel.ZxingBarcodeStr = barcode.Text;
      barcode.Text = string.Empty;
    }
  }


}
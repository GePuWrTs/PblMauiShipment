using AndroidX.Lifecycle;
using PblMauiShipment.ViewModels;
using ZXing.Net.Maui;

namespace PblMauiShipment.View;

public partial class RackIncoming : ContentPage {
  RackSannsViewModel _viewModel;

  public RackIncoming(RackSannsViewModel rackSannsViewModel) {
    InitializeComponent();
    Title = Properties.Resources.RackScanns;
    BindingContext = _viewModel = rackSannsViewModel;
  }



  private async void barcode_Completed(object sender, EventArgs e) {
    if (barcode.Text != string.Empty) {
      _viewModel.ZxingBarcodeStr = barcode.Text;
    }
  }

  private void barcode_TextChanged(object sender, TextChangedEventArgs e) {
  }

  public void ChaneBC(string bc) {

  }
}
using ZXing.Net.Maui;

namespace PblMauiShipment.View;

public partial class ZXingBarcodeReader : ContentPage {
  RackSannsViewModel _viewModel;

  //public ZXingBarcodeReader() {
  //  InitializeComponent(); 
  //  //BindingContext = _viewModel = rackSannsViewModel;
  //  barcodeReader.Options = new BarcodeReaderOptions() {
  //    Formats = BarcodeFormats.All,
  //    Multiple = false,
  //    TryHarder = true
  //  };
  //}

  public ZXingBarcodeReader(RackSannsViewModel rackSannsViewModel) {
    InitializeComponent();
    BindingContext = _viewModel = rackSannsViewModel;
    barcodeReader.Options = new BarcodeReaderOptions() {
      Formats = BarcodeFormats.All,
      Multiple = false,
      TryHarder = true
    };
  }

  private void barcodeReader_BarcodesDetected(object sender, BarcodeDetectionEventArgs e) {     
    Dispatcher.Dispatch(async () => {    
      barcodeResult.Text = $"{e.Results[0].Value} - {e.Results[0].Format}";
      await Shell.Current.GoToAsync($"..?ZxingBarcodeStr={barcodeResult.Text}");
    });
  }
}
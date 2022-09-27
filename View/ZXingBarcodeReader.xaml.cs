namespace PblMauiShipment.View;

public partial class ZXingBarcodeReader : ContentPage {
  string retunedBarcode = string.Empty;
  public ZXingBarcodeReader() {
    InitializeComponent();
    //BindingContext = _viewModel = rackSannsViewModel;
    barcodeReader.Options = new BarcodeReaderOptions() {
      Formats = BarcodeFormats.All
      ,Multiple = false
      ,AutoRotate = true
      //,TryHarder = true
    };
    barcodeReader.IsTorchOn = true;
  }

  private void barcodeReader_BarcodesDetected(object sender, BarcodeDetectionEventArgs e) {     
    Dispatcher.Dispatch(async () => {
      retunedBarcode = $"{e.Results[0].Value} - {e.Results[0].Format}";
      barcodeResult.Text = retunedBarcode;
      await Shell.Current.GoToAsync($"..?ZxingBarcodeStr={retunedBarcode}");
    });
  }
}
//using static Java.Util.Jar.Attributes;

namespace PblMauiShipment.View;
[QueryProperty(nameof(ScanType), "scantype")]

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
  }

  string mScantype = string.Empty;
  public string ScanType {
    set {
      mScantype = value;
    }
  }

  private void barcodeReader_BarcodesDetected(object sender, BarcodeDetectionEventArgs e) {
    switch (mScantype) {
      case "incoming":
        Dispatcher.Dispatch(async () => {
          //retunedBarcode = $"{e.Results[0].Value} - {e.Results[0].Format}";
          retunedBarcode = $"{e.Results[0].Value}";
          barcodeResult.Text = retunedBarcode;
          await Shell.Current.GoToAsync($"..?ZxingBarcodeStrIncomming={retunedBarcode}");
        });
        break;
      default:
        Dispatcher.Dispatch(async () => {
          retunedBarcode = $"{e.Results[0].Value} - {e.Results[0].Format}";
          barcodeResult.Text = retunedBarcode;
          await Shell.Current.GoToAsync($"..?ZxingBarcodeStrIncomming={retunedBarcode}");
        });
        break;
    }

  }
}
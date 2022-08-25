using AndroidX.Lifecycle;
using PblMauiShipment.View;
using ZXing.QrCode.Internal;

namespace PblMauiShipment.ViewModels {

  [QueryProperty(nameof(ZxingBarcodeStr), nameof(ZxingBarcodeStr))]

  public partial class RackSannsViewModel : BaseViewModel {
    public ObservableCollection<RackScan> RackScanns { get; set; } = new();
    RackScanService rackScanService;
    string zxingBarcodeStrOld = string.Empty;


    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ScanBarcodeCommand))]
    string zxingBarcodeStr;

    partial void OnZxingBarcodeStrChanged(string value) {
      //throw new NotImplementedException();
      if ((ZxingBarcodeStr != String.Empty) && (ZxingBarcodeStr != zxingBarcodeStrOld)) {
        zxingBarcodeStrOld = value;
        RackScan rs = new();
        rs.Barcode = value;
        rs.Type = ScanType.incoming;
        AddRackScannCommand.ExecuteAsync(rs);
        ZxingBarcodeStr = string.Empty;
      }
    }


    //public RackSannsViewModel(ObservableCollection<RackScan> racks, RackScanService rackScanService) {
    public RackSannsViewModel(RackScanService rackScanService) {
      Title = Properties.Resources.RackScanns;
      this.rackScanService = rackScanService;
      
    }

    [RelayCommand]
    async Task ScanBarcodeAsync() {
      
      await Shell.Current.GoToAsync(nameof(ZXingBarcodeReader), true);

    }

    //async Task ScanBarcodeAsync(RackSannsViewModel rackSannsViewModel) {
    //  await Shell.Current.GoToAsync(nameof(ZXingBarcodeReader), true, new Dictionary<string, object>
    //    {
    //        {"RackSannsViewModel", rackSannsViewModel }
    //    });
    //}


    //async Task ScanBarcodeAsync(RackScan rackScan) {      
    //  await Shell.Current.GoToAsync(nameof(ZXingBarcodeReader), true, new Dictionary<string, object>
    //    {
    //        {"RackScan", rackScan }
    //    });    
    //}

    [RelayCommand]
    async Task<bool> AddRackScannAsync(RackScan rackScan) {
      if (rackScan != null) {
        if (RackScanns.Count > 0) {
          rackScan.ItemID = RackScanns.Last().ItemID + 1;
        } else
          rackScan.ItemID = 1;
        rackScan.Scanned = DateTime.Now;
        RackScanns.Add(rackScan);
      }
      return await Task.FromResult(true);
    }


    [RelayCommand]
    async Task<bool> DeleteRackScannAsync(int itemID) {
      var oldRackScan = RackScanns.Where(r => r.ItemID == itemID).FirstOrDefault();
      RackScanns.Remove(oldRackScan);
      return await Task.FromResult(true);
    }

    [RelayCommand]
    async Task GetMockRackScannsAsync() {
      try {
        IsBusy = true;
        var rackScanns = await rackScanService.GetMockRackScanList();
        if (RackScanns.Count != 0)
          RackScanns.Clear();

        foreach (var rackscan in rackScanns)
          RackScanns.Add(rackscan);
      }
      catch (Exception ex) {
        Debug.WriteLine($"Unable to get Mock Rackscanns: {ex.Message}");
        await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
      }
      finally {
        IsBusy = false;
      }
    }

    [RelayCommand]
    async Task ClearRackScannsAsync() {
      try {
        IsBusy = true;
        RackScanns.Clear();
      }
      catch (Exception ex) {
        Debug.WriteLine($"Unable to clear Rackscanns: {ex.Message}");
        await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
      }
      finally {
        IsBusy = false;
      }
    }
  }
}

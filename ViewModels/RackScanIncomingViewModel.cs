using PblMauiShipment.Models;
using PblMauiShipment.Services;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;

namespace PblMauiShipment.ViewModels {

  [QueryProperty(nameof(ZxingBarcodeStrIncomming), nameof(ZxingBarcodeStrIncomming))]

  public partial class RackScanIncomingViewModel : BaseViewModel {

    public ObservableCollection<RackScan> RackScanns { get; set; } = new();
    public int RackScannsCount {
      get { return RackScanns.Count; }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ScanBarcodeIncommingCommand))]
    string zxingBarcodeStrIncomming;

    string zxingBarcodeStrOld = string.Empty;
    RackScanService rackScanService = new RackScanService();
    string jsonFileName = String.Empty;


    public RackScanIncomingViewModel(RackScanService rackScanService) {
      Title = Properties.Resources.RackScans;
      jsonFileName = string.Format(@"{0}/{1}.json", rackScanService.DataDirectory, rackScanService.RackIncomingPrefix);
      UpdateRackScanJson();
    }

    #region Command's
    [RelayCommand]
    async Task<int> SendFile() {
      int FileCount = 0;
     if(await rackScanService.SaveRackScanListToXml(RackScanns, ScanType.incoming)) {
        await ClearRackScannsAsync();
        FileInfo fi = new FileInfo(jsonFileName);
        if (fi.Exists) {
          fi.Delete();
        }
        FileCount = await rackScanService.UploadFiles();
      }
      return await Task.FromResult(FileCount);
    }

    [RelayCommand]
    async Task ScanBarcodeIncommingAsync() {
      await Shell.Current.GoToAsync($"{nameof(ZXingBarcodeReader)}?scantype={ScanType.incoming.ToString()}", true);
    }


    [RelayCommand]
    async Task<bool> AddRackScannAsync(RackScan rackScan) {
      if (rackScan != null) {
        if (RackScanns.Count == 0) {
          UpdateRackScanJson();
        }
        if (zxingBarcodeStrOld != ZxingBarcodeStrIncomming) {
          zxingBarcodeStrOld = ZxingBarcodeStrIncomming;

          var rs = RackScanns.FirstOrDefault(b => (b.Barcode == rackScan.Barcode));
          if (rs != null) {
            await PlayErrorAsync();
            await Shell.Current.DisplayAlert("Error!", $"Barcode {rackScan.Barcode} bereits in Liste!", "OK");
          } else {
            if (RackScanns.Count > 0) {
              rackScan.ItemID = RackScanns.Last().ItemID + 1;
            } else
              rackScan.ItemID = 1;
            rackScan.Scanned = DateTime.Now;
            RackScanns.Add(rackScan);
            UpdateRackScanJson();
            await PlayBeepAsync();
          }
        }
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
        var rackScannMock = await rackScanService.GetMockRackScanList();
        if (RackScanns.Count != 0)
          RackScanns.Clear();

        foreach (var rackscan in rackScannMock)
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


    [RelayCommand]
    public async Task<bool> SaveRAIRackScanListToXml() {
      if ((RackScanns != null) && (RackScanns.Count > 0)) {
        return await rackScanService.SaveRackScanListToXml(RackScanns, ScanType.incoming);
      }
      return await Task.FromResult(false);
    }
    #endregion

    partial void OnZxingBarcodeStrIncommingChanged(string value) {
      if (ZxingBarcodeStrIncomming != String.Empty) {
        RackScan rs = new();
        rs.Barcode = value;
        rs.Type = ScanType.incoming;
        AddRackScannCommand.ExecuteAsync(rs);
        ZxingBarcodeStrIncomming = string.Empty;
      }
    }

    public void UpdateRackScanJson() {


      FileInfo FiJson = new FileInfo(jsonFileName);
      if (FiJson.Exists) {
        if (RackScanns.Count == 0) {
          using (StreamReader r = new StreamReader(FiJson.FullName)) {
            string json = r.ReadToEnd();
            RackScanns = JsonSerializer.Deserialize<ObservableCollection<RackScan>>(json);
          }
        } else {
          string jsonString = JsonSerializer.Serialize(RackScanns, new JsonSerializerOptions() { WriteIndented = true });
          using (StreamWriter outputFile = new StreamWriter(FiJson.FullName)) {
            outputFile.WriteLine(jsonString);
          }
        }
      } else {
        if (RackScanns.Count > 0) {
          string jsonString = JsonSerializer.Serialize(RackScanns, new JsonSerializerOptions() { WriteIndented = true });
          using (StreamWriter outputFile = new StreamWriter(FiJson.FullName)) {
            outputFile.WriteLine(jsonString);
          }
        }
      }
    }

  }
}

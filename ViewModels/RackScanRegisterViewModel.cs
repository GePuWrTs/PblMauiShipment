using PblMauiShipment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PblMauiShipment.ViewModels {

  [QueryProperty(nameof(ZxingBarcodeStrRegister), nameof(ZxingBarcodeStrRegister))]

  public partial class RackScanRegisterViewModel : BaseViewModel {

    public ObservableCollection<RackScan> RackScannsRegister { get; set; } = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ScanBarcodeRegisterCommand))]
    string zxingBarcodeStrRegister;

    public string BarcodeStrRegister {
      get { return zxingBarcodeStrRegister; }
    }

    string zxingBarcodeStrRegisterOld = string.Empty;
    RackScanService rackScanServiceRegister = new RackScanService();
    string jsonFileNameRegister = string.Empty;
    string jsonFileNameOwnerPickerSelected = string.Empty;

    [ObservableProperty]
    public int filesToSendRegister;

    [ObservableProperty]
    public List<RackOwner> rackOwnerList;

    [ObservableProperty]
    public RackOwner rackOwnerSelected;

    public RackScanRegisterViewModel(RackScanService rackScanService) {
      Title = Properties.Resources.RackScans;
      jsonFileNameRegister = string.Format(@"{0}/{1}.json", rackScanServiceRegister.DataDirectory, rackScanServiceRegister.RackRegisterPrefix);
      jsonFileNameOwnerPickerSelected = string.Format(@"{0}/{1}.json", rackScanServiceRegister.DataDirectory, "OwnerSelected.json");
      UpdateRackScanJsonRegister();
      //UpdateOwnerPickerSelectedJsonRegister();
      filesToSendRegister = FilesToSendQtyRegister();
      RackOwnerList = rackScanServiceRegister.RackOwnerlistRackScanService;
    }

    #region Command's
    [RelayCommand]
    async Task<bool> DownloadFileRegister() {
      bool result = false;
      result = await rackScanServiceRegister.DownloadFile(rackScanServiceRegister.DataDirectory, "RackOwnerList.XML");
      return result;
    }

    [RelayCommand]
    async Task<int> SendFileRegister() {
      int FileCount = 0;
      int fileQty = FilesToSendQtyRegister();
      try {
        IsBusy = true;
        if (await rackScanServiceRegister.SaveRackScanListToXml(RackScannsRegister, ScanType.register) || (fileQty > 0)) {
          await ClearRackScannsRegisterAsync();
          FileInfo fi = new FileInfo(jsonFileNameRegister);
          if (fi.Exists) {
            fi.Delete();
          }
          fileQty = FilesToSendQtyRegister();
          FilesToSendRegister = fileQty;
          if (fileQty > 0) {
            FileCount = await rackScanServiceRegister.UploadFiles();
            if (FileCount == 0) {
              await Shell.Current.DisplayAlert("Error!", $"{FileCount} von {fileQty} Dateien gesendet!", "OK");
            } else {
              await Shell.Current.DisplayAlert("Upload", $"{FileCount} von {fileQty} Dateien gesendet", "OK");
            }
          }
        }
      }
      catch (Exception) {

        throw;
      }
      finally {
        IsBusy = true;
      }
      FilesToSendRegister = FilesToSendQtyRegister();

      return await Task.FromResult(FileCount);
    }

    [RelayCommand]
    async Task ScanBarcodeRegisterAsync() {
      await Shell.Current.GoToAsync($"{nameof(ZXingBarcodeReader)}?scantype={ScanType.register.ToString()}", true);
    }

    [RelayCommand]
    async Task<bool> AddRackScannRegisterAsync(RackScan rackScan) {
      if ((RackOwnerSelected == null) ||(RackOwnerSelected.ID == 0)) {
        await PlayErrorAsync();
        await Shell.Current.DisplayAlert("Error!", $"Bitte Gestellbesitzer wählen", "OK");
        return false;
      }
      if ((rackScan != null) && (rackOwnerSelected.ID != 0)) {
        if (RackScannsRegister.Count == 0) {
          UpdateRackScanJsonRegister();
          //UpdateOwnerPickerSelectedJsonRegister();
        }
        if (zxingBarcodeStrRegisterOld != ZxingBarcodeStrRegister) {
          zxingBarcodeStrRegisterOld = ZxingBarcodeStrRegister;

          var rs = RackScannsRegister.FirstOrDefault(b => (b.Barcode == rackScan.Barcode));
          if (rs != null) {
            await PlayErrorAsync();
            await Shell.Current.DisplayAlert("Error!", $"Barcode {rackScan.Barcode} bereits in Liste!", "OK");
          } else {
            if (RackScannsRegister.Count > 0) {
              rackScan.ItemID = RackScannsRegister.Last().ItemID + 1;
            } else
              rackScan.ItemID = 1;
            rackScan.Scanned = DateTime.Now;
            rackScan.OwnerID = rackOwnerSelected.ID;
            rackScan.OwnerName = rackOwnerSelected.OwnerName;
            RackScannsRegister.Add(rackScan);
            UpdateRackScanJsonRegister();
            //UpdateOwnerPickerSelectedJsonRegister();
            await PlayBeepAsync();
          }
        }
      }
      return await Task.FromResult(true);
    }


    [RelayCommand]
    async Task<bool> DeleteRackScannRegisterAsync(int itemID) {
      var oldRackScan = RackScannsRegister.Where(r => r.ItemID == itemID).FirstOrDefault();
      RackScannsRegister.Remove(oldRackScan);
      return await Task.FromResult(true);
    }

    [RelayCommand]
    async Task GetMockRackScannsRegisterAsync() {
      try {
        IsBusy = true;
        var rackScannMock = await rackScanServiceRegister.GetMockRackScanList();
        if (RackScannsRegister.Count != 0)
          RackScannsRegister.Clear();

        foreach (var rackscan in rackScannMock)
          RackScannsRegister.Add(rackscan);
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
    async Task ClearRackScannsRegisterAsync() {
      try {
        IsBusy = true;
        RackScannsRegister.Clear();
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
    public async Task<bool> SaveREGRackScanListToXml() {
      if ((RackScannsRegister != null) && (RackScannsRegister.Count > 0)) {
        return await rackScanServiceRegister.SaveRackScanListToXml(RackScannsRegister, ScanType.register);
      }
      return await Task.FromResult(false);
    }
    #endregion

    partial void OnZxingBarcodeStrRegisterChanged(string value) {
      if (ZxingBarcodeStrRegister != String.Empty) {
        RackScan rs = new();
        rs.Barcode = value;
        rs.Type = ScanType.register;
        AddRackScannRegisterCommand.ExecuteAsync(rs);
        ZxingBarcodeStrRegister = string.Empty;
      }
    }

    public int FilesToSendQtyRegister() {
      DirectoryInfo di = new DirectoryInfo(rackScanServiceRegister.RackFileDirectory);
      return di.GetFiles().Count();
    }

    public void UpdateRackScanJsonRegister() {
      FileInfo FiJson = new FileInfo(jsonFileNameRegister);
      if (FiJson.Exists) {
        if (RackScannsRegister.Count == 0) {
          using (StreamReader r = new StreamReader(FiJson.FullName)) {
            string json = r.ReadToEnd();
            RackScannsRegister = JsonSerializer.Deserialize<ObservableCollection<RackScan>>(json);
          }
        } else {
          string jsonString = JsonSerializer.Serialize(RackScannsRegister, new JsonSerializerOptions() { WriteIndented = true });
          using (StreamWriter outputFile = new StreamWriter(FiJson.FullName)) {
            outputFile.WriteLine(jsonString);
          }
        }
      } else {
        if (RackScannsRegister.Count > 0) {
          string jsonString = JsonSerializer.Serialize(RackScannsRegister, new JsonSerializerOptions() { WriteIndented = true });
          using (StreamWriter outputFile = new StreamWriter(FiJson.FullName)) {
            outputFile.WriteLine(jsonString);
          }
        }
      }
    }

    //public void UpdateOwnerPickerSelectedJsonRegister() {
    //  FileInfo FiJson = new FileInfo(jsonFileNameOwnerPickerSelected);
    //  if (FiJson.Exists) {
    //    if ((RackOwnerSelected != null) && (RackOwnerSelected.ID == 0)) {
    //      using (StreamReader r = new StreamReader(FiJson.FullName)) {
    //        string json = r.ReadToEnd();
    //        RackOwnerSelected = JsonSerializer.Deserialize<RackOwner>(json);
    //      }
    //    } else {
    //      string jsonString = JsonSerializer.Serialize(RackOwnerSelected, new JsonSerializerOptions() { WriteIndented = true });
    //      using (StreamWriter outputFile = new StreamWriter(FiJson.FullName)) {
    //        outputFile.WriteLine(jsonString);
    //      }
    //    }
    //  } else {
    //    if ((RackOwnerSelected != null) && (RackOwnerSelected.ID > 0)) {
    //      string jsonString = JsonSerializer.Serialize(RackOwnerSelected, new JsonSerializerOptions() { WriteIndented = true });
    //      using (StreamWriter outputFile = new StreamWriter(FiJson.FullName)) {
    //        outputFile.WriteLine(jsonString);
    //      }
    //    }
    //  }
    //}

  }
}

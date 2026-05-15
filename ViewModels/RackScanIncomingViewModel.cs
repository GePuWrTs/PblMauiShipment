using CommunityToolkit.Mvvm.ComponentModel;
using PblMauiShipment.Models;
using PblMauiShipment.Services;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;

namespace PblMauiShipment.ViewModels {

  [QueryProperty(nameof(ZxingBarcodeStrIncomming), nameof(ZxingBarcodeStrIncomming))]

  public partial class RackScanIncomingViewModel : BaseViewModel {

    public ObservableCollection<RackScan> RackScannsIncomming { get; set; } = new();


    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ScanBarcodeIncommingCommand))]
    string zxingBarcodeStrIncomming;

    public string BarcodeStrIncomming {
      get { return zxingBarcodeStrIncomming; }
    }

    string zxingBarcodeStrIncommingOld = string.Empty;
    RackScanService rackScanServiceIncomming = new RackScanService();
    string jsonFileNameIncomming = String.Empty;

    [ObservableProperty]
    public int filesToSendIncomming;

    string mDeviceModel = string.Empty;

    public string DeviceModel {
      get { return mDeviceModel; }
    }

    public RackScanIncomingViewModel(RackScanService rackScanService) {
      Title = Properties.Resources.RackScans;
      jsonFileNameIncomming = string.Format(@"{0}/{1}.json", rackScanService.DataDirectory, rackScanService.RackIncomingPrefix);
      mDeviceModel = rackScanService.DeviceInfo.DeviceModel;
      UpdateRackScanJsonIncomming();
      FilesToSendIncomming = FilesToSendQtyIncomming();
    }


    #region Command's
    [RelayCommand]
    async Task<int> SendFileIncomming() {
      int FileCount = 0;
      int fileQty = FilesToSendQtyIncomming();
      try {
                if (!await ConfirmSendFilesAsync(fileQty, RackScannsIncomming.Count))
                {
                    return FileCount;
                }
                IsBusy = true;
        if (await rackScanServiceIncomming.SaveRackScanListToXml(RackScannsIncomming, ScanType.incoming) || (fileQty > 0)) {
          await ClearRackScannsIncommingAsync();
          FileInfo fi = new FileInfo(jsonFileNameIncomming);
          if (fi.Exists) {
            fi.Delete();
          }
          fileQty = FilesToSendQtyIncomming();
          FilesToSendIncomming = fileQty;
          if (fileQty > 0) {
            FileCount = await rackScanServiceIncomming.UploadFiles();
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
        IsBusy = false;
      }
      FilesToSendIncomming = FilesToSendQtyIncomming(); 

      return await Task.FromResult(FileCount);
    }


        [RelayCommand]
        async Task<int> DeleteFilesToSendIncomming()
        {
            int fileQty = FilesToSendQtyIncomming();
            if (!await ConfirmDeleteFilesAsync(fileQty))
            {
                return 0;
            }

            try
            {
                IsBusy = true;
                int deletedFileCount = rackScanServiceIncomming.DeletePendingFiles();
                FilesToSendIncomming = FilesToSendQtyIncomming();
                await Shell.Current.DisplayAlert("Dateien löschen", $"{deletedFileCount} Datei(en) gelöscht.", "OK");
                return deletedFileCount;
            }
            finally
            {
                IsBusy = false;
            }
        }


        [RelayCommand]
    async Task ScanBarcodeIncommingAsync() {
      await Shell.Current.GoToAsync($"{nameof(ZXingBarcodeReader)}?scantype={ScanType.incoming.ToString()}", true);
    }

    [RelayCommand]
    async Task<bool> AddRackScannIncommingAsync(RackScan rackScan) {
      if (rackScan != null) {
        if (RackScannsIncomming.Count == 0) {
          UpdateRackScanJsonIncomming();
        }
        if (zxingBarcodeStrIncommingOld != ZxingBarcodeStrIncomming) {
          zxingBarcodeStrIncommingOld = ZxingBarcodeStrIncomming;

          var rs = RackScannsIncomming.FirstOrDefault(b => (b.Barcode == rackScan.Barcode));
          if (rs != null) {
            await PlayErrorAsync();
            await Shell.Current.DisplayAlert("Error!", $"Barcode {rackScan.Barcode} bereits in Liste!", "OK");
          } else {
            if (RackScannsIncomming.Count > 0) {
              rackScan.ItemID = RackScannsIncomming.Last().ItemID + 1;
            } else
              rackScan.ItemID = 1;
            rackScan.Scanned = DateTime.Now;
            RackScannsIncomming.Add(rackScan);
            UpdateRackScanJsonIncomming();
            await PlayBeepAsync();
          }
        }
      }
      return await Task.FromResult(true);
    }


    [RelayCommand]
        void DeleteRackScannIncomming(RackScan rackScan)
        {
            if (rackScan == null)
            {
                return;
            }

            if (RackScannsIncomming.Remove(rackScan))
            {
                if (zxingBarcodeStrIncommingOld == rackScan.Barcode)
                {
                    zxingBarcodeStrIncommingOld = string.Empty;
                }

                SaveRackScanJsonIncomming();
            }
        }

        [RelayCommand]
    async Task GetMockRackScannsIncommingAsync() {
      try {
        IsBusy = true;
        var rackScannMock = await rackScanServiceIncomming.GetMockRackScanList();
        if (RackScannsIncomming.Count != 0)
          RackScannsIncomming.Clear();

        foreach (var rackscan in rackScannMock)
          RackScannsIncomming.Add(rackscan);
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
    async Task ClearRackScannsIncommingAsync() {
      try {
        IsBusy = true;
        RackScannsIncomming.Clear();
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
      if ((RackScannsIncomming != null) && (RackScannsIncomming.Count > 0)) {
        return await rackScanServiceIncomming.SaveRackScanListToXml(RackScannsIncomming, ScanType.incoming);
      }
      return await Task.FromResult(false);
    }
    #endregion

    partial void OnZxingBarcodeStrIncommingChanged(string value) {
      if (ZxingBarcodeStrIncomming != String.Empty) {
        RackScan rs = new();
        rs.Barcode = value;
        rs.Type = ScanType.incoming;
        AddRackScannIncommingCommand.ExecuteAsync(rs);
        ZxingBarcodeStrIncomming = string.Empty;
      }
    }

    public int FilesToSendQtyIncomming() {
      DirectoryInfo di = new DirectoryInfo(rackScanServiceIncomming.RackFileDirectory);
      return di.GetFiles().Count();
    }

    public void UpdateRackScanJsonIncomming() {
      FileInfo FiJson = new FileInfo(jsonFileNameIncomming);
      if (FiJson.Exists) {
        if (RackScannsIncomming.Count == 0) {
          using (StreamReader r = new StreamReader(FiJson.FullName)) {
            string json = r.ReadToEnd();
            RackScannsIncomming = JsonSerializer.Deserialize<ObservableCollection<RackScan>>(json);
          }
        } else {
                    SaveRackScanJsonIncomming();
                }
      } else {
        if (RackScannsIncomming.Count > 0) {
                    SaveRackScanJsonIncomming();
                }
      }
    }


        void SaveRackScanJsonIncomming()
        {
            FileInfo fiJson = new FileInfo(jsonFileNameIncomming);
            if (RackScannsIncomming.Count == 0)
            {
                if (fiJson.Exists)
                {
                    fiJson.Delete();
                }

                return;
            }

            string jsonString = JsonSerializer.Serialize(RackScannsIncomming, new JsonSerializerOptions() { WriteIndented = true });
            using (StreamWriter outputFile = new StreamWriter(fiJson.FullName))
            {
                outputFile.WriteLine(jsonString);
            }
        }

    }
}

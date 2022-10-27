using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Text.Json;
using PblMauiShipment.Models;
using System.IO;

//using AndroidX.Fragment.App;
//using static Android.Graphics.ImageDecoder;

namespace PblMauiShipment.Services {
  public class RackScanService {
    private string mDataDirectory;
    private string mRackFileDirectory;
    private Device mDeviceInfo = new();
    CultureInfo mProvider = new CultureInfo("de-DE");
    private const string mRackIncomingPrefix = "RAI";
    private const string mRackOutgoingPrefix = "RAO";
    private const string mRackRegisterPrefix = "REG";

    private const string mRackOwnerListFileName = "RackOwnerList.XML";

 
    private Uri mBaseAddress = new("http://192.168.5.48:7077"); //PblFit01
    //private static Uri mBaseAddress = new("http://192.168.168.37:5107"); //NBPUF01 WRTS
    //private Uri mBaseAddress = new("http://localhost:5107"); //NBPUF01 WRTS


    #region Properies
    public List<RackOwner> RackOwnerlistRackScanService { get; set; }

    public string DataDirectory {
      get { return mDataDirectory; }
    }

    public string RackFileDirectory {
      get { return mRackFileDirectory; }
    }

    public string RackIncomingPrefix {
      get { return mRackIncomingPrefix; }
    }
    public string RackOutgoingPrefix {
      get { return mRackOutgoingPrefix; }
    }
    public string RackRegisterPrefix {
      get { return mRackRegisterPrefix; }
    }
    #endregion

    public RackScanService() {
      mDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "data");
      Directory.CreateDirectory(mDataDirectory);
      mRackFileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "rackFiles");
      Directory.CreateDirectory(mRackFileDirectory);
      mDeviceInfo.ReadDeviceInfo();
      var result = Task.Run<bool>(async () => await GetOwnerList()).Wait(new TimeSpan(0,0,10));
      if (result) {
        result = false;
      }
    }

    public async Task<bool> GetOwnerList() {
      await DownloadFile(DataDirectory, mRackOwnerListFileName);
      RackOwnerlistRackScanService = await CreateOwnerList();
      return await Task.FromResult(RackOwnerlistRackScanService.Count > 0);
    }


    public async Task<List<RackOwner>> CreateOwnerList() {
      List<RackOwner> Ol = new();
      FileInfo fi = new FileInfo(Path.Combine(mDataDirectory, mRackOwnerListFileName));
      if (fi.Exists) {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(fi.FullName);
        XmlNodeList xl = xmlDoc.SelectNodes("//RackOwners/Owners/Owner");
        foreach (XmlNode xn in xl) {
          RackOwner ro = new();
          ro.ID = int.Parse(xn.SelectSingleNode("ID").InnerText);
          ro.OwnerName = xn.SelectSingleNode("OwnerName").InnerText;
          Ol.Add(ro);
        }
      }
      return await Task.FromResult(Ol);
    }
  
    public async Task<bool> SaveRackScanListToXml(ObservableCollection<RackScan> rackScanList, ScanType scanType) {
      bool result = false;
      if ((rackScanList != null) && (rackScanList.Count > 0)) {

        StringBuilder sb = new StringBuilder(Path.Combine(mRackFileDirectory, BuildRackScanFileName(scanType)));
        string XRootNodeName = String.Empty;

        FileInfo FileInfoXDoc = new FileInfo(sb.ToString());
        // FileInfoXDoc.Create();

        switch (scanType) {
          case ScanType.incoming:
            XRootNodeName = mRackIncomingPrefix;
            break;
          case ScanType.outgoing:
            XRootNodeName = mRackOutgoingPrefix;
            break;
          case ScanType.register:
            XRootNodeName = mRackRegisterPrefix;
            break;
          default:
            XRootNodeName = "ERROR";
            break;
        }

        XmlDocument XDoc = new XmlDocument();
        XmlElement XRoot = XDoc.CreateElement(XRootNodeName);
        XDoc.AppendChild(XRoot);
        XmlElement deviceID = XDoc.CreateElement("DeviceID");
        deviceID.InnerText = mDeviceInfo.DeviceName;
        XRoot.AppendChild(deviceID);
        XmlElement created = XDoc.CreateElement("Created");
        created.InnerText = DateTime.Now.ToString(mProvider);
        XRoot.AppendChild(created);
        XmlElement closed = XDoc.CreateElement("Closed");
        closed.InnerText = DateTime.MinValue.ToString(mProvider);
        XRoot.AppendChild(closed);
        XmlElement XLines = XDoc.CreateElement("Items"); ;

        int index = 0;
        foreach (RackScan rack in rackScanList) {
          index += 1;
          if (index == 1) {
            created.InnerText = rack.Scanned.ToString(mProvider);
          }
          XmlElement XItem = XDoc.CreateElement("Item");
          XmlElement ItemId = XDoc.CreateElement("ItemID");
          ItemId.InnerText = index.ToString();
          XItem.AppendChild(ItemId);
          XmlElement Barcode = XDoc.CreateElement("Barcode");
          Barcode.InnerText = rack.Barcode;
          XItem.AppendChild(Barcode);
          XmlElement Scanned = XDoc.CreateElement("Scanned");
          Scanned.InnerText = rack.Scanned.ToString(mProvider);
          XItem.AppendChild(Scanned);
          if (scanType == ScanType.register) {
            XmlElement OwnerID = XDoc.CreateElement("OwnerID");
            OwnerID.InnerText = rack.OwnerID.ToString(mProvider);
            XItem.AppendChild(OwnerID);
            XmlElement OwnerName = XDoc.CreateElement("OwnerName");
            OwnerName.InnerText = rack.OwnerName.ToString(mProvider);
            XItem.AppendChild(OwnerName);
          }
          XLines.AppendChild(XItem);
        }
        XRoot.AppendChild(XLines);

        closed.InnerText = DateTime.Now.ToString(mProvider);
        XDoc.Save(FileInfoXDoc.FullName);
        result = FileInfoXDoc.Exists;

        // string response = await UploadFile(FileInfoXDoc.FullName);
      }
      return await Task.FromResult(result);

    }
  
    public string BuildRackScanFileName(ScanType scanType) {
      DateTime creationDate = DateTime.Now;
      string filename = string.Empty;

      switch (scanType) {
        case ScanType.incoming:
          filename = "RAI";
          break;
        case ScanType.outgoing:
          filename = "RAO";
          break;
        case ScanType.register:
          filename = "REG";
          break;
      }

      filename = string.Format("{0}-{1}-{2}.{3}.{4}-{5}.{6}.xml",
                                filename,
                                    mDeviceInfo.DeviceName,
                                        creationDate.Year,
                                             creationDate.Month.ToString().PadLeft(2, '0'),
                                                creationDate.Day.ToString().PadLeft(2, '0'),
                                                     creationDate.ToLongTimeString().Replace(':', '.'),
                                                        creationDate.Millisecond);

      return filename;
    }

   
    public async Task<bool> DownloadFile(string path, string fileName) {
      string url = $"{mBaseAddress.ToString()}downloadfile?fileName={Path.GetFileName(fileName)}";
      HttpClientHandler clientHandler = new HttpClientHandler();
      clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
      bool result = false;

      var client = new HttpClient(clientHandler);
      client.Timeout = new TimeSpan(0, 0, 5);

      try {
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        result = response.StatusCode == System.Net.HttpStatusCode.OK;
        if (response.IsSuccessStatusCode) {
          using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync()) {
            string fileNameToSave = Path.Combine(path, response.Content.Headers.ContentDisposition.FileNameStar);
            FileStream fs = new FileStream(fileNameToSave, FileMode.Create, FileAccess.Write);
            ((MemoryStream)streamToReadFrom).WriteTo(fs);
            fs.Close();
            streamToReadFrom.Dispose();
          }
        }
      }
      catch (Exception) {
        return result;
      }

      return result;
    }

    public async Task<int> UploadFiles() {
      DirectoryInfo di = new(mRackFileDirectory);
      int fileCount = 0;
      foreach (var fi in di.GetFiles()) {
        if (await UploadFile(fi)) {
          fileCount++;
        }else {
          return await Task.FromResult(fileCount);
        }
      }
      return await Task.FromResult(fileCount);
    }

    public async Task<bool> UploadFile(FileInfo fileInfo) {
      HttpClientHandler clientHandler = new HttpClientHandler();
      clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
      bool result = false; 

      var client = new HttpClient(clientHandler) {
        BaseAddress = mBaseAddress
      };
      client.Timeout = new TimeSpan(0, 0, 5);

      await using var stream = System.IO.File.OpenRead(fileInfo.FullName);
      using var request = new HttpRequestMessage(HttpMethod.Post, "uploadfile");
      using var content = new MultipartFormDataContent
      {
        { new StreamContent(stream), "file", Path.GetFileName(fileInfo.FullName) }
      };

      request.Content = content;
      try {
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        result = response.StatusCode == System.Net.HttpStatusCode.OK;
        if (result) {
          fileInfo.Delete();
        }
        var Content = await response.Content.ReadAsStringAsync();
      }
      catch (Exception) {
        return result;
      }
      finally {
      }
      return result;
    }

    public async Task<List<RackScan>> GetMockRackScanList() {
      List<RackScan> rackScanList = new();
      rackScanList.Add(new RackScan() { ItemID = 1, Barcode = "00123", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 08:01:00") });
      rackScanList.Add(new RackScan() { ItemID = 2, Barcode = "00124", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 09:02:00") });
      rackScanList.Add(new RackScan() { ItemID = 3, Barcode = "00125", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 10:03:00") });

      rackScanList.Add(new RackScan() { ItemID = 4, Barcode = "00223", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 08:01:00") });
      rackScanList.Add(new RackScan() { ItemID = 5, Barcode = "00224", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 09:02:00") });
      rackScanList.Add(new RackScan() { ItemID = 6, Barcode = "00225", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 10:03:00") });

      rackScanList.Add(new RackScan() { ItemID = 7, Barcode = "00323", Type = ScanType.register, Scanned = DateTime.Parse("03.08.2022 08:01:00"), OwnerID = 1, OwnerName = "Porta" });
      rackScanList.Add(new RackScan() { ItemID = 8, Barcode = "00324", Type = ScanType.register, Scanned = DateTime.Parse("03.08.2022 09:02:00"), OwnerID = 2, OwnerName = "Schuett" });
      rackScanList.Add(new RackScan() { ItemID = 9, Barcode = "00325", Type = ScanType.register, Scanned = DateTime.Parse("03.08.2022 10:03:00"), OwnerID = 3, OwnerName = "Scholl" });

      rackScanList.Add(new RackScan() { ItemID = 10, Barcode = "00423", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 08:01:00") });
      rackScanList.Add(new RackScan() { ItemID = 11, Barcode = "00424", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 09:02:00") });
      rackScanList.Add(new RackScan() { ItemID = 12, Barcode = "00425", Type = ScanType.incoming, Scanned = DateTime.Parse("01.08.2022 10:03:00") });

      rackScanList.Add(new RackScan() { ItemID = 13, Barcode = "00523", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 08:01:00") });
      rackScanList.Add(new RackScan() { ItemID = 14, Barcode = "00524", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 09:02:00") });
      rackScanList.Add(new RackScan() { ItemID = 15, Barcode = "00525", Type = ScanType.outgoing, Scanned = DateTime.Parse("02.08.2022 10:03:00") });

      return await Task.FromResult(rackScanList);
    }


  }
}


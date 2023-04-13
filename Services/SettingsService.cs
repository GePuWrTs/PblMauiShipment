using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace PblMauiShipment.Services {
  public class SettingsService {
    private string mDataDirectory;
    private FileInfo mSettingsFileInfo;
    private const string mSettingsFileName = @"ShipSettings.json";


    public Settings ShipSettings { get; set; }

    public SettingsService() {
      mDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "data");
      Directory.CreateDirectory(mDataDirectory);
      mSettingsFileInfo = new FileInfo(Path.Combine(mDataDirectory, mSettingsFileName));
      ReadSettings();
    }

    public bool ReadSettings() {
      if (!mSettingsFileInfo.Exists)
        InitSettings();
      mSettingsFileInfo.Refresh();
      if (mSettingsFileInfo.Exists) {
        string jsonStr = File.ReadAllText(mSettingsFileInfo.FullName);
        ShipSettings = JsonSerializer.Deserialize<Settings>(jsonStr);
      }
      return ShipSettings.RackServiceURI.Length > 0;
    }
    public bool SaveSettings() {
      string jsonStr = JsonSerializer.Serialize(ShipSettings);
      File.WriteAllText(mSettingsFileInfo.FullName, jsonStr);
      return ShipSettings.RackServiceURI.Length > 0;
    }

    public bool InitSettings() {
      ShipSettings= new Settings();
      ShipSettings.RackServiceURI = "http://192.168.168.57:5107";
      var options = new JsonSerializerOptions { WriteIndented = true };
      string jsonStr = JsonSerializer.Serialize(ShipSettings);
      File.WriteAllText(mSettingsFileInfo.FullName, jsonStr);
      return ShipSettings.RackServiceURI.Length > 0;
    }

  }
}

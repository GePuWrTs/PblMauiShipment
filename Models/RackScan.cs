
namespace PblMauiShipment.Models {

  public enum ScanType { incoming, outgoing, register }
  public class RackScan {
    public int ItemID { get; set; } = 0;
    public ScanType Type { get; set; } = ScanType.incoming;
    public string Barcode { get; set; } = string.Empty;
    public DateTime Scanned { get; set; } = DateTime.MinValue;
    public int OwnerID { get; set; } = 0;
    public string OwnerName { get; set; } = string.Empty;
  }
}

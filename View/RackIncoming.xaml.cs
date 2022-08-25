using PblMauiShipment.ViewModels;
using ZXing.Net.Maui;

namespace PblMauiShipment.View;

public partial class RackIncoming : ContentPage {
	RackSannsViewModel _viewModel;

	public RackIncoming(RackSannsViewModel rackSannsViewModel) {
		InitializeComponent();
		Title = Properties.Resources.RackScanns;
		BindingContext = _viewModel = rackSannsViewModel;
	}


  private async void barcode_Completed(object sender, EventArgs e) {
		if (barcode.Text != string.Empty) {
			RackScan rs = new();
			rs.Barcode = barcode.Text;
			rs.Scanned = DateTime.Now;
			rs.Type = ScanType.incoming;
			barcode.Text = string.Empty;
			await _viewModel.AddRackScannCommand.ExecuteAsync(rs);
		}
	}

	private void barcode_TextChanged(object sender, TextChangedEventArgs e) {
	}

}
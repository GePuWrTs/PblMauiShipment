namespace PblMauiShipment.View;

public partial class SettingPage : ContentPage
{
	SettingsService mSettingsService = new();
	public SettingPage()
	{
		InitializeComponent();
		server_uri.Text = mSettingsService.ShipSettings.RackServiceURI;
  }

  private void server_uri_TextChanged(object sender, TextChangedEventArgs e)
  {
    mSettingsService.ShipSettings.RackServiceURI = server_uri.Text;
    mSettingsService.SaveSettings();
  }
}
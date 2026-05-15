using ZXing.Net.Maui.Controls;

namespace PblMauiShipment;

public static class MauiProgram {
  public static MauiApp CreateMauiApp() {
    var builder = MauiApp.CreateBuilder();
    builder
      .UseMauiApp<App>()
      .UseBarcodeReader()
      .ConfigureFonts(fonts => {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
      });

    builder.Services.AddSingleton<RackScanService>();
    builder.Services.AddSingleton<RackScanIncomingViewModel>();
    builder.Services.AddSingleton<RackScanRegisterViewModel>();
    builder.Services.AddTransient<MainPage>();
    builder.Services.AddTransient<SettingPage>();

    builder.Services.AddTransient<RackIncoming>();
    builder.Services.AddTransient<RackOutgoing>();
    builder.Services.AddTransient<RackRegister>();
    builder.Services.AddTransient<ZXingBarcodeReader>();

    return builder.Build();
  }
}

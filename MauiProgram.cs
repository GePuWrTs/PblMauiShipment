using ZXing.Net.Maui;

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
      })
      .ConfigureMauiHandlers(h => {
        h.AddHandler(typeof(ZXing.Net.Maui.Controls.CameraBarcodeReaderView),
          typeof(CameraBarcodeReaderViewHandler));
        h.AddHandler(typeof(ZXing.Net.Maui.Controls.CameraView),
          typeof(CameraViewHandler));
        h.AddHandler(typeof(ZXing.Net.Maui.Controls.BarcodeGeneratorView),
          typeof(BarcodeGeneratorViewHandler));
      });

    builder.Services.AddSingleton<RackScanService>();
    builder.Services.AddSingleton<RackSannsViewModel>();
    builder.Services.AddTransient<MainPage>();

    builder.Services.AddTransient<RackIncoming>();
    builder.Services.AddTransient<RackOutgoing>();
    builder.Services.AddTransient<RackRegister>();
    builder.Services.AddTransient<ZXingBarcodeReader>();

    return builder.Build();
  }
}

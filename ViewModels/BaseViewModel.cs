
using PblMauiShipment.Models;
using System.Reflection;

namespace PblMauiShipment.ViewModels {
  public partial class BaseViewModel : ObservableObject {
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string title;

    public bool IsNotBusy => !IsBusy;


    public async Task PlaySound(string fileName) {
      try {
        using (Stream audioStream = await FileSystem.OpenAppPackageFileAsync($"{fileName}")) {
          if (audioStream != null) {
            using (var audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer()) {
              if (audio != null) {
                audio.Load(audioStream);
                audio.Play();
                while (audio.IsPlaying) {
                  Task.Delay(10).Wait();
                }
              }
            }
            audioStream.Close();
          }
        }
      }
      catch (Exception ex) {
        Debug.WriteLine($"Unable play sound: {ex.Message}");
        await Shell.Current.DisplayAlert("Error!", ex.ToString(), "OK");
      }
    }
  }
}

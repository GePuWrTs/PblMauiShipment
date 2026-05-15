using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Maui.Audio;

namespace PblMauiShipment.ViewModels {
  public partial class BaseViewModel : ObservableObject {
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string title;


    public bool IsNotBusy => !IsBusy;


    [RelayCommand]
    public async Task PlayBeepAsync() {
      await PlaySound("Resources/Audio/beep.mp3");
    }
    [RelayCommand]
    public async Task PlayErrorAsync() {
      await PlaySound("Resources/Audio/error.mp3");
    }

        protected async Task<bool> ConfirmSendFilesAsync(int pendingFileCount, int scanCount)
        {
            if (pendingFileCount == 0 && scanCount == 0)
            {
                return true;
            }

            string message = pendingFileCount > 0
              ? $"Es sind {pendingFileCount} Datei(en) zum Senden vorgemerkt. Jetzt senden?"
              : $"Aus {scanCount} Scan(s) wird eine Datei erstellt und gesendet. Jetzt senden?";

            return await Shell.Current.DisplayAlert("Dateien senden", message, "Senden", "Abbrechen");
        }

        public async Task PlaySound(string fileName) {
      try {
        await using Stream audioStream = await FileSystem.OpenAppPackageFileAsync(fileName);
        using IAudioPlayer audioPlayer = AudioManager.Current.CreatePlayer(audioStream);

        audioPlayer.Play();

        while (audioPlayer.IsPlaying) {
          await Task.Delay(10);
        }
      }
      catch (Exception ex) {
        Debug.WriteLine($"Unable play sound: {ex.Message}");
        await Shell.Current.DisplayAlert("Error!", ex.ToString(), "OK");
      }
    }

  }
}

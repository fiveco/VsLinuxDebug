using System.ComponentModel;
using System.Windows;
using Microsoft.VisualStudio.Shell;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  /// <summary>Remote X11 display settings.</summary>
  public class DisplayOptionsPage : UIElementDialogPage, INotifyPropertyChanged
  {
    private bool _remoteDebugDisplayGui = false;
    private string _remoteDebugDisplayNumber = ":0";

    public event PropertyChangedEventHandler PropertyChanged;

    [Category("Remote X11 Display")]
    [DisplayName("Debug Display GUI")]
    [Description(
      "Display application on remote machine. This is helpful for debugging " +
      "GUI applications on remote devices.")]
    public bool RemoteDebugDisplayGui
    {
      get => _remoteDebugDisplayGui;
      set { _remoteDebugDisplayGui = value; OnPropertyChanged(nameof(RemoteDebugDisplayGui)); }
    }

    [Category("Remote X11 Display")]
    [DisplayName("Remote X11 Display Number (optional)")]
    [Description("Remote X11 Display number (only if it was set). Defaults to ':0'.")]
    public string RemoteDebugDisplayNumber
    {
      get => _remoteDebugDisplayNumber;
      set { _remoteDebugDisplayNumber = value; OnPropertyChanged(nameof(RemoteDebugDisplayNumber)); }
    }

    protected override UIElement Child => new DisplayOptionsControl(this);

    private void OnPropertyChanged(string propertyName)
      => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}

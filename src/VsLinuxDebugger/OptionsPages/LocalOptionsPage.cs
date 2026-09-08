using System.ComponentModel;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using VsLinuxDebugger;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  /// <summary>Local-machine settings.</summary>
  public class LocalOptionsPage : UIElementDialogPage, INotifyPropertyChanged
  {
    private string _plinkPath = "";
    private bool _deleteLaunchJsonAfterBuild = false;
    private bool _autoSwitchLinuxDbgOutput = false;

    public event PropertyChangedEventHandler PropertyChanged;

    [Category("Local Settings")]
    [DisplayName("PLink Local Path (blank to use embedded)")]
    [Description(@"Full path to local PLINK.EXE file. (i.e. 'C:\temp\putty\plink.exe')")]
    public string PLinkPath
    {
      get => _plinkPath;
      set { _plinkPath = value; OnPropertyChanged(nameof(PLinkPath)); }
    }

    [Category("Local Settings")]
    [DisplayName("Delete 'launch.json' after build.")]
    [Description(@"The `launch.json` is generated in your build folder. You may keep this for debugging.")]
    public bool DeleteLaunchJsonAfterBuild
    {
      get => _deleteLaunchJsonAfterBuild;
      set { _deleteLaunchJsonAfterBuild = value; OnPropertyChanged(nameof(DeleteLaunchJsonAfterBuild)); }
    }

    [Category("Local Settings")]
    [DisplayName("Switch to LinuxDbg Output on Build")]
    [Description("Automatically show output for Linux Debugger on build (default = false).")]
    public bool SwitchLinuxDbgOutput
    {
      get => _autoSwitchLinuxDbgOutput;
      set
      {
        Logger.AutoSwitchToLinuxDbgOutput = value;
        _autoSwitchLinuxDbgOutput = value;
        OnPropertyChanged(nameof(SwitchLinuxDbgOutput));
      }
    }

    protected override UIElement Child => new LocalOptionsControl(this);

    private void OnPropertyChanged(string propertyName)
      => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}

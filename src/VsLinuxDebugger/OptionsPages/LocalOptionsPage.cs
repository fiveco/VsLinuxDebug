using System.Windows;
using VsLinuxDebugger;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  public class LocalOptionsPage : OptionsPageBase
  {
    private bool _useSSHExeEnabled = true;
    private string _plinkPath = "";
    private bool _deleteLaunchJsonAfterBuild = false;
    private bool _switchLinuxDbgOutput = false;

    public bool UseSSHExeEnabled { get => _useSSHExeEnabled; set => SetField(ref _useSSHExeEnabled, value); }

    public string PLinkPath { get => _plinkPath; set => SetField(ref _plinkPath, value); }

    public bool DeleteLaunchJsonAfterBuild { get => _deleteLaunchJsonAfterBuild; set => SetField(ref _deleteLaunchJsonAfterBuild, value); }

    public bool SwitchLinuxDbgOutput
    {
      get => _switchLinuxDbgOutput;
      set
      {
        Logger.AutoSwitchToLinuxDbgOutput = value;
        SetField(ref _switchLinuxDbgOutput, value);
      }
    }

    protected override UIElement Child => new LocalOptionsControl(this);
  }
}

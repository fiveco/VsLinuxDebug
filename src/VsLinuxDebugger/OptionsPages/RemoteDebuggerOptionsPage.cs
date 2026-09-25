using System.Windows;
using VsLinuxDebugger.Core;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  public class RemoteDebuggerOptionsPage : OptionsPageBase
  {
    private string _remoteDeployBasePath = "./VSLinuxDbg";
    private string _remoteDotNetPath = Constants.DefaultDotNetPath;
    private string _remoteVsDbgRootPath = Constants.DefaultVsdbgBasePath;
    private bool _remoteDebugDisplayGui = false;
    private string _remoteDebugDisplayNumber = ":0";

    public string RemoteDeployBasePath { get => _remoteDeployBasePath; set => SetField(ref _remoteDeployBasePath, value); }

    public string RemoteDotNetPath { get => _remoteDotNetPath; set => SetField(ref _remoteDotNetPath, value); }

    public string RemoteVsDbgRootPath { get => _remoteVsDbgRootPath; set => SetField(ref _remoteVsDbgRootPath, value); }

    public bool RemoteDebugDisplayGui { get => _remoteDebugDisplayGui; set => SetField(ref _remoteDebugDisplayGui, value); }

    public string RemoteDebugDisplayNumber { get => _remoteDebugDisplayNumber; set => SetField(ref _remoteDebugDisplayNumber, value); }

    protected override UIElement Child => new RemoteDebuggerOptionsControl(this);
  }
}

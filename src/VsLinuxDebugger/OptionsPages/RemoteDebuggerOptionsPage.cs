using System.ComponentModel;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using VsLinuxDebugger.Core;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  /// <summary>Remote deployment/debugger configuration.</summary>
  public class RemoteDebuggerOptionsPage : UIElementDialogPage, INotifyPropertyChanged
  {
    private string _remoteDeployBasePath = "./VSLinuxDbg";
    private string _remoteEnvironmentVariables = string.Empty;
    private string _remoteServiceName = string.Empty;
    private string _remoteDotNetPath = Constants.DefaultDotNetPath;
    private string _remoteVsDbgRootPath = Constants.DefaultVsdbgBasePath;
    private bool _useSudoForDebugger = false;
    private string _sudoCommand = Constants.DefaultSudoCommand;
    private bool _useSelfContainedDeployment = false;

    public event PropertyChangedEventHandler PropertyChanged;

    [Category("Remote Debugger")]
    [DisplayName("Upload to folder")]
    [Description("Folder for to transfer files to. For HOME folder, use './VSLinuxDbg' and not '~/VSLinuxDbg'")]
    public string RemoteDeployBasePath
    {
      get => _remoteDeployBasePath;
      set { _remoteDeployBasePath = value; OnPropertyChanged(nameof(RemoteDeployBasePath)); }
    }

    [Category("Remote Debugger")]
    [DisplayName("Environment Variables")]
    [Description(
      "Environment variables passed to the debuggee, one 'KEY=VALUE' pair per line. " +
      "Useful when the program reads required configuration from the environment " +
      "(i.e. values normally supplied by systemd's EnvironmentFile).")]
    public string RemoteEnvironmentVariables
    {
      get => _remoteEnvironmentVariables;
      set { _remoteEnvironmentVariables = value; OnPropertyChanged(nameof(RemoteEnvironmentVariables)); }
    }

    [Category("Remote Debugger")]
    [DisplayName("Service Name (optional)")]
    [Description(
      "Name of a systemd unit (without '.service') that normally runs the debuggee, i.e. " +
      "'myapp' for 'myapp.service'. When set, it is stopped (via 'sudo systemctl stop " +
      "<name>.service') before files are deployed, and started (via 'sudo systemctl start " +
      "<name>.service') before launch/attach, so the debuggee isn't fought over by a " +
      "supervisor restarting it underneath the debugger. Requires the configured user to " +
      "be able to run systemctl on this unit via sudo. Leave blank to disable.")]
    public string RemoteServiceName
    {
      get => _remoteServiceName;
      set { _remoteServiceName = value; OnPropertyChanged(nameof(RemoteServiceName)); }
    }

    [Category("Remote Debugger")]
    [DisplayName(".NET executable")]
    [Description("Path of the .NET executable on remote machine. (Samples: `dotnet`, `~/.dotnet/dotnet`)")]
    public string RemoteDotNetPath
    {
      get => _remoteDotNetPath;
      set { _remoteDotNetPath = value; OnPropertyChanged(nameof(RemoteDotNetPath)); }
    }

    [Category("Remote Debugger")]
    [DisplayName("Visual Studio Debugger Path")]
    [Description(
      "Root folder of Visual Studio Debugger. " +
      "(Samples: `~/.vs-debugger/`, `~/.vsdbg`)")]
    public string RemoteVsDbgRootPath
    {
      get => _remoteVsDbgRootPath;
      set { _remoteVsDbgRootPath = value; OnPropertyChanged(nameof(RemoteVsDbgRootPath)); }
    }

    [Category("Remote Debugger")]
    [DisplayName("Use Sudo for Debugger")]
    [Description(
      "Launch VSDBG on the remote machine via the command in 'Sudo Command'. " +
      "Use this when the debuggee process runs with elevated or ambient capabilities " +
      "that the debugger must also hold in order to attach (i.e. ptrace requires the " +
      "tracer's capabilities to be a superset of the tracee's). The configured user " +
      "must be able to run the debugger path via sudo (passwordless or otherwise).")]
    public bool UseSudoForDebugger
    {
      get => _useSudoForDebugger;
      set { _useSudoForDebugger = value; OnPropertyChanged(nameof(UseSudoForDebugger)); }
    }

    [Category("Remote Debugger")]
    [DisplayName("Sudo Command")]
    [Description(
      "Command used to elevate the debugger when 'Use Sudo for Debugger' is enabled. " +
      "'-E' preserves the environment (VSDBG needs it); '-n' fails fast instead of " +
      "prompting for a password. (Default: `sudo -n -E`)")]
    public string SudoCommand
    {
      get => _sudoCommand;
      set { _sudoCommand = value; OnPropertyChanged(nameof(SudoCommand)); }
    }

    [Category("Remote Debugger")]
    [DisplayName("Use Self-Contained Deployment")]
    [Description(
      "Launch the deployed program directly as a native executable instead of via " +
      "'dotnet <assembly>.dll'. Use this when the project is published self-contained " +
      "(or AOT) for the remote machine's runtime identifier.")]
    public bool UseSelfContainedDeployment
    {
      get => _useSelfContainedDeployment;
      set { _useSelfContainedDeployment = value; OnPropertyChanged(nameof(UseSelfContainedDeployment)); }
    }

    protected override UIElement Child => new RemoteDebuggerOptionsControl(this);

    private void OnPropertyChanged(string propertyName)
      => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}

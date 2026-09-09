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
    private string _remotePreDeployCommands = string.Empty;
    private string _remotePostDeployCommands = string.Empty;
    private bool _attachToRunningProcess = false;
    private string _remotePidCommand = string.Empty;
    private string _remoteDotNetPath = Constants.DefaultDotNetPath;
    private string _remoteVsDbgRootPath = Constants.DefaultVsdbgBasePath;
    private bool _useSudoForDebugger = false;
    private string _sudoCommand = Constants.DefaultSudoCommand;
    private bool _useSelfContainedDeployment = false;
    private string _remoteRuntimeIdentifier = "linux-arm64";

    public event PropertyChangedEventHandler PropertyChanged;

    [Category("Remote Debugger")]
    [DisplayName("Upload to folder")]
    [Description(
      "Folder files are deployed to, as-is (no per-project subfolder is added). For HOME " +
      "folder, use './VSLinuxDbg' and not '~/VSLinuxDbg'")]
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
    [DisplayName("Pre-Deploy Commands")]
    [Description(
      "Shell commands run on the remote machine before files are uploaded, one per line " +
      "(only when 'Deploy' runs). Use this to stop whatever is holding the deployed files " +
      "open, i.e. 'sudo systemctl stop myapp.service'. Leave blank to run nothing.")]
    public string RemotePreDeployCommands
    {
      get => _remotePreDeployCommands;
      set { _remotePreDeployCommands = value; OnPropertyChanged(nameof(RemotePreDeployCommands)); }
    }

    [Category("Remote Debugger")]
    [DisplayName("Post-Deploy Commands")]
    [Description(
      "Shell commands run on the remote machine after files are uploaded, one per line " +
      "(only when 'Deploy' runs). Use this to bring the debuggee back up under its normal " +
      "supervised environment, i.e. 'sudo systemctl reset-failed myapp.service' and " +
      "'sudo systemctl start myapp.service'. Leave blank to run nothing.")]
    public string RemotePostDeployCommands
    {
      get => _remotePostDeployCommands;
      set { _remotePostDeployCommands = value; OnPropertyChanged(nameof(RemotePostDeployCommands)); }
    }

    [Category("Remote Debugger")]
    [DisplayName("Attach to Already-Running Process")]
    [Description(
      "The debuggee is started/supervised externally (i.e. by systemd via the commands " +
      "above), so debugging must attach to its existing process instead of vsdbg launching " +
      "a new one -- a launched instance would not inherit the supervisor's EnvironmentFile " +
      "or ambient capabilities, and would leave two copies of the program running. When " +
      "unchecked (default), vsdbg launches and owns the debuggee process itself.")]
    public bool AttachToRunningProcess
    {
      get => _attachToRunningProcess;
      set { _attachToRunningProcess = value; OnPropertyChanged(nameof(AttachToRunningProcess)); }
    }

    [Category("Remote Debugger")]
    [DisplayName("PID Command")]
    [Description(
      "Shell command, run on the remote machine, whose output is the PID to attach to. " +
      "Only used when 'Attach to Already-Running Process' is checked. Examples: " +
      "'systemctl show myapp.service --property=MainPID --value', or 'pgrep -f myapp'.")]
    public string RemotePidCommand
    {
      get => _remotePidCommand;
      set { _remotePidCommand = value; OnPropertyChanged(nameof(RemotePidCommand)); }
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

    [Category("Remote Debugger")]
    [DisplayName("Remote Runtime Identifier")]
    [Description(
      "The .NET Runtime Identifier (RID) to publish for when 'Use Self-Contained " +
      "Deployment' is enabled, matching the remote machine's OS/architecture (i.e. " +
      "`linux-arm64`, `linux-x64`, `linux-arm`).")]
    public string RemoteRuntimeIdentifier
    {
      get => _remoteRuntimeIdentifier;
      set { _remoteRuntimeIdentifier = value; OnPropertyChanged(nameof(RemoteRuntimeIdentifier)); }
    }

    protected override UIElement Child => new RemoteDebuggerOptionsControl(this);

    private void OnPropertyChanged(string propertyName)
      => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}

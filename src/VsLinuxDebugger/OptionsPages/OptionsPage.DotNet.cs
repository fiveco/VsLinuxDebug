using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using VsLinuxDebugger.Core;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  public partial class OptionsPage : DialogPage
  {
    private const string Experimental = "Warning Experimental";
    private const string RemoteDebugger = "Remote Debugger";

    [Category(Experimental)]
    [DisplayName("Debug Display GUI")]
    [Description(
      "Display application on remote machine. This is helpful for debugging " +
      "GUI applications on remote devices.")]
    public bool RemoteDebugDisplayGui { get; set; } = false;

    [Category(RemoteDebugger)]
    [DisplayName("Upload to folder")]
    [Description("Folder for to transfer files to. For HOME folder, use './VSLinuxDbg' and not '~/VSLinuxDbg'")]
    public string RemoteDeployBasePath { get; set; } = $"./VSLinuxDbg"; // "LinuxDbg"

    [Category(RemoteDebugger)]
    [DisplayName(".NET executable")]
    [Description("Path of the .NET executable on remote machine. (Samples: `dotnet`, `~/.dotnet/dotnet`)")]
    public string RemoteDotNetPath { get; set; } = Constants.DefaultDotNetPath;

    [Category(RemoteDebugger)]
    [DisplayName("Visual Studio Debugger Path")]
    [Description(
      "Root folder of Visual Studio Debugger. " +
      "(Samples: `~/.vs-debugger/`, `~/.vsdbg`)")]
    public string RemoteVsDbgRootPath { get; set; } = Constants.DefaultVsdbgBasePath;

    [Category(RemoteDebugger)]
    [DisplayName("Use Sudo for Debugger")]
    [Description(
      "Launch VSDBG on the remote machine via the command in 'Sudo Command'. " +
      "Use this when the debuggee process runs with elevated or ambient capabilities " +
      "that the debugger must also hold in order to attach (i.e. ptrace requires the " +
      "tracer's capabilities to be a superset of the tracee's). The configured user " +
      "must be able to run the debugger path via sudo (passwordless or otherwise).")]
    public bool UseSudoForDebugger { get; set; } = false;

    [Category(RemoteDebugger)]
    [DisplayName("Sudo Command")]
    [Description(
      "Command used to elevate the debugger when 'Use Sudo for Debugger' is enabled. " +
      "'-E' preserves the environment (VSDBG needs it); '-n' fails fast instead of " +
      "prompting for a password. (Default: `sudo -n -E`)")]
    public string SudoCommand { get; set; } = Constants.DefaultSudoCommand;

    [Category(RemoteDebugger)]
    [DisplayName("Use Self-Contained Deployment")]
    [Description(
      "Launch the deployed program directly as a native executable instead of via " +
      "'dotnet <assembly>.dll'. Use this when the project is published self-contained " +
      "(or AOT) for the remote machine's runtime identifier.")]
    public bool UseSelfContainedDeployment { get; set; } = false;

    [Category(Experimental)]
    [DisplayName("Use Command Line Arguments")]
    [Description(
      "Apply command line arguments from Visual Studio Project Settings. " +
      "(Experimental : Project Settings -> Debugging -> Command Line Arguments)")]
    public bool UseCommandLineArgs { get; set; } = false;
  }
}

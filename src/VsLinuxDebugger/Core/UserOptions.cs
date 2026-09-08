namespace VsLinuxDebugger.Core
{
  public class UserOptions
  {
    public bool DeleteLaunchJsonAfterBuild { get; set; }

    public string HostIp { get; set; }
    public int HostPort { get; set; }

    public bool LocalPlinkEnabled { get; set; }
    public string LocalPLinkPath { get; set; }
    public bool LocalSwitchLinuxDbgOutput { get; set; }

    public bool RemoteDebugDisplayGui { get; set; }
    public string RemoteDebugDisplayNumber { get; set; }
    public string RemoteDeployBasePath { get; set; }

    /// <summary>Environment variables to pass to the debuggee, one `KEY=VALUE` pair per line.</summary>
    public string RemoteEnvironmentVariables { get; set; }
    /// <summary>Full path to `dotnet` executable.</summary>
    public string RemoteDotNetPath { get; set; }
    /// <summary>Base path to VSDBG (i.e. `~/.vsdbg`).</summary>
    public string RemoteVsDbgBasePath { get; set; }
    /// <summary>Full path to VS Debugger.</summary>
    public string RemoteVsDbgFullPath => LinuxPath.Combine(RemoteVsDbgBasePath, Constants.VS2022, Constants.AppVSDbg);

    public bool UseCommandLineArgs { get; set; }
    public bool UsePublish { get; set; }

    /// <summary>When enabled, launches the deployed program directly as a native executable
    /// (i.e. a self-contained/AOT publish) instead of via `dotnet &lt;assembly&gt;.dll`.</summary>
    public bool UseSelfContainedDeployment { get; set; } = false;

    public string UserGroupName { get; set; }
    public string UserName { get; set; }
    public string UserPass { get; set; }
    public bool UserPrivateKeyEnabled { get; set; }
    public string UserPrivateKeyPath { get; set; }
    public string UserPrivateKeyPassword { get; set; }

    public bool UseSSHExeEnabled { get; set; } = false;

    /// <summary>Command used to elevate the debugger process on the remote machine (i.e. `sudo -n -E`).</summary>
    public string SudoCommand { get; set; } = Constants.DefaultSudoCommand;

    /// <summary>When enabled, launches VSDBG via <see cref="SudoCommand"/> on the remote machine.
    /// Use this when the debuggee runs with elevated/ambient capabilities the debugger must match to attach.</summary>
    public bool UseSudoForDebugger { get; set; } = false;
  }
}

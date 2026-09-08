namespace VsLinuxDebugger.Core
{
  public static class Constants
  {
    /// <summary>Filename of Visual Studio Debugger.</summary>
    public const string AppVSDbg = "vsdbg";

    public const string DefaultDotNetPath = "dotnet";
    public const string VS2022 = "vs2022";
    public const string DefaultVsdbgBasePath = "~/.vs-debugger";
    public const string LaunchJson = "launch.json";

    /// <summary>Default command used to elevate the debugger when <c>UseSudoForDebugger</c> is enabled.</summary>
    public const string DefaultSudoCommand = "sudo -n -E";

    public const string PackageTarGz = "vsldBuildContents.tar.gz";
  }
}

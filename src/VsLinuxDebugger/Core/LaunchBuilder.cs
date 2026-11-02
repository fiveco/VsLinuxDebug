using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Security;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using VsLinuxDebugger.Core.Remote;

namespace VsLinuxDebugger.Core
{
  /// <summary>LaunchBuilder class for serialization.</summary>
  public class LaunchBuilder
  {
    public const string AdapterFileName = "launch.json";

    private UserOptions _opts;

    public LaunchBuilder(DTE2 dte, Project dteProject, UserOptions userOptions)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      _opts = userOptions;

      AssemblyName = dteProject.Properties.Item("AssemblyName").Value.ToString();
      ProjectConfigName = dteProject.ConfigurationManager.ActiveConfiguration.ConfigurationName;
      ProjectPlatformName = dteProject.ConfigurationManager.ActiveConfiguration.PlatformName;
      ProjectFileFullPath = dteProject.FullName;
      ProjectName = dteProject.Name;
      SolutionFileFullPath = dte.Solution.FullName;
      SolutionDirPath = Path.GetDirectoryName(dte.Solution.FullName);
      OutputDirName = dteProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
      OutputDirFullPath = Path.Combine(Path.GetDirectoryName(dteProject.FullName), OutputDirName);
      PublishDirFullPath = Path.Combine(Path.GetTempPath(), "VsLinuxDebuggerPublish", ProjectName);

      ReadSolutionConfiguration(dte, dteProject);
    }

    /// <summary>One project's mapping in the active solution configuration.</summary>
    public class ProjectConfigEntry
    {
      public Guid? ProjectGuid { get; set; }
      public string AbsolutePath { get; set; }
      public string ConfigName { get; set; }
      public string PlatformName { get; set; }
      public bool ShouldBuild { get; set; }
    }

    /// <summary>Active solution configuration name. I.E. "Debug".</summary>
    public string SolutionConfigName { get; set; }

    /// <summary>Active solution platform name. I.E. "Any CPU".</summary>
    public string SolutionPlatformName { get; set; }

    /// <summary>Startup project's platform in the active solution configuration. I.E. "Any CPU".</summary>
    public string ProjectPlatformName { get; set; }

    /// <summary>Per-project mapping of the active solution configuration.</summary>
    public List<ProjectConfigEntry> ProjectConfigEntries { get; } = new List<ProjectConfigEntry>();

    /// <summary>MSBuild `CurrentSolutionConfigurationContents` XML of the active solution configuration.</summary>
    public string SolutionConfigurationContents { get; set; } = string.Empty;

    /// <summary>"Any CPU" -> "AnyCPU" (MSBuild spelling).</summary>
    public static string NormalizePlatform(string platform) => (platform ?? string.Empty).Replace(" ", string.Empty);

    /// <summary>Builds the MSBuild `CurrentSolutionConfigurationContents` XML from the entries.</summary>
    public static string BuildSolutionConfigurationXml(IEnumerable<ProjectConfigEntry> entries)
    {
      var sb = new StringBuilder("<SolutionConfiguration>");

      foreach (var e in entries)
      {
        sb.Append("<ProjectConfiguration");
        if (e.ProjectGuid.HasValue)
          sb.Append($" Project=\"{e.ProjectGuid.Value.ToString("B").ToUpperInvariant()}\"");
        sb.Append($" AbsolutePath=\"{SecurityElement.Escape(e.AbsolutePath)}\"");
        sb.Append($" BuildProjectInSolution=\"{(e.ShouldBuild ? "True" : "False")}\">");
        sb.Append(SecurityElement.Escape($"{e.ConfigName}|{NormalizePlatform(e.PlatformName)}"));
        sb.Append("</ProjectConfiguration>");
      }

      return sb.Append("</SolutionConfiguration>").ToString();
    }

    /// <summary>Reads the active solution configuration and the startup project's mapping from it.</summary>
    private void ReadSolutionConfiguration(DTE2 dte, Project dteProject)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      var active = (SolutionConfiguration2)dte.Solution.SolutionBuild.ActiveConfiguration;
      if (active == null)
        return;

      SolutionConfigName = active.Name;
      SolutionPlatformName = active.PlatformName;

      var vsSolution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;

      foreach (SolutionContext ctx in active.SolutionContexts)
      {
        var path = Path.GetFullPath(Path.Combine(SolutionDirPath, ctx.ProjectName));
        Guid? guid = null;

        if (vsSolution != null
          && vsSolution.GetProjectOfUniqueName(ctx.ProjectName, out var hierarchy) == 0 && hierarchy != null)
        {
          if (vsSolution.GetGuidOfProject(hierarchy, out var projectGuid) == 0)
            guid = projectGuid;

          // Real path on disk (casing matters for the MSBuild lookup).
          if (hierarchy is IVsProject vsProject
            && vsProject.GetMkDocument((uint)VSConstants.VSITEMID.Root, out var mkDocument) == 0
            && !string.IsNullOrEmpty(mkDocument))
            path = mkDocument;
        }

        ProjectConfigEntries.Add(new ProjectConfigEntry
        {
          ProjectGuid = guid,
          AbsolutePath = path,
          ConfigName = ctx.ConfigurationName,
          PlatformName = ctx.PlatformName,
          ShouldBuild = ctx.ShouldBuild,
        });

        if (string.Equals(ctx.ProjectName, dteProject.UniqueName, StringComparison.OrdinalIgnoreCase))
        {
          ProjectConfigName = ctx.ConfigurationName;
          ProjectPlatformName = ctx.PlatformName;
        }
      }

      SolutionConfigurationContents = BuildSolutionConfigurationXml(ProjectConfigEntries);
    }

    /// <summary>Folder `dotnet publish` writes to for a self-contained deployment. A temp folder,
    /// not under the project's own `bin`/`obj`, so it never collides with the IDE's own build
    /// output and is safe to wipe before each publish.</summary>
    public string PublishDirFullPath { get; set; }

    /// <summary>Project assembly name. I.E. "ConsoleApp1"</summary>
    public string AssemblyName { get; set; }

    public string CommandLineArgs { get; set; } = string.Empty;

    /// <summary>Remove the `launch.json` file after building. Keep it around for debugging.</summary>
    public bool DeleteLaunchJsonAfterBuild => _opts.DeleteLaunchJsonAfterBuild;

    /// <summary>Full output folder path. I.E. "C:\\path\\Repos\\Porj\\bin\\Debug\\net6.0\\".</summary>
    public string OutputDirFullPath { get; set; }

    /// <summary>Partial path to the output directory. I.E. "bin\\Debug\\net6.0".</summary>
    public string OutputDirName { get; set; }

    /// <summary>Configuration build type. I.E. "Debug".</summary>
    public string ProjectConfigName { get; set; }

    /// <summary>Full project output path. I.E. "C:\\path\\Repos\\Proj\\ConsoleApp1.csproj"</summary>
    public string ProjectFileFullPath { get; set; }

    /// <summary>Project name (not always the same as AssemblyName). I.E. "Console App1"</summary>
    public string ProjectName { get; set; }

    /// <summary>Full path to the deployed native executable (always produced by `dotnet
    /// publish`, even framework-dependent). (i.e. `/home/USER/VLSDbg/ConsoleApp1`)</summary>
    public string RemoteDeployExecutableFilePath => LinuxPath.Combine(RemoteDeployProjectFolder, AssemblyName);

    /// <summary>Folder files are deployed to. This is the configured deploy path itself
    /// (i.e. `/home/USER/VLSDbg`) -- no per-project subfolder is added.</summary>
    public string RemoteDeployProjectFolder => _opts.RemoteDeployBasePath;

    public string RemoteHostIp => _opts.HostIp;

    public int RemoteHostPort => _opts.HostPort;

    public string RemoteUserName => _opts.UserName;

    public string RemoteUserPass => _opts.UserPass;

    /// <summary>Solution folder path. I.E. "C:\\path\Repos\"</summary>
    public string SolutionDirPath { get; set; }

    /// <summary>Full solution output path. I.E. "C:\\path\Repos\Proj.sln"</summary>
    public string SolutionFileFullPath { get; set; }

    /// <summary>Generates the project's `launch.json` file.</summary>
    /// <returns>Returns the local path to the file.</returns>
    public string GenerateLaunchJson(bool vsdbgLogging = false)
    {
      string adapter, adapterArgs;

      (adapter, adapterArgs) = GetAdapter(vsdbgLogging);

      // Always deployed via `dotnet publish`, which generates a native apphost executable
      // even when framework-dependent -- so launch it directly, never via `dotnet <dll>`.
      var obj = new Launch(
          RemoteDeployExecutableFilePath,
          new string[0],
          RemoteDeployProjectFolder,
          ParseEnvironmentVariables(_opts.RemoteEnvironmentVariables),
          false)
      {
        Adapter = adapter,
        AdapterArgs = adapterArgs,
      };

      return WriteLaunchJson(obj);
    }

    /// <summary>Generates a `launch.json` that attaches to an already-running remote process
    /// (i.e. one managed by a systemd service), instead of launching a new one.</summary>
    /// <param name="processId">Remote process ID to attach to.</param>
    /// <returns>Returns the local path to the file.</returns>
    public string GenerateAttachLaunchJson(int processId, bool vsdbgLogging = false)
    {
      string adapter, adapterArgs;

      (adapter, adapterArgs) = GetAdapter(vsdbgLogging);

      var obj = Launch.CreateAttach(processId);
      obj.Adapter = adapter;
      obj.AdapterArgs = adapterArgs;

      return WriteLaunchJson(obj);
    }

    private string WriteLaunchJson(Launch obj)
    {
      var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
      });

      // Create out file
      var outputPath = Path.Combine(OutputDirFullPath, "launch.json");

      try
      {
        File.WriteAllText(outputPath, json);
      }
      catch (Exception ex)
      {
        Logger.Output($"Error writing 'launch.json' to path, '{outputPath}'!\n{ex.Message}");
        outputPath = string.Empty;
      }

      return outputPath;
    }

    private (string adapterPath, string adapterArgs) GetAdapter(bool vsdbgLogging = false)
    {
      // NOTE: Removed ":{RemoteHostPort}" because it failed to launch with PLink
      // var sshEndpoint = $"{_opts.UserName}@{_opts.HostIp}:{_opts.HostPort}";
      var sshEndpoint = $"{RemoteUserName}@{RemoteHostIp}";

      var vsdbgLogPath = "";
      if (vsdbgLogging)
        vsdbgLogPath = $" --engineLogging={LinuxPath.Combine(RemoteDeployProjectFolder, "_vsdbg.log")}";

      ////if (!_opts.LocalPlinkEnabled)
      ////{
      ////  adapter = "ssh.exe";
      ////  adapterArgs = $"{sshPassword} {sshEndpoint} {_opts.RemoteVsDbgPath} --interpreter=vscode {vsdbgLogPath}";
      ////}
      ////else
      ////{

      string plinkPath = string.Empty;

      if (_opts.UseSSHExeEnabled)
      {
        plinkPath = "ssh.exe";
      }
      else
      {
        // Adapter Path:
        // PLink.exe - Use manual path or embedded
        if (!string.IsNullOrEmpty(_opts.LocalPLinkPath) && File.Exists(_opts.LocalPLinkPath))
        {
          plinkPath = _opts.LocalPLinkPath;
        }
        else
        {
          plinkPath = Path.Combine(GetExtensionDirectory(), "plink.exe").Trim('"');
        }
      }

      // Adapter Arguments:
      // NOTE:
      //  1. SSH Private Key ("-i PPK") fails with PLINK; PuTTY/plink requires its own
      //     ".ppk" key format, not an OpenSSH-format private key. Use ssh.exe (below) for
      //     OpenSSH-format keys, or convert the key to PPK for use with PLINK.
      //  2. Strict Host Key Checking is disabled by default; this doesn't need set.
      //
      // REF: https://linuxhint.com/ssh-stricthostkeychecking/
      string sshPassword = "";

      if (_opts.UseSSHExeEnabled)
      {
        // ssh.exe (OpenSSH) supports "-i <keyfile>" directly, and finds a matching
        // "<keyfile>-cert.pub" on its own; only override with -oCertificateFile if the
        // user pointed us at a certificate somewhere else.
        if (_opts.UserPrivateKeyEnabled && !string.IsNullOrEmpty(_opts.UserPrivateKeyPath))
        {
          sshPassword = $"-i \"{_opts.UserPrivateKeyPath}\"";
          if (!string.IsNullOrEmpty(_opts.UserCertificatePath))
            sshPassword += $" -oCertificateFile=\"{_opts.UserCertificatePath}\"";
        }
        else
        {
          sshPassword = ""; // Nothing to do; ssh.exe falls back to its own default key discovery (i.e. ~/.ssh/id_rsa).
        }
      }
      else
      {
        sshPassword = $"-pw {RemoteUserPass}";
      }

      if (!_opts.UseSSHExeEnabled && string.IsNullOrEmpty(RemoteUserPass))
        Logger.Output("You must provide a User Password to debug.");

      string adapter = plinkPath;
      string adapterArgs = "";
      string displayAdapter = "";

      if(_opts.RemoteDebugDisplayGui)
      {
        displayAdapter = !string.IsNullOrWhiteSpace(_opts.RemoteDebugDisplayNumber) ?
          $"DISPLAY={_opts.RemoteDebugDisplayNumber}" :
          "DISPLAY=:0";
      }

      // Optionally elevate the debugger itself, for debuggees running with capabilities
      // (ambient/file capabilities, setuid, etc.) that VSDBG must match in order to attach.
      var remoteVsDbgCommand = _opts.UseSudoForDebugger
        ? $"{_opts.SudoCommand} {_opts.RemoteVsDbgFullPath}"
        : _opts.RemoteVsDbgFullPath;

      if (_opts.UseSSHExeEnabled)
      {
        adapterArgs = $"{sshPassword} {sshEndpoint} -T {displayAdapter} {remoteVsDbgCommand} {vsdbgLogPath}";
        //// adapterArgs = $"-ssh {sshPassword} {sshEndpoint} -batch -T {remoteVsDbgCommand} --interpreter=vscode {vsdbgLogPath}";
      }
      else
      {
        adapterArgs= $"-ssh {sshPassword} {sshEndpoint} -T {displayAdapter} {remoteVsDbgCommand} {vsdbgLogPath}";
      }

      return (adapter, adapterArgs);
    }

    /// <summary>Parses `KEY=VALUE` pairs (one per line) into a dictionary for `launch.json`'s `env`.</summary>
    /// <param name="rawEnvVariables">Newline-separated `KEY=VALUE` pairs.</param>
    /// <returns>Dictionary of environment variables, or null if none were provided.</returns>
    private Dictionary<string, string> ParseEnvironmentVariables(string rawEnvVariables)
    {
      if (string.IsNullOrWhiteSpace(rawEnvVariables))
        return null;

      var env = new Dictionary<string, string>();

      foreach (var line in rawEnvVariables.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
      {
        var trimmedLine = line.Trim();
        if (trimmedLine.Length == 0 || trimmedLine.StartsWith("#"))
          continue;

        var separatorIndex = trimmedLine.IndexOf('=');
        if (separatorIndex <= 0)
        {
          Logger.Output($"Ignoring malformed environment variable line: '{trimmedLine}'");
          continue;
        }

        var key = trimmedLine.Substring(0, separatorIndex).Trim();
        var value = trimmedLine.Substring(separatorIndex + 1).Trim();
        env[key] = value;
      }

      return env.Count > 0 ? env : null;
    }

    /// <summary>Attempt to get the extension's local directory.</summary>
    /// <returns>Path of this VSIX or empty string.</returns>
    private string GetExtensionDirectory()
    {
      var path = string.Empty;
      try
      {
        var uri = new Uri(typeof(LaunchBuilder).Assembly.CodeBase, UriKind.Absolute);
        path = Path.GetDirectoryName(uri.LocalPath);
      }
      catch (Exception)
      {
      }

      return path;
    }
  }
}

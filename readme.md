# FCO Linux Debugger

> **This is a fork.** FCO Linux Debugger is a fork of [SuessLabs/VsLinuxDebug](https://github.com/SuessLabs/VsLinuxDebug) ("VS Linux Debugger" / "VS .NET Linux Debugger" on the marketplace), substantially modified by FiveCo for internal remote-debugging needs against embedded Linux devices. It is licensed under the same [MIT License](LICENSE) as the original project. If you're looking for the original, upstream, general-purpose extension, get it from [SuessLabs on the Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=SuessLabs.VSLinuxDebugger) instead — this fork is not a drop-in replacement and is not published on the marketplace.

Remotely deploy and debug your .NET C# apps via SSH to Linux using Visual Studio.

Source: `fiveco-gitea01.fiveco.local/FiveCo/vs-ext-linux-debugger`

## Overview

Build, deploy, and debug .NET projects on a remote Linux device (Ubuntu, Raspberry Pi, embedded targets, etc.) directly from Visual Studio, over SSH. Connect using a password, an OpenSSH private key, or an SSH CA-signed certificate.

### What this fork changes vs. upstream

This fork exists to support debugging a self-contained ARM64 service running under systemd with elevated Linux capabilities, on a device with SSH certificate-based authentication and a constrained root filesystem. Compared to upstream, it adds:

* **SSH CA certificate authentication** — supports a private key with an accompanying `<key>-cert.pub` certificate (auto-detected next to the key, or set explicitly), in addition to password and plain private-key auth.
* **Sudo-elevated debugger launch** — an opt-in setting launches `vsdbg` via a configurable `sudo` command, for debuggees running with elevated or ambient capabilities that the debugger must match to attach.
* **Self-contained deployment** — an opt-in setting launches the deployed program directly as a native executable (self-contained/AOT publish) instead of via `dotnet <assembly>.dll`, with the executable bit restored after transfer (lost by default over tar/scp from Windows).
* **Environment variables for the debuggee** — pass `KEY=VALUE` pairs through to the remote process, for programs that read required configuration from the environment.
* **Configurable pre/post-deploy commands and attach-to-running-process** — run arbitrary shell commands before/after each deploy (i.e. stopping/restarting a systemd service), and optionally attach the debugger to that already-running process (via a configurable PID-lookup command) instead of launching a new one, so the debuggee runs under its normal supervised environment.
* **Modernized Options UI** — settings are split across 6 focused Tools > Options pages (Remote Host, Remote Credentials, Remote Debugger, Local, Display (X11), Experimental) built with a real WPF UI (checkboxes, dynamic show/hide for password vs. private-key fields) instead of a single page with a WinForms PropertyGrid.
* Support for Visual Studio 2026 and newer .NET target frameworks (net8.0, net10.0) in the sample/test projects.

### Supported Remote OS

The following Linux distributions have been validated and are supported.

* Ubuntu (20.04 LTS, 22.04 LTS, 24.x LTS)
* Raspberry Pi OS
* Debian-based embedded Linux images (i.e. Yocto/OpenEmbedded targets), where `curl` and an SSH server are available

### Usage

* Build and upload to remote devices
* Remote debugging*
  * _Attach/launch is still evolving on this fork. Please use VS' Attach to Process if you have issues._
* FCO Linux Debugger will automatically detect and install `vsdbg` for you!

For GUI app debugging, you can use the _Build and Deploy_ feature, however, you must manually _Attach to Process_ via SSH using Visual Studio at this time.

### Getting Started

**Linux**, we'll need **SSH** and **cURL** for access and downloading any missing tools:

```bash
sudo apt install openssh-server
sudo apt install curl
```

**Windows**:

1. Open Visual Studio (VS) > Tools > Options > **FCO Linux Debugger**
2. Configure the **Remote Host** page (IP address) and **Remote Credentials** page (user name, and either a password or a private key)
3. VS > Extensions > **FCO Linux Debugger** > **Build, Deploy, Debug**

### Manually Attaching (for GUI apps)

For GUI projects, you can use **Build and Deploy** and then manually attach to the process via SSH by using Visual Studio's built-in tool

1. Deploy to remote machine via
   1. Extensions > FCO Linux Debugger > **"Build and Deploy"**
2. Run GUI app on remote machine
   1. `dotnet MyGuiApp.dll`
3. Debug > **"Attach to Process.."**
4. Connection Type: **SSH**
5. Connection Target: **(Remote machine's IP)**
6. (Select process)
7. Click, **Attach**
8. Check, **"Managed (.NET Core for Unix)"**
9. Click, **OK**

This will save you 1.5 minutes on every build of manual uploading and updating rights via `chown -R`.

### Manually Attaching (for Command line apps)

For CLI projects, you can use **Build and Deploy** and then manually attach to the process via SSH by using Visual Studio's built-in tool (similar to above).

You may have to manually interrupt your app via `Console.ReadLine();` high-up in your entry-point (i.e. `main()`).

1. Deploy to remote machine via
   1. Extensions > FCO Linux Debugger > **"Build and Deploy"**
2. Run your CLI app on remote machine
   1. `dotnet MyCliApp.dll`
3. Debug > **"Attach to Process.."**
   1. Connection Type: **SSH**
   2. Connection Target: **(Remote machine's IP)**
   3. (Select process)
   4. Click, **Attach**
   5. Check, **"Managed (.NET Core for Unix)"**
   6. Click, **OK**
4. Continue your application, if using a manual interrupt (i.e. `Console.ReadLine();`)

This will save you 1.5 minutes on every build of manual uploading and updating rights via `chown -R`.

## How To Generate a Private Key (optional)

The following steps are optional if you wish to use an SSH private key instead of a password. These steps were written for Windows, but the steps are similar on Linux.

1. Open PowerShell:
2. **Generate key** (_with old PEM format_)
   1. `ssh-keygen -m PEM -t rsa -b 4096`
3. Set output name (_default is okay for basic setups_)
4. Input a passphrase for the key _(OPTIONAL)_
5. Windows will now generate your RSA public/private key pair.
   1. Default location: `%UserProfile%\.ssh` (Windows)
   2. The public key will be stored as `id_rsa.pub` in the directory
6. **Upload the public key** to your remote machine
   1. Navigate to folder, `~/.ssh/` on Linux device
   2. If `~/.ssh/authorized_keys` exists, append the contents of `id_rsa.pub` to the next line.
   3. If it does not exist, simply upload `id_rsa.pub` and rename it to, `authorized_keys`
7. DONE!

If your remote device uses **SSH CA-signed certificates** instead of `authorized_keys` (i.e. `TrustedUserCAKeys` configured in `sshd_config`), point "SSH Private Key File" at your CA-issued private key; the matching `<key>-cert.pub` certificate is picked up automatically if it sits next to the key, or can be set explicitly via "SSH Certificate File" on the Remote Credentials options page.

## Acknowledgments

This fork builds entirely on the work of [Suess Labs](https://suesslabs.com) and [Xeno Innovations, Inc.](https://xenoinc.com), the original authors and maintainers of VS Linux Debugger. Please direct general feedback, feature requests unrelated to this fork's changes, and marketplace reviews to the [upstream project](https://github.com/SuessLabs/VsLinuxDebug) — they did the hard work this fork stands on.

## References

* [Upstream project: SuessLabs/VsLinuxDebug](https://github.com/SuessLabs/VsLinuxDebug)
* [PuTTY PLink](http://www.chiark.greenend.org.uk/~sgtatham/putty/download.html)
* [Extension Docs](https://docs.microsoft.com/en-us/visualstudio/extensibility/creating-a-settings-category?view=vs-2022)
* [Extension Sample](https://github.com/microsoft/VSSDK-Extensibility-Samples/tree/master/Options)
* [Offroad Debugging](https://github.com/Microsoft/MIEngine/wiki/Offroad-Debugging-of-.NET-Core-on-Linux---OSX-from-Visual-Studio)

---

_Original project copyright 2022-2024 Xeno Innovations, Inc. Fork changes copyright FiveCo._

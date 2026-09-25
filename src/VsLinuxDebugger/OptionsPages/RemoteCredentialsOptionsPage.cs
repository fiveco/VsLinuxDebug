using System;
using System.IO;
using System.Windows;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  public class RemoteCredentialsOptionsPage : OptionsPageBase
  {
    private string _userName = "pi";
    private string _userPass = "raspberry";
    private bool _userPrivateKeyEnabled = false;
    private string _userPrivateKeyPath = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
      ".ssh\\id_rsa");
    private string _userPrivateKeyPassword = "";

    public string UserName { get => _userName; set => SetField(ref _userName, value); }

    public string UserPass { get => _userPass; set => SetField(ref _userPass, value); }

    public bool UserPrivateKeyEnabled { get => _userPrivateKeyEnabled; set => SetField(ref _userPrivateKeyEnabled, value); }

    public string UserPrivateKeyPath { get => _userPrivateKeyPath; set => SetField(ref _userPrivateKeyPath, value); }

    public string UserPrivateKeyPassword { get => _userPrivateKeyPassword; set => SetField(ref _userPrivateKeyPassword, value); }

    protected override UIElement Child => new RemoteCredentialsOptionsControl(this);
  }
}

using System.Windows;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  public class RemoteHostOptionsPage : OptionsPageBase
  {
    private string _hostIp = "127.0.0.1";
    private int _hostPort = 22;
    private string _userGroupName = "";

    public string HostIp { get => _hostIp; set => SetField(ref _hostIp, value); }

    public int HostPort { get => _hostPort; set => SetField(ref _hostPort, value); }

    public string UserGroupName { get => _userGroupName; set => SetField(ref _userGroupName, value); }

    protected override UIElement Child => new RemoteHostOptionsControl(this);
  }
}

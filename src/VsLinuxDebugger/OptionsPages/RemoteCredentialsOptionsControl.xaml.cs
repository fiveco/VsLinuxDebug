using System.Windows.Controls;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  public partial class RemoteCredentialsOptionsControl : UserControl
  {
    public RemoteCredentialsOptionsControl(RemoteCredentialsOptionsPage page)
    {
      InitializeComponent();
      DataContext = page;
    }
  }
}

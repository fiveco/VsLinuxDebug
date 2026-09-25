using System.Windows.Controls;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  public partial class RemoteDebuggerOptionsControl : UserControl
  {
    public RemoteDebuggerOptionsControl(RemoteDebuggerOptionsPage page)
    {
      InitializeComponent();
      DataContext = page;
    }
  }
}

using System.Windows.Controls;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  public partial class RemoteHostOptionsControl : UserControl
  {
    public RemoteHostOptionsControl(RemoteHostOptionsPage page)
    {
      InitializeComponent();
      DataContext = page;
    }
  }
}

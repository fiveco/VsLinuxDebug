using System.Windows.Controls;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  public partial class LocalOptionsControl : UserControl
  {
    public LocalOptionsControl(LocalOptionsPage page)
    {
      InitializeComponent();
      DataContext = page;
    }
  }
}

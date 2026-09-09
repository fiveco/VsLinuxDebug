using System.Windows.Controls;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  /// <summary>WPF UI for <see cref="RemoteLaunchOptionsPage"/>.</summary>
  public partial class RemoteLaunchOptionsControl : UserControl
  {
    public RemoteLaunchOptionsControl(RemoteLaunchOptionsPage page)
    {
      InitializeComponent();
      DataContext = page;
    }
  }
}

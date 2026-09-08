using System.Windows.Controls;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  /// <summary>WPF UI for <see cref="DisplayOptionsPage"/>.</summary>
  public partial class DisplayOptionsControl : UserControl
  {
    public DisplayOptionsControl(DisplayOptionsPage page)
    {
      InitializeComponent();
      DataContext = page;
    }
  }
}

using System.Windows.Controls;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  /// <summary>WPF UI for <see cref="ExperimentalOptionsPage"/>.</summary>
  public partial class ExperimentalOptionsControl : UserControl
  {
    public ExperimentalOptionsControl(ExperimentalOptionsPage page)
    {
      InitializeComponent();
      DataContext = page;
    }
  }
}

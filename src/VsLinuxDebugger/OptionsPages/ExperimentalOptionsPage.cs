using System.ComponentModel;
using System.Windows;
using Microsoft.VisualStudio.Shell;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  /// <summary>Experimental settings.</summary>
  public class ExperimentalOptionsPage : UIElementDialogPage, INotifyPropertyChanged
  {
    private bool _useCommandLineArgs = false;

    public event PropertyChangedEventHandler PropertyChanged;

    [Category("Warning Experimental")]
    [DisplayName("Use Command Line Arguments")]
    [Description(
      "Apply command line arguments from Visual Studio Project Settings. " +
      "(Experimental : Project Settings -> Debugging -> Command Line Arguments)")]
    public bool UseCommandLineArgs
    {
      get => _useCommandLineArgs;
      set { _useCommandLineArgs = value; OnPropertyChanged(nameof(UseCommandLineArgs)); }
    }

    protected override UIElement Child => new ExperimentalOptionsControl(this);

    private void OnPropertyChanged(string propertyName)
      => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}

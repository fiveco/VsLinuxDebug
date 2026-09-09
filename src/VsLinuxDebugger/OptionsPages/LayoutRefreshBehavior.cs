using System.Windows;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  /// <summary>Attached behavior working around a UIElementDialogPage layout glitch: the WPF
  /// content is hosted inside a native Win32 dialog, and its first layout pass sometimes runs
  /// before that host has communicated its real width, wrapping TextBlocks at the wrong point
  /// (a stray vertical line cutting help text mid-sentence, until the page is revisited and
  /// re-measured against the correct width). Forcing one more measure/arrange pass once the
  /// element is actually loaded (and the host's real size is known) fixes it up front.</summary>
  public static class LayoutRefreshBehavior
  {
    public static readonly DependencyProperty RefreshOnLoadProperty = DependencyProperty.RegisterAttached(
      "RefreshOnLoad",
      typeof(bool),
      typeof(LayoutRefreshBehavior),
      new PropertyMetadata(false, OnRefreshOnLoadChanged));

    public static bool GetRefreshOnLoad(DependencyObject element) => (bool)element.GetValue(RefreshOnLoadProperty);

    public static void SetRefreshOnLoad(DependencyObject element, bool value) => element.SetValue(RefreshOnLoadProperty, value);

    private static void OnRefreshOnLoadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var element = d as FrameworkElement;
      if (element == null)
        return;

      element.Loaded -= Element_Loaded;

      if ((bool)e.NewValue)
        element.Loaded += Element_Loaded;
    }

#pragma warning disable VSTHRD001, VSTHRD110 // Plain WPF dispatcher callback, not JoinableTaskFactory work; nothing to await.
    private static void Element_Loaded(object sender, RoutedEventArgs e)
    {
      var element = sender as FrameworkElement;
      if (element == null)
        return;

      element.Dispatcher.BeginInvoke(new System.Action(() =>
      {
        element.InvalidateMeasure();
        element.InvalidateArrange();
        element.UpdateLayout();
      }), System.Windows.Threading.DispatcherPriority.Loaded);
    }
#pragma warning restore VSTHRD001, VSTHRD110
  }
}

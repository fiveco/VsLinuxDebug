using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  /// <summary>Attached behavior for multi-line Options fields (env vars, pre/post-deploy
  /// commands): pressing Enter inserts a newline in the TextBox instead of being picked up
  /// by the host dialog as its default "OK" action, which would otherwise close the whole
  /// Options window.</summary>
  public static class MultilineTextBoxBehavior
  {
    public static readonly DependencyProperty AcceptsEnterKeyProperty = DependencyProperty.RegisterAttached(
      "AcceptsEnterKey",
      typeof(bool),
      typeof(MultilineTextBoxBehavior),
      new PropertyMetadata(false, OnAcceptsEnterKeyChanged));

    public static bool GetAcceptsEnterKey(DependencyObject element) => (bool)element.GetValue(AcceptsEnterKeyProperty);

    public static void SetAcceptsEnterKey(DependencyObject element, bool value) => element.SetValue(AcceptsEnterKeyProperty, value);

    private static void OnAcceptsEnterKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var textBox = d as TextBox;
      if (textBox == null)
        return;

      textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;

      if ((bool)e.NewValue)
        textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
    }

    private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      var textBox = sender as TextBox;
      if (e.Key != Key.Enter || textBox == null)
        return;

      var caret = textBox.CaretIndex;
      textBox.Text = textBox.Text.Insert(caret, "\r\n");
      textBox.CaretIndex = caret + 2;
      e.Handled = true;
    }
  }
}

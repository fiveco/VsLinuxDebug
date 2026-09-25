using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;

namespace Xeno.VsLinuxDebug.OptionsPages
{
  public abstract class OptionsPageBase : UIElementDialogPage, INotifyPropertyChanged
  {
    // All settings lived on a single page before the Options UI was split.
    private const string LegacySettingsPath = "DialogPage\\Xeno.VsLinuxDebug.OptionsPages.OptionsPage";

    public event PropertyChangedEventHandler PropertyChanged;

    public override void LoadSettingsFromStorage()
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      base.LoadSettingsFromStorage();
      ImportLegacySettings();
    }

    protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
      field = value;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void ImportLegacySettings()
    {
      var store = new ShellSettingsManager(ServiceProvider.GlobalProvider).GetReadOnlySettingsStore(SettingsScope.UserSettings);
      if (store.CollectionExists(SettingsRegistryPath) || !store.CollectionExists(LegacySettingsPath))
        return;

      foreach (var property in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
      {
        if (!property.CanWrite || !store.PropertyExists(LegacySettingsPath, property.Name))
          continue;

        var value = store.GetString(LegacySettingsPath, property.Name);
        property.SetValue(this, TypeDescriptor.GetConverter(property.PropertyType).ConvertFromInvariantString(value));
      }

      SaveSettingsToStorage();
    }
  }
}

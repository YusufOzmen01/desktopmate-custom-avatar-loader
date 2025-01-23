#if BEPINEX
using BepInEx.Configuration;

namespace CustomAvatarLoader.Settings;

public class BepInExSettings : ISettingsProvider 
{
    private readonly string _defaultCategory;
    private ConfigFile Config => BepInExPlugin.Instance.Config;

    public BepInExSettings(string defaultCategory)
    {
        _defaultCategory = defaultCategory;
    }

    public T Get<T>(string setting, T defaultValue)
    {
        if (string.IsNullOrEmpty(setting))
        {
            return defaultValue;
        }

        if (Config.TryGetEntry<T>(_defaultCategory, setting, out var entry))
        {
            return entry.Value;
        }

        Config.Bind(_defaultCategory, setting, defaultValue);
        return defaultValue;
    }

    public bool Set<T>(string setting, T value)
    {
        if (string.IsNullOrEmpty(setting))
        {
            return false;
        }

        if (Config.TryGetEntry<T>(_defaultCategory, setting, out var entry))
        {
            entry.Value = value;
            return true;
        }

        return true;
    }

    public void SaveSettings()
    {
        Core.Msg("Preferences saved!");
        Config.Save();
    }
}
#endif
#if MELON
using MelonLoader;

namespace CustomAvatarLoader.Settings;

public class MelonLoaderSettings : ISettingsProvider
{
    private readonly string _defaultCategory;

    public MelonLoaderSettings(string defaultCategory)
    {
        _defaultCategory = defaultCategory;
    }

    public T Get<T>(string setting, T defaultValue)
    {
        if (string.IsNullOrEmpty(setting))
        {
            return defaultValue;
        }
        
        var settingSections = setting.Split(".");
        var category = settingSections.Length > 1 ? settingSections[0] : _defaultCategory;
        var key = settingSections.Length > 1 ? settingSections[1] : settingSections[0];

        var settingsCategory = MelonPreferences.CreateCategory(category);
        
        return settingsCategory.HasEntry(key) ? settingsCategory.GetEntry<T>(key).Value : settingsCategory.CreateEntry(key, defaultValue).Value;
    }

    public bool Set<T>(string setting, T value)
    {
        if (string.IsNullOrEmpty(setting))
        {
            return false;
        }
        
        var settingSections = setting.Split(".");
        var category = settingSections.Length > 1 ? settingSections[0] : _defaultCategory;
        var key = settingSections.Length > 1 ? settingSections[1] : settingSections[0];
        var settingsCategory = MelonPreferences.CreateCategory(category);

        if (settingsCategory.HasEntry(key)) settingsCategory.GetEntry<T>(key).Value = value;
        else settingsCategory.CreateEntry(key, value);

        return true;
    }

    public void SaveSettings()
    {
        MelonPreferences.Save();
    }
}
#endif
namespace CustomAvatarLoader.Logging;

using CustomAvatarLoader.Settings;

public class PlayerLogManager
{
    public PlayerLogManager(ISettingsProvider settingsProvider)
    {
        SettingsProvider = settingsProvider;
    }

    protected virtual ISettingsProvider SettingsProvider { get; }

    public void ClearLog(string logPath)
    {
        bool disableReadOnly = SettingsProvider.Get("disable_log_readonly", false);
        SettingsProvider.SaveSettings();

        if (File.Exists(logPath))
        {
            File.SetAttributes(logPath, FileAttributes.Normal);
            try
            {
                File.WriteAllText(logPath, string.Empty);
            }
            catch (Exception ex)
            {
                Core.Warn($"Failed to clear log file at {logPath}\n" + ex);
            }

            File.SetAttributes(logPath, disableReadOnly ? FileAttributes.Normal : FileAttributes.ReadOnly);
        }
    }
}
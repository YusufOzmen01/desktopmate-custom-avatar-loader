namespace CustomAvatarLoader.Logging;

using BepInEx.Logging;
using System;

internal class BepInExLogger : ILogger
{
    public BepInExLogger(ManualLogSource logger)
    {
        Logger = logger;
    }

    protected virtual ManualLogSource Logger { get; }

    public void Debug(string message)
    {
        Logger.LogDebug(message);
    }

    public void Error(string message, Exception ex = null)
    {
        Logger.LogError(message);
    }

    public void Fatal(string message, Exception ex = null)
    {
        Logger.LogFatal(message);
    }

    public void Info(string message)
    {
        Logger.LogInfo(message);
    }

    public void Warn(string message, Exception ex = null)
    {
        Logger.LogWarning(message);
    }
}
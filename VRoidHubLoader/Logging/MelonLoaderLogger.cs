namespace CustomAvatarLoader.Logging;

using MelonLoader;

internal class MelonLoaderLogger : ILogger
{
    public MelonLoaderLogger(MelonLogger.Instance logger)
    {
        Logger = logger;
    }

    protected virtual MelonLogger.Instance Logger { get; }

    public void Debug(string message)
    {
        Logger.Msg(message);
    }

    public void Info(string message)
    {
        Logger.Msg(message);
    }

    public void Warn(string message, Exception ex = null)
    {
        Logger.Warning(message);
    }

    public void Error(string message, Exception ex = null)
    {
        Logger.Error(message, ex);
    }

    public void Fatal(string message, Exception ex = null)
    {
        Logger.Error(message, ex);
    }
}
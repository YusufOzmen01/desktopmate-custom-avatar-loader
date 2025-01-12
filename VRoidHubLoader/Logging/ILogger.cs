namespace CustomAvatarLoader.Logging;

public interface ILogger
{
    void Debug(string message);

    void Info(string message);

    void Warn(string message, Exception ex = null);

    void Error(string message, Exception ex = null);

    void Fatal(string message, Exception ex = null);
}
namespace CustomAvatarLoader.Logging;

public interface ILogger
{
    Action<object> LogMessage { get; }
    Action<object> LogWarning { get; }
    Action<object> LogError { get; }
}
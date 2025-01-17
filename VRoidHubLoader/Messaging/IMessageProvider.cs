using UnityEngine;

namespace CustomAvatarLoader.Messaging;

public interface IMessageProvider
{
    bool HasWindowsOpen();

    void ShowMessageBox(string title, string message);

    void ShowMessageBox(string title, string message, Color color);

    void OnGUI();
}

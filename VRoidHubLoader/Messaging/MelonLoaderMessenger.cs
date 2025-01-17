using Il2Cpp;
using UnityEngine;

namespace CustomAvatarLoader.Messaging;

public class MelonLoaderMessenger : IMessageProvider
{
    private readonly List<MessageBox> OpenMessageBoxes = [];

    public bool HasWindowsOpen()
    {
        return OpenMessageBoxes.Count > 0;
    }

    public void OnGUI()
    {
        for (int i = 0; i < OpenMessageBoxes.Count; i++)
        {
            MessageBox box = OpenMessageBoxes[i];
            if (!box.Initialized)
                box.Init();
            box.Draw(i);
        }
    }

    public void ShowMessageBox(string title, string message)
    {
        ShowMessageBox(title, message, Color.gray);
    }

    public void ShowMessageBox(string title, string message, Color color)
    {
        MessageBox box = new(title, message, color);
        box.OnClose += OnMessageBoxClosed;
        OpenMessageBoxes.Add(box);
    }

    private void OnMessageBoxClosed(object? sender, EventArgs e)
    {
        if (sender is MessageBox boxSender)
        {
            OpenMessageBoxes.Remove(boxSender);
        }
    }

}

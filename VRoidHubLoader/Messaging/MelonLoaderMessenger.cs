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
            MenuManager.Instance._IsOpen_k__BackingField = true;
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
            if (OpenMessageBoxes.Count == 0)
            {
                // this can cause a desync (i.e. the menu being open when this is set to false)
                // from testing this doesn't cause any issues
                // but if a better way of doing this would be preferred
                MenuManager.Instance._IsOpen_k__BackingField = false;
            }
        }
    }

}

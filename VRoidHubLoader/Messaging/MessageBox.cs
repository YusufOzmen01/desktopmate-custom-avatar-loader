using UnityEngine;

namespace CustomAvatarLoader.Messaging
{
    public class MessageBox
    {
        protected string Title { get; private set; }
        protected string Message { get; private set; }
        protected Color Color { get; private set; }
        public bool Initialized { get; private set; } = false;

        private Rect Window;
        private Vector2 Size;

        public event EventHandler OnClose;

        #pragma warning disable CS8618 // Suppress warnings about OnClose being null
        public MessageBox(string title, string message)
        {
            Title = title;
            Message = message;
            Color = Color.gray;
        }

        public MessageBox(string title, string message, Color color)
        {
            Title = title;
            Message = message;
            Color = color;
        }
        #pragma warning restore CS8618

        public void Init()
        {
            if (!Initialized)
            {
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUI.skin.button.alignment = TextAnchor.MiddleCenter;
                Size = GUI.skin.label.CalcSize(new GUIContent(Message));
                Window = new Rect(1100, 700, Size.x + 120, Size.y + 90);
                Initialized = true;
            }
        }

        public void Draw(int windowID)
        {
            GUI.color = Color;
            Window = GUI.Window(windowID, Window, (GUI.WindowFunction)DrawGUI, Title);
        }

        private void DrawGUI(int windowID)
        {
            GUI.Label(new Rect(60, 40, Size.x, Size.y), Message);
            if (GUI.Button(new Rect((Window.width / 2) - 50, Window.height - 40, 100, 20), "Ok"))
            {
                OnOkButtonClicked();
            }
            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }

        private void OnOkButtonClicked()
        {
            OnClose?.Invoke(this, EventArgs.Empty);
        }

    }
}

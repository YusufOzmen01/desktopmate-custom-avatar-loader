using CustomAvatarLoader.Modules;

public class AsyncHelper
{
    private List<Action> _tasksForMainThread = new List<Action>(3);

    public void RunOnMainThread(Action task)
    {
        _tasksForMainThread.Add(task);
    }

    public void OnUpdate()
    {
        if (_tasksForMainThread.Count > 0)
        {
            foreach (var task in _tasksForMainThread)
            {
                task();
            }
            _tasksForMainThread.Clear();
        }

    }
}
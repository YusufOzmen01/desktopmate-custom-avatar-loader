using System.Diagnostics;
using System.Runtime.InteropServices;
using CustomAvatarLoader.Logging;
using Il2CppKirurobo;

namespace CustomAvatarLoader.Helpers;

public class WindowHelper
{
    ILogger _logger;
    
    private delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);
    
    [DllImport("user32.dll")]
    static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

    [DllImport("kernel32.dll")]
    static extern int GetCurrentThreadId();
    
    [DllImport("user32.dll")]
    static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    
    [DllImport("user32.dll")]
    static extern Int32 SetForegroundWindow(IntPtr hWnd);   
    
    private const uint WM_SYSCOMMAND = 0x0112;
    private const uint SC_MINIMIZE = 0xF020;
    private const uint SC_RESTORE = 0xF120;

    public WindowHelper(ILogger logger)
    {
        _logger = logger;
    }

    public void MinimizeGameWindow(IntPtr hWnd)
    {
        PostMessage(hWnd, WM_SYSCOMMAND,  (IntPtr) SC_MINIMIZE, (IntPtr) 0);
    }

    public void UnminimizeGameWindow(IntPtr hWnd)
    {
        PostMessage(hWnd, WM_SYSCOMMAND,  (IntPtr) SC_RESTORE, (IntPtr) 0);
    }
    
    public void SetWindowForeground(IntPtr hWnd)
    {
        SetForegroundWindow(hWnd);
    }

    public IntPtr GetUnityGameHwnd()
    {
        IntPtr hWnd = IntPtr.Zero;
        foreach (Process pList in Process.GetProcesses())
        {
            if (pList.MainWindowTitle.EndsWith("DesktopMate"))
            {
                hWnd = pList.MainWindowHandle;
            }
        }
        return hWnd;
    }
}
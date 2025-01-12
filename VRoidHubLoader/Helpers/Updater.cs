using System.Diagnostics;
using System.Runtime.InteropServices;
using CustomAvatarLoader.Logging;

namespace CustomAvatarLoader.Helpers;

public class Updater
{
    private const uint MB_YESNO = 0x00000004;
    private const uint MB_ICONQUESTION = 0x00000020;
    
    private string RepositoryName {get; set;}

    private ILogger Logger;
    
    public Updater(string repository, ILogger logger)
    {
        RepositoryName = repository;
        Logger = logger;
    }
    
    public void ShowUpdateMessageBox()
    {
        Logger.Info("[VersionCheck] New version available.");
        
        int result = CreateMessageBox(
            "A new version of the custom avatar loader is available - do you want to download it?",
            "Update Available");

        switch (result)
        {
            case 6: // IDYES
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"https://github.com/{RepositoryName}/releases",
                    UseShellExecute = true
                });
                break;

            case 7: // IDNO
                Logger.Info("[VersionCheck] User chose to skip update for now.");
                break;
        }
    }
    
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

    private static int CreateMessageBox(string text, string caption, uint type = MB_YESNO | MB_ICONQUESTION)
    {
        return MessageBox(IntPtr.Zero, text, caption, type);
    }
}
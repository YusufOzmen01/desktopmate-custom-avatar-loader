using System.Runtime.InteropServices;
using CustomAvatarLoader.Logging;

namespace CustomAvatarLoader.Helpers;

public class FileHelper
{
    public Task<string> OpenFileDialog()
    {
        var ofn = GetOpenFileName();

        return Task.Run(() => GetOpenFileName(ofn) ? ofn.file : null);
    }
    
    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

    private OpenFileName GetOpenFileName()
    {
        OpenFileName ofn = new();

        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "VRM Files\0*.vrm\0";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = UnityEngine.Application.dataPath;
        ofn.title = "Open VRM File";
        ofn.flags = 0x00080000 | 0x00000008; // OFN_EXPLORER | OFN_FILEMUSTEXIST

        return ofn;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class OpenFileName
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public String filter = null;
        public String customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public String file = null;
        public int maxFile = 0;
        public String fileTitle = null;
        public int maxFileTitle = 0;
        public String initialDir = null;
        public String title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public String defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public String templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
    }
}
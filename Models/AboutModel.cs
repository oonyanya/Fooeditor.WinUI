using System.Diagnostics;
using System.Reflection;

namespace FooEditor.WinUI.Models
{
    public class AboutModel
    {
        static FileVersionInfo _fileVersionInfo;

        private static  FileVersionInfo GetVersionInfo()
        {
            if (_fileVersionInfo == null)
            {
                var fullPath = typeof(App).GetTypeInfo().Assembly.Location;
                _fileVersionInfo = FileVersionInfo.GetVersionInfo(fullPath);
            }
            return _fileVersionInfo;
        }

        public static string AppName
        {
            get
            {
                return GetVersionInfo().ProductName;
            }
        }

        public static string Version
        {
            get
            {
                return GetVersionInfo().ProductVersion;
            }
        }
    }
}

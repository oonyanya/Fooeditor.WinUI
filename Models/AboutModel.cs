using System.Reflection;

namespace FooEditor.WinUI.Models
{
    public class AboutModel
    {
        public static string AppName
        {
            get
            {
                return typeof(App).GetTypeInfo().Assembly.GetName().Name;
            }
        }

        public static string Version
        {
            get
            {
                return typeof(App).GetTypeInfo().Assembly.GetName().Version.ToString();
            }
        }
    }
}

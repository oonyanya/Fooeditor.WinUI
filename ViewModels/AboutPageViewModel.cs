using FooEditor.WinUI.Models;

namespace FooEditor.WinUI.ViewModels
{
    class AboutPageViewModel : ViewModelBase
    {
        public string AppName
        {
            get
            {
                return AboutModel.AppName;
            }
        }

        public string Version
        {
            get
            {
                return AboutModel.Version;
            }
        }
    }
}

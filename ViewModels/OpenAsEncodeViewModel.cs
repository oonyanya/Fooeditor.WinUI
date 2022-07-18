using System.Text;
using FooEditor.WinUI.Models;
using System.Collections.ObjectModel;

namespace FooEditor.WinUI.ViewModels
{
    class OpenAsEncodeViewModel : ViewModelBase
    {
        Encoding _Encoding;
        public Encoding Encoding
        {
            get
            {
                return _Encoding;
            }
            set
            {
                SetProperty(ref _Encoding, value);
            }
        }

        public ObservableCollection<Encoding> EncodeCollection
        {
            get
            {
                return AppSettings.SupportEncodeCollection;
            }
        }
    }
}

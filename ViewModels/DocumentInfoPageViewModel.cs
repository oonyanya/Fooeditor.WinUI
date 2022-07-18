using EncodeDetect;
using FooEditor.WinUI.Models;
using FooEditor.WinUI.Services;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace FooEditor.WinUI.ViewModels
{
    class DocumentInfoPageViewModel : ViewModelBase
    {
        IMainViewService MainViewService;
        DocumentCollection _doc_list;
        public DocumentInfoPageViewModel(IMainViewService mainViewService)
        {
            this._doc_list = DocumentCollection.Instance;
            this.MainViewService = mainViewService;
        }

        public DocumentCollection DocumentList
        {
            get
            {
                return this._doc_list;
            }
            set
            {
                SetProperty(ref this._doc_list, value);
            }
        }

        public ObservableCollection<FileType> FileTypeCollection
        {
            get
            {
                return AppSettings.Current.FileTypeCollection;
            }
        }

        public ObservableCollection<Encoding> EncodeCollection
        {
            get
            {
                return AppSettings.SupportEncodeCollection;
            }
        }

        static ObservableCollection<LineFeedType> lineFeedCollection = new ObservableCollection<LineFeedType>() {
            LineFeedType.CR,
            LineFeedType.CRLF,
            LineFeedType.LF
        };
        public ObservableCollection<LineFeedType> LineFeedCollection
        {
            get
            {
                return lineFeedCollection;
            }
        }

        public Encoding DocumentEncode
        {
            get
            {
                return this._doc_list.Current.Encode;
            }
            set
            {
                if (this._doc_list.Current.FilePath == null)
                {
                    this._doc_list.Current.Encode = value;
                    this.OnPropertyChanged();
                    return;
                }
                if(this._doc_list.Current.Encode.WebName != value.WebName)
                {
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                    var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                    var task = this.MainViewService.Confirm(
                        loader.GetString("ReopenConfirm"),
                        loader.GetString("YesButton"),
                        loader.GetString("NoButton"));
                    task.ContinueWith(async (s) => {
                        if (await s == true)
                            await this._doc_list.Current.ReloadFileAsync(value);
                        //ファイルを再読み込みしなかった場合でも呼び出さないといけないらしい
                        this.OnPropertyChanged();
                    }, taskScheduler);
                }
            }
        }

        public FileType DocumentType
        {
            get
            {
                return this._doc_list.Current.DocumentModel.DocumentType;
            }
            set
            {
                var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                var task = this._doc_list.Current.DocumentModel.SetDocumentType(value);
                task.ContinueWith((s) => {
                    this.OnPropertyChanged();
                }, taskScheduler);
            }
        }
    }
}

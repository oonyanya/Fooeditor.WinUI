using System.Collections.ObjectModel;
using System.Windows.Input;
using FooEditor.WinUI.Models;
using CommunityToolkit.Mvvm.Input;

namespace FooEditor.WinUI.ViewModels
{
    class FileTypesPageViewModel : ViewModelBase
    {
        ObservableCollection<FileType> _FileTypeCollection = AppSettings.Current.FileTypeCollection;
        public ObservableCollection<FileType> FileTypeCollection
        {
            get
            {
                return this._FileTypeCollection;
            }
            set
            {
                this._FileTypeCollection = value;
                this.OnPropertyChanged();
            }
        }

        string _NewFileType;
        public string NewFileType
        {
            get
            {
                return _NewFileType;
            }
            set
            {
                _NewFileType = value;
                this.OnPropertyChanged();
            }
        }


        public ICommand ShowDetailCommand
        {
            get
            {
                return new RelayCommand<FileType>((e) =>
                {
                    NavigationService.Navigate("FileTypeDetail", e.DocumentTypeName);
                });
            }
        }
        public ICommand RemoveCommand
        {
            get
            {
                return new RelayCommand<FileType>((e) =>
                {
                    this._FileTypeCollection.Remove(e);
                });
            }
        }

        public ICommand AddCommand
        {
            get
            {
                return new RelayCommand<string>((e) =>
                {
                    this._FileTypeCollection.Add(new FileType(_NewFileType, ""));
                });
            }
        }
    }
}

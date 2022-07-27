using System.Collections.ObjectModel;
using System.Windows.Input;
using FooEditEngine;
using FooEditor.WinUI.Models;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using FooEditor.WinUI.Services;

namespace FooEditor.WinUI.ViewModels
{
    class FileTypeDetailPageViewModel : ViewModelBase
    {
        FileType _FileType;
        public FileType FileType
        {
            get
            {
                return this._FileType;
            }
            set
            {
                this._FileType = value;
                this.OnPropertyChanged();
            }
        }

        string _NewExtension;
        public string NewExtension
        {
            get
            {
                return this._NewExtension;
            }
            set
            {
                this._NewExtension = value;
                this.OnPropertyChanged();
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                return new RelayCommand<string>((e) =>
                {
                    this.FileType.ExtensionCollection.Remove(e);
                });
            }
        }

        public ICommand AddCommand
        {
            get
            {
                return new RelayCommand<string>((e) =>
                {
                    this.FileType.ExtensionCollection.Add(this.NewExtension);
                });
            }
        }

        ObservableCollection<LineBreakMethod> _LineBreakMethodList;
        public ObservableCollection<LineBreakMethod> LineBreakMethodList
        {
            get
            {
                if (this._LineBreakMethodList == null)
                {
                    this._LineBreakMethodList = new ObservableCollection<LineBreakMethod>(EnumListGenerator.GetList<LineBreakMethod>());
                }
                return _LineBreakMethodList;
            }
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
            string doctype = e.Parameter as string;
            this._FileType = AppSettings.Current.FileTypeCollection.Where((s) => { return s.DocumentTypeName == doctype; }).First();
        }
    }
}

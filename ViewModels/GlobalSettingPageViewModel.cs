using System.Collections.ObjectModel;
using System.Windows.Input;
using FooEditor.WinUI.Models;
using FooEditEngine;
using System;
using CommunityToolkit.Mvvm.Input;
using Windows.Storage.Pickers;
using FooEditor.WinUI.Services;

namespace FooEditor.WinUI.ViewModels
{
    class GlobalSettingPageViewModel : ViewModelBase
    {
        IMainViewService MainViewService;

        public GlobalSettingPageViewModel(IMainViewService mainViewService)
        {
            this.MainViewService = mainViewService;
        }

        public ICommand OpenWorkfilePathPickerCommand
        {
            get
            {
                return new RelayCommand<object>(async (e) =>
                {
                    FolderPicker folderPicker = this.MainViewService.GetFolderPicker();
                    folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
                    folderPicker.FileTypeFilter.Add("*");
                    var folder = await folderPicker.PickSingleFolderAsync();
                    if(folder != null)
                        AppSettings.Current.WorkfilePath = folder.Path;
                });
            }
        }

        public ICommand OpenConfigureFolderCommand
        {
            get
            {
                return new RelayCommand<object>(async (e) =>
                {
                    await Windows.System.Launcher.LaunchFolderAsync(Windows.Storage.ApplicationData.Current.LocalFolder);
                });
            }
        }

        ObservableCollection<string> _FontFamilyList;
        public ObservableCollection<string> FontFamilyList
        {
            get
            {
                if (this._FontFamilyList == null)
                    this._FontFamilyList = new ObservableCollection<string>(FontFamillyCollection.GetFonts());
                return this._FontFamilyList;
            }
        }

        ObservableCollection<LineBreakMethod> _LineBreakMethodList;
        public ObservableCollection<LineBreakMethod> LineBreakMethodList
        {
            get
            {
                if(this._LineBreakMethodList == null)
                {
                    this._LineBreakMethodList = new ObservableCollection<LineBreakMethod>(EnumListGenerator.GetList<LineBreakMethod>());
                }
                return _LineBreakMethodList;
            }
        }

        public ObservableCollection<System.Text.Encoding> EncodeCollection
        {
            get
            {
                return AppSettings.SupportEncodeCollection;
            }
        }

        AppSettings _Setting = AppSettings.Current;
        public AppSettings Setting
        {
            get
            {
                return this._Setting;
            }
            set
            {
                this._Setting = value;
                this.OnPropertyChanged();
            }
        }

    }
}

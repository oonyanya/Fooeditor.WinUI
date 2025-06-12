using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using FooEditor.WinUI.Services;
using FooEditor.WinUI.Models;
using System;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;
using Windows.Graphics.Printing;
using Windows.System.Threading;
using CommunityToolkit.Mvvm.Input;

namespace FooEditor.WinUI.ViewModels
{
    class SettingPageViewModel : ViewModelBase
    {

        public RelayCommand<object> GlobalSettingCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    NavigationService.Navigate("GlobalSetting", null);
                });
            }
        }

        public RelayCommand<object> FileTypeSettingCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    NavigationService.Navigate("FileTypes", null);
                });
            }
        }

        public RelayCommand<object> PrintSettingCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    NavigationService.Navigate("PrintSettings", null);
                });
            }
        }

        public RelayCommand<object> AboutPageCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    NavigationService.Navigate("About", null);
                });
            }
        }

    }
}

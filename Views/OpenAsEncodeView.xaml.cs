using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using FooEditor.WinUI.ViewModels;

// コンテンツ ダイアログの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace FooEditor.WinUI.Views
{
    public sealed partial class OpenAsEncodeView : ContentDialog
    {
        public OpenAsEncodeView()
        {
            this.InitializeComponent();
            this.DataContext = new OpenAsEncodeViewModel();
        }

        public System.Text.Encoding SelectedEncoding
        {
            get;
            private set;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var vm = this.DataContext as OpenAsEncodeViewModel;
            this.SelectedEncoding = vm.Encoding;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}

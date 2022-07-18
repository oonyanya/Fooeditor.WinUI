using FooEditor.WinUI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;

// コンテンツ ダイアログの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace FooEditor.WinUI.Views
{
    public sealed partial class OpenRecentlyView : ContentDialog
    {
        public OpenRecentlyView()
        {
            this.InitializeComponent();
            this.Loaded += OpenRecentlyView_Loaded;
            this.DataContext = new OpenRecentlyViewModel();
        }

        public ObservableCollection<RecentFile> SelectedFiles
        {
            get;
            private set;
        }

        private async void OpenRecentlyView_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as OpenRecentlyViewModel;
            if (vm != null)
                await vm.Initalize();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var vm = this.DataContext as OpenRecentlyViewModel;
            this.SelectedFiles = vm.SelectedFiles;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listbox = (ListBox)sender;
            var vm = this.DataContext as OpenRecentlyViewModel;
            vm.SelectedFiles = new ObservableCollection<RecentFile>(listbox.SelectedItems.Cast<RecentFile>());
        }
    }
}

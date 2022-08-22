using FooEditEngine.WinUI;
using FooEditor.WinUI.Services;
using FooEditor.WinUI.ViewModels;
using FooEditor.WinUI.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.Graphics.Printing.OptionDetails;
using Windows.System;
using Windows.UI.Core;
using FooEditor.WinUI.Models;

namespace FooEditor.WinUI
{
    public sealed partial class MainWindow : Window
    {
        MainViewService mainViewService;
        NavigationService navigationService;

        public MainWindow()
        {
            this.InitializeComponent();
            mainViewService = new MainViewService();
            mainViewService.MainPage = this;
            navigationService = new NavigationService();
            navigationService.ActivatorViewModel = (e) => {
                if(e.TargetViewMode == typeof(DocumentInfoPageViewModel))
                {
                    return new DocumentInfoPageViewModel(mainViewService);
                }
                return null;
            };
            this.ViewModel = new MainPageViewModel(navigationService, mainViewService);
            this.RootPanel.DataContext = this.ViewModel;
            this.Activated += MainWindow_Activated;
        }

        private void MainWindow_Activated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
        {
            AppSettings.Current.Dpi = (float)this.RootPanel.XamlRoot.RasterizationScale * 96.0f;
        }

        public void SetRootFrame(Frame frame)
        {
            this.navigationService.frame = frame;
            this.NavigationContent.Content = frame;
        }

        PrintTaskRequestedDeferral printTaskRequestedDeferral;
        void MainPage_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () => {
                MainPageViewModel vm = this.RootPanel.DataContext as MainPageViewModel;
                var source = vm.CurrentDocument.CreatePrintDocument();
                PrintTask task = null;
                task = args.Request.CreatePrintTask(vm.CurrentDocument.DocumentModel.Title, (e) =>
                {
                    e.SetSource(source.PrintDocument);
                });
                task.Completed += (s, e) => {
                    System.Diagnostics.Debug.WriteLine("finshed printing");
                };
                PrintOptionBuilder<DocumentSource> builder = new PrintOptionBuilder<DocumentSource>(source);
                builder.BuildPrintOption(PrintTaskOptionDetails.GetFromPrintTaskOptions(task.Options));
                printTaskRequestedDeferral.Complete();
            });
            printTaskRequestedDeferral = args.Request.GetDeferral();
        }

        public MainPageViewModel ViewModel { get; private set; }


        bool _inited = false;
        public async Task Init(object param, bool require_restore, Dictionary<string, object> viewModelState)
        {
            //VM内で追加する設定が反映されないので、ここで追加する
            MainPageViewModel vm = this.RootPanel.DataContext as MainPageViewModel;
            if(_inited == false)
            {
                this.mainViewService.GetPrintManager().PrintTaskRequested += MainPage_PrintTaskRequested;
                _inited = true;
            }
            await vm.Init(param, require_restore, viewModelState);
        }

        public async Task Suspend(bool suspending, Dictionary<string, object> viewModelState)
        {
            MainPageViewModel vm = this.RootPanel.DataContext as MainPageViewModel;
            this.mainViewService.GetPrintManager().PrintTaskRequested -= MainPage_PrintTaskRequested;
            _inited = false;
            await vm.Suspend(viewModelState, suspending);            
        }

        public async void OpenFromArgs(object args)
        {
            MainPageViewModel vm = this.RootPanel.DataContext as MainPageViewModel;
            await vm.OpenFromArgs(args);
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
            e.Handled = true;
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            var d = e.GetDeferral();

            if (e.DataView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
            {
                MainPageViewModel vm = this.RootPanel.DataContext as MainPageViewModel;
                // ファイルのパス一覧を取得する
                var items = await e.DataView.GetStorageItemsAsync();
                vm.OpenFilesCommand.Execute(items);
            }

            d.Complete();
        }

        private async void OpenAsEncodeButton_Click(object sender, RoutedEventArgs e)
        {
            MainPageViewModel vm = this.RootPanel.DataContext as MainPageViewModel;
            var dialog = new OpenAsEncodeView();
            dialog.XamlRoot = this.RootPanel.XamlRoot;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                vm.OpenFileCommand.Execute(dialog.SelectedEncoding);
        }

        private async void SaveAsEncodeButton_Click(object sender, RoutedEventArgs e)
        {
            MainPageViewModel vm = this.RootPanel.DataContext as MainPageViewModel;
            var dialog = new SaveAsEncodeView();
            dialog.XamlRoot = this.RootPanel.XamlRoot;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                vm.SaveAsCommand.Execute(dialog.SelectedEncoding);
        }

        private async void OpenFromMRU_Click(object sender, RoutedEventArgs e)
        {
            MainPageViewModel vm = this.RootPanel.DataContext as MainPageViewModel;
            var dialog = new OpenRecentlyView();
            dialog.XamlRoot = this.RootPanel.XamlRoot;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                var file_paths = from select_file in dialog.SelectedFiles select select_file.FilePath;
                vm.OpenFilePathCommand.Execute(file_paths);
            }

        }

        private void TabViewItem_CloseRequested(Microsoft.UI.Xaml.Controls.TabViewItem sender, Microsoft.UI.Xaml.Controls.TabViewTabCloseRequestedEventArgs args)
        {
            MainPageViewModel vm = this.RootPanel.DataContext as MainPageViewModel;
            vm.RemoveDocumentCommand.Execute(args.Item);
        }
    }
}
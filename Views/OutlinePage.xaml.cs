using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FooEditor.WinUI.ViewModels;
using FooEditor.WinUI.Models;

namespace FooEditor.WinUI.Views
{
    public class AnalyzePattern
    {
        public string Type;
        public string[] Patterns;
        public override string ToString()
        {
            return this.Type;
        }
        public AnalyzePattern(string type, string[] patterns)
        {
            this.Type = type;
            this.Patterns = patterns;
        }
    }
    /// <summary>
    /// OutlineWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class OutlinePage : Page
    {
        public OutlinePage()
        {
            InitializeComponent();
            this.DataContext = new OutlinePageViewModel();
        }

        private void TextBlock_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            OutlinePageViewModel vm = this.DataContext as OutlinePageViewModel;
            vm.JumpCommand.Execute(this.TreeView.SelectedItem as OutlineTreeItem);
        }

        private void CutMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            OutlinePageViewModel vm = this.DataContext as OutlinePageViewModel;
            vm.CutCommand.Execute(this.TreeView.SelectedItem as OutlineTreeItem);
        }

        private void CopyMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            OutlinePageViewModel vm = this.DataContext as OutlinePageViewModel;
            vm.CopyCommand.Execute(this.TreeView.SelectedItem as OutlineTreeItem);
        }
        private void PaseAsChildMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            OutlinePageViewModel vm = this.DataContext as OutlinePageViewModel;
            vm.PasteAsChildCommand.Execute(this.TreeView.SelectedItem as OutlineTreeItem);
        }
        private void UpLevelMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            OutlinePageViewModel vm = this.DataContext as OutlinePageViewModel;
            vm.UpLevelCommand.Execute(this.TreeView.SelectedItem as OutlineTreeItem);
        }
        private void DownLevelMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            OutlinePageViewModel vm = this.DataContext as OutlinePageViewModel;
            vm.DownLevelCommand.Execute(this.TreeView.SelectedItem as OutlineTreeItem);
        }
    }
}

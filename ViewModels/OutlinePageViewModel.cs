using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FooEditEngine;
using FooEditor.WinUI.Models;
using Windows.ApplicationModel.DataTransfer;
using CommunityToolkit.Mvvm.Input;

namespace FooEditor.WinUI.ViewModels
{
    class OutlinePageViewModel : ViewModelBase
    {
        public OutlinePageViewModel()
        {
            this.DocumentList = DocumentCollection.Instance;
            this.DocumentList.ActiveDocumentChanged += DocumentList_ActiveDocumentChanged;
        }

        private DocumentCollection DocumentList;
        private Document Document;
        private IFoldingStrategy FoldingStrategy;

        ObservableCollection<OutlineTreeItem> _Items;
        public ObservableCollection<OutlineTreeItem> Items
        {
            get
            {
                return _Items;
            }
            set
            {
                SetProperty(ref _Items, value);
            }
        }

        private void DocumentList_ActiveDocumentChanged(object sender, DocumentCollectionEventArgs e)
        {
            var doclist = (DocumentCollection)sender;
            this.Document = doclist.Current.DocumentModel.Document;
            this.FoldingStrategy = doclist.Current.DocumentModel.FoldingStrategy;
            if (this.Items != null)
                this.Items.Clear();
        }

        public RelayCommand<object> AnalyzeCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    this.Document = this.DocumentList.Current.DocumentModel.Document;
                    this.Document.LayoutLines.GenerateFolding(true);
                    this.FoldingStrategy = this.DocumentList.Current.DocumentModel.FoldingStrategy;
                    this.Items = OutlineAnalyzer.Analyze(this.FoldingStrategy, this.Document.LayoutLines, this.Document);
                });
            }
        }

        public RelayCommand<OutlineTreeItem> JumpCommand
        {
            get
            {
                return new RelayCommand<OutlineTreeItem>((s) =>
                {
                    if (s == null)
                        return;

                    OutlineTreeItem item = s;
                    OutlineActions.JumpNode(item, this.Document);
                });
            }
        }

        public RelayCommand<OutlineTreeItem> CutCommand
        {
            get
            {
                return new RelayCommand<OutlineTreeItem>((s) =>
                {
                    if (s == null || this.FoldingStrategy is not WZTextFoldingGenerator)
                        return;

                    OutlineTreeItem treeitem = s;
                    this.SetToClipboard(treeitem);
                    this.Document.Remove(treeitem.Start, treeitem.End - treeitem.Start + 1);
                    this.Document.RequestRedraw();

                    this.Items = OutlineAnalyzer.Analyze(this.FoldingStrategy, this.Document.LayoutLines, this.Document);
                },
                (s) => {
                    return this.FoldingStrategy is WZTextFoldingGenerator;
                });
            }
        }

        public RelayCommand<OutlineTreeItem> CopyCommand
        {
            get
            {
                return new RelayCommand<OutlineTreeItem>((s) =>
                {
                    if (s == null || this.FoldingStrategy is not WZTextFoldingGenerator)
                        return;

                    OutlineTreeItem treeitem = s;
                    this.SetToClipboard(treeitem);
                },
                (s) => {
                    return this.FoldingStrategy is WZTextFoldingGenerator;
                });
            }
        }

        public RelayCommand<OutlineTreeItem> PasteAsChildCommand
        {
            get
            {
                return new RelayCommand<OutlineTreeItem>(async (s) =>
                {
                    if (s == null || this.FoldingStrategy is not WZTextFoldingGenerator)
                        return;

                    OutlineTreeItem treeitem = s;
                    var view = Clipboard.GetContent();
                    string text = OutlineActions.FitOutlineLevel(await view.GetTextAsync(), treeitem, treeitem.Level + 1);
                    this.Document.Replace(treeitem.End + 1, 0, text);
                    this.Document.RequestRedraw();

                    this.Items = OutlineAnalyzer.Analyze(this.FoldingStrategy, this.Document.LayoutLines, this.Document);
                },
                (s) => {
                    return this.FoldingStrategy is WZTextFoldingGenerator;
                });
            }
        }

        public RelayCommand<OutlineTreeItem> UpLevelCommand
        {
            get
            {
                return new RelayCommand<OutlineTreeItem>((s) =>
                {
                    if (s == null)
                        return;

                    if (this.FoldingStrategy is WZTextFoldingGenerator)
                    {
                        OutlineTreeItem item = s;
                        string text = this.Document.ToString(item.Start, item.End - item.Start + 1);
                        text = OutlineActions.FitOutlineLevel(text, item, item.Level + 1);

                        Document.Replace(item.Start, item.End - item.Start + 1, text);
                        Document.RequestRedraw();

                        this.Items = OutlineAnalyzer.Analyze(this.FoldingStrategy, this.Document.LayoutLines, this.Document);
                    }
                },
                (s) => {
                    return this.FoldingStrategy is WZTextFoldingGenerator;
                });
            }
        }

        public RelayCommand<OutlineTreeItem> DownLevelCommand
        {
            get
            {
                return new RelayCommand<OutlineTreeItem>((s) =>
                {
                    if (s == null)
                        return;

                    if (this.FoldingStrategy is WZTextFoldingGenerator)
                    {
                        OutlineTreeItem item = s;
                        string text = this.Document.ToString(item.Start, item.End - item.Start + 1);
                        text = OutlineActions.FitOutlineLevel(text, item, item.Level - 1);

                        Document.Replace(item.Start, item.End - item.Start + 1, text);
                        Document.RequestRedraw();

                        this.Items = OutlineAnalyzer.Analyze(this.FoldingStrategy, this.Document.LayoutLines, this.Document);
                    }
                },
                (s)=> {
                    return this.FoldingStrategy is WZTextFoldingGenerator;
                });
            }
        }

        void SetToClipboard(OutlineTreeItem treeitem)
        {
            string text = this.Document.ToString(treeitem.Start, treeitem.End - treeitem.Start + 1);
            var dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }
    }
}

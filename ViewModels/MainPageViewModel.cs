using FooEditor.WinUI.Models;
using FooEditor.WinUI.Services;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace FooEditor.WinUI.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        IMainViewService MainViewService;
        DispatcherTimer timer;
        bool IsRequierDelayCleanStatusMessage = false;

        public MainPageViewModel(INavigationService navigationService,IMainViewService mainViewService)
        {
            this.DocumentList = DocumentCollection.Instance;
            this.NavigationService = navigationService;
            this.MainViewService = mainViewService;
        }

        public const string KeywordFolderName = "Keywords";

        public async Task Init(object param,bool require_restore , Dictionary<string, object> viewModelState)
        {
            this.DocumentList.ActiveDocumentChanged += DocumentList_ActiveDocumentChanged;

            await FolderModel.CopyFilesFromInstalledFolderToLocalSetting(KeywordFolderName);

            //復元する必要がある
            if (require_restore)
            {
                if (await this.MainViewService.ConfirmRestoreUserState())
                    await RestoreUserDocument(null);
            }

            await this.OpenFromArgs(param);

            if (this.DocumentList.Count == 0)
                this.DocumentList.AddNewDocument();

            //前回保存したときのごみが残っていることがある
            await DocumentCollection.CleanUp();

            this.timer = new DispatcherTimer();
            this.timer.Interval = new TimeSpan(0, 0, DocumentCollection.TimerTickInterval);
            this.timer.Tick += Timer_Tick;
            this.timer.Start();

        }

        public async Task OpenFromArgs(object args)
        {
            if (args != null)
            {
                ObjectToXmlConverter conv = new ObjectToXmlConverter();
                var files = conv.ConvertBack(args, typeof(string[]), null, null) as string[];
                if (files != null)
                {
                    foreach (string filepath in files)
                    {
                        try
                        {
                            var file = await FileModel.GetFileModel(FileModelBuildType.AbsolutePath, filepath);
                            await this.DocumentList.AddFromFile(file);
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            await this.MainViewService.MakeMessageBox(ex.Message);
                        }
                    }
                }
            }
        }

        private async Task RestoreUserDocument(Dictionary<string, object> viewModelState)
        {
            try
            {
                FileModel file = await FileModel.GetFileModel(FileModelBuildType.LocalFolder, "DocumentCollection.xml");
                await this.DocumentList.RestoreFrom(file);
                if (viewModelState != null && viewModelState.Count > 0)
                {
                    var selIndex = viewModelState["CurrentDocumentIndex"] as int?;
                    if (selIndex != null && selIndex < this.DocumentList.Count)
                        this.DocumentList.ActiveDocument(selIndex.Value);
                }
                await file.DeleteAsync();
                System.Diagnostics.Debug.WriteLine("restored previous document");
            }
            catch (FileNotFoundException)
            {

            }
        }

        private async void Timer_Tick(object sender, object e)
        {
            if(AppSettings.Current.EnableAutoSave)
            {
                await this.DocumentList.SaveDocumentCollection();
            }
            if (this.IsRequierDelayCleanStatusMessage)
            {
                this.StatusMessage = string.Empty;
                this.IsRequierDelayCleanStatusMessage = false;
            }
        }

        public async Task Suspend(Dictionary<string, object> viewModelState, bool suspending)
        {
            if (suspending)
            {
                viewModelState["CurrentDocumentIndex"] = this.DocumentList.CurrentDocumentIndex;    //選択中のドキュメントは別途保存する必要がある

                await AppSettings.Current.Save();
            }

            this.DocumentList.ActiveDocumentChanged -= DocumentList_ActiveDocumentChanged;
            if (this.timer != null)
            {
                this.timer.Stop();
                this.timer.Tick -= Timer_Tick;
                this.timer = null;
            }

        }

        private void DocumentList_ActiveDocumentChanged(object sender, DocumentCollectionEventArgs e)
        {
            this.OnPropertyChanged("CurrentDocument");
            this.DocumentList.Current.OnActivate();
        }

        DocumentCollection _doc_list;
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

        public DocumentInfoViewModel CurrentDocument
        {
            get
            {
                if (_doc_list == null)
                    return null;
                return this._doc_list.Current;
            }
            set
            {
                if (_doc_list == null)
                    return;
                this._doc_list.ActiveDocument(value);
                this.OnPropertyChanged();
            }
        }

        public RelayCommand<object> DoHilightCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    this.CurrentDocument.DocumentModel.Document.LayoutLines.HilightAll();
                });
            }
        }

        public RelayCommand<object> AddDocumentCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    this._doc_list.AddNewDocument();
                });
            }
        }

        public RelayCommand<object> RemoveDocumentCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    if(this._doc_list.Count > 1)
                    {
                        DocumentInfoViewModel doc = s as DocumentInfoViewModel;
                        this._doc_list.RemoveDocument(doc);
                    }
                });
            }
        }

        public RelayCommand<System.Text.Encoding> OpenFileCommand
        {
            get
            {
                return new RelayCommand<System.Text.Encoding>(async (s) => {
                    try
                    {
                        FileOpenPicker openPicker = this.MainViewService.GetFileOpenPicker();

                        openPicker.ViewMode = PickerViewMode.List;

                        openPicker.FileTypeFilter.Add("*");

                        openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

                        StorageFile file = await openPicker.PickSingleFileAsync();

                        if (file != null)
                        {
                            await this._doc_list.AddFromFile(file, s);
                        }
                    }catch(Exception ex)
                    {
                        await this.MainViewService.MakeMessageBox(ex.Message);
                    }
                });
            }
        }

        public RelayCommand<IEnumerable<string>> OpenFilePathCommand
        {
            get
            {
                return new RelayCommand<IEnumerable<string>>(async (s) => {
                    if (s == null)
                        return;
                    try
                    {
                        foreach (var file_path in s)
                        {
                            var file = await FileModel.GetFileModel(FileModelBuildType.AbsolutePath, file_path);
                            await this.DocumentList.AddFromFile(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        await this.MainViewService.MakeMessageBox(ex.Message);
                    }
                });
            }
        }

        public RelayCommand<IEnumerable<IStorageItem>> OpenFilesCommand
        {
            get
            {
                return new RelayCommand<IEnumerable<IStorageItem>>(async (s) => {
                    if (s == null)
                        return;
                    try
                    {
                        foreach (StorageFile file in s)
                        {
                            await this.DocumentList.AddFromFile(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        await this.MainViewService.MakeMessageBox(ex.Message);
                    }
                });
            }
        }

        public RelayCommand<object> SaveCommand
        {
            get
            {
                return new RelayCommand<object>(async (s) => {
                    if (this._doc_list.Current.DocumentModel.CurrentFilePath == null)
                    {
                        await SaveAs(null);
                        return;
                    }
                    FileModel fileModel = null;
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                    try
                    {
                        fileModel = await FileModel.GetFileModel(FileModelBuildType.AbsolutePath, this._doc_list.Current.DocumentModel.CurrentFilePath);
                        if (fileModel != null)
                        {
                            if (fileModel.IsNeedUserActionToSave())
                                await this.SaveAs(fileModel);
                            else
                            {
                                await this._doc_list.Current.DocumentModel.SaveFile(fileModel, null);
                                var str = string.Format(loader.GetString("NotifySaveCompleteText"), fileModel.Name);
                                this.StatusMessage = str;
                                this.IsRequierDelayCleanStatusMessage = true;
                            }
                        }
                    }
                    catch(UnauthorizedAccessException)
                    {
                        bool result = await this.MainViewService.Confirm(loader.GetString("UnauthorizedAccessMessage"), loader.GetString("YesButton"), loader.GetString("NoButton"));
                        if (result && fileModel != null)
                        {
                            try
                            {
                                await this._doc_list.Current.DocumentModel.SaveFile(fileModel, null, true);
                            }
                            catch(TaskCanceledException)
                            {
                                await this.MainViewService.MakeMessageBox(loader.GetString("SaveAsAdminFailedMessage"));
                            }
                            catch (PlatformNotSupportedException)
                            {
                                await this.MainViewService.MakeMessageBox(loader.GetString("NotSupportedMessage"));
                            }
                            catch (Win32Exception win32ex)  //UACのキャンセル時に発生するので
                            {
                                if (win32ex.NativeErrorCode != 1223)
                                    throw;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await this.MainViewService.MakeMessageBox(ex.Message);
                    }
                });
            }
        }

        public RelayCommand<System.Text.Encoding> SaveAsCommand
        {
            get
            {
                return new RelayCommand<System.Text.Encoding>(async (enc) => {
                    FileModel fileModel = null;
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                    try
                    {
                        if (this._doc_list.Current.DocumentModel.CurrentFilePath == null)
                        {
                            await SaveAs(null, enc);
                        }
                        else
                        {
                            fileModel = await FileModel.GetFileModel(FileModelBuildType.AbsolutePath, this._doc_list.Current.DocumentModel.CurrentFilePath);
                            await SaveAs(fileModel, enc);
                        }
                    }
                    /*
                     * ファイルダイアログで保存しようとしても管理者権限がないと保存ボタンが押せないので、UnauthorizedAccessException時のことは考えなくていい
                    */
                    catch (Exception ex)
                    {
                        await this.MainViewService.MakeMessageBox(ex.Message);
                    }
                });
            }
        }

        private async Task<StorageFile> ShowSaveFileDialog(FileModel suggestFile)
        {
            FileSavePicker savePicker = this.MainViewService.GetFileSavePicker();

            //これをつけないとファイルダイアログで拡張子を変えることができなくなる
            List<string> currentFileTypes = new List<string> { "." };
            if (suggestFile == null)
            {
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            }
            else
            {
                //現在開いているファイルの拡張子を追加すると未知の拡張子でもファイルダイアログに表示される
                currentFileTypes.Add(suggestFile.Extension);
            }
            savePicker.FileTypeChoices.Add("Current File Type", currentFileTypes);

            //ファイルタイプを追加した後じゃないとちょうどいい場所を提案してくれない
            if (suggestFile != null)
                savePicker.SuggestedSaveFile = suggestFile;

            ObservableCollection<FileType> collection = AppSettings.Current.FileTypeCollection;
            foreach (FileType type in collection)
            {
                savePicker.FileTypeChoices.Add(type.DocumentTypeName, type.ExtensionCollection);
            }

            StorageFile file = await savePicker.PickSaveFileAsync();

            return file;
        }

        private async Task SaveAs(FileModel suggestFile, System.Text.Encoding enc = null)
        {
            var file = await ShowSaveFileDialog(suggestFile);
            if (file != null)
            {
                FileModel fileModel = await FileModel.GetFileModel(file);
                this.timer.Stop();
                await this._doc_list.Current.DocumentModel.SaveFile(fileModel,enc);
                this.timer.Start();
                StorageApplicationPermissions.MostRecentlyUsedList.Add(file, file.Name);

                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                var str = string.Format(loader.GetString("NotifySaveCompleteText"), file.Name);
                this.StatusMessage = str;
                this.IsRequierDelayCleanStatusMessage = true;
            }
        }

        public RelayCommand<object> UndoCommand
        {
            get
            {
                return new RelayCommand<object>((param) =>
                {
                    this._doc_list.Current.DocumentModel.Document.UndoManager.undo();
                    this._doc_list.Current.DocumentModel.Document.RequestRedraw();
                });
            }
        }

        public RelayCommand<object> RedoCommand
        {
            get
            {
                return new RelayCommand<object>((param) =>
                {
                    this._doc_list.Current.DocumentModel.Document.UndoManager.redo();
                    this._doc_list.Current.DocumentModel.Document.RequestRedraw();
                });
            }
        }
        public RelayCommand<object> OpenSettingPageCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    NavigationService.Navigate("Setting", null);
                    this.IsNavPaneOpen = true;
                });
            }
        }

        public RelayCommand<object> PrintCommand
        {
            get
            {
                return new RelayCommand<object>(async (s) => {
                    try
                    {
                        await this.MainViewService.ShowPrintDialogAsync();
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine("Printer is not stanby");
                    }
                });
            }
        }

        public RelayCommand<object> CloseSideBarCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    this.NavigationService.ClearHistory();
                    this.IsNavPaneOpen = false;
                });
            }
        }

        public RelayCommand<object> BackSideBarCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    this.NavigationService.GoBack();
                });
            }
        }

        public RelayCommand<object> OpenOutlineCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    this.NavigationService.Navigate("Outline", null);
                    this.IsNavPaneOpen = true;
                });
            }
        }

        public RelayCommand<object> OpenFindAndReplaceCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    this.NavigationService.Navigate("FindReplace", null);
                    this.IsNavPaneOpen = true;
                });
            }
        }

        public RelayCommand<object> OpenGoToCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    this.NavigationService.Navigate("Goto", null);
                    this.IsNavPaneOpen = true;
                });
            }
        }

        public RelayCommand<object> OpenSnipeetCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    this.NavigationService.Navigate("Snipeet", null);
                    this.IsNavPaneOpen = true;
                });
            }
        }

        public RelayCommand<object> OpenDocumentInfoCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    this.NavigationService.Navigate("DocumentInfo", null);
                    this.IsNavPaneOpen = true;
                });
            }
        }

        string _StatusMessage;
        public string StatusMessage
        {
            get
            {
                return this._StatusMessage;
            }
            set
            {
                SetProperty(ref this._StatusMessage, value);
            }
        }

        #region Panel
        bool _IsOutlineOpen;
        public bool IsNavPaneOpen
        {
            get
            {
                return this._IsOutlineOpen;
            }
            set
            {
                SetProperty(ref this._IsOutlineOpen, value);
            }
        }
        #endregion

    }
}

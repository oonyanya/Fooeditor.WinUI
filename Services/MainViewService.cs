using FooEditor.WinUI.Models;
using FooEditor.WinUI.Views;
using CommunityToolkit.WinUI.Notifications;
using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows​.Data​.Xml​.Dom;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using WinRT.Interop;
using Windows.Graphics.Printing;

namespace FooEditor.WinUI.Services
{
    public interface IMainViewService
    {
        MainWindow MainPage
        {
            get;
            set;
        }
        Task<bool> ConfirmRestoreUserState();
        Task<bool> Confirm(string s, string yes_label, string no_label);
        Task MakeMessageBox(string s);
        void MakeNotifaction(string text);
        FileOpenPicker GetFileOpenPicker();
        FileSavePicker GetFileSavePicker();
        PrintManager GetPrintManager();
        Task ShowPrintDialogAsync();
    }

    public class MainViewService : IMainViewService
    {
        public MainWindow MainPage
        {
            get;
            set;
        }

        public async Task<bool> Confirm(string s, string yes_label, string no_label)
        {
            var msg = new MessageDialog(s, "");
            InitializeWithWindow.Initialize(msg, WindowNative.GetWindowHandle(this.MainPage));
            msg.Commands.Add(new UICommand(yes_label));
            msg.Commands.Add(new UICommand(no_label));
            var res = await msg.ShowAsync();
            return res.Label == yes_label;
        }

        public async Task<bool> ConfirmRestoreUserState()
        {
            if (!AppSettings.Current.EnableAutoSave)
                return false;
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            return await this.Confirm(loader.GetString("RestoreUsareStateMessage"),
                loader.GetString("YesButton"),
                loader.GetString("NoButton")
                );
        }

        public async Task MakeMessageBox(string s)
        {
            var msg = new MessageDialog(s);
            InitializeWithWindow.Initialize(msg, WindowNative.GetWindowHandle(this.MainPage));
            await msg.ShowAsync();
        }

        public FileOpenPicker GetFileOpenPicker()
        {
            var filepicker = new FileOpenPicker();
            InitializeWithWindow.Initialize(filepicker, WindowNative.GetWindowHandle(this.MainPage));
            return filepicker;
        }

        public FileSavePicker GetFileSavePicker()
        {
            var filepicker = new FileSavePicker();
            InitializeWithWindow.Initialize(filepicker, WindowNative.GetWindowHandle(this.MainPage));
            return filepicker;
        }

        public PrintManager GetPrintManager()
        {
            return PrintManagerInterop.GetForWindow(WindowNative.GetWindowHandle(this.MainPage));
        }

        public async Task ShowPrintDialogAsync()
        {
            await PrintManagerInterop.ShowPrintUIForWindowAsync(WindowNative.GetWindowHandle(this.MainPage));
        }

        public void MakeNotifaction(string text)
        {
            ToastContent toastContent = new ToastContent()
            {
                Scenario = ToastScenario.Default,
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = text
                            },
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = "ms-appx:///Assets/StoreLogo.png"
                        }
                    }
                }
            };


            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(toastContent.GetContent());

            var toast = new ToastNotification(xmlDoc);
            ToastNotificationManager.CreateToastNotifier().Show(toast); // Display toast

        }
    }
}

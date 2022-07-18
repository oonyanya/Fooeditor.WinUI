using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FooEditEngine.WinUI;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FooEditor.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            //EncodeDetectを正常に動作させるために必要
            //http://www.atmarkit.co.jp/ait/articles/1509/30/news039.html
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

#if !DEBUG
            AppCenter.Start("7fc70d1f-9bf6-4da7-b9f2-aa49d827b0fe",typeof(Analytics), typeof(Crashes));
#endif
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            FooTextBox.OwnerWindow = m_window;
            m_window.SetRootFrame(new Frame());
            await m_window.Init(null, false, null);
            m_window.Activate();
        }

        private MainWindow m_window;
    }
}

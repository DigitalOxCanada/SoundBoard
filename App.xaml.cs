using SoundBoardWPF.Views;
using System.Windows;

namespace SoundBoardWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string THEMEFOLDER = "themes";
        public string selectedThemeName = "theme1";
        public string selectedThemePath { get; set; }

        public bool ShowVideos { get; set; }
        public bool IsEditModeActive { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            selectedThemePath = $"{THEMEFOLDER}/{selectedThemeName}";
            ShowVideos = true;
            IsEditModeActive = false;
            ShutdownMode = ShutdownMode.OnMainWindowClose;

            MainWindow wnd = new MainWindow();
            MainWindow = wnd;
            wnd.Title = "SoundBoard";
            wnd.Show();
        }
    }
}

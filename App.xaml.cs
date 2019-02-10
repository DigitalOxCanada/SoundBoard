using NAudio.Wave;
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
        public string SelectedThemePath { get; set; }

        public bool ShowVideos { get; set; }
        public bool IsEditModeActive { get; set; }

        WaveOutEvent outputDevice;
        AudioFileReader audioFile;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SelectedThemePath = $"{THEMEFOLDER}/{selectedThemeName}";
            ShowVideos = true;
            IsEditModeActive = false;
            ShutdownMode = ShutdownMode.OnMainWindowClose;

            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }


            MainWindow wnd = new MainWindow();
            MainWindow = wnd;
            wnd.Title = "SoundBoard";
            wnd.Show();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (audioFile == null) return;
            audioFile.Dispose();
            audioFile = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="videofn"></param>
        public void PlayVideo(string videofn)
        {
            if (string.IsNullOrEmpty(videofn))
            {
                return;
            }

            if (ShowVideos)
            {
                ((MainWindow)MainWindow).subWindow.LoadVideo($"{SelectedThemePath}/{videofn}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audiofn"></param>
        public void PlaySound(string audiofn)
        {
            if (string.IsNullOrEmpty(audiofn))
            {
                return;
            }

            if (audioFile == null)
            {
                audioFile = new AudioFileReader($"{SelectedThemePath}/{audiofn}");
                outputDevice.Init(audioFile);
            }
            outputDevice.Play();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            outputDevice.Dispose();
            outputDevice = null;
        }
    }
}

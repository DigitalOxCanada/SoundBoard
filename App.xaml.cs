using DigitalOx.SoundBoard.Plugin;
using NAudio.Wave;
using Newtonsoft.Json;
using DigitalOx.SoundBoard.ViewModels;
using DigitalOx.SoundBoard.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;

namespace DigitalOx.SoundBoard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IPluginHost
    {
        public string THEMEFOLDER = "themes";
        public string selectedThemeName = "theme1";
        public string SelectedThemePath { get; set; }

        public MainViewModel mainViewModel { get; set; }

        public bool ShowVideos { get; set; }
        public bool IsEditModeActive { get; set; }
        public List<IPlugin> Plugins { get; private set; }

        WaveOutEvent outputDevice;
        AudioFileReader audioFile;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var s = new SplashScreen("Views/splashscreen.jpg");
            s.Show(false);

            SelectedThemePath = $"{THEMEFOLDER}/{selectedThemeName}";
            ShowVideos = true;
            IsEditModeActive = false;
            ShutdownMode = ShutdownMode.OnMainWindowClose;

            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }

            mainViewModel = new MainViewModel();

            LoadPlugins();

            MainWindowView wnd = new MainWindowView(mainViewModel);
            MainWindow = wnd;
            wnd.Show();

            s.Close(TimeSpan.FromSeconds(1));
        }

        private void LoadPlugins()
        {
            string path = Directory.GetCurrentDirectory() + @"\plugins";
            string[] pluginFiles = Directory.GetFiles(path, "*.dll", new EnumerationOptions() { RecurseSubdirectories = true });
            Plugins = new List<IPlugin>();

            foreach (var pluginFile in pluginFiles)
            {
                string pluginName = Path.GetFileNameWithoutExtension(pluginFile);
                if (pluginName=="IPlugin")
                {
                    //skip the Interface dll
                    continue;
                }

                IPlugin ip;
                Type ObjType = null;
                // load the dll
                try
                {
                    Assembly ass = null;
                    //ass = Assembly.Load(args);
                    ass = Assembly.LoadFile(pluginFile);
                    if (ass != null)
                    {
                        ObjType = ass.GetType($"DigitalOx.SoundBoard.Plugin.{pluginName}");
                    }
                }
                catch (Exception ex)
                {
                    //TODO we should be logging
                    MessageBox.Show(ex.Message);
                }

                try
                {

                    if (ObjType != null)
                    {
                        ip = (IPlugin)Activator.CreateInstance(ObjType);
                        ip.Host = this;
                        Plugins.Add(ip);

                        ip.IsEnabled = true;
                    }
                }
                catch(MissingMethodException ex)
                {
                    //ignore
                    //this is when it attempts to load the IPlugin.dll which is not an actual plugin, just the interface definition.
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

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
                ((MainWindowView)MainWindow).SubWindow.LoadVideo($"{SelectedThemePath}/{videofn}");
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
                //TODO fix crash if the file doesn't exist
                audioFile = new AudioFileReader($"{SelectedThemePath}/{audiofn}");
                outputDevice.Init(audioFile);
            }
            outputDevice.Play();
        }

        public void SaveTheme()
        {
            var json = JsonConvert.SerializeObject(mainViewModel);

            File.WriteAllText($"{SelectedThemePath}/{selectedThemeName}.json", json);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SaveTheme();

            outputDevice.Dispose();
            outputDevice = null;
        }

        public bool Register(IPlugin ipi)
        {
            return true;
        }
    }
}

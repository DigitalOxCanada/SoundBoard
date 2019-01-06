using GlobalHotKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Wave;
using Newtonsoft.Json;

namespace SoundBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //TODO editor mode
        //TODO allow drag and drop of media files onto buttons while in edit mode
        //TODO wire up menu items
        //TODO add functionality to stop video and audio currently playing
        //TODO change themes from within the gui
        //TODO varied sizes of buttons defined in the theme file (can't with wrap grid so what is alternative)
        //TODO xaml template for look and feel of the buttons (font size, colour, etc.)
        //TODO use databinding on button props
        const string THEMEFOLDER = "themes";
        HotKeyManager hotKeyManager = new HotKeyManager();
        WaveOutEvent outputDevice;
        AudioFileReader audioFile;

        List<ActionButton> hotKeyButtons = new List<ActionButton>();

        private SubWindow subWindow = new SubWindow();

        App myApp;

        const string selectedThemeName = "theme1";
        
        //TODO make this dynamic
        private string selectedThemePath = $"{THEMEFOLDER}/{selectedThemeName}";

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            myApp = (App)Application.Current;
            menuShowVideos.IsChecked = myApp.ShowVideos;
            ShowVideosChanged();

            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }

            //TODO search for available themes in output folder (themes/themename/themename.json)

            //TODO if there is only 1 theme then just automatically use it, otherwise maybe prompt for which one?

            string json = System.IO.File.ReadAllText($"{selectedThemePath}/{selectedThemeName}.json");

            //deserialize the json theme information as hotkeybuttons
            hotKeyButtons = JsonConvert.DeserializeObject<List<ActionButton>>(json);

            //for each button assign the hotkey and create the visual button with details
            foreach (var btn in hotKeyButtons)
            {
                bool AudioFileExists = false;
                bool VideoFileExists = false;


                if (!String.IsNullOrEmpty(btn.Audio))
                {
                    if(System.IO.File.Exists($"{selectedThemePath}/{btn.Audio}"))
                    {
                        AudioFileExists = true;
                    }
                }

                if (!String.IsNullOrEmpty(btn.Video))
                {
                    if (System.IO.File.Exists($"{selectedThemePath}/{btn.Video}"))
                    {
                        VideoFileExists = true;
                        btn.imgVideo.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    btn.imgVideo.Visibility = Visibility.Hidden;
                }

                if (!string.IsNullOrEmpty(btn.Color))
                {
                    //newButton.Background = new SolidColorBrush(Colors.AliceBlue);
                    var color = (Color)ColorConverter.ConvertFromString(btn.Color);
                    btn.Background = new SolidColorBrush(color);
                }


                if (AudioFileExists | VideoFileExists)
                {
                    //only wire up the hotkey if one is specified and that there is something to play (audio or video), otherwise the user must click the button
                    if (btn.TheKey != null)
                    {
                        btn.TheHotKey = hotKeyManager.Register((Key)btn.TheKey, ModifierKeys.Control | ModifierKeys.Alt);
                    }
                }
                else
                {
                    btn.IsEnabled = false;
                }

                
                string title = $"{btn.Title}";

                if (string.IsNullOrEmpty(btn.Title))
                {
                    title = btn.Audio;
                }

                btn.txtRow1.Text = title;

                if (btn.TheKey != null)
                {
                    btn.txtRow2.Text = $"{btn.TheKey}";
                }
                else
                {
                    btn.txtRow2.Text = "";
                }
                
                ImageBrush brush = new ImageBrush();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                string img = "nopic.png";

                //only if image is specified we try to load the image, if no image we use default nopic.png
                if (!String.IsNullOrEmpty(btn.Image)) {
                    if(System.IO.File.Exists($"{selectedThemePath}/{btn.Image}"))
                    {
                        img = $"{selectedThemePath}/{btn.Image}";
                    }
                    bitmap.UriSource = new Uri($"{img}", UriKind.Relative);
                    bitmap.EndInit();
                    brush.ImageSource = bitmap;
                    btn.Background = brush;
                }

                btn.Click += PlaySoundButton_Click;
                wrapPanel.Children.Add(btn);
            }

            hotKeyManager.KeyPressed += HotKeyManagerPressed;

            if (myApp.ShowVideos)
            {
                subWindow.Show();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaySoundButton_Click(object sender, RoutedEventArgs e)
        {
            ActionButton btn = (ActionButton)sender;
            PlaySound(btn.Audio);
            PlayVideo(btn.Video);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="video"></param>
        private void PlayVideo(string video)
        {
            if (string.IsNullOrEmpty(video)) return;

            if (myApp.ShowVideos)
            {
                subWindow.LoadVideo($"{selectedThemePath}/{video}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            var btn = (from p in hotKeyButtons where p.TheKey == e.HotKey.Key select p).SingleOrDefault();
            if(btn!=null)
            {
                PlaySound(btn.Audio);
                PlayVideo(btn.Video);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audiofn"></param>
        private void PlaySound(string audiofn)
        {
            if (string.IsNullOrEmpty(audiofn)) return;

            if (audioFile == null)
            {
                audioFile = new AudioFileReader($"{selectedThemePath}/{audiofn}");
                outputDevice.Init(audioFile);
            }
            outputDevice.Play();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            audioFile.Dispose();
            audioFile = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            outputDevice.Dispose();
            outputDevice = null;

            // Unregister Ctrl+Alt+Key hotkey.
            var collection = wrapPanel.Children.OfType<ActionButton>().ToList();
            foreach (var btn in collection)
            {
                if (btn.TheHotKey != null)
                {
                    hotKeyManager.Unregister(btn.TheHotKey);
                }
            }

            // Dispose the hotkey manager.
            hotKeyManager.Dispose();
        }


        private void ShowVideos_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            myApp.ShowVideos = !myApp.ShowVideos;
            ShowVideosChanged();
        }

        private void ShowVideosChanged()
        {
            menuShowVideos.IsChecked = myApp.ShowVideos;
            if (!myApp.ShowVideos)
            {
                subWindow.Hide();
            }
            else
            {
                subWindow.Show();
            }
        }

        private void Exit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            myApp.Shutdown();
        }
    }
}

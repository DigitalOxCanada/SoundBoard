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
        //TODO change themes from within the gui
        //TODO varied sizes of buttons defined in the theme file
        //TODO xaml template for look and feel of the buttons (font size, colour, etc.)
        const string THEMEFOLDER = "themes";
        const string selectedThemeName = "theme1";


        HotKeyManager hotKeyManager = new HotKeyManager();
        WaveOutEvent outputDevice;
        AudioFileReader audioFile;

        List<HotKeyButton> hotKeyButtons = new List<HotKeyButton>();

        private SubWindow subWindow = new SubWindow();

        private string selectedThemePath = $"{THEMEFOLDER}/{selectedThemeName}";

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }

            //TODO search for available themes in output folder (themes/themename/themename.json)

            //TODO if there is only 1 theme then just automatically use it, otherwise maybe prompt for which one?

            string json = System.IO.File.ReadAllText($"{selectedThemePath}/{selectedThemeName}.json");

            //deserialize the json theme information as hotkeybuttons
            hotKeyButtons = JsonConvert.DeserializeObject<List<HotKeyButton>>(json);

            //for each button assign the hotkey and create the visual button with details
            foreach (var hkb in hotKeyButtons)
            {
                bool AudioFileExists = false;

                if(!String.IsNullOrEmpty(hkb.Audio))
                {
                    if(System.IO.File.Exists($"{selectedThemePath}/{hkb.Audio}"))
                    {
                        AudioFileExists = true;
                    }
                }

                MyButton newButton = new MyButton();
                newButton.TheHkb = hkb;
                if (AudioFileExists)
                {
                    newButton.TheHotKey = hotKeyManager.Register(hkb.TheKey, ModifierKeys.Control | ModifierKeys.Alt);
                }else
                {
                    newButton.IsEnabled = false;
                }

                newButton.Margin = new Thickness(5);
                var sp = new StackPanel();
                var tb1 = new TextBlock() { 
                    Text = $"{hkb.Title}",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                };

                var tb2 = new TextBlock()
                {
                    Text = $"{hkb.TheKey}",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                };
                sp.Children.Add(tb1);
                sp.Children.Add(tb2);
                newButton.Content = sp;
                ImageBrush brush = new ImageBrush();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                string img = "nopic.png";

                if (!String.IsNullOrEmpty(hkb.Image)) {
                    if(System.IO.File.Exists($"{selectedThemePath}/{hkb.Image}"))
                    {
                        img = $"{selectedThemePath}/{hkb.Image}";
                    }
                }

                bitmap.UriSource = new Uri($"{img}", UriKind.Relative);
                bitmap.EndInit();
                brush.ImageSource = bitmap;
                newButton.Background = brush;
                newButton.Click += PlaySoundButton_Click;
                wrapPanel.Children.Add(newButton);
            }

            hotKeyManager.KeyPressed += HotKeyManagerPressed;

            subWindow.Show();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaySoundButton_Click(object sender, RoutedEventArgs e)
        {
            MyButton btn = (MyButton)sender;
            PlaySound(btn.TheHkb.Audio);
            PlayVideo(btn.TheHkb.Video);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="video"></param>
        private void PlayVideo(string video)
        {
            if (string.IsNullOrEmpty(video)) return;

            subWindow.LoadVideo($"{selectedThemePath}/{video}");
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
            var collection = wrapPanel.Children.OfType<MyButton>().ToList();
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
    }
}

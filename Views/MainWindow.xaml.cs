using ColorPickerWPF;
using GlobalHotKey;
using Microsoft.Win32;
using NAudio.Wave;
using Newtonsoft.Json;
using SoundBoardWPF.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SoundBoardWPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //TODO allow drag and drop of media files onto buttons while in edit mode
        //TODO add functionality to stop video and audio currently playing
        //TODO change themes from within the gui
        //TODO varied sizes of buttons defined in the theme file (can't with wrap grid so what is alternative)
        readonly HotKeyManager hotKeyManager = new HotKeyManager();
        WaveOutEvent outputDevice;
        AudioFileReader audioFile;

        private readonly SubWindow subWindow = new SubWindow();

        public MyViewModel myViewModel { get; set; }

        readonly App myApp;


        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            myViewModel = new MyViewModel();
            DataContext = myViewModel;
            myApp = (App)Application.Current;

            InitializeComponent();

            menuShowVideos.IsChecked = myApp.ShowVideos;

            ShowVideosChanged();


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayActionButton_Click(object sender, RoutedEventArgs e)
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
            if (string.IsNullOrEmpty(video))
            {
                return;
            }

            if (myApp.ShowVideos)
            {
                subWindow.LoadVideo($"{myApp.selectedThemePath}/{video}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            var btn = (from p in myViewModel.ActionButtons where p.TheKey == e.HotKey.Key select p).SingleOrDefault();
            if (btn != null)
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
            if (string.IsNullOrEmpty(audiofn))
            {
                return;
            }

            if (audioFile == null)
            {
                audioFile = new AudioFileReader($"{myApp.selectedThemePath}/{audiofn}");
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
            if (audioFile == null) return;
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
            foreach (var btn in myViewModel.ActionButtons)
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
            //TODO detect if changes were made and save theme
            myApp.Shutdown();
        }

        private void Edit_NewActionButton_Click(object sender, RoutedEventArgs e)
        {
            //add a blank new action button the scene
            ActionButton btn = new ActionButton();
            btn.Title = "New";
            myViewModel.ActionButtons.Add(btn);
            btn.UpdateMedia();
            WireUpActionButton(btn);
            myViewModel.SelectedActionButton = btn;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            myApp.IsEditModeActive = true;
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            myApp.IsEditModeActive = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }

            //TODO search for available themes in output folder (themes/themename/themename.json)

            //TODO if there is only 1 theme then just automatically use it, otherwise maybe prompt for which one?

            string json = System.IO.File.ReadAllText($"{myApp.selectedThemePath}/{myApp.selectedThemeName}.json");

            //deserialize the json theme information
            myViewModel = JsonConvert.DeserializeObject<MyViewModel>(json);
            DataContext = myViewModel;

            //for each button assign the hotkey and create the visual button with details
            foreach (var btn in myViewModel.ActionButtons)
            {
                //load up images on buttons
                btn.UpdateMedia();
                WireUpActionButton(btn);
                if (btn.TheKey != null)
                {
                    btn.TheHotKey = hotKeyManager.Register((Key)btn.TheKey, ModifierKeys.Control | ModifierKeys.Alt);
                }
            }

            hotKeyManager.KeyPressed += HotKeyManagerPressed;

            if (myApp.ShowVideos)
            {
                subWindow.Show();
            }
        }

        private void WireUpActionButton(ActionButton btn)
        {
            btn.MouseDoubleClick += PlayActionButton_Click;
            btn.Click += SelectAction_Click;
        }

        private void SelectAction_Click(object sender, RoutedEventArgs e)
        {
            myViewModel.SelectedActionButton = sender as ActionButton;
        }

        private void ChangeColorButton_Click(object sender, RoutedEventArgs e)
        {
            Color color;
            bool ok = ColorPickerWindow.ShowDialog(out color);
            if(ok)
            {
                myViewModel.SelectedActionButton.Color = color.ToString();
            }
        }

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                //imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
                //copy the file from source to the current theme path keeping the filename.
                string outputFn = System.IO.Path.Combine(myApp.selectedThemePath, System.IO.Path.GetFileName(op.FileName));
                System.IO.File.Copy(op.FileName, outputFn, true);
                myViewModel.SelectedActionButton.Image = System.IO.Path.GetFileName(op.FileName);
            }
        }

        private void PasteFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var imgdata = Clipboard.GetImage();
                if(imgdata==null) return;
                string newfilename = myViewModel.SelectedActionButton.Title + ".bmp";
                string outputFn = System.IO.Path.Combine(myApp.selectedThemePath, newfilename);
                using (var fileStream = new FileStream(outputFn, FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(imgdata));
                    encoder.Save(fileStream);
                }
                myViewModel.SelectedActionButton.Image = newfilename;
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Paste failed", MessageBoxButton.OK);
            }
        }
        private byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
            {
                return null;
            }
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }
}

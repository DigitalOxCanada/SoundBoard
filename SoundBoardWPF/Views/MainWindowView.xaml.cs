using ColorPickerWPF;
using GlobalHotKey;
using Microsoft.Win32;
using NAudio.Wave;
using Newtonsoft.Json;
using DigitalOx.SoundBoard.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace DigitalOx.SoundBoard.Views
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView
    {
        //TODO allow drag and drop of media files onto buttons while in edit mode
        //TODO add functionality to stop video and audio currently playing
        //TODO change profiles from within the gui
        //TODO varied sizes of buttons defined in the profile file (can't with wrap grid so what is alternative)
        readonly HotKeyManager hotKeyManager = new HotKeyManager();

        public SubWindowView SubWindow { get; set; }

        readonly App myApp;


        /// <summary>
        /// 
        /// </summary>
        public MainWindowView(MainViewModel mvm)
        {
            myApp = (App)Application.Current;
            SubWindow = new SubWindowView();
            DataContext = mvm;

            InitializeComponent();

            menuShowVideos.IsChecked = myApp.ShowVideos;

            ShowVideosChanged();


        }

        public MainWindowView()
        {
        }



        private void LoadProfile()
        {
            string json = File.ReadAllText($"{myApp.SelectedProfilePath}/{myApp.selectedProfileName}.json");

            //deserialize the json profile information
            myApp.mainViewModel = JsonConvert.DeserializeObject<MainViewModel>(json);
            DataContext = myApp.mainViewModel;

            //for each button assign the hotkey and create the visual button with details
            foreach (var btn in myApp.mainViewModel.ActionButtons)
            {
                //load up images on buttons
                btn.UpdateMedia();
                WireUpActionButton(btn);
                if (btn.TheKey != null)
                {
                    btn.TheHotKey = hotKeyManager.Register((Key)btn.TheKey, ModifierKeys.Control | ModifierKeys.Alt);
                }
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //TODO search for available profiles in output folder (profiles/profilename/profilename.json)

            //TODO if there is only 1 profile then just automatically use it, otherwise maybe prompt for which one?

            LoadProfile();

            hotKeyManager.KeyPressed += HotKeyManagerPressed;

            if (myApp.ShowVideos)
            {
                SubWindow.Show();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            var btn = (from p in myApp.mainViewModel.ActionButtons where p.TheKey == e.HotKey.Key select p).SingleOrDefault();
            if (btn != null)
            {
                //PlaySound(btn.Audio);
                //PlayVideo(btn.Video);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

            // Unregister Ctrl+Alt+Key hotkey.
            foreach (var btn in myApp.mainViewModel.ActionButtons)
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
                SubWindow.Hide();
            }
            else
            {
                SubWindow.Show();
            }
        }

        private void Exit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            myApp.Shutdown();
        }

        private void Edit_NewActionButton_Click(object sender, RoutedEventArgs e)
        {
            //add a blank new action button the scene
            ActionButton btn = new ActionButton();
            btn.Title = "New";
            myApp.mainViewModel.ActionButtons.Add(btn);
            btn.UpdateMedia();
            WireUpActionButton(btn);
            myApp.mainViewModel.SelectedActionButton = btn;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            myApp.IsEditModeActive = true;
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            myApp.IsEditModeActive = false;
        }


        private void WireUpActionButton(ActionButton btn)
        {
            //btn.MouseDoubleClick += PlayActionButton_Click;
            btn.Click += SelectAction_Click;
        }

        private void SelectAction_Click(object sender, RoutedEventArgs e)
        {
            myApp.mainViewModel.SelectedActionButton = sender as ActionButton;
        }

        private void ChangeColorButton_Click(object sender, RoutedEventArgs e)
        {
            Color color;
            bool ok = ColorPickerWindow.ShowDialog(out color);
            if(ok)
            {
                myApp.mainViewModel.SelectedActionButton.Color = color.ToString();
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
                //copy the file from source to the current profile path keeping the filename.
                string outputFn = System.IO.Path.Combine(myApp.SelectedProfilePath, System.IO.Path.GetFileName(op.FileName));
                System.IO.File.Copy(op.FileName, outputFn, true);
                myApp.mainViewModel.SelectedActionButton.Image = System.IO.Path.GetFileName(op.FileName);
            }
        }

        private void PasteFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var imgdata = Clipboard.GetImage();
                if(imgdata==null) return;
                string newfilename = myApp.mainViewModel.SelectedActionButton.Title + ".bmp";
                string outputFn = System.IO.Path.Combine(myApp.SelectedProfilePath, newfilename);
                using (var fileStream = new FileStream(outputFn, FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(imgdata));
                    encoder.Save(fileStream);
                }
                myApp.mainViewModel.SelectedActionButton.Image = newfilename;
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

        private void ItemsPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            myApp.mainViewModel.SelectedActionButton = null;
        }

        private void RemoveActionButton_Click(object sender, RoutedEventArgs e)
        {
            myApp.mainViewModel.ActionButtons.Remove(myApp.mainViewModel.SelectedActionButton);
            myApp.mainViewModel.SelectedActionButton = null;
        }

        private void NewActionButton_Click(object sender, RoutedEventArgs e)
        {
            var nav = new NewActionView();
            bool? dialogResult = nav.ShowDialog();
            if(dialogResult==true)
            {
                //user clicked the add
                myApp.mainViewModel.SelectedActionButton.Actions.Add(new ActionBase() { IsPlugin = true, Action = nav.nvm.SelectedPlugin.GetType().Name, Data = nav.nvm.DataTemplateValue });
            }
        }

        private void RemovePicture_Click(object sender, RoutedEventArgs e)
        {
            myApp.mainViewModel.SelectedActionButton.Image = null;
        }
        private void RemoveActionFromButton_Click(object sender, RoutedEventArgs e)
        {
            Button cmd = (Button)sender;
            if (cmd.DataContext is ActionBase)
            {
                ActionBase act = (ActionBase)cmd.DataContext;
                myApp.mainViewModel.SelectedActionButton.Actions.Remove(act);
            }
        }

        private async void PlayAction_Click(object sender, RoutedEventArgs e)
        {
            Button cmd = (Button)sender;
            if(cmd.DataContext is ActionBase)
            {
                ActionBase act = (ActionBase)cmd.DataContext;
                await myApp.ExecuteActionAsync(act);
            }
        }
    }
}

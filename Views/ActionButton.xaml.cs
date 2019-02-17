using GlobalHotKey;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;

namespace DigitalOx.SoundBoard.Views
{

    public class ActionBase
    {
        public bool IsPlugin { get; set; }
        public string Action { get; set; }
        public string Data { get; set; }
    }

    /// <summary>
    /// Interaction logic for ActionButton.xaml
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public partial class ActionButton : Button, INotifyPropertyChanged
    {
        [JsonProperty]
        public ObservableCollection<ActionBase> Actions { get; set; }

        private Key? theKey;

        [JsonProperty]
        public Key? TheKey
        {
            get { return theKey; }
            set
            {
                if (value != this.theKey)
                {
                    this.theKey = value;
                    OnPropertyChanged("TheKey");
                }
            }
        }

         
        private string title;

        [JsonProperty]
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (value != this.title)
                {
                    this.title = value;
                    OnPropertyChanged("Title");
                }
            }
        }


        private string _image;
        [JsonProperty]
        public string Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged("Image"); }
        }

        private string _color;

        [JsonProperty]
        public string Color
        {
            get { return _color; }
            set
            {
                if (value != this._color)
                {
                    this._color = value;
                    OnPropertyChanged("Color");
                }
            }
        }

        public bool HasVideo { get { return hasVideo; } set { hasVideo = value; OnPropertyChanged("HasVideo"); } }

        public HotKey TheHotKey { get; set; }

        private App myApp;
        private bool hasVideo;

        public ActionButton()
        {
            InitializeComponent();

            Actions = new ObservableCollection<ActionBase>();

            myApp = (App)Application.Current;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChangedEventHandler eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        string[] SupportedVideoExtensions = new string[] { ".mpg", ".mpeg", ".wmv", ".mp4" };

        public void UpdateMedia()
        {
            //imgVideo.Visibility = Visibility.Hidden;

            foreach(var action in Actions)
            {
                if(action.Action == "PlayMedia")
                {
                    if(SupportedVideoExtensions.Contains(Path.GetExtension(action.Data))) 
                    {
                        HasVideo = true;
                    }
                }
            }


            if (string.IsNullOrEmpty(Title))
            {
                Title = "No Title";
            }
        }

//        private void Button_Drop(object sender, DragEventArgs e)
//        {
//            if (e.Data.GetDataPresent(DataFormats.FileDrop))
//            {
//                // Note that you can have more than one file.
//                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
//                string file = files[0];

//                //copy the file from source to the current theme path keeping the filename.
//                System.IO.File.Copy(file, System.IO.Path.Combine(((App)Application.Current).SelectedThemePath, System.IO.Path.GetFileName(file)), true);

//                //TODO move the file to the current theme path
//                //TODO associate the file to image or video or audio depending on extension.
//                switch (System.IO.Path.GetExtension(file).ToLower())
//                {
//                    //audio
//                    case ".wav":
//                    case ".mp3":
////                        Audio = System.IO.Path.GetFileName(file);
//                        break;

//                    //video
//                    case ".mpg":
//                    case ".mpeg":
//                    case ".wmv":
//                    case ".mp4":
////                        Video = System.IO.Path.GetFileName(file);
//                        break;

//                    //image
//                    case ".jpg":
//                    case ".jpeg":
//                    case ".png":
//                        //TODO add background brush
//                        break;
//                }
//            }
//        }

        private async void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ActionButton btn = (ActionButton)sender;

            foreach(var action in btn.Actions)
            {
                if (action.IsPlugin)
                {
                    //deal with plugin
                    var p = (from plugin in myApp.Plugins where plugin.GetType().Name == action.Action && plugin.IsEnabled select plugin).SingleOrDefault();
                    if(p!=null)
                    {
                        var ret = await p.DoWorkAsync(action.Data);
                    }
                }
                else
                {
                    //no plugin, so just process action directly.

                    switch (action.Action)
                    {
                        case "PlayMedia":
                            switch (System.IO.Path.GetExtension(action.Data).ToLower())
                            {
                                //audio
                                case ".wav":
                                case ".mp3":
                                    myApp.PlaySound(action.Data);
                                    break;

                                //video
                                case ".mpg":
                                case ".mpeg":
                                case ".wmv":
                                case ".mp4":
                                    myApp.PlayVideo(action.Data);
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        //private void OpenUrl(string url)
        //{
        //    try
        //    {
        //        Process.Start(url);
        //    }
        //    catch
        //    {
        //        // hack because of this: https://github.com/dotnet/corefx/issues/10361
        //        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        //        {
        //            url = url.Replace("&", "^&");
        //            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        //        }
        //        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        //        {
        //            Process.Start("xdg-open", url);
        //        }
        //        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        //        {
        //            Process.Start("open", url);
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

    }

}

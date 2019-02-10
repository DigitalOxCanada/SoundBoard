using GlobalHotKey;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SoundBoardWPF.Views
{
    /// <summary>
    /// Interaction logic for ActionButton.xaml
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public partial class ActionButton : Button, INotifyPropertyChanged
    {

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


        [JsonProperty]
        public string Audio { get; set; }

        private string _image;
        [JsonProperty]
        public string Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged("Image"); }
        }

        [JsonProperty]
        public string Video { get; set; }

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
        //public string Color { get { return _color; } set {
        //        if (!string.IsNullOrEmpty(value))
        //        {
        //            var color = (Color)ColorConverter.ConvertFromString(value);
        //            BGColor = new SolidColorBrush(color);
        //            _color = value;
        //            OnPropertyChanged("BGColor");
        //        }
        //    }
        //}

        //public SolidColorBrush BGColor { get; set; }

        public Visibility IsVideo { get { if (Video != null) return Visibility.Visible; else return Visibility.Hidden; } }

        public HotKey TheHotKey { get; set; }

        private App myApp;

        public ActionButton()
        {
            InitializeComponent();

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

        public void UpdateMedia()
        {
            bool AudioFileExists = false;
            bool VideoFileExists = false;

            imgVideo.Visibility = Visibility.Hidden;

            if (!String.IsNullOrEmpty(Audio))
            {
                if (System.IO.File.Exists($"{myApp.selectedThemePath}/{Audio}"))
                {
                    AudioFileExists = true;
                }
            }

            if (!String.IsNullOrEmpty(Video))
            {
                if (System.IO.File.Exists($"{myApp.selectedThemePath}/{Video}"))
                {
                    VideoFileExists = true;
                    imgVideo.Visibility = Visibility.Visible;
                }
            }
            else
            {
                imgVideo.Visibility = Visibility.Hidden;
            }


            if (AudioFileExists | VideoFileExists)
            {
                //only wire up the hotkey if one is specified and that there is something to play (audio or video), otherwise the user must click the button
                //if (TheKey != null)
                //{
                //    TheHotKey = hotKeyManager.Register((Key)TheKey, ModifierKeys.Control | ModifierKeys.Alt);
                //}
            }
            else
            {
                //IsEnabled = false;
            }

            if (string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Audio))
            {
                Title = Audio;
            }


            //ImageBrush brush = new ImageBrush();
            //BitmapImage bitmap = new BitmapImage();
            //bitmap.BeginInit();
            //string img = "nopic.png";

            ////only if image is specified we try to load the image, if no image we use default nopic.png
            //if (!string.IsNullOrEmpty(Image))
            //{
            //    if (System.IO.File.Exists($"{myApp.selectedThemePath}/{Image}"))
            //    {
            //        img = $"{myApp.selectedThemePath}/{Image}";
            //    }
            //    bitmap.UriSource = new Uri($"{img}", UriKind.Relative);
            //    bitmap.EndInit();
            //    brush.ImageSource = bitmap;
            //    Background = brush;
            //}

        }

        private void Button_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string file = files[0];

                //copy the file from source to the current theme path keeping the filename.
                System.IO.File.Copy(file, System.IO.Path.Combine(((App)Application.Current).selectedThemePath, System.IO.Path.GetFileName(file)), true);

                //TODO move the file to the current theme path
                //TODO associate the file to image or video or audio depending on extension.
                switch (System.IO.Path.GetExtension(file).ToLower())
                {
                    //audio
                    case ".wav":
                    case ".mp3":
                        Audio = System.IO.Path.GetFileName(file);
                        break;

                    //video
                    case ".mpg":
                    case ".mpeg":
                    case ".wmv":
                    case ".mp4":
                        Video = System.IO.Path.GetFileName(file);
                        break;

                    //image
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                        //TODO add background brush
                        break;
                }
            }
        }
    }
}

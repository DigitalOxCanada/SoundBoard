using GlobalHotKey;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SoundBoardWPF.Views
{
    public class ActionBase
    {
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


        //[JsonProperty]
        //public string Audio { get; set; }

        private string _image;
        [JsonProperty]
        public string Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged("Image"); }
        }

        //[JsonProperty]
        //public string Video { get; set; }

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

//        public Visibility IsVideo { get { if (Video != null) return Visibility.Visible; else return Visibility.Hidden; } }

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
            //bool AudioFileExists = false;
            //bool VideoFileExists = false;

            imgVideo.Visibility = Visibility.Hidden;

            //if (!String.IsNullOrEmpty(Audio))
            //{
            //    if (System.IO.File.Exists($"{myApp.selectedThemePath}/{Audio}"))
            //    {
            //        AudioFileExists = true;
            //    }
            //}

            //if (!String.IsNullOrEmpty(Video))
            //{
            //    if (System.IO.File.Exists($"{myApp.selectedThemePath}/{Video}"))
            //    {
            //        VideoFileExists = true;
            //        imgVideo.Visibility = Visibility.Visible;
            //    }
            //}
            //else
            //{
            //    imgVideo.Visibility = Visibility.Hidden;
            //}


            //if (string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Audio))
            //{
            //    Title = Audio;
            //}
            if(string.IsNullOrEmpty(Title))
            {
                Title = "No Title";
            }

        }

        private void Button_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string file = files[0];

                //copy the file from source to the current theme path keeping the filename.
                System.IO.File.Copy(file, System.IO.Path.Combine(((App)Application.Current).SelectedThemePath, System.IO.Path.GetFileName(file)), true);

                //TODO move the file to the current theme path
                //TODO associate the file to image or video or audio depending on extension.
                switch (System.IO.Path.GetExtension(file).ToLower())
                {
                    //audio
                    case ".wav":
                    case ".mp3":
//                        Audio = System.IO.Path.GetFileName(file);
                        break;

                    //video
                    case ".mpg":
                    case ".mpeg":
                    case ".wmv":
                    case ".mp4":
//                        Video = System.IO.Path.GetFileName(file);
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

        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ActionButton btn = (ActionButton)sender;
            foreach(var action in btn.Actions)
            {
                switch(action.Action)
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
}

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

        private async void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ActionButton btn = (ActionButton)sender;

            foreach(var action in btn.Actions)
            {
                await myApp.ExecuteActionAsync(action);
            }
        }

    }
}

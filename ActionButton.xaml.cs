using GlobalHotKey;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace SoundBoardWPF
{
    /// <summary>
    /// Interaction logic for ActionButton.xaml
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public partial class ActionButton : Button, INotifyPropertyChanged
    {
        
        //public Key? TheKey { get; set; }

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
                    OnPropertyChanged("TheKeyStr");
                }
            }
        }

        public string TheKeyStr
        {
            get { if (theKey != null) return theKey.ToString(); else return ""; }
        }


        private string title;

        [JsonProperty]
        public string Title
        {
            get { return title; }
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
        [JsonProperty]
        public string Image { get; set; }
        [JsonProperty]
        public string Video { get; set; }
        [JsonProperty]
        public string Color { get; set; }

        public Visibility IsVideo { get { if (Video != null) return Visibility.Visible; else return Visibility.Hidden; } }

        public HotKey TheHotKey { get; set; }

        public ActionButton()
        {
            InitializeComponent();

            imgVideo.Visibility = Visibility.Hidden;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Button_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var file = files[0];

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

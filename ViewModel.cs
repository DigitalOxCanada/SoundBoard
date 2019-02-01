using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundBoardWPF
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MyViewModel : INotifyPropertyChanged
    {
        private string themeName;

        [JsonProperty]
        public string ThemeName
        {
            get { return themeName; }
            set
            {
                if (value != this.themeName)
                {
                    this.themeName = value;
                    OnPropertyChanged("ThemeName");
                }
            }
        }

        [JsonProperty]
        public ObservableCollection<ActionButton> ActionButtons { get; set; }

        public MyViewModel()
        {
            //ThemeName = "unknown";
            //ActionButtons = new ObservableCollection<ActionButton>()
            //{
            //    new ActionButton { Title = "Action 1" },
            //    new ActionButton { Title = "Action 2" }
            //};
            //ActionButtons[0].TheKey = System.Windows.Input.Key.NumPad0;
            //ActionButtons[1].TheKey = System.Windows.Input.Key.NumPad1;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

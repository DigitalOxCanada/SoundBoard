using Newtonsoft.Json;
using DigitalOx.SoundBoard.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DigitalOx.SoundBoard.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MainViewModel : INotifyPropertyChanged
    {
        private string profile;
        private ActionButton selectedActionButton;

        [JsonProperty]
        public string Profile
        {
            get { return profile; }
            set
            {
                if (value != this.profile)
                {
                    this.profile = value;
                    OnPropertyChanged("Profile");
                }
            }
        }

        public ActionButton SelectedActionButton {
            get { return selectedActionButton; }
            set
            {
                if (value != this.selectedActionButton)
                {
                    this.selectedActionButton = value;
                    OnPropertyChanged("SelectedActionButton");
                }
            }
        }

        [JsonProperty]
        public ObservableCollection<ActionButton> ActionButtons { get; set; }

        public MainViewModel()
        {
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

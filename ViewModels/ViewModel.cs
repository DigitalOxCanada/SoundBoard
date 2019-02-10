using Newtonsoft.Json;
using SoundBoardWPF.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SoundBoardWPF.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MyViewModel : INotifyPropertyChanged
    {
        private string themeName;
        private ActionButton selectedActionButton;

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

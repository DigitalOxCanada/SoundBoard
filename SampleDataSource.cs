using System.Collections.ObjectModel;
using System.Linq;

namespace SoundBoardWPF
{
    class SampleDataSource
    {
        public string ThemeName { get; set; }
        public ObservableCollection<ActionButton> ActionButtons{ get; set; }

        public SampleDataSource()
        {
            ThemeName = "Sample Theme";

            ActionButtons = new ObservableCollection<ActionButton>
            {
                new ActionButton { Title = "Action 1" },
                new ActionButton { Title = "Action 2" }
            };
            ActionButtons[0].TheKey = System.Windows.Input.Key.NumPad0;
            ActionButtons[1].TheKey = System.Windows.Input.Key.NumPad1;
        }

    }
}

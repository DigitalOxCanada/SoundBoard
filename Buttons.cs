using GlobalHotKey;
using System.Windows.Controls;
using System.Windows.Input;

namespace SoundBoard
{
    public class HotKeyButton
    {
        public Key TheKey { get; set; }
        public string Title { get; set; }
        public string Audio { get; set; }
        public string Image { get; set; }
        public string Video { get; set; }
    }

    public class MyButton : Button
    {
        public HotKeyButton TheHkb { get; set; }
        public HotKey TheHotKey { get; set; }
    }

}

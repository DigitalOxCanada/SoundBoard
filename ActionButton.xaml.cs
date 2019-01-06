using GlobalHotKey;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

namespace SoundBoard
{
    /// <summary>
    /// Interaction logic for ActionButton.xaml
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public partial class ActionButton : Button
    {
        //public HotKeyButton TheHkb { get; set; }
        [JsonProperty]
        public Key? TheKey { get; set; }
        [JsonProperty]
        public string Title { get; set; }
        [JsonProperty]
        public string Audio { get; set; }
        [JsonProperty]
        public string Image { get; set; }
        [JsonProperty]
        public string Video { get; set; }
        [JsonProperty]
        public string Color { get; set; }

        public HotKey TheHotKey { get; set; }

        public ActionButton()
        {
            InitializeComponent();
        }
    }
}

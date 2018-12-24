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
using System.Windows.Shapes;

namespace SoundBoard
{
    /// <summary>
    /// Interaction logic for SubWindow.xaml
    /// </summary>
    public partial class SubWindow : Window
    {
        public SubWindow()
        {
            InitializeComponent();

            TheVideo.Loaded += TheVideo_Loaded;
            TheVideo.MediaFailed += TheVideo_MediaFailed;
            TheVideo.MediaEnded += TheVideo_MediaEnded;
//            TheVideo.Source = new Uri("fire.mpg", UriKind.Relative);
        }

        public void LoadVideo(string filename)
        {
            TheVideo.Source = new Uri(filename, UriKind.Relative);
            TheVideo.Visibility = Visibility.Visible;
            TheVideo.Play();

        }

        private void TheVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            TheVideo.Source = null;
            TheVideo.Visibility = Visibility.Hidden;
        }

        private void TheVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ErrorException.Message);
        }

        private void TheVideo_Loaded(object sender, RoutedEventArgs e)
        {
            //TheVideo.Visibility = Visibility.Visible;
            //((MediaElement)sender).Play();
        }
    }
}

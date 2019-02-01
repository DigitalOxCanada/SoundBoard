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
using System.Windows.Threading;

namespace SoundBoardWPF
{
    /// <summary>
    /// Interaction logic for SubWindow.xaml
    /// </summary>
    public partial class SubWindow : Window
    {
        bool IsVideoPlaying = false;
        App myApp;

        public SubWindow()
        {
            InitializeComponent();

            myApp = (App)Application.Current;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            TheVideo.Loaded += TheVideo_Loaded;
            TheVideo.MediaFailed += TheVideo_MediaFailed;
            TheVideo.MediaEnded += TheVideo_MediaEnded;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //if(IsVideoPlaying)
            //{
            //    if(TheVideo.Source != null)
            //    {
            //        if(TheVideo.NaturalDuration.HasTimeSpan)
            //        {
            //            if (TheVideo.Position > new TimeSpan(0,0,5+5))
            //            {
            //                IsVideoPlaying = false;
            //                TheVideo.Stop();
            //            }
            //        }
            //    }
            //}
        }

        public void LoadVideo(string filename)
        {
            TheVideo.Source = new Uri(filename, UriKind.Relative);
            TheVideo.Visibility = Visibility.Visible;
            //TheVideo.Position = new TimeSpan(0, 0, 5);
            IsVideoPlaying = true;
            TheVideo.Play();
        }

        private void TheVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            TheVideo.Source = null;
            TheVideo.Visibility = Visibility.Hidden;
            IsVideoPlaying = false;
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

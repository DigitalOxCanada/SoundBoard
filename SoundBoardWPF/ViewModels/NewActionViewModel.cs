using DigitalOx.SoundBoard.Plugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace DigitalOx.SoundBoard.ViewModels
{
    public class NewActionViewModel : INotifyPropertyChanged
    {
        App myApp;

        public ObservableCollection<PluginDef> PluginDefList { get; set; }

        public IPlugin SelectedPlugin { get; set; }
        public string DataTemplateValue { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        public NewActionViewModel()
        {
            myApp = (App)Application.Current;
            PluginDefList = new ObservableCollection<PluginDef>();

            foreach (var p in myApp.Plugins)
            {
                PluginDef d = new PluginDef() { Name = p.Name, Plugin = p };
                PluginDefList.Add(d);
            }

        }


        public class PluginDef
        {
            public string Name { get; set; }

            public IPlugin Plugin { get; set; }
        }


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

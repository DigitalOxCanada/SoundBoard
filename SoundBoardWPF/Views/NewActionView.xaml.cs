using DigitalOx.SoundBoard.Plugin;
using DigitalOx.SoundBoard.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DigitalOx.SoundBoard.Views
{
    public partial class NewActionView : Window, INotifyPropertyChanged
    {
        public NewActionViewModel nvm = new NewActionViewModel();

        public event PropertyChangedEventHandler PropertyChanged;


        public NewActionView()
        {
            InitializeComponent();
            this.DataContext = nvm;
        }

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChangedEventHandler eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //this is the item double clicked on
            var SelectedItem = ((ListBox)sender).SelectedItem;

            //TODO - show user json string to edit for the options
            //the json string should come from the plugin as the template designed by the plugin.
            tbDataTemplate.Text = FormatJson(((NewActionViewModel.PluginDef)SelectedItem).Plugin.DataTemplate);
        }

        private string FlattenJson(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.None);
        }


        private string FormatJson(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //this is so I have a handle to the selected one when this dialog box closes
            nvm.SelectedPlugin = ((NewActionViewModel.PluginDef)PluginsListBox.SelectedItem).Plugin;
            nvm.DataTemplateValue = FlattenJson(tbDataTemplate.Text);
            Window.GetWindow(this).DialogResult = true;
            this.Close();
        }
    }
}

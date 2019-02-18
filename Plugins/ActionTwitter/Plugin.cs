using Newtonsoft.Json;
using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DigitalOx.SoundBoard.Plugin
{
    public class ActionData
    {
        public string URL { get; set; }
    }

    class ActionTwitter : IPlugin
    {
        #region Interface members
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }

        private IPluginHost host;
        public IPluginHost Host
        {
            get { return host; }
            set
            {
                host = value;
                host.Register(this);
            }
        }
        #endregion

        #region Local members
        #endregion

        public ActionTwitter()
        {
            Name = "Action Twitter";
            Description = "Tweet";
            Id = "0DF8EF11-48FA-46A0-B67E-584C8548DA20";
        }

        public async Task<PluginResponse> DoWorkAsync(object actionData)
        {
            if (string.IsNullOrEmpty((string)actionData))
            {
                return PluginResponse.Fail;
            }

            ActionData data = JsonConvert.DeserializeObject<ActionData>(actionData.ToString());

            //perform work
            //await Task.Run(() => OpenUrl(data.URL));

            return PluginResponse.Ok;
        }


    }
}

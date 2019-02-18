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

    class ActionURL : IPlugin
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

        public ActionURL()
        {
            Name = "Action URL";
            Description = "Launches a URL in the default browser.";
            Id = "E1ABE34F-CBA7-4EAB-97A7-C404E20A136F";
        }

        public async Task<PluginResponse> DoWorkAsync(object actionData)
        {
            if (string.IsNullOrEmpty((string)actionData))
            {
                return PluginResponse.Fail;
            }

            ActionData data = JsonConvert.DeserializeObject<ActionData>(actionData.ToString());

            //perform work
            await Task.Run(() => OpenUrl(data.URL));

            return PluginResponse.Ok;
        }


        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }

        }
    }
}

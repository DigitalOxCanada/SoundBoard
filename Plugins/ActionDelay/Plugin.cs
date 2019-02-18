using Newtonsoft.Json;
using System;
using System.Data;
using System.Threading.Tasks;

namespace DigitalOx.SoundBoard.Plugin
{
    public class ActionData
    {
        public int Duration { get; set; }
    }

    class ActionDelay : IPlugin
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

        public ActionDelay()
        {
            Name = "Action Delay";
            Description = "Creates a delay before returning for the next action.";
            Id = "4B12FEE3-62F9-4BF4-AB3E-94B7247C1755";
        }

        public async Task<PluginResponse> DoWorkAsync(object actionData)
        {
            if(string.IsNullOrEmpty((string)actionData)) {
                return PluginResponse.Fail;
            }

            ActionData data = JsonConvert.DeserializeObject<ActionData>(actionData.ToString());
            
            //perform work
            await Task.Delay((int)data.Duration);
            
            return PluginResponse.Ok;
        }
       
    }
}

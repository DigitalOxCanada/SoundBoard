using System;
using System.Threading.Tasks;

namespace DigitalOx.SoundBoard.Plugin
{
    public enum PluginResponse
    {
        Unknown = -1,
        Ok = 1,
        Skip = 2,
        Fail = 3,
        Ignore = 4,
        Reject = 5
    }

    /// <summary>
    /// Generic plugin interface
    /// </summary>
    public interface IPlugin
    {
        string Id { get; set; }
        bool IsEnabled { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        IPluginHost Host { get; set; }

        Task<PluginResponse> DoWorkAsync(object a);
    }



}

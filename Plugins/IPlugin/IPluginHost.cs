using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalOx.SoundBoard.Plugin
{
    /// <summary>
    /// The host
    /// </summary>
    public interface IPluginHost
    {
        bool Register(IPlugin ipi);
    }
}

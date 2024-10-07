using System.IO;
using System.Threading;
using HappyMods.Core.Unity;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace HappyMods.Core.Config;

public class SerilogConfiguration(UnityConstants unityConstants)
{
    public void Init(string modPrefix)
    {
        Log.Logger = new LoggerConfiguration()
                     .WriteTo.File(Path.Combine($"{unityConstants.PersistentDataPath}", $"Happy.{modPrefix}.log"))
                     .CreateLogger();
    }
}
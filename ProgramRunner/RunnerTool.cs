using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;

// ReSharper disable UnusedType.Global

namespace ProgramRunner
{
    [PluginName("Program Runner")]
    public class RunnerTool : RunnerBase, ITool
    {
        public bool Initialize()
        {
            Start();
            return true;
        }

        public override void Dispose()
        {
            Stop();
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

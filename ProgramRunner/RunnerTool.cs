using OpenTabletDriver.Plugin;

namespace ProgramRunner
{
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
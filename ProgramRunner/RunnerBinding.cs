﻿using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;

namespace ProgramRunner
{
    [PluginName("Program Runner")]
    public class RunnerBinding : RunnerBase, IStateBinding
    {
        public void Press(TabletReference tablet, IDeviceReport report)
        {
            StartStop();
        }

        public void Release(TabletReference tablet, IDeviceReport report)
        {
        }
    }
}
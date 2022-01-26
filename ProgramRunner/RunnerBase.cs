using System.Diagnostics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace ProgramRunner
{
    public class RunnerBase : IDisposable
    {
        private string? _programPath;
        private string? _workingDirectory;

        protected const string LOG_GROUP = "ProgramRunner";

        [Property("Program Path")]
        public string? ProgramPath
        {
            set
            {
                _programPath = value;
                if (!File.Exists(_programPath))
                {
                    Log.Write(LOG_GROUP, "No file was found at the program path provided.", LogLevel.Warning);
                }
            }
            get => _programPath;
        }

        [Property("Working Directory")]
        public string? WorkingDirectory
        {
            set
            {
                _workingDirectory = value;
                if (!string.IsNullOrWhiteSpace(_workingDirectory) && !Directory.Exists(_workingDirectory))
                {
                    Log.Write(LOG_GROUP, "The working directory at the specified path does not exist.", LogLevel.Warning);
                }
            }
            get => _workingDirectory;
        }

        private Process? Process { set; get; }

        protected void Start()
        {
            if (Process is not null)
                return;

            if (File.Exists(ProgramPath))
            {
                string directory = Directory.Exists(WorkingDirectory)
                    ? WorkingDirectory
                    : Directory.GetParent(ProgramPath)!.FullName;

                Log.Write(LOG_GROUP, $"Starting process: {ProgramPath}");
                var startInfo = new ProcessStartInfo
                {
                    FileName = ProgramPath,
                    WorkingDirectory = directory
                };

                Process = Process.Start(startInfo);
            }
            else
            {
                Log.Write(LOG_GROUP, $"The specified program at \"{ProgramPath}\" does not exist. Process will not be started.");
            }
        }

        protected void Stop()
        {
            if (Process is null)
                return;

            try
            {
                Process.Kill(true);
                Process = null;
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        protected void StartStop()
        {
            if (Process is null)
                Start();
            else
                Stop();
        }

        public virtual void Dispose()
        {
            Process?.Dispose();
            Process = null;
            GC.SuppressFinalize(this);
        }
    }
}
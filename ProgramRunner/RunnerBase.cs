using System.Diagnostics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ProgramRunner
{
    public class RunnerBase : IDisposable
    {
        private string? _programPath, _workingDirectory;
        private Process? _process;

        private const string LOG_GROUP = "ProgramRunner";

        [Property("Program Path")]
        public string ProgramPath
        {
            set
            {
                _programPath = value;
                if (!File.Exists(_programPath))
                {
                    Log.Write(LOG_GROUP, "No file was found at the program path provided.", LogLevel.Warning);
                }
            }
            get => _programPath!;
        }

        [Property("Arguments")]
        public string? Arguments { set; get; }

        [Property("Working Directory")]
        public string? WorkingDirectory
        {
            set
            {
                _workingDirectory = value;
                if (!string.IsNullOrWhiteSpace(_workingDirectory) && !Directory.Exists(_workingDirectory))
                {
                    Log.Write(LOG_GROUP, "The working directory at the specified path does not exist.",
                        LogLevel.Warning);
                }
            }
            get => Directory.Exists(_workingDirectory) ? _workingDirectory : Directory.GetParent(ProgramPath)!.FullName;
        }

        [BooleanProperty("Redirect standard output", "Redirect standard output to Log")]
        public bool RedirectStandardOutput { set; get; }

        protected void Start()
        {
            if (_process is not null)
                return;

            if (File.Exists(ProgramPath))
            {
                Log.Write(LOG_GROUP, $"Starting process: {ProgramPath}");
                var startInfo = new ProcessStartInfo
                {
                    FileName = ProgramPath,
                    Arguments = Arguments,
                    WorkingDirectory = WorkingDirectory,

                    // Redirecting stdout requires disabling shell execute
                    RedirectStandardOutput = RedirectStandardOutput,
                    UseShellExecute = !RedirectStandardOutput
                };

                try
                {
                    _process = Process.Start(startInfo)!;

                    if (RedirectStandardOutput)
                    {
                        _process.OutputDataReceived += ConsoleToLog;
                        _process.BeginOutputReadLine();
                    }
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                    _process?.Dispose();
                    _process = null;
                }
            }
            else
            {
                Log.Write(LOG_GROUP, $"The specified program at \"{ProgramPath}\" does not exist. Process will not be started.");
            }
        }

        protected void Stop()
        {
            if (_process is null)
                return;

            try
            {
                _process.Kill(true);
                _process = null;
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        protected void StartStop()
        {
            if (_process is null)
                Start();
            else
                Stop();
        }

        public virtual void Dispose()
        {
            _process?.Dispose();
            _process = null;
            GC.SuppressFinalize(this);
        }

        private void ConsoleToLog(object? sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Log.Write(LOG_GROUP, e.Data);
            }
        }
    }
}

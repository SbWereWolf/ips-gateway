using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace archivist
{
    /* base on https://markpelf.com/1072/logging-proxy-in-c/ */
    public class IndependentFileArchivist
    {
        private string CorrelationId = string.Empty;
        private string InvokeId = string.Empty;
        private string ClassName = string.Empty;
        private string MethodName = string.Empty;

        private Object LogLocker = new();

        protected const string SettingsKey =
            "independent-logger-settings-file";
        private string? LogFilename = null;

        public bool WasInitialized { get; private set; }

        public IndependentFileArchivist()
        {
            WasInitialized = false;
        }

        private static string ProcureLogFilename
        {
            get
            {
                var secretConfig = new ConfigurationBuilder()
                    .AddUserSecrets<FileArchivist>()
                    .Build();
                var fileWithSettings = secretConfig[SettingsKey] ?? "";

                var config = File.ReadAllText(fileWithSettings);
                config = config.Trim(Environment.NewLine.ToArray<char>());


                return config ?? string.Empty;
            }
        }

        private void MakeLogFilename()
        {
            if (LogFilename == null)
            {
                var filename = ProcureLogFilename;

                var isFileDefined = !string.IsNullOrEmpty(filename);
                if (!isFileDefined)
                {
                    throw new Exception("Log filename is not defined");
                }

                LogFilename =
                new StringBuilder().
                AppendJoin(
                '_',
                new string?[] {
                    filename,
                    CorrelationId,
                    DateTime.Now.Ticks.ToString(),
                    $"{InvokeId}.log"
                }
                ).
                ToString();
            }
        }

        public void Initialize(string correlationId, MethodBase method)
        {
            ClassName = method.DeclaringType?.FullName ?? string.Empty;
            MethodName = method.Name;
            CorrelationId = correlationId;
            InvokeId = $"invokeId={Guid.NewGuid()}";

            MakeLogFilename();
        }

        public async void Before(
            DateTime now,
            object?[]? args)
        {
            args ??= Array.Empty<object>();

            var arguments = new List<string?>();
            foreach (var obj in args)
            {
                string serialized = SerializeWithJson(obj);
                arguments.Add(serialized);
            }
            var values = new StringBuilder().
                AppendJoin('\u00A0', arguments.ToArray());

            var message = new StringBuilder().
                AppendJoin(
                ' ',
                new string?[] {
                    now.Ticks.ToString(),
                    CorrelationId,
                    InvokeId,
                    "RUN",
                    ClassName,
                    "->",
                    MethodName,
                    $"input ({values})",
                }
                ).
                ToString();

            await WriteLogToFile(message);
        }

        private static string SerializeWithJson(object? obj)
        {
            var serialized = JsonConvert.SerializeObject(obj);
            if (serialized == "{}")
            {
                serialized = obj?.ToString();
            }

            return serialized ?? "non-serializable-value";
        }

        public async void After(
            DateTime now,
            object? result
            )
        {
            var jsonResult = SerializeWithJson(result);

            var message = new StringBuilder().
                AppendJoin(
                ' ',
                new string?[] {
                    now.Ticks.ToString(),
                    CorrelationId,
                    InvokeId,
                    "END",
                    ClassName,
                    "->",
                    MethodName,
                    $"output {jsonResult}",
                }
                ).
                ToString();

            await WriteLogToFile(message);
        }

        public async void Exception(
            DateTime now,
            Exception exception)
        {
            var message = new StringBuilder().
                AppendJoin(
                ' ',
                new string?[] {
                    now.Ticks.ToString(),
                    CorrelationId,
                    InvokeId,
                    "EXC",
                    ClassName,
                    "->",
                    MethodName,
                    exception.Message,
                }
                ).
                ToString();

            await WriteLogToFile(message);
        }

        private Task WriteLogToFile(string logMessage)
        {
            lock (LogLocker)
            {
                if (!string.IsNullOrWhiteSpace(LogFilename))
                {
                    File.AppendAllText(
                        LogFilename,
                        logMessage + Environment.NewLine
                        );
                }
            }

            return Task.CompletedTask;

        }
    }
}

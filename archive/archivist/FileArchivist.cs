using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace archivist
{
    /* base on https://markpelf.com/1072/logging-proxy-in-c/ */
    public class FileArchivist
    {
        private string CorrelationId = string.Empty;
        private string RunId = string.Empty;
        private string ClassName = string.Empty;
        private string MethodName = string.Empty;

        private Object LogLocker = new();

        protected const string SettingsKey =
            "integrated-logger-settings-file";
        private string? LogFilename = null;

        private string ProcureLogFilename()
        {
            var secretConfig = new ConfigurationBuilder()
                .AddUserSecrets<FileArchivist>()
                .Build();
            var fileWithSettings = secretConfig[SettingsKey] ?? "";

            var config = File.ReadAllText(fileWithSettings);
            config = config.Trim(Environment.NewLine.ToArray<char>());


            return config ?? string.Empty;
        }

        private void MakeLogFilename(
            string runId,
            string? correlationId
            )
        {
            if (LogFilename == null)
            {
                var filename = ProcureLogFilename();

                var isFileDefined = !string.IsNullOrEmpty(filename);
                if (!isFileDefined)
                {
                    throw new Exception($"Log filename is not defined");
                }

                LogFilename =
                new StringBuilder().
                AppendJoin(
                '_',
                new string?[] {
                    filename,
                    correlationId,
                    DateTime.Now.Ticks.ToString(),
                    $"{runId}.log"
                }
                ).
                ToString();
            }
        }

        public void Initialize(
            string? correlationId,
            string runId,
            string className,
            MethodInfo? methodInfo
            )
        {
            CorrelationId = correlationId ?? string.Empty;
            RunId = runId;
            ClassName = className;
            MethodName = methodInfo?.Name ?? string.Empty;

            MakeLogFilename(RunId, CorrelationId);

        }

        public async void Before(
            DateTime now,
            object?[]? args)
        {
            args ??= Array.Empty<object>();

            var arguments = new List<string?>();
            foreach (var obj in args)
            {
                string? serialized = serializeWithJson(obj);
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
                    RunId,
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

        private static string? serializeWithJson(object? obj)
        {
            var serilized = JsonConvert.SerializeObject(obj);
            if (serilized == "{}")
            {
                serilized = obj?.ToString();
            }

            return serilized ?? "null";
        }

        public async void After(
            DateTime now,
            object? result
            )
        {
            var jsonResult = serializeWithJson(result);

            var message = new StringBuilder().
                AppendJoin(
                ' ',
                new string?[] {
                    now.Ticks.ToString(),
                    CorrelationId,
                    RunId,
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
                    RunId,
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

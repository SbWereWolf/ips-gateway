using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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

        private void MakeLogFilename(string runId)
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
                new string[] {
                    filename,
                    DateTime.Now.Ticks.ToString(),
                    $"{runId}.log"
                }
                ).
                ToString();
            }
        }

        public async void Before(
            string runId,
            DateTime now,
            object[]? args,
            string classname,
            MethodInfo? methodInfo
            )
        {
            MakeLogFilename(runId);

            args ??= Array.Empty<object>();

            var arguments = new List<string>();
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
                new string[] {
                    DateTime.Now.Ticks.ToString(),
                    runId,
                    "RUN",
                    classname,
                    "->",
                    $"{methodInfo?.Name}({values})"
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
            string runId, 
            DateTime now, 
            object? result
            )
        {
            MakeLogFilename(runId);

            var jsonResult = serializeWithJson(result);

            var message = new StringBuilder().
                AppendJoin(
                ' ',
                new string[] {
                    now.Ticks.ToString(),
                    runId,
                    "END",
                    jsonResult
                }
                ).
                ToString();

            await WriteLogToFile(message);
        }

        public async void Exception(
            string runId, 
            DateTime now, 
            Exception exception
            )
        {
            MakeLogFilename(runId);

            var message = new StringBuilder().
                AppendJoin(
                ' ',
                new string[] {
                    now.Ticks.ToString(),
                    runId,
                    "EXC",
                    exception.Message
                }
                ).
                ToString();

            await WriteLogToFile(message);
        }

        private Task WriteLogToFile(string logMessage)
        {
            lock (LogLocker)
            {
                File.AppendAllText(
                    LogFilename,
                    logMessage + Environment.NewLine
                    );
            }

            return Task.CompletedTask;

        }
    }
}

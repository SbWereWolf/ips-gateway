using archivist;

namespace ReadingInboxLibrary
{
    public class DirectoryObserver : IReadingInbox, ILogable
    {
        string BodySeparator = $"{Environment.NewLine}{Environment.NewLine}";
        private const char OptionValueSeparator = ';';
        string Path = string.Empty;

        private string CorrelationId;

        public DirectoryObserver(string correlationId)
        {
            CorrelationId = correlationId;
        }

        public string GetCorrelationId()
        {
            return CorrelationId;
        }

        [Log]
        public bool GetReadyForReading(string options)
        {
            Path =  options.TrimEnd(OptionValueSeparator);
            var isExists  = Directory.Exists(Path);
            if (!isExists)
            {
                throw new Exception($"Path `{Path}` is not exists");
            }


            return true;
        }

        [Log]
        public IEnumerable<string> LetReadTheMessages()
        {
            var files = Directory.GetFiles(Path,"*.http");

            var content = string.Empty;
            foreach (var filefullname in files)
            {
                content = File.ReadAllText(filefullname);

                var pos = content.IndexOf(BodySeparator);


                var body = string.Empty;
                if (pos != -1)
                {
                    body = content.Substring(pos + BodySeparator.Length);
                }

                yield return body;
            }
        }
    }
}

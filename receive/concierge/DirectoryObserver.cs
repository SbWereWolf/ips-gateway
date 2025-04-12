using ReadingInboxLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ReadingInboxLibrary
{
    public class DirectoryObserver : IReadingInbox
    {
        string BodySeparator = $"{Environment.NewLine}{Environment.NewLine}";
        private const char OptionValueSeparator = ';';
        string Path = string.Empty; 

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

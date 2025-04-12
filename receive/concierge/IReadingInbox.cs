namespace ReadingInboxLibrary
{
    public interface IReadingInbox
    {
        public bool GetReadyForReading(string options);
        public IEnumerable<string> LetReadTheMessages();

    }
}
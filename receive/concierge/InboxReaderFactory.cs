namespace ReadingInboxLibrary
{
    public class InboxReaderFactory : IConstructingInboxReaders
    {
        public IReadingInbox Make(string kind, string correlationId)
        {
            return new DirectoryObserver(correlationId);
        }
    }
}

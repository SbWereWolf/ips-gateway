namespace ReadingInboxLibrary
{
    public class InboxReaderFactory : IConstructingInboxReaders
    {
        public IReadingInbox Make(string kind, string correlationId)
        {
            return archivist.
                LoggingDecorator<IReadingInbox>.
                Create(new DirectoryObserver(correlationId), correlationId);
        }
    }
}

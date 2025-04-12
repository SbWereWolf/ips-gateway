namespace ReadingInboxLibrary
{
    public class InboxReaderFactory : IConstructingInboxReaders
    {
        public IReadingInbox Make(string kind)
        {
            return archivist.
                LoggingDecorator<IReadingInbox>.
                Create(new DirectoryObserver());
        }
    }
}

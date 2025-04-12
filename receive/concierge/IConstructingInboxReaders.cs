using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingInboxLibrary
{
    public interface IConstructingInboxReaders
    {
        public IReadingInbox Make(string kind);
    }
}

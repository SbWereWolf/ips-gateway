using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archivist
{
    public class ArrayStringPrinter
    {
        private readonly string[] Items;

        public ArrayStringPrinter(string[] items)
        {
            this.Items = items ?? Array.Empty<string>();
        }

        public string[] Output()
        {
            return Items;
        }

        public override string ToString()
        {
            var result = $"{string.Join("\u00A0,\u00A0", Items)}";

            return result;
        }
    }
}

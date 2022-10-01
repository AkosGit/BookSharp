using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookProvider.Exceptions
{
    public class NoProviderException : Exception
    {
        public override string Message
        {
            get
            {
                return "No providers given, new provider can be added with AddProvider method!";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDeviceItems.Exceptions
{
    public class CommandException : Exception
    {
        public CommandException() : base() { }
        public CommandException(string message) : base(message) { }
        public CommandException(string message, Exception inner) : base(message, inner) { }
    }
}

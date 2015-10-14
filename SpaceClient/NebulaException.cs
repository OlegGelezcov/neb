using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client
{
    public class NebulaException : Exception
    {
        public NebulaException(string message)
            : base(message)
        { }

        public NebulaException() : base()
        { }

        public NebulaException(string message, Exception innerException)
            : base(message, innerException)
        { }

    }
}

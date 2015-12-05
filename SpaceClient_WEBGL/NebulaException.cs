using System;

namespace Nebula.Client {
    public class NebulaException : Exception {
        public NebulaException(string message)
            : base(message) { }

        public NebulaException() : base() { }

        public NebulaException(string message, Exception innerException)
            : base(message, innerException) { }

    }
}

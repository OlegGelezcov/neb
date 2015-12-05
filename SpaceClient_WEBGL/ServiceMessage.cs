using System;
using Common;

namespace Nebula.Client {
    public class ServiceMessage : IServiceMessage {

        public ServiceMessage(ServiceMessageType type, string message) {
            this.Type = type;
            this.Message = message;
            this.time = DateTime.UtcNow;
        }

        public Common.ServiceMessageType Type {
            get;
            private set;
        }

        public string Message {
            get;
            private set;
        }

        public DateTime time {
            get;
            private set;
        }
    }
}

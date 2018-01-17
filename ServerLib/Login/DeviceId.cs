using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Server.Login {
    public class DeviceId {

        public string Value { get; private set; } = string.Empty;

        public DeviceId(string deviceId ) {
            Value = deviceId;
            if(Value == null) {
                Value = string.Empty;
            }
        }
    }
}

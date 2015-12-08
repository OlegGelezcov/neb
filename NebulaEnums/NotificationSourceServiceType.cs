using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerClientCommon {

    /// <summary>
    /// What service notification belong
    /// </summary>
    public enum NotificationSourceServiceType : int {
        Guild,
        Server,
        Group,
        Election,
        Friends,
    }
}

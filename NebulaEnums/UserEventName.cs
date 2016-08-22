using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
    public enum UserEventName : int {
        empty = 1,
        rotate_camera = 2,
        dialog_completed = 3,
        start_moving = 4,
        object_scanner_select_ship = 5
    }
}

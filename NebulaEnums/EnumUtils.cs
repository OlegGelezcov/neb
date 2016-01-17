using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
    public static class EnumUtils {
        public static ObjectColor Pet2ObjColor(PetColor color) {
            switch (color) {
                case PetColor.gray:
                case PetColor.white:
                    return ObjectColor.white;
                case PetColor.blue:
                    return ObjectColor.blue;
                case PetColor.yellow:
                    return ObjectColor.yellow;
                case PetColor.green:
                    return ObjectColor.green;
                case PetColor.orange:
                    return ObjectColor.orange;
                default:
                    return ObjectColor.white;
            }
        }
    }
}

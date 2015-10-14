using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Engine {
    public class PropertyMissedException : Exception {

        public PropertyMissedException(object propertyName, string objectName )
            : base(string.Format("Object {0} missed property {1}", objectName, propertyName)) {

        }
    }
}
